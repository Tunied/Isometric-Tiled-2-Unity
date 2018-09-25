using System;
using System.IO;
using Eran.ITU.Data.Tileset;
using UnityEditor;
using UnityEngine;

namespace Eran.ITU.Utils
{
    public class ITU_FIleUtils
    {
        /// <summary>
        /// Warp Function for CopyTilesetImage & SaveMaterialFromTemplate
        /// </summary>
        public static Material SaveTilsetImageAndMaterial(ITU_TsxRootMeta _tileset, Material _template, string _imgSaveToRelatePath, string _matSaveToRelatePath)
        {
            _imgSaveToRelatePath = Application.dataPath + _imgSaveToRelatePath;
            _matSaveToRelatePath = Application.dataPath + _matSaveToRelatePath;
            CopyTilesetImage(_tileset, _imgSaveToRelatePath);
            // Path.DirectorySeparatorChar
            var imgFullPath = _imgSaveToRelatePath + "/" + _tileset.imgNameWithExtension;
            var matFullPath = _matSaveToRelatePath + "/" + Path.GetFileNameWithoutExtension(_tileset.imgNameWithExtension) + ".mat";
            return SaveMaterialFromTemplate(_template, imgFullPath, matFullPath);
        }

        public static void DeleteNotUsedImageAndMaterial(ITU_TsxRootMeta _tileset, string _imgSaveToRelatePath, string _matSaveToRelatePath)
        {
            if (_tileset.isUesdInRender) return;
            _imgSaveToRelatePath = Application.dataPath + _imgSaveToRelatePath;
            _matSaveToRelatePath = Application.dataPath + _matSaveToRelatePath;
            var imgFullPath = _imgSaveToRelatePath + "/" + _tileset.imgNameWithExtension;
            var matFullPath = _matSaveToRelatePath + "/" + Path.GetFileNameWithoutExtension(_tileset.imgNameWithExtension) + ".mat";
            FileUtil.DeleteFileOrDirectory(imgFullPath);
            FileUtil.DeleteFileOrDirectory(matFullPath);
            AssetDatabase.Refresh();
        }


        /// <summary>
        /// Copy tileset image file to point out folder , and change the image import setting fit for tileset rendering
        /// </summary>
        public static void CopyTilesetImage(ITU_TsxRootMeta _tileset, string _saveToFolderFullPath)
        {
            var saveToFileFullPath = _saveToFolderFullPath + Path.DirectorySeparatorChar + _tileset.imgNameWithExtension;
            if (File.Exists(saveToFileFullPath))
            {
                File.Delete(saveToFileFullPath);
            }

            if (!Directory.Exists(_saveToFolderFullPath))
            {
                Directory.CreateDirectory(_saveToFolderFullPath);
            }

            FileUtil.CopyFileOrDirectory(_tileset.imgSourceFullPath, saveToFileFullPath);
            AssetDatabase.Refresh();

            var imgFilePath = FileUtil.GetProjectRelativePath(saveToFileFullPath);
            var importer = AssetImporter.GetAtPath(imgFilePath) as TextureImporter;
            var textureSettings = new TextureImporterSettings();
            importer.ReadTextureSettings(textureSettings);

            textureSettings.textureType = TextureImporterType.Default;
            textureSettings.textureShape = TextureImporterShape.Texture2D;
            textureSettings.streamingMipmaps = false;
            textureSettings.wrapMode = TextureWrapMode.Clamp;
            textureSettings.filterMode = FilterMode.Point;

            importer.SetTextureSettings(textureSettings);
            importer.SaveAndReimport();
        }


        /// <summary>
        /// Create a new material from template , and set the mainTexture with $textureImgPath, then save to $saveToPath
        /// </summary>
        /// <param name="_template">Material Template (new material clone from this)</param>
        /// <param name="_imgFullPath">full path of the img file, normally it came from ITU_TsxRootMeta.imgSourceFullPath</param>
        /// <param name="_saveToFullPath">full path that what you want save the material</param>
        public static Material SaveMaterialFromTemplate(Material _template, string _imgFullPath, string _saveToFullPath)
        {
            if (_template == null)
            {
                _template = new Material(Shader.Find("Unlit/Transparent"));
            }

            var imgFilePath = FileUtil.GetProjectRelativePath(_imgFullPath);
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(imgFilePath);
            if (texture == null)
            {
                throw new Exception("ITU->SaveMaterialFromTemplate can't find image file at : " + _imgFullPath + " project relative path is: " + imgFilePath);
            }

            var saveFileDicPath = Path.GetDirectoryName(_saveToFullPath);
            if (!Directory.Exists(saveFileDicPath))
            {
                Directory.CreateDirectory(saveFileDicPath);
            }

            if (File.Exists(_saveToFullPath))
            {
                var oldMat = AssetDatabase.LoadAssetAtPath<Material>(FileUtil.GetProjectRelativePath(_saveToFullPath));
                if (oldMat == null)
                {
                    throw new Exception("Can't load material at path : " + _saveToFullPath);
                }

                oldMat.CopyPropertiesFromMaterial(_template);
                oldMat.shader = _template.shader;
                oldMat.mainTexture = texture;
                AssetDatabase.SaveAssets();
                return oldMat;
            }
            else
            {
                var saveReltivePath = FileUtil.GetProjectRelativePath(_saveToFullPath);
                var mat = new Material(_template) {mainTexture = texture};

                AssetDatabase.CreateAsset(mat, saveReltivePath);
                AssetDatabase.SaveAssets();
                return mat;
            }
        }

        public static Mesh SaveMesh(Mesh _mesh, string _saveToRelativeFolderPath, string _fileName)
        {
            if (!_saveToRelativeFolderPath.StartsWith("/"))
            {
                _saveToRelativeFolderPath = "/" + _saveToRelativeFolderPath;
            }

            //Path.DirectorySeparatorChar
            var fileRelativePath = _saveToRelativeFolderPath + "/" + _fileName + ".asset";

            var dicFolder = Application.dataPath + _saveToRelativeFolderPath;
            if (!Directory.Exists(dicFolder))
            {
                Directory.CreateDirectory(dicFolder);
            }

            MeshUtility.Optimize(_mesh);
            AssetDatabase.CreateAsset(_mesh, "Assets" + fileRelativePath);
            AssetDatabase.SaveAssets();
            var loadMesh = AssetDatabase.LoadAssetAtPath<Mesh>("Assets" + fileRelativePath);
            if (loadMesh == null)
            {
                Debug.LogError("Can't load mesh at : Assets" + fileRelativePath);
            }

            return loadMesh;
        }

        public static GameObject SavePrefab(GameObject _obj, string _saveToRelativeFolderPath, string _fileName)
        {
            if (!_saveToRelativeFolderPath.StartsWith("/"))
            {
                _saveToRelativeFolderPath = "/" + _saveToRelativeFolderPath;
            }

            //Path.DirectorySeparatorChar
            var fileRelativePath = _saveToRelativeFolderPath + "/" + _fileName + ".prefab";

            var dicFolder = Application.dataPath + _saveToRelativeFolderPath;
            if (!Directory.Exists(dicFolder))
            {
                Directory.CreateDirectory(dicFolder);
            }

            if (File.Exists(Application.dataPath + fileRelativePath))
            {
                //Already have preafab , replace it
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets" + fileRelativePath);
                return PrefabUtility.ReplacePrefab(_obj, prefab, ReplacePrefabOptions.ReplaceNameBased);
            }
            else
            {
                //No prefab ,create new one
                return PrefabUtility.CreatePrefab("Assets" + fileRelativePath, _obj, ReplacePrefabOptions.Default);
            }
        }
    }
}