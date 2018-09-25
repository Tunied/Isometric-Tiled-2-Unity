using System.IO;
using Eran.IsometricTiled2Unity.Example.Common.RunInEditor;
using Eran.ITU;
using Eran.ITU.Render;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_Nav_Prefab.Scripts
{
    public class ITU_Nav_Prefab_Demo : ITU_RunInEditorTemplate
    {
        public string txmFilePath = "/Plugins/Eran/IsometricTiled2Unity/Example/Res/Raw/Tiled/Maps/ITU_Nav_Prefab_Demo.tmx";
        public string saveProjectToPath = "/Plugins/Eran/IsometricTiled2Unity/Example/Res/InGame";

        public Material navTempBlockMaterial;

        public GameObject PrefabBoxDeco;
        public GameObject PrefabSupportDeco;

        public override void OnClickRunBtn()
        {
     
            var itu = new ITU_Facade();

            //Attach extension first
            var extNav = new ITU_GenNavMeshBlockExtension {navTempBlockMat = navTempBlockMaterial};
            itu.AddExtensions(extNav);

            var extPrefab = new ITU_ReplaceWithPrefabExtension();
            extPrefab.PrefabBoxDeco = PrefabBoxDeco;
            extPrefab.PrefabSupportDeco = PrefabSupportDeco;
            itu.AddExtensions(extPrefab);

            //Load Txm map
            itu.LoadTxmFile(Application.dataPath + txmFilePath);

            //Render
            var rednerSetting = new ITU_RenderSetting();
            rednerSetting.SetSaveToFolder(saveProjectToPath, Path.GetFileNameWithoutExtension(txmFilePath));
            rednerSetting.ignoreRenderLayerNameList.Add("Deco");
        
            itu.SetRenderSetting(rednerSetting);
            itu.RenderMapToCurrentScene();

            //Handle Property
            itu.HandleMapPropertyAndObject();
        }
    }
}