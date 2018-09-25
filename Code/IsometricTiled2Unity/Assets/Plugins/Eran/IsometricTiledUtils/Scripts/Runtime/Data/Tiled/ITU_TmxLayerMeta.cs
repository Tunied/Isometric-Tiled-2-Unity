using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Eran.ITU.Data.Properties;

namespace Eran.ITU.Data.Tiled
{
    public class ITU_TmxLayerMeta
    {
        public string layerName;

        public int width;

        public int height;

        public float offsetX;

        public float offsetY;

        public List<ITU_TmxLayerChunkMeta> chunkList;

        public ITU_PropertieRootMeta property;

        public void Deserialize(XElement _element, bool _isInifite)
        {
            layerName = _element.Attribute("name").Value;

            width = Convert.ToInt32(_element.Attribute("width").Value);
            height = Convert.ToInt32(_element.Attribute("height").Value);

            offsetX = _element.Attribute("offsetx") != null ? Convert.ToSingle(_element.Attribute("offsetx").Value) : 0;
            offsetY = _element.Attribute("offsety") != null ? Convert.ToSingle(_element.Attribute("offsety").Value) : 0;

            chunkList = new List<ITU_TmxLayerChunkMeta>();
            //tmx format is different between inifite and not infinite
            if (_isInifite)
            {
                var allChunkEl = _element.Element("data").Elements("chunk");

                foreach (var el in allChunkEl)
                {
                    var chunkMeta = new ITU_TmxLayerChunkMeta();
                    chunkMeta.Deserialize(el);
                    chunkList.Add(chunkMeta);
                }
            }
            else
            {
                var chunkMeta = new ITU_TmxLayerChunkMeta();
                chunkMeta.startX = 0;
                chunkMeta.startY = 0;
                chunkMeta.width = width;
                chunkMeta.height = height;
                chunkMeta.DeserializeData(_element.Element("data").Value);
                chunkList.Add(chunkMeta);
            }

            if (_element.Element("properties") != null)
            {
                property = new ITU_PropertieRootMeta();
                property.Deserialize(_element.Element("properties"));
            }
        }
    }
}