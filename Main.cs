using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;
using System.Windows;
using Wox.Plugin;
using System.IO;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;
using System.Linq;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class Main : IPlugin, IDelayedExecutionPlugin, IContextMenu, ISettingProvider, IPluginI18n
	{
		public static string PluginID => "048FCB4CE3034FD9ACD9486F9FAB1F9E";
		public string Name => "Its-MyPic";
		public static string PluginDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
		public string Description => "MyGO Screenshot Quick Copy";
		public static bool CopyImage { get; set; }

		public IEnumerable<PluginAdditionalOption> AdditionalOptions => [
			new(){
				PluginOptionType = PluginAdditionalOption.AdditionalOptionType.Checkbox,
				Key = "copyType",
				DisplayLabel = "Copy Image instead of copy file",
				DisplayDescription = "Copy Image when enabled, otherwise copy file.",
				Value = false
			}
			];

		private readonly Data DB = new();


		public List<Result> Query(Query query)
		{
			return Query(query, false);
		}
		public List<Result> Query(Query query, bool delayedExecution)
		{
			var search = query.Search.ToLower();
			var results = DB.GetMatchedSubtitleDatas(search, delayedExecution);
			return results;
		}
		public void Init(PluginInitContext context)
		{
			CopyImage = false;
		}

		public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
		{
			return [
					new()
					{
						PluginName = "It's My Pic!",
						Title = "Copy (Shift+Enter)",
						Glyph = "\xE8C8",
						FontFamily = "Segoe Fluent Icons, Segoe MDL2 Assets",
						AcceleratorKey = Key.Enter,
						AcceleratorModifiers = ModifierKeys.Shift,
						Action = _ =>
						{
							Clipboard.SetText(selectedResult.Title);
							return true;
						}
					}
				];
		}

		public Control CreateSettingPanel()
		{
			throw new System.NotImplementedException();
		}

		public void UpdateSettings(PowerLauncherPluginSettings settings)
		{
			var option = settings.AdditionalOptions.FirstOrDefault(x => x.Key == "copyType");
			CopyImage = option.Value;
		}

		public string GetTranslatedPluginTitle()
		{
			throw new System.NotImplementedException();
		}

		public string GetTranslatedPluginDescription()
		{
			throw new System.NotImplementedException();
		}
	}
}
