using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Extension;
using UnityEditor;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_Nav_Prefab.Scripts
{
    public class ITU_ReplaceWithPrefabExtension : ITU_ExtensionBase
    {
        public GameObject PrefabBoxDeco;
        public GameObject PrefabSupportDeco;

        private GameObject prefabDecoRoot;

        public override void HandleMapPropertyAndObjectBegin()
        {
            prefabDecoRoot = GameObject.Find("PrefabDecoRoot");
            if (prefabDecoRoot != null)
            {
                GameObject.DestroyImmediate(prefabDecoRoot);
            }

            prefabDecoRoot = new GameObject("PrefabDecoRoot");
        }

        public override void HandelTileWithProperty(Vector2Int _tileIndex, Vector2 _tileTopCenterScenePos, float _tileWidthInUnit, float _tileHeightInUnit, ITU_TmxLayerMeta _layerMeta,
            ITU_TsxEachTileMeta _tileMeta)
        {
            //Only replace tile in deco layer with prefab
            //the support deco is also used in Ground layer,and it will not replace.
            if (_layerMeta.layerName != "Deco") return;

            if (_tileMeta.property.HasProperty("IsDeco"))
            {
                var ID = _tileMeta.property.GetProperty("ID").value;
                var PrefabDeco = ID == "Box" ? PrefabBoxDeco : PrefabSupportDeco;
                var go = PrefabUtility.InstantiatePrefab(PrefabDeco) as GameObject;
                go.transform.parent = prefabDecoRoot.transform;
                go.transform.localEulerAngles = new Vector3(90, 0, 0);
                go.transform.position = new Vector3(_tileTopCenterScenePos.x, 1f, _tileTopCenterScenePos.y - +_tileHeightInUnit / 2f);
            }
        }
    }
}