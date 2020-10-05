using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServersInterrogator.Configuration
{
    [XmlRoot("Settings")]
    [Serializable]
    public class Setting
    {
        public Setting() { }

        [XmlAttribute]
        public string Url { get; set; }

        [XmlAttribute]
        public int Threads { get; set; }

        [XmlAttribute]
        public int Interval { get; set; }
    }
}
