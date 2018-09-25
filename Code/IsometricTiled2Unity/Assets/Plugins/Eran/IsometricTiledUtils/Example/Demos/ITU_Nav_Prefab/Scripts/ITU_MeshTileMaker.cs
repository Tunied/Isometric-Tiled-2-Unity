using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_Nav_Prefab.Scripts
{
    public class ITU_MeshTileMaker
    {
        private const int A = 0;
        private const int B = 1;
        private const int C = 2;
        private const int D = 3;

        private const int AP = 4;
        private const int BP = 5;
        private const int CP = 6;
        private const int DP = 7;

        public static GameObject MakeTile(float _tileWidth, float _tileHeight, float _tileLength, Material _material)
        {
            var tileGo = new GameObject();

            var mesh = new Mesh {name = "TileMesh"};

            var filter = tileGo.AddComponent<MeshFilter>();
            filter.mesh = mesh;

            var HALF_WIDHT = _tileWidth / 2;
            var HALF_HEIGH = _tileHeight / 2;

            var vertices = new Vector3[8];
            vertices[A] = new Vector3(-HALF_WIDHT, 0, 0);
            vertices[B] = new Vector3(0, 0, HALF_HEIGH);
            vertices[C] = new Vector3(HALF_WIDHT, 0, 0);
            vertices[D] = new Vector3(0, 0, -HALF_HEIGH);

            vertices[AP] = new Vector3(-HALF_WIDHT, _tileLength, 0);
            vertices[BP] = new Vector3(0, _tileLength, HALF_HEIGH);
            vertices[CP] = new Vector3(HALF_WIDHT, _tileLength, 0);
            vertices[DP] = new Vector3(0, _tileLength, -HALF_HEIGH);


            var triangles = new int[12 * 3];
            var startIndex = 0;
            DoPushTriangles(ref triangles, ref startIndex, D, B, A);
            DoPushTriangles(ref triangles, ref startIndex, C, B, D);

            DoPushTriangles(ref triangles, ref startIndex, AP, BP, DP);
            DoPushTriangles(ref triangles, ref startIndex, BP, CP, DP);

            DoPushTriangles(ref triangles, ref startIndex, AP, DP, A);
            DoPushTriangles(ref triangles, ref startIndex, A, DP, D);

            DoPushTriangles(ref triangles, ref startIndex, D, DP, CP);
            DoPushTriangles(ref triangles, ref startIndex, D, CP, C);

            DoPushTriangles(ref triangles, ref startIndex, B, BP, AP);
            DoPushTriangles(ref triangles, ref startIndex, B, AP, A);

            DoPushTriangles(ref triangles, ref startIndex, C, CP, BP);
            DoPushTriangles(ref triangles, ref startIndex, C, BP, B);

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateTangents();

            tileGo.AddComponent<MeshRenderer>().material = _material;

            return tileGo;
        }

        private static void DoPushTriangles(ref int[] _triangles, ref int _startIndex, int _A, int _B, int _C)
        {
            _triangles[_startIndex] = _A;
            _triangles[_startIndex + 1] = _B;
            _triangles[_startIndex + 2] = _C;
            _startIndex += 3;
        }
    }
}