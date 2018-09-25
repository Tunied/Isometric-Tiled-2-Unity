using Eran.ITU.Data.Properties;
using UnityEngine;

namespace Eran.ITU.Data.Tileset
{
    public class ITU_TsxEachTileMeta
    {
        //用于渲染时候引用相应的Material
        public string tilesetName;

        public int gid;

        //在当前Tileset中的ID,用于关联Property
        public int subTileID;

        public ITU_PropertieRootMeta property = new ITU_PropertieRootMeta();

        public int tileWidthInPixel;
        public int tileHeightInPixel;
        public Vector2 uv_p0;
        public Vector2 uv_p1;
        public Vector2 uv_p2;
        public Vector2 uv_p3;

        public bool isImageCollectionAndNotRender;
    }
}