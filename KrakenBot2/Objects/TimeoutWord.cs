using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class TimeoutWord
    {
        private string word;
        private int seconds;

        public string Word { get { return word; } }
        public int Seconds { get { return seconds; } }

        public TimeoutWord(JToken data)
        {
            word = data.SelectToken("word").ToString();
            seconds = int.Parse(data.SelectToken("seconds").ToString());
        }
    }
}
