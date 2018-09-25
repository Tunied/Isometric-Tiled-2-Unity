using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Extension;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_Nav_Prefab.Scripts
{
    public class ITU_GenNavMeshBlockExtension : ITU_ExtensionBase
    {
        public Material navTempBlockMat;
        private GameObject mNavTempRootGo;

        public override void OnRenderMapProcessFinish(GameObject _mapGameObject)
        {
            //Roate the map to x-z plant
            _mapGameObject.transform.localEulerAngles = new Vector3(90f, 0, 0);

            mNavTempRootGo = GameObject.Find("NavTempRoot");
            if (mNavTempRootGo != null)
            {
                GameObject.DestroyImmediate(mNavTempRootGo);
            }

            mNavTempRootGo = new GameObject("NavTempRoot");
            //Kenney tilest is not regular 2:1 (e.g 64x32,256x128),it's 256x149,so the nav generate have some offset
            mNavTempRootGo.transform.position = new Vector3(0.07f, 0, 0.13f);
        }

        public override void HandelTileWithProperty(Vector2Int _tileIndex, Vector2 _tileTopCenterScenePos, float _tileWidthInUnit, float _tileHeightInUnit, ITU_TmxLayerMeta _layerMeta,
            ITU_TsxEachTileMeta _tileMeta)
        {
            if (_tileMeta.property.HasProperty("IsWalkable"))
            {
                var pos = new Vector3(_tileTopCenterScenePos.x, -0.2f, _tileTopCenterScenePos.y - _tileHeightInUnit / 2f);
                var navTile = ITU_MeshTileMaker.MakeTile(_tileWidthInUnit, _tileHeightInUnit, 0.2f, navTempBlockMat);
                navTile.name = "NavTempBlock";
                navTile.isStatic = true;
                navTile.transform.parent = mNavTempRootGo.transform;
                navTile.transform.localPosition = pos;
            }
        }
    }
}