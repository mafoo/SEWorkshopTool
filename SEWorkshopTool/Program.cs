using System;
using System.Reflection;
using Phoenix.WorkshopTool;

namespace Phoenix.SEWorkshopTool
{
    public class Program : ProgramBase
    {
        private static string m_gamePath;

        public static int Main(string[] args)
        {
            m_gamePath = AssemblyHelper.FindSteamGameRoot("SpaceEngineers");
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;

            try
            {
                var game = new SpaceGame();
                return game.InitGame(args);
            }
            catch
            {
                CheckForUpdate();
                throw;
            }
        }

        private static Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(
                AssemblyHelper.ResolvePathWithRoot(new AssemblyName(args.Name).Name, ".exe", m_gamePath));
        }

        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.LoadFrom(
                AssemblyHelper.ResolvePathWithRoot(new AssemblyName(args.Name).Name, ".dll", m_gamePath));
        }
    }
}