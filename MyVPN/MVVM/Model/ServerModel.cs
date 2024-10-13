using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVPN.MVVM.Model
{
    internal class ServerModel
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string FlagImagePath { get; set; }
        public bool OptimizedForWebSurfing { get; set; }    
    }
}
