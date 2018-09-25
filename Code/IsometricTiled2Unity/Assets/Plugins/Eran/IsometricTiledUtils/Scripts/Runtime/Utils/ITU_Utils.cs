using System;
using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using UnityEditor;
using UnityEngine;

namespace Eran.ITU.Utils
{
    public static class ITU_Utils
    {
        private const StaticEditorFlags staticFlag = StaticEditorFlags.BatchingStatic
                                                     | StaticEditorFlags.LightmapStatic
                                                     | StaticEditorFlags.OccludeeStatic
                                                     | StaticEditorFlags.OccluderStatic
                                                     | StaticEditorFlags.ReflectionProbeStatic
                                                     | StaticEditorFlags.OffMeshLinkGeneration;

        /// <summary>
        /// 根据在当前地图中指定的Gid,返回其所对应的TileMeta
        /// </summary>
        public static void GetTileMetaByGid(ITU_TmxRootMeta _mapMeta, int _gid, out ITU_TsxRootMeta _tilesetMeta, out ITU_TsxEachTileMeta _eachTileMeta)
        {
            foreach (var tilesetMeta in _mapMeta.tilsetList)
            {
                foreach (var tileMeta in tilesetMeta.eachTileMetaList)
                {
                    if (tileMeta.gid == _gid)
                    {
                        _tilesetMeta = tilesetMeta;
                        _eachTileMeta = tileMeta;
                        return;
                    }
                }
            }

            throw new Exception(string.Format("Can't find tile meta with gid {0} in Txm file {1}", _gid, _mapMeta.fileName));
        }

        /// <summary>
        /// 根据在当前地图中指定的Gid,返回其所对应的TileMeta
        /// </summary>
        public static ITU_TsxEachTileMeta GetTileMetaByGid(ITU_TmxRootMeta _mapMeta, int _gid)
        {
            foreach (var tilesetMeta in _mapMeta.tilsetList)
            {
                foreach (var tileMeta in tilesetMeta.eachTileMetaList)
                {
                    if (tileMeta.gid == _gid)
                    {
                        return tileMeta;
                    }
                }
            }

            throw new Exception(string.Format("Can't find tile meta with gid {0} in Txm file {1}", _gid, _mapMeta.fileName));
        }


        public static void MakeAllSubGameObjectStatic(GameObject _go)
        {
            //Not navigation
            GameObjectUtility.SetStaticEditorFlags(_go, staticFlag);
            var childNum = _go.transform.childCount;
            for (var index = 0; index < childNum; index++)
            {
                MakeAllSubGameObjectStatic(_go.transform.GetChild(index).gameObject);
            }
        }
    }
}