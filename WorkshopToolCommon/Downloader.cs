#if SE
using VRage.Compression;
#else
using VRage.Library.Compression;
#endif
using System.IO;
using System.Linq;
using Sandbox;
using VRage.FileSystem;
using VRage.GameServices;

namespace Phoenix.WorkshopTool
{
    internal class Downloader : IMod
    {
        private string m_extractPath;
        private ulong m_modId = 0;
        private string m_modPath;
        private string[] m_tags = new string[0];
        private string m_title;

        public Downloader(string extractPath, MyWorkshopItem item)
        {
            m_modId = item.Id;
            m_title = item.Title;
            m_modPath = item.Folder;
            m_extractPath = extractPath;
            m_tags = item.Tags.ToArray();
        }

        public string Title => m_title;

        public ulong ModId => m_modId;

        public string ModPath => m_modPath;

        public bool Extract()
        {
            var sanitizedTitle = Path.GetInvalidFileNameChars()
                .Aggregate(Title, (current, c) => current.Replace(c.ToString(), "_"));
            var dest = Path.Combine(m_extractPath,
                string.Format("{0} {1} ({2})", Constants.SEWT_Prefix, sanitizedTitle, m_modId.ToString()));

            MySandboxGame.Log.WriteLineAndConsole(string.Format("Extracting item: '{0}' to: \"{1}\"", m_title, dest));
            if (Directory.Exists(m_modPath))
                MyFileSystem.CopyAll(m_modPath, dest);
            else
                MyZipArchive.ExtractToDirectory(m_modPath, dest);

            return true;
        }
    }
}