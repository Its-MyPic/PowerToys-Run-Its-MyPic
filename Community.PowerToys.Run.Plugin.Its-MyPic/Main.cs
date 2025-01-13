using ManagedCommon;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Wox.Plugin;
using Wox.Plugin.Logger;
using System.Text.Json;
using System.IO;
using System.Reflection;
using LazyCache;
using System.Text.RegularExpressions;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Specialized;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class Main : IPlugin
	{
		readonly JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
		private readonly Dictionary<string, SubtitleData> subtitleDatas = [];
		private readonly HttpClient client = new();

		public static string PluginID => "048FCB4CE3034FD9ACD9486F9FAB1F9E";
		public string Name => "Its-MyPic";
		public static string PluginDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
		public string Description => "MyGO快速複製圖片";

		private CachingService _cache;

		private PluginInitContext Context { get; set; }

		private string IconPath { get; set; }

		public List<Result> Query(Query query)
		{
			List<Result> results = [];

			var search = query.Search.ToLower();

			// filt out the subtitleDatas that match the search

			var filteredDatas = _cache.GetOrAdd(search, () => subtitleDatas.Where(data => data.Key.Contains(search)).Take(5).ToList());

			_ = PrepareImage(filteredDatas);

			foreach (var filteredData in filteredDatas)
			{
				var subtitleData = filteredData.Value;
				var IcoPath = $"{PluginDirectory}/Images/{subtitleData.file_name}";
				if (!File.Exists(IcoPath))
				{
					IcoPath = IconPath;
				}
				results.Add(new Result
				{
					QueryTextDisplay = search,
					IcoPath = IcoPath,
					Title = subtitleData.name,
					SubTitle = $"{subtitleData.description}",
					//ToolTipData = new ToolTipData("Title", "Text"),
					Action = _ =>
					{
						var list = new StringCollection
						{
							$"{PluginDirectory}/Images/{subtitleData.file_name}"
						};
						Clipboard.SetFileDropList(list);
						return true;
					},
					ContextData = subtitleData.name,
				});
			}
			return results;
		}

		private async Task PrepareImage(List<KeyValuePair<string, SubtitleData>> filteredDatas)
		{
			IEnumerable<Task> tasks = filteredDatas.Select(d => DownloadImage(d.Value));
			await Task.WhenAll(tasks);
		}

		private async Task DownloadImage(SubtitleData subtitleData)
		{
			var file_name = subtitleData.file_name;
			var file_path = $"{PluginDirectory}/Images/{file_name}";
			if (!File.Exists(file_path))
			{
				var url = $"https://drive.miyago9267.com/d/file/img/mygo/{file_name}";
				using HttpResponseMessage message = await client.GetAsync(url);
				var stream = await message.Content.ReadAsByteArrayAsync();
				await File.WriteAllBytesAsync(file_path, stream);
			}
		}

		public void Init(PluginInitContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			Context.API.ThemeChanged += OnThemeChanged;
			UpdateIconPath(Context.API.GetCurrentTheme());

			_cache = new CachingService();

			var datas = JsonSerializer.Deserialize<List<SubtitleData>>(File.ReadAllText($"{PluginDirectory}/data.json"), options);
			foreach (var subtitleData in datas)
			{
				subtitleDatas[subtitleData.name.ToLower()] = subtitleData;
			}
		}

		private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/its_mypic.light.png" : "Images/its_mypic.dark.png";

		private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

	}
}
