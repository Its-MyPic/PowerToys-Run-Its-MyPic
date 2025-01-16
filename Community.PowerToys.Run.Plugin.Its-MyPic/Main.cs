using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;
using System.Windows;
using Wox.Plugin;
using System.IO;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class Main : IPlugin, IDelayedExecutionPlugin, IContextMenu
	{
		public static string PluginID => "048FCB4CE3034FD9ACD9486F9FAB1F9E";
		public string Name => "Its-MyPic";
		public static string PluginDirectory => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
		public string Description => "MyGO Screenshot Quick Copy";
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
		}

		public List<ContextMenuResult> LoadContextMenus(Result selectedResult)
		{
			return [
					new()
					{
						PluginName = "It's My Pic!",
						Title = "Copy (Enter)",
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
	}
}
