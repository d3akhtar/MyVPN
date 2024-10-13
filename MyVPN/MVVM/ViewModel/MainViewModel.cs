using MyVPN.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyVPN.MVVM.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public static event EventHandler OnWindowClose;

        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set {
                _currentView = value;
                OnPropertyChanged(); 
            }
        }

        public RelayCommand MoveWindowCommand { get; set; }
        public RelayCommand FullscreenCommand { get; set; }
        public RelayCommand MinimizeWindowCommand { get; set; }
        public RelayCommand CloseWindowCommand { get; set; }
        public RelayCommand ShowProtectionView { get; set; }
        public RelayCommand ShowSettingsView { get; set; }


        public ProtectionViewModel ProtectionViewModel { get; set; }
        public SettingsViewModel SettingsViewModel { get; set; }

        public MainViewModel()
        {
            ProtectionViewModel = new ProtectionViewModel();
            SettingsViewModel = new SettingsViewModel();
            CurrentView = ProtectionViewModel;

            Application.Current.MainWindow.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            
            MoveWindowCommand = new RelayCommand(o =>
            {
                Application.Current.MainWindow.DragMove();
            });
            FullscreenCommand = new RelayCommand(o =>
            {
                if (Application.Current.MainWindow.WindowState == WindowState.Maximized){
                    Application.Current.MainWindow.WindowState = WindowState.Normal;
                }
                else{
                    Application.Current.MainWindow.WindowState = WindowState.Maximized;
                }
            });
            MinimizeWindowCommand = new RelayCommand(o =>
            {
                Application.Current.MainWindow.WindowState = WindowState.Minimized;
            });
            CloseWindowCommand = new RelayCommand(o =>
            {
                OnWindowClose?.Invoke(this, EventArgs.Empty);
                GlobalViewModel.Instance.WriteSettings();
                Application.Current.Shutdown();
            });
            ShowProtectionView = new RelayCommand(o =>
            {
                CurrentView = ProtectionViewModel;
            });
            ShowSettingsView = new RelayCommand(o =>
            {
                CurrentView = SettingsViewModel;
            });
        }
    }
}
