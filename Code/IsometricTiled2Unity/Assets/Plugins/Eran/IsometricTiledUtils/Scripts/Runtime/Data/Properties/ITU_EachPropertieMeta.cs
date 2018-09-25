using System.Xml.Linq;

namespace Eran.ITU.Data.Properties
{
    public class ITU_EachPropertieMeta
    {
        public const string TYPE_STRING = "string";
        public const string TYPE_BOOL = "bool";
        public const string TYPE_COLOR = "color";
        public const string TYPE_FLOAT = "float";
        public const string TYPE_INT = "int";

        public string key;
        public string value;
        public string type;

        public void Deserialize(XElement _element)
        {
            key = _element.Attribute("name").Value;
            type = _element.Attribute("type") != null ? _element.Attribute("type").Value : TYPE_STRING;
            value = _element.Attribute("value").Value;
        }
    }
}