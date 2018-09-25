using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Eran.ITU.Data.Tiled;
using Eran.ITU.Extension;
using Eran.ITU.Render;
using Eran.ITU.Utils;
using UnityEditor;
using UnityEngine;

namespace Eran.ITU
{
    public class ITU_Facade
    {
        private ITU_RenderSetting mRenderSetting;
        private ITU_TmxRootMeta mMapMeta;
        private List<ITU_ExtensionBase> mAllExtensionList = new List<ITU_ExtensionBase>();


        public void AddExtensions(ITU_ExtensionBase _extension)
        {
            mAllExtensionList.Add(_extension);
        }

        public void SetRenderSetting(ITU_RenderSetting _setting)
        {
            mRenderSetting = _setting;
        }

        public ITU_TmxRootMeta LoadTxmFile(string _fileFullPath)
        {
            var tmxFile = XDocument.Load(_fileFullPath);
            mMapMeta = new ITU_TmxRootMeta();
            mMapMeta.Deserialize(tmxFile.Root, _fileFullPath);
            return mMapMeta;
        }

        public void RenderMapToCurrentScene()
        {
            if (mMapMeta == null)
            {
                Debug.LogError("ITU_Facade not load any map, please call LoadTxmFile function first");
                return;
            }


            try
            {
                if (mRenderSetting == null)
                {
                    Debug.LogWarning("not set rendering setting , use default rending setting inside");
                    mRenderSetting = new ITU_RenderSetting();
                    mRenderSetting.SetSaveToFolder("/ITU_DefaultProject", mMapMeta.fileName);
                }

                if (mRenderSetting.renderChunkSize <= 0)
                {
                    mRenderSetting.renderChunkSize = 10;
                    Debug.LogWarning("render setting -> renderChunkSize size <=0  set as default value 10");
                }

                if (mRenderSetting.pixelsPreUnit <= 0)
                {
                    mRenderSetting.pixelsPreUnit = 100;
                    Debug.LogWarning("render setting -> pixelsPreUnit unit <=0 set as default value 100");
                }


                if (mMapMeta.orientation != ITU_TmxRootMeta.ORIENTAION_STAGGERED && mMapMeta.orientation != ITU_TmxRootMeta.ORIENTATION_ISOMETRIC)
                {
                    Debug.LogError(string.Format("Can't render map {0}, beacuse is not Isometric or Staggered. ITU only support those two type right now :( ", mMapMeta.fileName));
                    return;
                }

                if (SortingLayer.NameToID(mRenderSetting.defaultSortLayerName) == 0)
                {
                    foreach (var layerMeta in mMapMeta.layerList)
                    {
                        var layerName = layerMeta.layerName;
                        string sortLayerName;
                        var isFound = false;
                        if (mRenderSetting.customSortLayerDic.TryGetValue(layerName, out sortLayerName))
                        {
                            if (SortingLayer.NameToID(sortLayerName) != 0)
                            {
                                isFound = true;
                            }
                        }

                        if (!isFound)
                        {
                            Debug.LogError(string.Format(
                                "Can't find default sort layer <color=red> {1} </color> and no sortLayer define for Tiled layer <color=red> {0} </color>," +
                                "please first define it in <color=red>Edit -> Project Settings -> Tags and Layers</color> " +
                                "check the document under $ITU_Plugin_Folder/Document if you need more help",
                                layerName, mRenderSetting.defaultSortLayerName));
                            return;
                        }
                    }
                }


                mAllExtensionList.ForEach(x =>
                {
                    x.SetRenderSetting(mRenderSetting);
                    x.SetTmxMapMeta(mMapMeta);
                });


                var render = new ITU_TmxRender();
                render.InitTxmRender(mMapMeta, mRenderSetting, mAllExtensionList);

                mMapMeta.tilsetList.ForEach(tileset =>
                {
                    if (!tileset.isUesdInRender || tileset.isImageCollectionAndNotRender) return;

                    Material matTemplate;
                    if (!mRenderSetting.tilesetMaterials.TryGetValue(tileset.name, out matTemplate))
                    {
                        matTemplate = mRenderSetting.defaultMaterial;
                    }

                    var mat = ITU_FIleUtils.SaveTilsetImageAndMaterial(tileset, matTemplate,
                        mRenderSetting.imgSaveToPath,
                        mRenderSetting.materailSaveToPath);

                    mRenderSetting.tilesetMaterials.Add(tileset.name, mat);
                });

                mAllExtensionList.ForEach(x => x.OnRenderMapProcessBegin());
                var go = render.RenderTxmToScene();
                mAllExtensionList.ForEach(x => x.OnRenderMapProcessFinish(go));


                //Save Mesh
                render.SaveMesh(mRenderSetting.meshSaveToPath, mRenderSetting.isDeleteAllMeshUnderFolderFirst);

                if (mRenderSetting.isMakeAllGameObjectStatic)
                {
                    render.MakeAllStatic();
                }

                //Save Prefab
                var prefab = render.SavePrefab(mRenderSetting.prefabSaveToPath);

                //Delte templet go
                GameObject.DestroyImmediate(go);

                if (mRenderSetting.isCreateInstanceOnSceneWhenFinish)
                {
                    PrefabUtility.InstantiatePrefab(prefab);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch (Exception _e)
            {
                var msg = "Oops ,seemed there have been some error when you run ITU, if you need some help you can contact me via iamzeaotwang@126.com";
                Debug.LogError(msg);
                Debug.LogError(_e);
                return;
            }

            Debug.Log(string.Format("<color=green>ITU import {0} succeed! </color>", mMapMeta.fileName));
        }


        public void HandleMapPropertyAndObject()
        {
            if (mMapMeta == null)
            {
                Debug.LogError("ITU_Facade not load any map, please call LoadTxmFile function first");
                return;
            }

            //Begin
            mAllExtensionList.ForEach(x =>
            {
                x.SetRenderSetting(mRenderSetting);
                x.SetTmxMapMeta(mMapMeta);
                x.HandleMapPropertyAndObjectBegin();
            });

            //Map
            mAllExtensionList.ForEach(x =>
            {
                if (mMapMeta.property != null)
                {
                    x.HandleMapProperty(mMapMeta.property);
                }
            });

            //Layer
            mAllExtensionList.ForEach(x =>
            {
                mMapMeta.layerList.ForEach(layer =>
                {
                    if (layer.property != null)
                    {
                        x.HandleLayerProperty(layer, layer.property);
                    }
                });
            });

            //Object
            mAllExtensionList.ForEach(x => { mMapMeta.objectList.ForEach(x.HandleMapObject); });

            var pixelsPreUnit = mRenderSetting != null ? mRenderSetting.pixelsPreUnit : 100;
            var tileWidthInUnit = (float) mMapMeta.tileWidthInPixel / pixelsPreUnit;
            var tileHeightInUnit = (float) mMapMeta.tileHeightInPixel / pixelsPreUnit;

            //Tile
            mAllExtensionList.ForEach(x =>
            {
                mMapMeta.layerList.ForEach(layer =>
                {
                    layer.chunkList.ForEach(chunk =>
                    {
                        for (var index = 0; index < chunk.data.Length; index++)
                        {
                            var gid = chunk.data[index];
                            //为0的砖块则表示不需要渲染
                            if (gid == 0) continue;
                            var tileMeta = ITU_Utils.GetTileMetaByGid(mMapMeta, gid);
                            if (tileMeta != null && !tileMeta.property.IsEmpty())
                            {
                                var tileX = chunk.startX + index % chunk.width;
                                var tileY = chunk.startY + (int) Mathf.Ceil(index / chunk.width);
                                Vector2 topPos;
                                ITU_MathUtils.TileTopCorner2World(tileX, tileY, tileWidthInUnit, tileHeightInUnit, mMapMeta.IsIsoStaggered(), out topPos);
                                x.HandelTileWithProperty(new Vector2Int(tileX, tileY), topPos, tileWidthInUnit, tileHeightInUnit, layer, tileMeta);
                            }
                        }
                    });
                });
            });

            //Finish
            mAllExtensionList.ForEach(x => { x.HandleMapPropertyAndObjectFinish(); });
        }
    }
}