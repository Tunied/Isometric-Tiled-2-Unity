using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Eran.ITU.Data.Objects;
using Eran.ITU.Data.Properties;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Utils;

namespace Eran.ITU.Data.Tiled
{
    public class ITU_TmxRootMeta
    {
        private const string NODE_TPYE_TILE_LAYER = "layer";
        private const string NODE_TYPE_GROUP = "group";
        private const string NODE_TYPE_OBJECT_LAYER = "objectgroup";
        private const string NODE_TYPE_MAP_PROPERTY = "properties";
        private const string NODE_TYPE_TILESET = "tileset";

        public const string ORIENTATION_ISOMETRIC = "isometric";
        public const string ORIENTAION_STAGGERED = "staggered";

        public bool isInifite;

        public string orientation;

        public string fileName;

        public int tileWidthInPixel;

        public int tileHeightInPixel;

        public int mapWidth;

        public int mapHeight;

        public readonly List<ITU_TmxLayerMeta> layerList = new List<ITU_TmxLayerMeta>();

        public readonly List<ITU_TsxRootMeta> tilsetList = new List<ITU_TsxRootMeta>();

        public readonly List<ITU_EachObjectMeta> objectList = new List<ITU_EachObjectMeta>();

        public ITU_PropertieRootMeta property;

        public bool IsIsoStaggered()
        {
            return orientation == ORIENTAION_STAGGERED;
        }

        public void Deserialize(XElement _element, string _tmxPath)
        {
            var originalPath = Environment.CurrentDirectory;
            try
            {
                var dicPath = Path.GetDirectoryName(_tmxPath);
                Environment.CurrentDirectory = dicPath;

                fileName = Path.GetFileNameWithoutExtension(_tmxPath);
                isInifite = _element.Attribute("infinite").Value == "1";
                tileWidthInPixel = Convert.ToInt32(_element.Attribute("tilewidth").Value);
                tileHeightInPixel = Convert.ToInt32(_element.Attribute("tileheight").Value);
                mapWidth = Convert.ToInt32(_element.Attribute("width").Value);
                mapHeight = Convert.ToInt32(_element.Attribute("height").Value);
                orientation = _element.Attribute("orientation").Value;

                foreach (var xNode in _element.Nodes())
                {
                    TryToHandelNode(xNode as XElement);
                }

                LinkTilesetObjectMeta();
            }
            finally
            {
                Environment.CurrentDirectory = originalPath;
            }
        }


        private void TryToHandelNode(XElement _node)
        {
            if (_node == null) return;
            var nodeName = _node.Name.LocalName;
            switch (nodeName)
            {
                case NODE_TPYE_TILE_LAYER:
                    HandleTileLayerNode(_node);
                    break;
                case NODE_TYPE_MAP_PROPERTY:
                    HandleMapPropertyNode(_node);
                    break;
                case NODE_TYPE_OBJECT_LAYER:
                    HandleObjectGroupNode(_node);
                    break;
                case NODE_TYPE_TILESET:
                    HandleTilesetNode(_node);
                    break;
                case NODE_TYPE_GROUP:
                    foreach (var xNode in _node.Nodes())
                    {
                        TryToHandelNode(xNode as XElement);
                    }

                    break;
            }
        }

        private void HandleMapPropertyNode(XElement _node)
        {
            property = new ITU_PropertieRootMeta();
            property.Deserialize(_node);
        }

        private void HandleTilesetNode(XElement _node)
        {
            var tsxFilePath = new FileInfo(_node.Attribute("source").Value).FullName;
            var tsxFile = XDocument.Load(tsxFilePath);
            var fistGid = Convert.ToInt32(_node.Attribute("firstgid").Value);

            var tileset = new ITU_TsxRootMeta();
            tileset.Deserialize(tsxFile.Root, tsxFilePath, fistGid);
            tilsetList.Add(tileset);
        }

        private void HandleTileLayerNode(XElement _node)
        {
            var layerMeta = new ITU_TmxLayerMeta();
            layerMeta.Deserialize(_node, isInifite);
            layerList.Add(layerMeta);
        }


        private void HandleObjectGroupNode(XElement _node)
        {
            var layerName = _node.Attribute("name").Value;
            foreach (var xNode in _node.Nodes())
            {
                var objectMeta = new ITU_EachObjectMeta();
                objectMeta.Deserialize(xNode as XElement, layerName);
                objectList.Add(objectMeta);
            }
        }


        private void LinkTilesetObjectMeta()
        {
            objectList.ForEach(x =>
            {
                if (x.gid != 0)
                {
                    x.tileMeta = ITU_Utils.GetTileMetaByGid(this, x.gid);
                }
            });
        }
    }
}