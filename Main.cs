using System.Collections.Generic;
using System.Windows.Input;
using System.Reflection;
using System.Windows;
using Wox.Plugin;
using System.IO;
using Microsoft.PowerToys.Settings.UI.Library;
using System.Windows.Controls;
using System.Linq;
using Community.PowerToys.Run.Plugin.Update;
using Wox.Infrastructure.Storage;
using Wox.Plugin.Logger;

namespace Community.PowerToys.Run.Plugin.Its_MyPic
{
	public class Main : IPlugin, IDelayedExecutionPlugin, IContextMenu, ISettingProvider, IPluginI18n, ISavable
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

		private PluginJsonStorage<SampleSettings> Storage { get; set; }
		public void Save() => Storage.Save();

		private SampleSettings Settings { get; set; }

		private PluginUpdateHandler Updater { get; set; }
		private PluginInitContext Context { get; set; }

		public List<Result> Query(Query query)
		{
			return Query(query, false);
		}
		public List<Result> Query(Query query, bool delayedExecution)
		{
			var search = query.Search.ToLower();
			List<Result> results;
			if (Updater.IsUpdateAvailable())
			{
				Log.Info("Update available", GetType());
				results = Updater.GetResults();
			}
			else
			{
				results = DB.GetMatchedSubtitleDatas(search, delayedExecution);
			}
			return results;
		}
		public void Init(PluginInitContext context)
		{
			CopyImage = false;
			Context = context;

			Storage = new PluginJsonStorage<SampleSettings>();
			Settings = Storage.Load();

			Log.Info(context.CurrentPluginMetadata.Version, GetType());

			Updater = new PluginUpdateHandler(Settings.Update);
			Updater.UpdateInstalling += OnUpdateInstalling;
			Updater.UpdateInstalled += OnUpdateInstalled;
			Updater.UpdateSkipped += OnUpdateSkipped;

			Updater.Init(Context);
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


		private void OnUpdateInstalling(object sender, PluginUpdateEventArgs e)
		{
			Log.Info("UpdateInstalling: " + e.Version, GetType());
		}

		private void OnUpdateInstalled(object sender, PluginUpdateEventArgs e)
		{
			Log.Info("UpdateInstalled: " + e.Version, GetType());
			Context!.API.ShowNotification($"{Name} {e.Version}", "Update installed");
		}

		private void OnUpdateSkipped(object sender, PluginUpdateEventArgs e)
		{
			Log.Info("UpdateSkipped: " + e.Version, GetType());
			Save();
			Context?.API.ChangeQuery(Context.CurrentPluginMetadata.ActionKeyword, true);
		}
	}
}
