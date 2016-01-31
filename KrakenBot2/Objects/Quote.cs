﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace KrakenBot2.Objects
{
    public class Quote
    {
        private string author;
        private int id;
        private string quoteContents;

        public string Author { get { return author; } }
        public int ID { get { return id; } }
        public string QuoteContents { get { return quoteContents; } }

        public Quote(JToken quoteProperties)
        {
            author = quoteProperties.SelectToken("author").ToString();
            id = int.Parse(quoteProperties.SelectToken("id").ToString());
            quoteContents = quoteProperties.SelectToken("contents").ToString();
            if(quoteContents.Contains('-'))
            {
                string newCnts = "";
                foreach(string section in quoteContents.Split('-'))
                {
                    if (section.ToLower() != " burkeblack" && section.ToLower() != " burke")
                        newCnts = section;
                }
                quoteContents = newCnts;
            }
        }
    }
}
