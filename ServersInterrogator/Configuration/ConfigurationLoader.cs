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
                config = serializer.Deserialize(reader) as Config;//вообще здесь-то всё и провалидируется, причём по схеме из класса.
            }
            return config;//сразу return можно
        }

        private bool IsConfigValid()
        {
            var settings = GetXmlReaderSettings();//https://docs.microsoft.com/ru-ru/dotnet/api/system.xml.xmldocument.validate?view=netcore-3.1
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
