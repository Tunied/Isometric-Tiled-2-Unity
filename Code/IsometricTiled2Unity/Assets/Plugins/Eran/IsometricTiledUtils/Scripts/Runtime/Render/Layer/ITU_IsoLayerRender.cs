using System;
using System.Collections.Generic;
using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Utils;
using UnityEngine;

namespace Eran.ITU.Render.Layer
{
    public class ITU_IsoLayerRender : ITU_LayerRenderBasic
    {
        private Mesh mMesh;
        private List<Vector3> mVertices;
        private List<int> mTriangles;
        private List<Vector2> mUVList;

        protected override void DoRenderChunk(GameObject _chunkGo, List<ITU_RenderTileData> _needRenderTileDataList)
        {
            var dic = new Dictionary<string, List<ITU_RenderTileData>>();
            _needRenderTileDataList.ForEach(data =>
            {
                List<ITU_RenderTileData> list;
                if (!dic.TryGetValue(data.tileMeta.tilesetName, out list))
                {
                    list = new List<ITU_RenderTileData>();
                    dic[data.tileMeta.tilesetName] = list;
                }

                list.Add(data);
            });

            foreach (var keyValuePair in dic)
            {
                var tilesetGo = new GameObject(keyValuePair.Key);
                tilesetGo.transform.parent = _chunkGo.transform;
                tilesetGo.transform.localPosition = Vector3.zero;

                Material mat;
                if (!mRenderSetting.tilesetMaterials.TryGetValue(keyValuePair.Value[0].tileMeta.tilesetName, out mat) || mat == null)
                {
                    throw new Exception("Can't render chunk because not found Material define (or it's null) in render setting for tileset : " + keyValuePair.Value[0].tileMeta.tilesetName);
                }

                DoRenderEachTileset(tilesetGo, keyValuePair.Value, mat);
            }
        }


        private void DoRenderEachTileset(GameObject _rootGo, List<ITU_RenderTileData> _needRenderTileDataList, Material _mat)
        {
            mMesh = new Mesh();
            mVertices = new List<Vector3>();
            mTriangles = new List<int>();
            mUVList = new List<Vector2>();

            _needRenderTileDataList.ForEach(data => { DoPushTile(data.x, data.y, data.tileMeta); });

            mMesh.vertices = mVertices.ToArray();
            mMesh.triangles = mTriangles.ToArray();
            mMesh.uv = mUVList.ToArray();

            var normal = new Vector3(0, 0, -1);
            var normals = new Vector3[mMesh.vertices.Length];
            for (var i = 0; i < normals.Length; i++)
            {
                normals[i] = normal;
            }

            mMesh.normals = normals;
            mMesh.RecalculateTangents();

            var filter = _rootGo.AddComponent<MeshFilter>();
            filter.mesh = mMesh;
            var redner = _rootGo.AddComponent<MeshRenderer>();
            redner.material = _mat;

            string sortLayerName;
            mRenderSetting.customSortLayerDic.TryGetValue(mLayerMeta.layerName, out sortLayerName);
            if (sortLayerName == null)
            {
                sortLayerName = mRenderSetting.defaultSortLayerName;
            }

            redner.sortingLayerName = sortLayerName;
            redner.sortingOrder = ITU_TmxRender.SORT_INDEX;
            ITU_TmxRender.SORT_INDEX++;
        }


        private void DoPushTile(int _posX, int _posY, ITU_TsxEachTileMeta _tileMeta)
        {
            var tileWidthInUnit = (float) mMapMeta.tileWidthInPixel / mRenderSetting.pixelsPreUnit;
            var tileHeighInUnit = (float) mMapMeta.tileHeightInPixel / mRenderSetting.pixelsPreUnit;
            var halfTileWidthInUnit = tileWidthInUnit / 2;

            var tilesetHeightInUnit = (float) _tileMeta.tileHeightInPixel / mRenderSetting.pixelsPreUnit;

            Vector2 topCenterPos;
            ITU_MathUtils.TileTopCorner2World(_posX, _posY, tileWidthInUnit, tileHeighInUnit,
                mMapMeta.orientation == ITU_TmxRootMeta.ORIENTAION_STAGGERED, out topCenterPos);
            var bottomCenterPos = new Vector2(topCenterPos.x, topCenterPos.y - tileHeighInUnit);


            var v2 = new Vector3(bottomCenterPos.x - halfTileWidthInUnit, bottomCenterPos.y, 0);
            var v1 = new Vector3(bottomCenterPos.x + halfTileWidthInUnit, bottomCenterPos.y + tilesetHeightInUnit, 0);
            var v0 = new Vector3(v2.x, v1.y, 0);
            var v3 = new Vector3(v1.x, v2.y, 0);

            mVertices.Add(v0);
            mUVList.Add(_tileMeta.uv_p0);
            var indexV0 = mVertices.Count - 1;

            mVertices.Add(v1);
            mUVList.Add(_tileMeta.uv_p1);
            var indexV1 = mVertices.Count - 1;

            mVertices.Add(v2);
            mUVList.Add(_tileMeta.uv_p2);
            var indexV2 = mVertices.Count - 1;

            mVertices.Add(v3);
            mUVList.Add(_tileMeta.uv_p3);
            var indexV3 = mVertices.Count - 1;

            mTriangles.Add(indexV0);
            mTriangles.Add(indexV1);
            mTriangles.Add(indexV2);

            mTriangles.Add(indexV2);
            mTriangles.Add(indexV1);
            mTriangles.Add(indexV3);

            var tileIndex = new Vector2Int(_posX, _posY);
            mAllExtensionList.ForEach(x => x.OnRenderTile(tileIndex, topCenterPos, tileWidthInUnit, tileHeighInUnit, mLayerMeta, _tileMeta));
        }
    }
}