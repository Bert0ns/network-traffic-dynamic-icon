namespace network_traffic_dynamic_icon.app
{
    internal static class AutoStartManager
    {
        public static string GetStartupShortcutPath()
        {
            var startup = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            return Path.Combine(startup, "NetworkTrafficDynamicIcon.lnk");
        }

        public static bool IsEnabled()
        {
            return File.Exists(GetStartupShortcutPath());
        }

        public static void Enable()
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule!.FileName!;
            var linkPath = GetStartupShortcutPath();

            // Usa COM WScript.Shell
            var type = Type.GetTypeFromProgID("WScript.Shell");
            if (type == null)
            {
                throw new InvalidOperationException("Impossibile ottenere il tipo WScript.Shell");
            }
            dynamic wsh = Activator.CreateInstance(type!);
            if (wsh == null)
            {
                throw new InvalidOperationException("Impossibile creare l'istanza di WScript.Shell");
            }
            var shortcut = wsh.CreateShortcut(linkPath);
            shortcut.TargetPath = exePath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(exePath);
            shortcut.WindowStyle = 1;
            shortcut.Description = "Avvio automatico Network Traffic Dynamic Icon";
            shortcut.Save();
        }

        public static void Disable()
        {
            var link = GetStartupShortcutPath();
            if (File.Exists(link))
            {
                File.Delete(link);
            }
        }
    }
}
