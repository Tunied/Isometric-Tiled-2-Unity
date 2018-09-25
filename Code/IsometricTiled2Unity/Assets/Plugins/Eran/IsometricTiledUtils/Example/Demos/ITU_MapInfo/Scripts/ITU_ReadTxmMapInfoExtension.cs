using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Extension;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_MapInfo.Scripts
{
    public class ITU_ReadTxmMapInfoExtension : ITU_ExtensionBase
    {
        private float minPosX = float.MaxValue;
        private float minPosY = float.MaxValue;

        private float maxPosX = float.MinValue;
        private float maxPosY = float.MinValue;

        public override void OnRenderTile(Vector2Int _tileIndex, Vector2 _tileTopCenterScenePos, float _tileWidthInUnit, float _tileHeightInUnit, ITU_TmxLayerMeta _layerMeta,
            ITU_TsxEachTileMeta _tileMeta)
        {
            var minX = _tileTopCenterScenePos.x - _tileWidthInUnit / 2f;
            var maxX = _tileTopCenterScenePos.x + _tileWidthInUnit / 2f;
            var maxY = _tileTopCenterScenePos.y;
            var minY = _tileTopCenterScenePos.y - _tileHeightInUnit;
            minPosX = Mathf.Min(minPosX, minX);
            minPosY = Mathf.Min(minPosY, minY);
            maxPosX = Mathf.Max(maxPosX, maxX);
            maxPosY = Mathf.Max(maxPosY, maxY);
        }


        public override void OnRenderMapProcessFinish(GameObject _mapGameObject)
        {
            var sp = _mapGameObject.AddComponent<TxmMapInfo>();
            sp.mapFileName = mMapMeta.fileName;
            sp.mapWidthInUnit = Mathf.Abs(maxPosX - minPosX);
            sp.mapHeightInUnit = Mathf.Abs(maxPosY - minPosY);
            sp.isStaggered = mMapMeta.IsIsoStaggered();
            sp.mapLeftTopPosV2 = new Vector2(minPosX, maxPosY);
            sp.mapRightBottomPosV2 = new Vector2(maxPosX, minPosY);
        }
    }
}