using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class SubtitleInfo
	{
		public string Text { get; set; }
		public string Episode { get; set; }
		public int Frame_start { get; set; }
		public int UsedCount { get; set; }
		public int Segment_id { get; set; }
		public string FileName => $"{Episode}_{Frame_start}.jpg";

	}
	public class Data
	{
		private readonly HttpClient client = new();
		private static int UpdateCount = 0;
		readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true, };
		public static string PluginDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;

		private readonly List<SubtitleInfo> subtitleDatas = [];
		public Data()
		{
			subtitleDatas = JsonSerializer.Deserialize<List<SubtitleInfo>>(File.ReadAllText($"{PluginDirectory}/data.json"), options);
			foreach (var data in subtitleDatas)
			{
				//data.UsedCount = System.Windows.Properties.Settings.Default;
			}
		}
		public List<SubtitleInfo> GetSubtitleDatas(string search)
		{
			search = search.ToLower();
			var ret = subtitleDatas
				.Where(data => data.Text.Contains(search))
				.OrderByDescending(e => e.UsedCount)
				.ThenByDescending(e => e.UsedCount)
				.Take(25).ToList();
			PrepareImage(ret).Wait();
			Console.WriteLine($"GetSubtitleDatas {search} {ret.Count}, downloaded");


			return ret;
		}
		public void UpdateUsedCount(int Segment_id)
		{
			var index = subtitleDatas.FindIndex(e => e.Segment_id == Segment_id);
			if (index != -1)
			{
				UpdateCount++;
				subtitleDatas[index].UsedCount++;
			}
			if (UpdateCount > 10)
			{
				UpdateCount = 0;
				File.WriteAllText($"{PluginDirectory}/data.json", JsonSerializer.Serialize(subtitleDatas, options));
			}
		}

		private async Task PrepareImage(List<SubtitleInfo> filteredDatas)
		{
			IEnumerable<Task> tasks = filteredDatas.Select(d => DownloadImage(d));
			await Task.WhenAll(tasks);
		}

		private async Task DownloadImage(SubtitleInfo subtitleData)
		{
			var file_name = subtitleData.FileName;
			var file_path = $"{PluginDirectory}/Images/{file_name}";
			if (!File.Exists(file_path))
			{
				var url = $"https://raw.githubusercontent.com/jeffpeng3/PowerToys-Run-Its-MyPic/assets/images/{file_name}";
				using HttpResponseMessage message = await client.GetAsync(url);
				var stream = await message.Content.ReadAsByteArrayAsync();
				for (int i = 0; i < 3; i++)
				{
					try
					{
						await File.WriteAllBytesAsync(file_path, stream);
						break;
					}
					catch (Exception e)
					{
						Console.WriteLine($"retrying safe Image {file_name} {i}: {e.Message}");
					}
				}
				Console.WriteLine($"Downloaded {file_name}");
			}
		}
	}

}
