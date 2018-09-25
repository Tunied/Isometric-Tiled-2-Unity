using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace Eran.ITU.Data.Tileset
{
    public class ITU_TsxRootMeta
    {
        public string fileFullPath;
        public int firstGid;

        public string name;

        public int imgWidth;
        public int imgHeight;
        public string imgSourceFullPath; //e.g : /Users/eran/Desktop/Tiled/Tileset/Tileset01.tsx
        public string imgNameWithExtension; //e.g: Tileset01.png
        public string imgNameOnly;

        public int tileWidth;
        public int tileHeight;

        public int tileCount;
        public int columns;
        public int margin;
        public int spacing;

        /// <summary>
        /// 当前Tileset是否在渲染中被使用了,如果没有被使用则不会将Tileset相关的Material和Png图片拷贝到资源目录中去
        /// </summary>
        public bool isUesdInRender = false;

        /// <summary>
        /// 当前Tileset是否为Image合集,仅处理Property,不渲染到场景中
        /// Is Current tileset is a collections of image ,
        /// if so no tile will be rendered to the scence, only use this tileset to handle property
        /// </summary>
        public bool isImageCollectionAndNotRender = false;

        public List<ITU_TsxEachTileMeta> eachTileMetaList;


        public void Deserialize(XElement _element, string _tsxPath, int _fistGid)
        {
            fileFullPath = _tsxPath;
            firstGid = _fistGid;

            name = _element.Attribute("name").Value;
            tileWidth = Convert.ToInt32(_element.Attribute("tilewidth").Value);
            tileHeight = Convert.ToInt32(_element.Attribute("tileheight").Value);
            tileCount = Convert.ToInt32(_element.Attribute("tilecount").Value);
            columns = Convert.ToInt32(_element.Attribute("columns").Value);

            margin = _element.Attribute("margin") != null ? Convert.ToInt32(_element.Attribute("margin").Value) : 0;
            spacing = _element.Attribute("spacing") != null ? Convert.ToInt32(_element.Attribute("spacing").Value) : 0;


            var imgEl = _element.Element("image");

            if (imgEl != null)
            {
                isImageCollectionAndNotRender = false;
                var originalPath = Environment.CurrentDirectory;
                Environment.CurrentDirectory = Path.GetDirectoryName(fileFullPath);
                var imgFileInfo = new FileInfo(imgEl.Attribute("source").Value);
                imgSourceFullPath = imgFileInfo.FullName;
                imgNameWithExtension = imgFileInfo.Name;
                Environment.CurrentDirectory = originalPath;

                imgWidth = Convert.ToInt32(imgEl.Attribute("width").Value);
                imgHeight = Convert.ToInt32(imgEl.Attribute("height").Value);
            }
            else
            {
                isImageCollectionAndNotRender = true;
            }

            if (isImageCollectionAndNotRender)
            {
                HandleEachTileAsCollectionOfImages(_element);
            }
            else
            {
                HandleEachTileAsNormalTiset(_element);
            }
        }

        private void HandleEachTileAsNormalTiset(XElement _element)
        {
            eachTileMetaList = new List<ITU_TsxEachTileMeta>();
            for (var i = 0; i < tileCount; i++)
            {
                var tileMeta = new ITU_TsxEachTileMeta();
                tileMeta.tilesetName = name;

                tileMeta.gid = firstGid + i;
                tileMeta.subTileID = i;

                tileMeta.isImageCollectionAndNotRender = false;

                var gridX = i % columns;
                var gridY = Mathf.FloorToInt(i / columns);

                //Tiled坐标系转到Unity坐标系. 两个坐标系是相反的,Tiled左上到右下,Unity左下到右上 所以uv的v要用1减
                tileMeta.uv_p0 = new Vector2((float) (gridX * tileWidth + margin + gridX * spacing) / imgWidth, 1 - (float) (gridY * tileHeight + margin + gridY * spacing) / imgHeight);
                tileMeta.uv_p3 = new Vector2((float) ((gridX + 1) * tileWidth + margin + gridX * spacing) / imgWidth,
                    1 - (float) ((gridY + 1) * tileHeight + margin + gridY * spacing) / imgHeight);
                tileMeta.uv_p1 = new Vector2(tileMeta.uv_p3.x, tileMeta.uv_p0.y);
                tileMeta.uv_p2 = new Vector2(tileMeta.uv_p0.x, tileMeta.uv_p3.y);

                tileMeta.tileWidthInPixel = tileWidth;
                tileMeta.tileHeightInPixel = tileHeight;

                eachTileMetaList.Add(tileMeta);
            }

            var allTileElList = _element.Elements("tile");
            if (allTileElList != null)
            {
                foreach (var xTileNode in allTileElList)
                {
                    //Tileset可能还会有其他绑定数据,不能认为只要有tile节点则一定会有properties
                    if (xTileNode.Element("properties") != null)
                    {
                        var subTileID = Convert.ToInt32(xTileNode.Attribute("id").Value);
                        var tileMeta = eachTileMetaList.Find(x => x.subTileID == subTileID);
                        if (tileMeta == null)
                        {
                            Debug.LogError(string.Format("ITU try to handle properties but can't find id {0} in current tileset [NormalTilest]", subTileID));
                        }
                        else
                        {
                            tileMeta.property.Deserialize(xTileNode.Element("properties"));
                        }
                    }
                }
            }
        }

        private void HandleEachTileAsCollectionOfImages(XElement _element)
        {
            eachTileMetaList = new List<ITU_TsxEachTileMeta>();
            var allTileElList = _element.Elements("tile");
            foreach (var xTileNode in allTileElList)
            {
                var subTileID = Convert.ToInt32(xTileNode.Attribute("id").Value);
                var tileMeta = new ITU_TsxEachTileMeta();
                tileMeta.tilesetName = name;
                tileMeta.gid = firstGid + subTileID;
                tileMeta.subTileID = subTileID;
                tileMeta.isImageCollectionAndNotRender = true;
                eachTileMetaList.Add(tileMeta);

                //Tileset可能还会有其他绑定数据,不能认为只要有tile节点则一定会有properties
                if (xTileNode.Element("properties") != null)
                {
                    tileMeta.property.Deserialize(xTileNode.Element("properties"));
                }
            }

            if (eachTileMetaList.Count == 0)
            {
                Debug.LogWarning(string.Format("ITU find empty tileset {0} [Collecion of Image]", name));
            }
        }
    }
}