using MyVPN.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyVPN.MVVM.ViewModel
{
    internal class GlobalViewModel
    {
        public static GlobalViewModel Instance { get; set; } = new GlobalViewModel();

        public Settings Settings { get; set; }
        private bool test = false;

        public bool Test
        {
            get { return test; }
            set { test = value; }
        }


        public GlobalViewModel()
        {
            ReadSettings();
        }

        private void ReadSettings()
        {
            if (!File.Exists(StaticData.Instance.SettingsFilePath))
            {
                if (!Directory.Exists(StaticData.Instance.AppdataFolderPath)) Directory.CreateDirectory(StaticData.Instance.AppdataFolderPath);
                FileStream fs = new FileStream(StaticData.Instance.SettingsFilePath, FileMode.Create);
                WriteSettings(fs);
            }
            else
            {
                string settingsJson = File.ReadAllText(StaticData.Instance.SettingsFilePath);
                Settings = JsonSerializer.Deserialize<Settings>(settingsJson)!;
            }
        }

        public void WriteSettings()
        {
            string settingsJson = JsonSerializer.Serialize(Settings);
            File.WriteAllText(StaticData.Instance.SettingsFilePath, settingsJson);
        }

        public void WriteSettings(FileStream fs)
        {
            Settings = new Settings()
            {
                DisconnectOnClose = false,
                Password = "b6xnvt9",
                ShowTerminalWindowForProcess = false,
            };

            string settingsJson = JsonSerializer.Serialize(Settings);
            fs.Write(Encoding.ASCII.GetBytes(settingsJson));
        }


    }
}
