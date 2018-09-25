using System.Collections.Generic;
using System.Xml.Linq;

namespace Eran.ITU.Data.Properties
{
    public class ITU_PropertieRootMeta
    {
        private readonly List<ITU_EachPropertieMeta> properties = new List<ITU_EachPropertieMeta>();

        public void Deserialize(XElement _element)
        {
            foreach (var xNode in _element.Nodes())
            {
                var eachProperty = new ITU_EachPropertieMeta();
                eachProperty.Deserialize(xNode as XElement);
                properties.Add(eachProperty);
            }
        }

        public bool IsEmpty()
        {
            return properties.Count == 0;
        }

        public bool HasProperty(string _key)
        {
            return properties.Exists(x => x.key == _key);
        }

        public ITU_EachPropertieMeta GetProperty(string _key)
        {
            return properties.Find(x => x.key == _key);
        }
    }
}