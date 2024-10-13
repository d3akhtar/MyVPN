using HtmlAgilityPack;
using MyVPN.Core;
using MyVPN.MVVM.Model;
using MyVPN.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MyVPN.MVVM.ViewModel
{
    internal class ProtectionViewModel : ObservableObject
    {
        public ObservableCollection<ServerModel> Servers { get; set; }
        private string _connectionStatus;
        private string _connectButtonContent = "Connect";

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance; // to access any global props

        public string ConnectButtonContent
        {
            get { return _connectButtonContent; }
            set { 
                _connectButtonContent = value;
                OnPropertyChanged();
            }
        }
        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set { 
                _connectionStatus = value; 
                OnPropertyChanged(); 
            }
        }
        private ServerModel _selectedServer;

        public ServerModel SelectedServer
        {
            get { return _selectedServer; }
            set { 
                _selectedServer = value;
                OnPropertyChanged();
            }
        }


        public RelayCommand ConnectCommand { get; set; }
        public ProtectionViewModel()
        {
            Servers = new();

            ConnectCommand = new RelayCommand(o =>
            {
                Task.Run(() =>
                {
                    if (ConnectButtonContent == "Connect")
                    {
                        ConnectionStatus = "Connecting...";
                        var process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        process.StartInfo.ArgumentList.Add(@$"/c rasdial MyServer vpnbook {Global.Settings.Password} /phonebook:./VPN/{SelectedServer.Address}.pbk");
                        //process.StartInfo.ArgumentList.Add(@"/c mkdir C:\Users\Danyal\Documents\random"); // random example

                        process.StartInfo.UseShellExecute = Global.Settings.ShowTerminalWindowForProcess;
                        process.StartInfo.CreateNoWindow = !Global.Settings.ShowTerminalWindowForProcess;

                        process.Start();
                        process.WaitForExit();

                        switch (process.ExitCode)
                        {
                            case 0:
                                Console.WriteLine("Success!");
                                ConnectionStatus = "Connected!";
                                ConnectButtonContent = "Disconnect";
                                break;
                            case 691:
                                Console.WriteLine("Wrong credentials!");
                                break;
                            default:
                                Console.WriteLine($"Unknown error, code: {process.ExitCode}");
                                break;
                        }
                    }
                    else if (ConnectButtonContent == "Disconnect")
                    {
                        ConnectionStatus = "Disconnecting...";
                        var process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        process.StartInfo.ArgumentList.Add(@"/c rasdial /d");
                        //process.StartInfo.ArgumentList.Add(@"/c mkdir C:\Users\Danyal\Documents\random"); // random example

                        process.StartInfo.UseShellExecute = Global.Settings.ShowTerminalWindowForProcess;
                        process.StartInfo.CreateNoWindow = !Global.Settings.ShowTerminalWindowForProcess;

                        process.Start();
                        process.WaitForExit();

                        switch (process.ExitCode)
                        {
                            case 0:
                                Console.WriteLine("Success!");
                                ConnectionStatus = "Disconnected!";
                                ConnectButtonContent = "Connect";
                                break;
                            case 691:
                                Console.WriteLine("Wrong credentials!");
                                break;
                            default:
                                Console.WriteLine($"Unknown error, code: {process.ExitCode}");
                                break;
                        }
                    }
                });
            }, o => SelectedServer != null);

            bool isConnected = CheckForConnection();
            if (isConnected){
                ConnectButtonContent = "Disconnect";
            }
            else{
                ConnectButtonContent = "Connect";
            }

            ServerBuilder();

            MainViewModel.OnWindowClose += MainView_OnWindowClose;
            SettingsViewModel.OnReloadPbkFilesButtonClicked += SettingsViewModel_OnReloadPbkFilesButtonClicked;
        }

        private void SettingsViewModel_OnReloadPbkFilesButtonClicked(object? sender, EventArgs e)
        {
            Servers = new();
            ServerBuilder();
            SelectedServer = null;
        }

        private void MainView_OnWindowClose(object? sender, EventArgs e)
        {
            if (Global.Settings.DisconnectOnClose)
            {
                var process = new Process();
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                process.StartInfo.ArgumentList.Add(@"/c rasdial /d");
                //process.StartInfo.ArgumentList.Add(@"/c mkdir C:\Users\Danyal\Documents\random"); // random example

                // prevent cmd window from popping up
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                process.WaitForExit();
            }
        }

        private async void ServerBuilder()
        {
            var addressesWithIndicator = await ScrapePptpVpnAddresses();

            foreach (string addressWithIndicator in addressesWithIndicator)
            {
                var address = addressWithIndicator.Substring(1).ToLower();
                var FolderPath = $"{Directory.GetCurrentDirectory()}/VPN";
                var pbkPath = $"{FolderPath}/{address}.pbk";

                Servers.Add(new ServerModel
                {
                    Address = address,
                    Country = address.Substring(0,2).ToUpper(),
                    FlagImagePath = GetCountryFlagIconPath(address),
                    OptimizedForWebSurfing = addressWithIndicator[0] == '1'
                });

                if (!Directory.Exists(FolderPath))
                {
                    Directory.CreateDirectory(FolderPath);
                }

                if (File.Exists(pbkPath))
                {
                    // MessageBox.Show("Connection already exists!");
                    continue;
                }

                var sb = new StringBuilder();
                sb.AppendLine("[MyServer]");
                sb.AppendLine("MEDIA=rastapi");
                sb.AppendLine("Port=VPN2-0");
                sb.AppendLine("Device=WAN Miniport (IKEv2)");
                sb.AppendLine("DEVICE=vpn");
                sb.AppendLine($"PhoneNumber={address}");
                File.WriteAllText(pbkPath, sb.ToString());
            }
        }

        private bool CheckForConnection()
        {
            var checkForConnection = new Process();
            checkForConnection.StartInfo.FileName = "cmd.exe";
            checkForConnection.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            checkForConnection.StartInfo.ArgumentList.Add(@"/c rasdial");
            checkForConnection.StartInfo.RedirectStandardOutput = true;
            checkForConnection.StartInfo.UseShellExecute = false;
            checkForConnection.StartInfo.CreateNoWindow = true;

            checkForConnection.Start();
            checkForConnection.WaitForExit();

            string output = checkForConnection.StandardOutput.ReadToEnd();
            return output.Contains("MyServer");
        }

        private string GetCountryFlagIconPath(string address)
        {
            string countryCode = address.Substring(0, 2).ToUpper();
            return StaticData.Instance.FlagImagePaths[countryCode];
        }

        private async Task<List<string>> ScrapePptpVpnAddresses()
        {
            List<string> addresses = new();

            var httpClient = new HttpClient();
            var htmlContent = await httpClient.GetStringAsync("https://www.vpnbook.com/freevpn");

            // Parse using HtmlAgility package
            var doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var addressNodes = doc.DocumentNode
                .SelectNodes("//ul[@class='disc']")
                .First()
                .ChildNodes;

            bool isWebSurfingOptimized = false;
            foreach (var addressNode in addressNodes)
            {
                var addressNodeHtml = addressNode.InnerHtml;
                if (addressNodeHtml.Contains("Username")) continue;
                if (addressNodeHtml.Contains("red")) isWebSurfingOptimized = true;
                int addressContainerTagStartIndex = addressNodeHtml.IndexOf("<strong>");
                if (addressContainerTagStartIndex != -1)
                {
                    int addressContainerTagEndIndex = addressNodeHtml.IndexOf("</strong>");
                    var address = (isWebSurfingOptimized == true ? "1":"0") + addressNodeHtml.Substring(
                        addressContainerTagStartIndex + 8, 
                        addressContainerTagEndIndex - addressContainerTagStartIndex - 8);
                    
                    addresses.Add(address);
                }
            }

            return addresses;
        }
    }
}
