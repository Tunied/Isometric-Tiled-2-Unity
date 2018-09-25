using Eran.ITU.Data.Objects;
using Eran.ITU.Data.Properties;
using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Render;
using UnityEngine;

namespace Eran.ITU.Extension
{
    public class ITU_ExtensionBase
    {
        protected ITU_RenderSetting mRenderSetting;
        protected ITU_TmxRootMeta mMapMeta;

        public void SetTmxMapMeta(ITU_TmxRootMeta _mapMeta)
        {
            mMapMeta = _mapMeta;
        }

        public void SetRenderSetting(ITU_RenderSetting _renderSetting)
        {
            mRenderSetting = _renderSetting;
        }


        //------------------
        // Render
        //------------------

        public virtual void OnRenderTile(Vector2Int _tileIndex, Vector2 _tileTopCenterScenePos, float _tileWidthInUnit, float _tileHeightInUnit, ITU_TmxLayerMeta _layerMeta, ITU_TsxEachTileMeta _tileMeta)
        {
        }

        public virtual void OnRenderMapProcessBegin()
        {
        }

        public virtual void OnRenderMapProcessFinish(GameObject _mapGameObject)
        {
        }


        //-------------------
        // Property & Object
        //-------------------

        public virtual void HandleMapPropertyAndObjectBegin()
        {
        }

        public virtual void HandleMapPropertyAndObjectFinish()
        {
        }

        public virtual void HandleMapProperty(ITU_PropertieRootMeta _mapProperty)
        {
        }

        public virtual void HandleLayerProperty(ITU_TmxLayerMeta _layerMeta, ITU_PropertieRootMeta _layerProperty)
        {
        }

        public virtual void HandleMapObject(ITU_EachObjectMeta _objectMeta)
        {
        }

        public virtual void HandelTileWithProperty(Vector2Int _tileIndex, Vector2 _tileTopCenterScenePos,
            float _tileWidthInUnit, float _tileHeightInUnit,
            ITU_TmxLayerMeta _layerMeta,
            ITU_TsxEachTileMeta _tileMeta)
        {
        }
    }
}