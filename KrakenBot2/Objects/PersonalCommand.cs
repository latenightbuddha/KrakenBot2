using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2.Objects
{
    public class PersonalCommand
    {
        private string username;
        private string data;

        public string Username { get { return username; } }
        public string Data { get { return data; } set { data = value; } }

        public PersonalCommand(string username, string data)
        {
            this.username = username;
            this.data = data;
        }
    }
}
