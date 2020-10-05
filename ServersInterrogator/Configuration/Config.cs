using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ServersInterrogator.Configuration
{
    [XmlRoot(ElementName = "Settings", Namespace = "urn:config-schema")]
    [Serializable]
    public class Config
    {
        public Config() { }

        [XmlElement(ElementName = "Setting")]
        public Setting[] Settings { get; set; }
    }
}
