using System;
using System.Collections.Generic;
using Eran.ITU.Data.Tiled;
using Eran.ITU.Data.Tileset;
using Eran.ITU.Extension;
using Eran.ITU.Utils;
using UnityEngine;

namespace Eran.ITU.Render.Layer
{
    public abstract class ITU_LayerRenderBasic
    {
        protected ITU_TmxLayerMeta mLayerMeta;
        protected ITU_TmxRootMeta mMapMeta;

        protected ITU_RenderSetting mRenderSetting;
        protected List<ITU_RenderTileData> mAllNeedRenderTileList;
        protected Vector2 mLeftTopCornerBound;
        protected Vector2 mRightBottomCornerBound;

        protected List<ITU_ExtensionBase> mAllExtensionList;
        protected GameObject mLayerRootGo;

        private int mMaxMayRenderChunkNum;
        private int mFinishTryRenderChunkNum;


        public void InitRenderProperty(ITU_TmxLayerMeta _layerMeta, ITU_TmxRootMeta _mapMeta, GameObject _mapRootGo, ITU_RenderSetting _renderSetting, float _offsetZ,
            List<ITU_ExtensionBase> _allExtensionList)
        {
            mLayerMeta = _layerMeta;
            mMapMeta = _mapMeta;
            mRenderSetting = _renderSetting;
            mAllExtensionList = _allExtensionList;

            mLayerRootGo = new GameObject(mLayerMeta.layerName);
            mLayerRootGo.transform.parent = _mapRootGo.transform;
            mLayerRootGo.transform.localPosition = new Vector3(mLayerMeta.offsetX / mRenderSetting.pixelsPreUnit, -mLayerMeta.offsetY / mRenderSetting.pixelsPreUnit, _offsetZ);

            DoInitRenderProperty();
        }

        public void Render()
        {
            if (mAllNeedRenderTileList.Count > 0)
            {
                //忽略空层级
                DoRenderLayer();
            }
        }

        private void DoInitRenderProperty()
        {
            var minPosX = float.MaxValue;
            var minPosY = float.MaxValue;

            var maxPosX = float.MinValue;
            var maxPosY = float.MinValue;

            var tileWidthInUnit = (float) mMapMeta.tileWidthInPixel / mRenderSetting.pixelsPreUnit;
            var tileHeightInUnit = (float) mMapMeta.tileHeightInPixel / mRenderSetting.pixelsPreUnit;


            mAllNeedRenderTileList = new List<ITU_RenderTileData>();
            mLayerMeta.chunkList.ForEach(chunk =>
            {
                for (var index = 0; index < chunk.data.Length; index++)
                {
                    var gid = chunk.data[index];
                    //为0的砖块则表示不需要渲染
                    if (gid == 0) continue;

                    var renderTileData = new ITU_RenderTileData();
                    renderTileData.x = chunk.startX + index % chunk.width;
                    renderTileData.y = chunk.startY + (int) Mathf.Ceil(index / chunk.width);

                    ITU_TsxRootMeta _tilesetMeta;
                    ITU_TsxEachTileMeta _eachTileMeta;
                    ITU_Utils.GetTileMetaByGid(mMapMeta, gid, out _tilesetMeta, out _eachTileMeta);

                    _tilesetMeta.isUesdInRender = true;
                    renderTileData.tileMeta = _eachTileMeta;
                    mAllNeedRenderTileList.Add(renderTileData);

                    Vector2 nowPos;
                    ITU_MathUtils.TileTopCorner2World(renderTileData.x, renderTileData.y, tileWidthInUnit, tileHeightInUnit, mMapMeta.IsIsoStaggered(), out nowPos);

                    minPosX = Mathf.Min(minPosX, nowPos.x);
                    minPosY = Mathf.Min(minPosY, nowPos.y);
                    maxPosX = Mathf.Max(maxPosX, nowPos.x);
                    maxPosY = Mathf.Max(maxPosY, nowPos.y);
                }
            });

            //左上和右下两个Bound是左边点向两边个扩充2.5个Tile,.5是为了让坐标点在中心. 2个Tile是增大Bound防止周围Tile未渲染
            //因为后续渲染会检测空Tile,所以区间扩充大一些没有关系
            mLeftTopCornerBound = new Vector2(minPosX - tileWidthInUnit * 2.5f, maxPosY + tileHeightInUnit * mRenderSetting.renderChunkSize * 2.5f);
            mRightBottomCornerBound = new Vector2(maxPosX + tileWidthInUnit * 2.5f, minPosY - tileHeightInUnit * mRenderSetting.renderChunkSize * 2.5f);


            //此值不准确,只是为了大概显示一下进度条,防止界面卡顿导致以为是死机
            var xNum = Mathf.CeilToInt(Math.Abs(mRightBottomCornerBound.x - mLeftTopCornerBound.x) / (mRenderSetting.renderChunkSize * tileWidthInUnit));
            var yNum = Mathf.CeilToInt(Math.Abs(mRightBottomCornerBound.y - mLeftTopCornerBound.y) / (mRenderSetting.renderChunkSize * tileHeightInUnit * 0.5f));
            mMaxMayRenderChunkNum = (xNum + 4) * (yNum + 4);
        }


