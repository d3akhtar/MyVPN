using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVPN.Util
{
    internal class Settings
    {
        public string Password { get; set; }
        public bool ShowTerminalWindowForProcess { get; set; }
        public bool DisconnectOnClose { get; set; }
    }
}
