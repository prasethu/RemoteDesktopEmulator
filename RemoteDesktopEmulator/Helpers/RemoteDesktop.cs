using Microsoft.Win32;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RemoteDesktopEmulator
{
    internal class RemoteDesktop
    {
        private const string MCSKey     = "MonitorConfigSource";
        private const string MonitorKey = "Monitors";
        private const string TSCKeyPath = @"Software\Microsoft\Terminal Server Client";

        public static void Cleanup()
        {
            try
            {
                using (RegistryKey userHive = Registry.CurrentUser)
                using (RegistryKey tscKey = userHive.OpenSubKey(TSCKeyPath, writable: true))
                {
                    List<string> values = tscKey.GetValueNames().ToList();
                    if (values.Contains(MCSKey))
                        tscKey.DeleteValue(MCSKey);

                    List<string> subKeys = tscKey.GetSubKeyNames().ToList();
                    if (subKeys.Contains(MonitorKey))
                        tscKey.DeleteSubKeyTree(MonitorKey);
                }
            }
            catch
            {
                // Do nothing
            }
        }

        public static void Connect(Configuration configuration)
        {
            if (configuration == null)
                return;

            Cleanup();

            using (RegistryKey userHive = Registry.CurrentUser)
            using (RegistryKey tscKey = userHive.OpenSubKey(TSCKeyPath, writable: true))
            {
                tscKey.SetValue(MCSKey, 1);
                tscKey.CreateSubKey(@"Default\AddIns\RDPDR");

                int left   = GetInitialLeft(configuration.Displays);
                int top    = 0;
                int right  = 0;
                int bottom = 0;

                for (int ii = 0; ii < configuration.Displays.Count; ii++)
                {
                    Display display = configuration.Displays[ii];
                    Resolution resolution = display.Resolution;

                    right = left + resolution.Width - 1;
                    bottom = top + resolution.Height - 1;

                    using (RegistryKey monitorsKey = tscKey.CreateSubKey(MonitorKey))
                    using (RegistryKey monitorKey = monitorsKey.CreateSubKey(ii.ToString()))
                    {
                        monitorKey.SetValue("left", left);
                        monitorKey.SetValue("top", top);
                        monitorKey.SetValue("right", right);
                        monitorKey.SetValue("bottom", bottom);
                        monitorKey.SetValue("flags", display.IsPrimary ? 1 : 0);
                        monitorKey.SetValue("physicalHeight", resolution.Height / 5);
                        monitorKey.SetValue("physicalWidth", resolution.Width / 5);
                        monitorKey.SetValue("orientation", 0);
                        monitorKey.SetValue("desktopScaleFactor", display.Scale.Value);
                        monitorKey.SetValue("deviceScaleFactor", 0x000000b4);
                    }

                    left += resolution.Width;
                    right = left + resolution.Height;
                }
            }

            StartRemoteDesktopProcess();
        }

        private static int GetInitialLeft(IList<Display> displays)
        {
            if (displays == null)
                return 0;

            int calculatedLeft = 0;
            foreach (Display display in displays)
            {
                if (!display.IsPrimary)
                    calculatedLeft += display.Resolution.Width;
                else
                    break;
            }

            return -calculatedLeft;
        }

        private static void StartRemoteDesktopProcess()
        {
            Process mstsc = new Process();
            mstsc.StartInfo.FileName = "mstsc";
            mstsc.StartInfo.Arguments = "/multimon";
            mstsc.Start();
        }
    }
}