        private void DoRenderLayer()
        {
            //以最小点向左上移动两个Chunk大小区域为起点,逐行扫描每个Chunk,直到Chunk的TopTile 大于MaxTile为止
            //这样进行Chunk渲染保证了正确的渲染顺序,右压左,下压上.(每个Chunk之内的Tile渲染也需要按此顺序)

            var tileWidthInUnit = (float) mMapMeta.tileWidthInPixel / mRenderSetting.pixelsPreUnit;
            var tileHeightInUnit = (float) mMapMeta.tileHeightInPixel / mRenderSetting.pixelsPreUnit;


            var chunkIndex = 0;
            var renderStep = mRenderSetting.renderChunkSize;
            var isStaggered = mMapMeta.IsIsoStaggered();

            Vector2Int startPos;
            ITU_MathUtils.World2Tile(mLeftTopCornerBound.x, mLeftTopCornerBound.y, tileWidthInUnit, tileHeightInUnit, mMapMeta.IsIsoStaggered(), out startPos);
            //IsometricMathUtils.GetNowTileLeftTopTileWithStep(startPos.x, startPos.y, renderStep * 2, isStaggered, out startPos);
            var isTurnLeft = true;

            var lineFistTile = startPos;
            var nowPointTile = lineFistTile;

            while (true)
            {
                //渲染一行
                while (true)
                {
                    //nowPointTile默认指向的是当前渲染Chunk的TopTile
                    //此Tile向左下方移动chunkSize大小则为当前Chunk的最左侧Tile
                    Vector2Int chunkMinLeftTile;
                    ITU_MathUtils.GetNowTileLeftBottomTileWithStep(nowPointTile.x, nowPointTile.y, renderStep, isStaggered, out chunkMinLeftTile);
                    Vector2 chunkCheckPos;
                    ITU_MathUtils.TileTopCorner2World(chunkMinLeftTile.x, chunkMinLeftTile.y, tileWidthInUnit, tileHeightInUnit, isStaggered, out chunkCheckPos);
                    //应该是上角点减去 tileWidthInUnit/2即可.此处只是为了容错.
                    if (chunkCheckPos.x - tileWidthInUnit > mRightBottomCornerBound.x)
                    {
                        break;
                    }

                    var isDrawChunk = TryToRenderChunk(nowPointTile, mRenderSetting.renderChunkSize, chunkIndex);
                    chunkIndex = isDrawChunk ? chunkIndex + 1 : chunkIndex;
                    ITU_MathUtils.GetNowTileRightTileWithStep(nowPointTile.x, nowPointTile.y, renderStep, isStaggered, out nowPointTile);
                }

                //移动到下一行
                if (isTurnLeft)
                {
                    ITU_MathUtils.GetNowTileLeftBottomTileWithStep(lineFistTile.x, lineFistTile.y, renderStep, isStaggered, out lineFistTile);
                }
                else
                {
                    ITU_MathUtils.GetNowTileRightBottomTileWithStep(lineFistTile.x, lineFistTile.y, renderStep, isStaggered, out lineFistTile);
                }

                nowPointTile = lineFistTile;
                isTurnLeft = !isTurnLeft;

                Vector2 bottomCheckPos;
                ITU_MathUtils.TileTopCorner2World(nowPointTile.x, nowPointTile.y, tileWidthInUnit, tileHeightInUnit, isStaggered, out bottomCheckPos);

                //Unity坐标是反向的,所以要反向检测
                if (bottomCheckPos.y < mRightBottomCornerBound.y)
                {
                    break;
                }
            }
        }


