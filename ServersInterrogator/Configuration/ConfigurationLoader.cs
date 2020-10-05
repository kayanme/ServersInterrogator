using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ServersInterrogator.Configuration
{
    public class ConfigurationLoader
    {
        private const string _configFileName = "ConfigurationFile.xml";
        private const string _schemaName = "ConfigurationFile.xsd";

        private List<string> _errors = new List<string>();
        public Config LoadConfig()
        {
            var path = Path.Combine(Environment.CurrentDirectory, _configFileName);

            if (!File.Exists(path))
            {
                Logger.Logger.Log.Error(String.Format(Properties.Resources.file_not_exists, path));
                throw new FileNotFoundException(_configFileName);
            }

            if (!IsConfigValid())
                throw new FileFormatException(_configFileName);
            

            var serializer = new XmlSerializer(typeof(Config));
            Config config;

            using (var reader = new StreamReader(File.OpenRead(path)))
            {
                config = serializer.Deserialize(reader) as Config;
            }
            return config;
        }

        private bool IsConfigValid()
        {
            var settings = GetXmlReaderSettings();
            var xmlReader = XmlReader.Create(_configFileName, settings);
            using(xmlReader)
            while (xmlReader.Read()) { }

            return !_errors.Any();
        }

        private XmlReaderSettings GetXmlReaderSettings()
        {
            var sc = new XmlSchemaSet();
            sc.Add("urn:config-schema", _schemaName);
            var settings = new XmlReaderSettings()
            {
                Schemas = sc,
                ValidationType = ValidationType.Schema,
            };
            settings.ValidationEventHandler += ValidationEventHandler;

            return settings;
        }

        private void ValidationEventHandler(object sender, ValidationEventArgs args)
        {
            Logger.Logger.Log.Error(args.Message);
            _errors.Add(args.Message);
        }
    }
}
