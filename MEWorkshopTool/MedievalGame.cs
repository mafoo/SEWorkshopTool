using System.Reflection;
using Medieval;
using Phoenix.WorkshopTool;
using Sandbox;
using Sandbox.Engine.Platform;
using Sandbox.Game;
using VRage.Engine;
using VRage.Engine.Util;
using VRage.FileSystem;
using VRage.Logging;
using VRage.Plugins;

namespace Phoenix.MEWorkshopTool
{
    internal class MedievalGame : GameBase
    {
        protected override bool SetupBasicGameInfo()
        {
            MyInitializer.InvokeBeforeRun(AppId, MyPerGameSettings.BasicGameInfo.ApplicationName + "ModTool");

            if (!m_startup.Check64Bit()) return false;

            MyMedievalGame.SetupPerGameSettings();

            return true;
        }

        public override int InitGame(string[] args)
        {
            MyMedievalGame.SetupBasicGameInfo();
            m_startup = new MyCommonProgramStartup(args);
            MyFileSystem.Init(MyPerGameSettings.BasicGameInfo.ApplicationName);

            var appInformation = new AppInformation("Medieval Engineers", MyMedievalGame.ME_VERSION, "", "", "",
                MyMedievalGame.VersionString);
            var vrageCore = new VRageCore(appInformation, true);
            var configuration = CoreProgram.LoadParameters("MEWT.config");
            vrageCore.GetType().GetMethod("LoadSystems", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(vrageCore, new[] {configuration});
            vrageCore.GetType().GetField("m_state", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.SetValue(vrageCore, 1);
            vrageCore.GetType().GetMethod("InitSystems", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(vrageCore, new object[] {configuration.SystemConfiguration, true});
            vrageCore.GetType().GetMethod("LoadMetadata", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(vrageCore, new[] {configuration});
            vrageCore.GetType().GetMethod("InitSystems", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(vrageCore, new object[] {configuration.SystemConfiguration, false});

            m_steamService = new MySteamService();
            ((MySteamService) m_steamService).Init(new VRage.Steam.MySteamService.Parameters()
                {Server = Game.IsDedicated, AppId = AppId});

            MyLog.Default = MySandboxGame.Log = new MyLog();
            MySandboxGame.Log.Init(MyPerGameSettings.BasicGameInfo.ApplicationName + "ModTool.log", null);
            MyPlugins.Load();
            return base.InitGame(args);
        }

        protected override MySandboxGame InitGame()
        {
            return new MyMedievalGame();
        }
    }
}