        private bool TryToRenderChunk(Vector2Int _topTile, int _chunkSize, int _chunkIndex)
        {
#if UNITY_EDITOR
            mFinishTryRenderChunkNum++;
            if (mFinishTryRenderChunkNum > mMaxMayRenderChunkNum)
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Render Layer " + mLayerMeta.layerName,
                    string.Format("Please wait.... [{1}] {0}", mFinishTryRenderChunkNum, _chunkIndex), 1f);
            }
            else
            {
                UnityEditor.EditorUtility.DisplayProgressBar("Render Layer " + mLayerMeta.layerName,
                    string.Format("Please wait.... [{2}] {0}/{1}", mFinishTryRenderChunkNum, mMaxMayRenderChunkNum, _chunkIndex),
                    (float) mFinishTryRenderChunkNum / mMaxMayRenderChunkNum);
            }
#endif
            var renderTileList = new List<ITU_RenderTileData>();
            var lineFistTile = _topTile;

            var lineTileNum = 1;
            for (var i = 0; i < _chunkSize; i++)
            {
                TryToPushLineRenderTileData(lineFistTile, lineTileNum, renderTileList);
                ITU_MathUtils.GetNowTileLeftBottomTileWithStep(lineFistTile.x, lineFistTile.y, 1, mMapMeta.IsIsoStaggered(), out lineFistTile);
                lineTileNum++;
            }

            //For循环最后一次时候向左下移动了一格,所以进入底部渲染时候需要先向右侧移动一格
            ITU_MathUtils.GetNowTileRightTileWithStep(lineFistTile.x, lineFistTile.y, 1, mMapMeta.IsIsoStaggered(), out lineFistTile);

            lineTileNum = _chunkSize - 1;
            for (var i = 0; i < _chunkSize - 1; i++)
            {
                TryToPushLineRenderTileData(lineFistTile, lineTileNum, renderTileList);
                ITU_MathUtils.GetNowTileRightBottomTileWithStep(lineFistTile.x, lineFistTile.y, 1, mMapMeta.IsIsoStaggered(), out lineFistTile);
                lineTileNum--;
            }

            if (renderTileList.Count <= 0) return false;

            var chunkGo = new GameObject("Chunk_" + _chunkIndex);
            chunkGo.transform.parent = mLayerRootGo.transform;
            chunkGo.transform.localPosition = Vector3.zero;
            DoRenderChunk(chunkGo, renderTileList);
            return true;
        }

        /// <summary>
        /// 将给定线段长度的TileRenderData Push到需要渲染的渲染列表中去
        /// </summary>
        private void TryToPushLineRenderTileData(Vector2Int _startTile, int _count, List<ITU_RenderTileData> _list)
        {
            var nowPointTile = _startTile;
            for (var i = 0; i < _count; i++)
            {
                var tileData = mAllNeedRenderTileList.Find(data => data.x == nowPointTile.x && data.y == nowPointTile.y);
                if (tileData != null && !tileData.tileMeta.isImageCollectionAndNotRender)
                {
                    _list.Add(tileData);
                }

                ITU_MathUtils.GetNowTileRightTileWithStep(nowPointTile.x, nowPointTile.y, 1, mMapMeta.IsIsoStaggered(), out nowPointTile);
            }
        }

        protected abstract void DoRenderChunk(GameObject _chunkGo, List<ITU_RenderTileData> _needRenderTileDataList);
    }
}