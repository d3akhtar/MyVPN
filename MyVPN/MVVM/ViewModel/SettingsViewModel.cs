using MyVPN.Core;
using MyVPN.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVPN.MVVM.ViewModel
{
    internal class SettingsViewModel : ObservableObject
    {
        public static event EventHandler OnReloadPbkFilesButtonClicked;
        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;
        public RelayCommand ReloadPbkFiles { get; set; }
        private Settings _settings;

        public Settings Settings
        {
            get { return _settings; }
            set { 
                _settings = value;
                Global.Settings = value;
                OnPropertyChanged();
            }
        }


        public SettingsViewModel()
        {
            _settings = Global.Settings;

            ReloadPbkFiles = new RelayCommand(o =>
            {
                var folderPath = $"{Directory.GetCurrentDirectory()}/VPN";
                var pbkFiles = Directory.GetFiles(folderPath);
                foreach (var pbkFile in pbkFiles){
                    File.Delete(pbkFile);
                }
                OnReloadPbkFilesButtonClicked?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
