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

		private void Attach(byte index)
		{
			switch (index)
			{
				case 0:
					this.AttachDelegate("expect5", () => Settings.CCvalue[0]);
					break;
				case 1:
					this.AttachDelegate("expect6", () => Settings.CCvalue[1]);
					break;
				case 2:
					this.AttachDelegate("Unexpected.InitCount", () => Settings.CCvalue[2]);
					break;
				default:
					Info($"Attach({index}): unsupported value");
					break;
			}
		}
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
			return;
		}

		/// <summary>
		/// Called at plugin manager stop, close/dispose anything needed here !
		/// Plugins are rebuilt at game change
		/// </summary>
		/// <param name="pluginManager"></param>
		public void End(PluginManager pluginManager)
		{
			string s = "End():\n";

			for (byte i = 0; i < now.Length; i++)
				if (now[i] != Settings.CCvalue[i])
				{
					s += $"\tCCvalue[{i}] changed from {now[i]} to {Settings.CCvalue[i]}\n";
					now[i] = Settings.CCvalue[i];
				}

			if (8 < s.Length)
				Info(s);

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
			Attach(0);
			Attach(1);
			Attach(2);
			Info($"Init() InitCount:  {++Settings.CCvalue[2]}");
			Settings.CCvalue[1] = Settings.CCvalue[1];								// matters in MIDIio; not here..??
		}																			// Init()
	}
}
