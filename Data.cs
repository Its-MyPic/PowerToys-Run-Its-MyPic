using System.Collections.Specialized;
using System.Collections.Generic;
using Wox.Infrastructure.Storage;
using System.Threading.Tasks;
using Wox.Infrastructure;
using Wox.Plugin.Logger;
using System.Text.Json;
using System.Net.Http;
using System.Windows;
using System.Linq;
using Wox.Plugin;
using System.IO;
using System;
using System.Windows.Media.Imaging;
using System.Security.Cryptography;
using System.Text;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class History
	{
		public int Version { get; set; }
		public List<int> Histroy { get; set; }
	}
	public class SubtitleInfo
	{
		private static int modifiedCount = 0;
		public string Text { get; set; }
		public string Episode { get; set; }
		public int Frame_start { get; set; }
		public int UsedCount { get; set; }
		public int Segment_id { get; set; }
		public string FileName => $"{Episode}_{Frame_start}.jpg";

		public Result ToResult(string search, Data data)
		{
			var r = StringMatcher.FuzzySearch(search, Text);
			var path = $"{Main.PluginDirectory}/Images/{FileName}";
			return new Result
			{
				QueryTextDisplay = search,
				IcoPath = path,
				Title = Text,
				TitleHighlightData = r.MatchData,
				Score = UsedCount,
				Action = _ =>
				{
					UsedCount++;
					modifiedCount++;
					if (modifiedCount > 5)
					{
						modifiedCount = 0;
						data.Save();
						Log.Debug("Data Saved", GetType());
					}
					if (Main.CopyImage)
					{
						Clipboard.SetImage(new BitmapImage(new Uri(path)));
					}
					else
					{
						Clipboard.SetFileDropList([path]);
					}
					return true;
				},
				ContextData = this,
			};
		}

	}
	public class Data
	{
		private static readonly int DATA_VERSION = BitConverter.ToInt32(SHA256.HashData(Encoding.UTF8.GetBytes("なんで春日影やったの！？")), 0);
		private readonly HttpClient client = new();
		readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, };
		public static string PluginDirectory => Main.PluginDirectory;

		private readonly List<SubtitleInfo> subtitleDatas = [];

		private readonly PluginJsonStorage<History> storage;
		private readonly History history;
		public Data()
		{
			if (!Directory.Exists($"{PluginDirectory}/Images"))
			{
				Directory.CreateDirectory($"{PluginDirectory}/Images");
			}
			subtitleDatas = JsonSerializer.Deserialize<List<SubtitleInfo>>(File.ReadAllText($"{PluginDirectory}/data/data.json"), options);
			foreach (var data in subtitleDatas)
			{
				data.Text = data.Text.Replace("妳", "你").ToLower();
			}
			Log.Info($"Loaded {subtitleDatas.Count} Subtitle Data", GetType());

			storage = new();

			history = storage.Load();
			Log.Debug($"{history.Version} {DATA_VERSION}", GetType());
			if (history.Version != DATA_VERSION)
			{
				Log.Info("History is outdated, updateing", GetType());
				history.Version = DATA_VERSION;
				history.Histroy = null;
			}
			if (history.Histroy is null)
			{
				Log.Info("History is empty, Initializing", GetType());
				history.Histroy = new List<int>(new int[subtitleDatas.Count]);
				storage.Save();
			}
			for (int i = 0; i < history.Histroy.Count; i++)
			{
				subtitleDatas[i].UsedCount = history.Histroy[i];
			}
		}
		public List<Result> GetMatchedSubtitleDatas(string SEARCH, bool waitable = true)
		{
			var search = SEARCH.ToLower();
			var ret = subtitleDatas
				.Where(data => data.Text.Contains(search.Replace("妳", "你"), StringComparison.CurrentCultureIgnoreCase))
				.OrderByDescending(e => e.UsedCount)
				.ThenByDescending(e => e.UsedCount)
				.Take(25);
			if (waitable)
			{
				PrepareImage(ret).Wait();
			}
			else
			{
				ret = ret.Where(e => File.Exists($"{PluginDirectory}/Images/{e.FileName}"));
			}
			return ret.Select(e => e.ToResult(SEARCH, this)).ToList();
		}
		private async Task PrepareImage(IEnumerable<SubtitleInfo> filteredDatas)
		{
			IEnumerable<Task> tasks = filteredDatas.Select(d => DownloadImage(d));
			await Task.WhenAll(tasks);
		}
		private async Task DownloadImage(SubtitleInfo subtitleData)
		{
			var file_name = subtitleData.FileName;
			var file_path = $"{PluginDirectory}/Images/{file_name}";
			if (File.Exists(file_path))
			{
				return;
			}
			var url = $"https://media.githubusercontent.com/media/jeffpeng3/MyPicDB/assets/images/{file_name}";
			using HttpResponseMessage message = await client.GetAsync(url);
			var stream = await message.Content.ReadAsByteArrayAsync();
			for (int i = 0; i < 3; i++)
			{
				try
				{
					await File.WriteAllBytesAsync(file_path, stream);
					return;
				}
				catch (Exception e)
				{
					Log.Warn($"Failed to download {file_name} retrying {i + 1} {e}", GetType());
				}
			}
			Log.Error($"Failed to download {file_name}", GetType());
		}

		public void Save()
		{
			history.Histroy = subtitleDatas.Select(e => e.UsedCount).ToList();
			storage.Save();
		}
	}
}
