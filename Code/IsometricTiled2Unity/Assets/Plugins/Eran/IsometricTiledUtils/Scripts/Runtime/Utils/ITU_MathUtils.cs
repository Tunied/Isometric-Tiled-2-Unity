using UnityEngine;

namespace Eran.ITU.Utils
{
    public class ITU_MathUtils
    {
        /// <summary>
        /// Tile坐标到屏幕坐标,返回的坐标对应的是当前Tile顶点坐标(菱形的最上面那个点)
        /// </summary>
        public static void TileTopCorner2World(int _tileX, int _tileY, float _tileWidth, float _tileHeight, bool _isStaggered, out Vector2 _worldPos)
        {
            if (_isStaggered)
            {
                if (_tileY % 2 == 0)
                {
                    //偶数行
                    _worldPos = new Vector2((_tileX + 0.5f) * _tileWidth, -_tileY * 0.5f * _tileHeight);
                }
                else
                {
                    //奇数行
                    _worldPos = new Vector2((_tileX + 1) * _tileWidth, -_tileY * 0.5f * _tileHeight);
                }
            }
            else
            {
                _worldPos = new Vector2
                {
                    x = (_tileX - _tileY) * _tileWidth / 2,
                    y = -(_tileX + _tileY) * _tileHeight / 2
                };
            }
        }

        /// <summary>
        ///  https://gamedev.stackexchange.com/questions/45103/staggered-isometric-map-calculate-map-coordinates-for-point-on-screen
        /// </summary>
        public static void World2Tile(float _worldPosX, float _worldPosY, float _tileWidth, float _tileHeight, bool _isStaggered, out Vector2Int _tileIndex)
        {
            if (_isStaggered)
            {
                var checkP = new Vector2(_worldPosX, _worldPosY);

                var evenTileIndex = new Vector2Int(Mathf.FloorToInt(checkP.x / _tileWidth), Mathf.FloorToInt(checkP.y / -_tileHeight));
                var oddTileIndex = new Vector2Int(Mathf.FloorToInt((checkP.x - _tileWidth / 2) / _tileWidth), Mathf.FloorToInt((checkP.y + _tileHeight / 2) / -_tileHeight));

                var isEvenTile = IsPointInDiamond(new Vector2(checkP.x - evenTileIndex.x * _tileWidth, checkP.y + evenTileIndex.y * _tileHeight), _tileWidth, _tileHeight);
                if (isEvenTile)
                {
                    _tileIndex = new Vector2Int
                    {
                        x = evenTileIndex.x,
                        y = evenTileIndex.y * 2
                    };
                }
                else
                {
                    _tileIndex = new Vector2Int
                    {
                        x = oddTileIndex.x,
                        y = oddTileIndex.y * 2 + 1
                    };
                }
            }
            else
            {
                _tileIndex = new Vector2Int
                {
                    x = -Mathf.CeilToInt(_worldPosY / _tileHeight - _worldPosX / _tileWidth),
                    y = -Mathf.CeilToInt(_worldPosX / _tileWidth + _worldPosY / _tileHeight)
                };
            }
        }


        /// <summary>
        /// https://stackoverflow.com/questions/2049582/how-to-determine-if-a-point-is-in-a-2d-triangle
        /// http://totologic.blogspot.com/2014/01/accurate-point-in-triangle-test.html
        /// http://blackpawn.com/texts/pointinpoly/
        /// </summary>
        private static bool IsPointInDiamond(Vector2 _localPoint, float _tileWidth, float _tileHeigh)
        {
            var pa = new Vector2(_tileWidth / 2, 0);
            var pb = new Vector2(_tileWidth, -_tileHeigh / 2);
            var pc = new Vector2(_tileWidth / 2, -_tileHeigh);
            var pd = new Vector2(0, -_tileHeigh / 2);
            return PointInTriangle(_localPoint, pa, pb, pc) || PointInTriangle(_localPoint, pd, pa, pc);
        }

        private static float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        private static bool PointInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            bool b1, b2, b3;

