using System;
using System.Xml.Linq;
using Eran.ITU.Data.Properties;
using Eran.ITU.Data.Tileset;

namespace Eran.ITU.Data.Objects
{
    public class ITU_EachObjectMeta
    {
        public string layerName;
        public int id;
        public string name;
        public string type;
        public float x;
        public float y;
        public float width;
        public float height;
        public int gid;
        public string geometryRawValue;

        public ITU_TsxEachTileMeta tileMeta;
        public ITU_PropertieRootMeta property;

        public void Deserialize(XElement _element, string _layerName)
        {
            layerName = _layerName;
            id = Convert.ToInt32(_element.Attribute("id").Value);
            name = _element.Attribute("name") != null ? _element.Attribute("name").Value : null;
            type = _element.Attribute("type") != null ? _element.Attribute("type").Value : null;
            x = _element.Attribute("x") != null ? float.Parse(_element.Attribute("x").Value) : 0f;
            y = _element.Attribute("y") != null ? float.Parse(_element.Attribute("y").Value) : 0f;
            gid = _element.Attribute("gid") != null ? int.Parse(_element.Attribute("gid").Value) : 0;
            width = _element.Attribute("width") != null ? float.Parse(_element.Attribute("width").Value) : 0f;
            height = _element.Attribute("height") != null ? float.Parse(_element.Attribute("height").Value) : 0f;

            property = new ITU_PropertieRootMeta();
            foreach (var xNode in _element.Nodes())
            {
                var nodeEl = xNode as XElement;
                var nodeName = nodeEl.Name.LocalName;
                if (nodeName == "properties")
                {
                    property.Deserialize(nodeEl);
                }
                else
                {
                    geometryRawValue = nodeEl.Attribute("points") != null ? nodeEl.Attribute("points").Value : null;
                }
            }
        }
    }
}