using System.IO;
using Eran.ITU.Render;
using UnityEditor;
using UnityEngine;

namespace Eran.ITU.Editor
{
    public class ITU_ImportTxmWindow : EditorWindow
    {
        [MenuItem("Tools/IsometricTiled2Unity/Import Txm")]
        private static void Init()
        {
            var window = (ITU_ImportTxmWindow) GetWindow(typeof(ITU_ImportTxmWindow));
            window.Show();
        }

        private string tmxFilePath;
        private string saveProjectToPath;

        public int pixelPerUnit = 100;
        public int chunkSize = 10;
        public bool isStatic = true;
        public bool createInstance = true;
        public Material tileMaterial;
        public string[] ignoreLayers;
        public string sortLayerName = "ITU_Tiled";

        private void OnGUI()
        {
            GUILayout.Label("Import Txm to Unity", EditorStyles.largeLabel);

            bool isDrawNext;
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            isDrawNext = DrawChoiceTxmFileGUI();
            if (!isDrawNext) return;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            isDrawNext = DrawChoiceSaveToFolderGUI();
            if (!isDrawNext) return;

            EditorGUILayout.Space();
            EditorGUILayout.Space();
            isDrawNext = DrawRenderSettingGUI();
            if (!isDrawNext) return;

            GUILayout.FlexibleSpace();

            GUILayoutUtility.GetRect(20f, 20f);
            DrawRunButtonGUI();
            GUILayoutUtility.GetRect(20f, 20f);
        }

        //---------------------
        // Choice Txm Part
        //---------------------

        private bool DrawChoiceTxmFileGUI()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Txm File:  ", EditorStyles.largeLabel);
            if (GUILayout.Button("Open"))
            {
                OnClickChoiceTxmFile();
            }

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(tmxFilePath))
            {
                GUILayout.Label(tmxFilePath, EditorStyles.largeLabel);
                return true;
            }

            return false;
        }

        private void OnClickChoiceTxmFile()
        {
            var path = EditorUtility.OpenFilePanel("Open Txm File", "Assets/", "tmx");
            if (string.IsNullOrEmpty(path)) return;
            tmxFilePath = path;
        }

        //---------------------
        // Choice Save To Folder
        //---------------------


        private bool DrawChoiceSaveToFolderGUI()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("SaveTo Folder:  ", EditorStyles.largeLabel);
            if (GUILayout.Button("Choice"))
            {
                OnClickChoiceSaveFolder();
            }

            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(saveProjectToPath))
            {
                GUILayout.Label(saveProjectToPath, EditorStyles.largeLabel);
                return true;
            }

            return false;
        }

        private void OnClickChoiceSaveFolder()
        {
            var path = EditorUtility.SaveFolderPanel("Choice Folder", "Assets/", "");
            if (string.IsNullOrEmpty(path)) return;
            path = FileUtil.GetProjectRelativePath(path);
            if (string.IsNullOrEmpty(path)) return;
            saveProjectToPath = path;
        }

        //---------------------
        // Render Propery
        //---------------------

        private bool DrawRenderSettingGUI()
        {
            GUILayout.Label("Render Setting", EditorStyles.largeLabel);

            ScriptableObject target = this;
            var so = new SerializedObject(target);

            var pixelPerUnit = so.FindProperty("pixelPerUnit");
            EditorGUILayout.PropertyField(pixelPerUnit);

            var chunkSize = so.FindProperty("chunkSize");
            EditorGUILayout.PropertyField(chunkSize);

            var isStatic = so.FindProperty("isStatic");
            EditorGUILayout.PropertyField(isStatic);

            var createInstance = so.FindProperty("createInstance");
            EditorGUILayout.PropertyField(createInstance);

            var tileMaterial = so.FindProperty("tileMaterial");
            EditorGUILayout.PropertyField(tileMaterial);

            var sortLayerName = so.FindProperty("sortLayerName");
            EditorGUILayout.PropertyField(sortLayerName);

            var ignoreLayers = so.FindProperty("ignoreLayers");
            EditorGUILayout.PropertyField(ignoreLayers, true);

            so.ApplyModifiedProperties();

            return true;
        }


        //---------------------
        // Run
        //---------------------

        private void DrawRunButtonGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Run", GUILayout.Width(100), GUILayout.Height(50)))
            {
                OnClickRunBtn();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void OnClickRunBtn()
        {
            var setting = new ITU_RenderSetting
            {
                pixelsPreUnit = pixelPerUnit,
                renderChunkSize = chunkSize,
                defaultMaterial = tileMaterial == null ? new Material(Shader.Find("Unlit/Transparent")) : tileMaterial,
                isMakeAllGameObjectStatic = isStatic,
                isCreateInstanceOnSceneWhenFinish = createInstance,
                defaultSortLayerName = sortLayerName
            };

            var savePath = saveProjectToPath.Substring(6); //Remove first "Assets"
            setting.SetSaveToFolder(savePath, Path.GetFileNameWithoutExtension(tmxFilePath));

            if (ignoreLayers != null && ignoreLayers.Length > 0)
            {
                foreach (var ignoreLayer in ignoreLayers)
                {
                    setting.ignoreRenderLayerNameList.Add(ignoreLayer);
                }
            }

            var itu = new ITU_Facade();
            itu.LoadTxmFile(tmxFilePath);
            itu.SetRenderSetting(setting);
            itu.RenderMapToCurrentScene();
        }
    }
}