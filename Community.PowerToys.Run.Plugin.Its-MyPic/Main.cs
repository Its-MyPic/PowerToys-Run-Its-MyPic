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
using Wox.Infrastructure;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class Main : IPlugin
	{
		private readonly Data DB = new();

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
			var subtitles = DB.GetSubtitleDatas(search);
			foreach (var subtitle in subtitles)
			{
				var IcoPath = $"{PluginDirectory}/Images/{subtitle.FileName}";
				var r = StringMatcher.FuzzySearch(query.Search, subtitle.Text);
				results.Add(new Result
				{
					QueryTextDisplay = search,
					IcoPath = IcoPath,
					Title = subtitle.Text,
					TitleHighlightData = r.MatchData,
					Score = r.Score,
					//SubTitle = $"{subtitleData.description}",
					//ToolTipData = new ToolTipData("Title", "Text"),
					Action = _ =>
					{
						var list = new StringCollection
						{
							$"{PluginDirectory}/Images/{subtitle.FileName}"
						};
						Clipboard.SetFileDropList(list);
						return true;
					},
					ContextData = subtitle,
				});
			}
			return results;
		}


		public void Init(PluginInitContext context)
		{
			Context = context ?? throw new ArgumentNullException(nameof(context));
			Context.API.ThemeChanged += OnThemeChanged;
			UpdateIconPath(Context.API.GetCurrentTheme());

			//_cache = new CachingService();

		}

		private void UpdateIconPath(Theme theme) => IconPath = theme == Theme.Light || theme == Theme.HighContrastWhite ? "Images/its_mypic.light.png" : "Images/its_mypic.dark.png";

		private void OnThemeChanged(Theme currentTheme, Theme newTheme) => UpdateIconPath(newTheme);

	}
}
