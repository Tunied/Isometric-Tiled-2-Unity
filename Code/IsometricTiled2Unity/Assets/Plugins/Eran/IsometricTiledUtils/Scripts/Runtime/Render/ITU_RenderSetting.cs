using System.Collections.Generic;
using UnityEngine;

namespace Eran.ITU.Render
{
    public class ITU_RenderSetting
    {
        public int pixelsPreUnit = 100;

        /// <summary>
        /// 渲染中忽略渲染的层级, 一般用于使用特殊Tile进行相关的标识操作. 比如单独建立一个NavLayer,然后标记出所有Walkable Area
        /// 此层级不需要进行渲染,只需要解析其内部的相关数据即可.
        /// </summary>
        public List<string> ignoreRenderLayerNameList = new List<string>();

        public int renderChunkSize = 10;

        public Dictionary<string, Material> tilesetMaterials = new Dictionary<string, Material>();

        /// <summary>
        /// 每个渲染层级Z值的偏移,用于渲染排序
        /// </summary>
        public float layerOffsetZ = 0.01f;

        public Material defaultMaterial = new Material(Shader.Find("Unlit/Transparent"));

        public bool isMakeAllGameObjectStatic = true;

        public bool isCreateInstanceOnSceneWhenFinish = true;

        public bool isDeleteAllMeshUnderFolderFirst = true;

        public string defaultSortLayerName = "ITU_Tiled";

        public Dictionary<string, string> customSortLayerDic = new Dictionary<string, string>();


        public string meshSaveToPath;
        public string prefabSaveToPath;

        /// <summary>
        /// If not set,the prefab will save as txmFileName
        /// </summary>
        public string prefabSaveName = null;


        public string imgSaveToPath;
        public string materailSaveToPath;

        public void SetSaveToFolder(string _path, string _txmFileName)
        {
            meshSaveToPath = _path + "/Maps/" + _txmFileName + "/Meshes";
            prefabSaveToPath = _path + "/Maps/" + _txmFileName + "/Prefab";
            imgSaveToPath = _path + "/Tilesets/Imgs";
            materailSaveToPath = _path + "/Tilesets/Materials";
        }
    }
}