            b1 = Sign(pt, v1, v2) < 0.0f;
            b2 = Sign(pt, v2, v3) < 0.0f;
            b3 = Sign(pt, v3, v1) < 0.0f;

            return b1 == b2 && b2 == b3;
        }


        /// <summary>
        /// ↖ 取得给定TileIndex向左上移动指定Step后的TileIndex, 用于渲染遍历
        /// </summary>
        public static void GetNowTileLeftTopTileWithStep(int _nowTileIndexX, int _nowTileIndexY, int _step, bool _isStaggered, out Vector2Int _outTileIndex)
        {
            if (_isStaggered)
            {
                //比较笨的计算方法,目前采用这种计算方法有几个原因
                //1` 公式没推出来,但是花时间应该可以推到出来,不过应该会很复杂(还是傻.. )
                //2` 函数只是在渲染时候调用,不用考虑性能问题,且Step不会很大
                //3` 逻辑相比公式来说简单好理解.

                //根据观察,如果当前Y值是奇数则下一步X是不会变的,偶数则下一步会变. Y值每一步都会改变
                var resultX = _nowTileIndexX;
                var resultY = _nowTileIndexY;
                var isSubX = _nowTileIndexY % 2 == 0;
                for (var i = 0; i < _step; i++)
                {
                    resultX = isSubX ? resultX - 1 : resultX;
                    resultY--;
                    isSubX = !isSubX;
                }

                _outTileIndex = new Vector2Int(resultX, resultY);
            }
            else
            {
                _outTileIndex = new Vector2Int(_nowTileIndexX - _step, _nowTileIndexY);
            }
        }

        /// <summary>
        /// ↙ 取得给定TileIndex向左下移动指定Step后的TileIndex,用于渲染遍历
        /// </summary>
        public static void GetNowTileLeftBottomTileWithStep(int _nowTileIndexX, int _nowTileIndexY, int _step, bool _isStaggered, out Vector2Int _outTileIndex)
        {
            if (_isStaggered)
            {
                var resultX = _nowTileIndexX;
                var resultY = _nowTileIndexY;
                var isSubX = _nowTileIndexY % 2 == 0;
                for (var i = 0; i < _step; i++)
                {
                    resultX = isSubX ? resultX - 1 : resultX;
                    resultY++;
                    isSubX = !isSubX;
                }

                _outTileIndex = new Vector2Int(resultX, resultY);
            }
            else
            {
                _outTileIndex = new Vector2Int(_nowTileIndexX, _nowTileIndexY + _step);
            }
        }


        /// <summary>
        /// ↘ 取得给定TileIndex向右下移动指定Step后的TileIndex,用于渲染遍历
        /// </summary>
        public static void GetNowTileRightBottomTileWithStep(int _nowTileIndexX, int _nowTileIndexY, int _step, bool _isStaggered, out Vector2Int _outTileIndex)
        {
            if (_isStaggered)
            {
                var resultX = _nowTileIndexX;
                var resultY = _nowTileIndexY;
                var isAddX = _nowTileIndexY % 2 != 0;
                for (var i = 0; i < _step; i++)
                {
                    resultX = isAddX ? resultX + 1 : resultX;
                    resultY++;
                    isAddX = !isAddX;
                }

                _outTileIndex = new Vector2Int(resultX, resultY);
            }
            else
            {
                _outTileIndex = new Vector2Int(_nowTileIndexX + _step, _nowTileIndexY);
            }
        }


        /// <summary>
        /// → 取得给定TileIndex向右移动指定Step后的TileIndex,用于渲染遍历
        /// </summary>
        public static void GetNowTileRightTileWithStep(int _nowTileIndexX, int _nowTileIndexY, int _step, bool _isStaggered, out Vector2Int _outTileIndex)
        {
            if (_isStaggered)
            {
                _outTileIndex = new Vector2Int(_nowTileIndexX + _step, _nowTileIndexY);
            }
            else
            {
                _outTileIndex = new Vector2Int(_nowTileIndexX + _step, _nowTileIndexY - _step);
            }
        }
    }
}