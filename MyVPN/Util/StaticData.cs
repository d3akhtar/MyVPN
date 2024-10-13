using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVPN.Util
{
    public class StaticData
    {
        public static StaticData Instance { get; set; } = new StaticData();
        private string flagImageFolderPath = "/Images/FlagIcons";
        public Dictionary<string, string> FlagImagePaths { get; set; }
        public string AppdataFolderPath;
        public string SettingsFilePath;

        public StaticData()
        {
            Trace.WriteLine("Loading settings from appdata...");

            FlagImagePaths = new Dictionary<string, string>()
            {
                {"US",$"{flagImageFolderPath}/usa.png" },
                {"DE",$"{flagImageFolderPath}/germany.png" },
                {"FR",$"{flagImageFolderPath}/france.png" },
                {"PL",$"{flagImageFolderPath}/poland.png" },
                {"UK",$"{flagImageFolderPath}/uk.png" },
                {"CA",$"{flagImageFolderPath}/canada.png" },
            };

            AppdataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MyVPN");
            SettingsFilePath = Path.Combine(AppdataFolderPath, "settings.json");

            Trace.WriteLine(SettingsFilePath);
        }
    }
}
