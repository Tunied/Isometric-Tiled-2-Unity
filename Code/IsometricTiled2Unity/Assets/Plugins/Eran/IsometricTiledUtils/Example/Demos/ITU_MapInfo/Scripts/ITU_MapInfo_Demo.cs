using System.IO;
using Eran.IsometricTiled2Unity.Example.Common.RunInEditor;
using Eran.ITU;
using Eran.ITU.Render;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_MapInfo.Scripts
{
    public class ITU_MapInfo_Demo : ITU_RunInEditorTemplate
    {
        public string txmFilePath = "/Plugins/Eran/IsometricTiled2Unity/Example/Res/Raw/Tiled/Maps/ITU_Basic_01.tmx";
        public string saveProjectToPath = "/Plugins/Eran/IsometricTiled2Unity/Example/Res/InGame";

        public override void OnClickRunBtn()
        {
            var setting = new ITU_RenderSetting();
            setting.SetSaveToFolder(saveProjectToPath, Path.GetFileNameWithoutExtension(txmFilePath));

            var itu = new ITU_Facade();

            //Attach extension first
            itu.AddExtensions(new ITU_ReadTxmMapInfoExtension());

            //Load Txm map
            itu.LoadTxmFile(Application.dataPath + txmFilePath);

            //Render
            itu.SetRenderSetting(setting);
            itu.RenderMapToCurrentScene();
        }
    }
}