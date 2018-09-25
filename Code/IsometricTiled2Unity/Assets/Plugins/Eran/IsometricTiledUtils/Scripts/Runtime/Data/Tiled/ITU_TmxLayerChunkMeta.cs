using System;
using System.Xml.Linq;

namespace Eran.ITU.Data.Tiled
{
    public class ITU_TmxLayerChunkMeta
    {
        public int startX;
        public int startY;
        public int width;
        public int height;
        public int[] data;

        public void Deserialize(XElement _element)
        {
            startX = Convert.ToInt32(_element.Attribute("x").Value);
            startY = Convert.ToInt32(_element.Attribute("y").Value);
            width = Convert.ToInt32(_element.Attribute("width").Value);
            height = Convert.ToInt32(_element.Attribute("height").Value);
            DeserializeData(_element.Value);
        }

        public void DeserializeData(string _dataStr)
        {
            //需要去除回车换行符,然后按逗号分隔
            var dataStrArray = _dataStr.Replace("\n", "").Split(',');
            data = new int[dataStrArray.Length];
            for (var i = 0; i < dataStrArray.Length; i++)
            {
                data[i] = Convert.ToInt32(dataStrArray[i]);
            }
        }
    }
}