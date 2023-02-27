using GameReaderCommon;
using SimHub.Plugins;
using System;

namespace SimHubPlugin
{
	// these must be public
    public class MysterySettings // saved while plugin restarts
    {
		public byte[] CCvalue { get; set; } = { 0,0,0 };
	}

	[PluginDescription("Unexpected 0 value properties from ReadCommonSettings()")]
	[PluginAuthor("blekenbleu")]
	[PluginName("Unexpected")]
	public class Unexpected : IPlugin, IDataPlugin
	{
		internal MysterySettings Settings;

		/// <summary>
		/// wraps SimHub.Logging.Current.Info() with prefix
		/// </summary>
		internal static bool Info(string str)
		{
			SimHub.Logging.Current.Info("Unexpected." + str);	// bool Info()
			return true;
		}

		private byte[] now;

		/// <summary>
		/// Instance of the current plugin manager
		/// </summary>
		public PluginManager PluginManager { get; set; }

		/// <summary>
		/// Called one time per game data update, contains all normalized game data,
		/// raw data are intentionnally "hidden" under a generic object type (A plugin SHOULD NOT USE IT)
		///
		/// This method is on the critical path, it must execute as fast as possible and avoid throwing any error
		///
		/// </summary>
		/// <param name="pluginManager"></param>
		/// <param name="data">Current game data, including current and previous data frame.</param>
		public void DataUpdate(PluginManager pluginManager, ref GameData data)
		{
			for (byte i = 0; i < now.Length; i++)
				if (now[i] != Settings.CCvalue[i])
				{
					Info($"DataUpdate(): CCvalue[{i}] changed from {now[i]} to {Settings.CCvalue[i]}");
					now[i] = Settings.CCvalue[i];
				}
		}

		/// <summary>
		/// Called at plugin manager stop, close/dispose anything needed here !
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void End(PluginManager pluginManager)
		{
			Settings.CCvalue[0] = 5;
			Settings.CCvalue[1] = 6;
			this.SaveCommonSettings("GeneralSettings", Settings);
		}

		/// Called at SimHub start then after game changes
		/// </summary>
		/// <param name="pluginManager"></param>
		public void Init(PluginManager pluginManager)
		{

			now = new byte[] { 0,0,0 };

// Load settings
			Settings = this.ReadCommonSettings<MysterySettings>("GeneralSettings", () => new MysterySettings());
			this.AttachDelegate("expect5", () => Settings.CCvalue[0]);
			this.AttachDelegate("expect6", () => Settings.CCvalue[1]);
			this.AttachDelegate("Unexpected.InitCount", () => Settings.CCvalue[2]);
			Settings.CCvalue[2]++;
			Settings.CCvalue[1] = Settings.CCvalue[1];
		}																			// Init()
	}
}
