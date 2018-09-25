using System.Collections.Generic;
using System.IO;
using Eran.ITU.Data.Tiled;
using Eran.ITU.Extension;
using Eran.ITU.Render.Layer;
using Eran.ITU.Utils;
using UnityEngine;

namespace Eran.ITU.Render
{
    public class ITU_TmxRender
    {
        public static int SORT_INDEX = 1;

        private ITU_TmxRootMeta mMapMeta;
        private ITU_RenderSetting mRenderSetting;

        private GameObject mTxmMapGo;

        private List<ITU_LayerRenderBasic> mAllLayerRenderList;

        public void InitTxmRender(ITU_TmxRootMeta _mapMeta, ITU_RenderSetting _renderSetting, List<ITU_ExtensionBase> _allExtensionList)
        {
            SORT_INDEX = 1;
            mAllLayerRenderList = new List<ITU_LayerRenderBasic>();

            mMapMeta = _mapMeta;
            mRenderSetting = _renderSetting;
            mTxmMapGo = new GameObject {name = _mapMeta.fileName};

            mMapMeta.tilsetList.ForEach(x => x.isUesdInRender = false);

            var nowOffsetZ = 0f;
            foreach (var layerMeta in _mapMeta.layerList)
            {
                if (_renderSetting.ignoreRenderLayerNameList.Contains(layerMeta.layerName)) continue;
                var render = new ITU_IsoLayerRender();
                render.InitRenderProperty(layerMeta, mMapMeta, mTxmMapGo, mRenderSetting, nowOffsetZ, _allExtensionList);
                nowOffsetZ -= mRenderSetting.layerOffsetZ;
                mAllLayerRenderList.Add(render);
            }
        }

        /// <summary>
        /// 将Txm地图渲染到 **当前** 场景中
        /// </summary>
        public GameObject RenderTxmToScene()
        {
            try
            {
                mAllLayerRenderList.ForEach(x => x.Render());
                return mTxmMapGo;
            }
            finally
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
            }
        }


        public void SaveMesh(string _saveToRelativeFolder, bool _isDeleteAllMeshUnderFolderFirst)
        {
            try
            {
                if (_isDeleteAllMeshUnderFolderFirst)
                {
                    var fullPath = Application.dataPath + _saveToRelativeFolder;
                    if (Directory.Exists(fullPath))
                    {
                        Directory.Delete(fullPath, true);
                    }

                    Directory.CreateDirectory(fullPath);
                }


                var meshList = mTxmMapGo.GetComponentsInChildren<MeshFilter>();
                var index = 0;
                var totalNum = meshList.Length;
                foreach (var meshFilter in meshList)
                {
                    index++;
#if UNITY_EDITOR
                    UnityEditor.EditorUtility.DisplayProgressBar("Save Mesh", string.Format("Saving mesh : {0}/{1}", index, totalNum), (float) index / totalNum);
#endif
                    meshFilter.sharedMesh = ITU_FIleUtils.SaveMesh(meshFilter.sharedMesh, _saveToRelativeFolder, mTxmMapGo.name + "_Mesh_" + index);
                }
#if UNITY_EDITOR
                UnityEditor.AssetDatabase.SaveAssets();
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            finally
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.ClearProgressBar();
#endif
            }
        }

        /// <summary>
        /// Save mTxmMapGo as a prefab,and return the Prefab reference
        /// </summary>
        /// <param name="_saveToRelativeFolder">the foler you want save, relate to project folder e.g "/Demo" </param>
        /// <returns></returns>
        public GameObject SavePrefab(string _saveToRelativeFolder)
        {
            return ITU_FIleUtils.SavePrefab(mTxmMapGo, _saveToRelativeFolder, mRenderSetting.prefabSaveName == null ? mTxmMapGo.name : mRenderSetting.prefabSaveName);
        }

        public void MakeAllStatic()
        {
            ITU_Utils.MakeAllSubGameObjectStatic(mTxmMapGo);
        }
    }
}