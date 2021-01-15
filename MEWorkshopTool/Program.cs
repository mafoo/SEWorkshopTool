using System;
using System.Linq;
using System.Reflection;
using Phoenix.WorkshopTool;

namespace Phoenix.MEWorkshopTool
{
    public class Program : ProgramBase
    {
        private static string m_gamePath;

        public static int Main(string[] args)
        {
            m_gamePath = AssemblyHelper.FindSteamGameRoot("MedievalEngineers");
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;

            if (args != null && args.Any(arg => arg == "--vrage-error-log-upload"))
            {
                return 0;
            }

            try
            {
                var game = new MedievalGame();
                var resultCode = game.InitGame(args);
                return resultCode;
            }
            catch
            {
                CheckForUpdate();
                throw;
            }
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(
                AssemblyHelper.ResolvePathWithRoot(new AssemblyName(args.Name).Name, ".dll", m_gamePath));
        }
    }
}