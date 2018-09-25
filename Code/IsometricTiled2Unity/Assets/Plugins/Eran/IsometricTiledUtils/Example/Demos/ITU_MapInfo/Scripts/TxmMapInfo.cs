using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_MapInfo.Scripts
{
    public class TxmMapInfo : MonoBehaviour
    {
        public string mapFileName;
        public Vector2 mapLeftTopPosV2;
        public Vector2 mapRightBottomPosV2;
        public float mapWidthInUnit;
        public float mapHeightInUnit;
        public bool isStaggered;


        private void OnDrawGizmosSelected()
        {
            var topLeft = transform.TransformPoint(new Vector3(mapLeftTopPosV2.x, mapLeftTopPosV2.y, 0));
            var bottomRight = transform.TransformPoint(new Vector3(mapRightBottomPosV2.x, mapRightBottomPosV2.y, 0));
            var topRight = new Vector3(bottomRight.x, topLeft.y, 0);
            var bottomLeft = new Vector3(topLeft.x, bottomRight.y, 0);

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);
        }
    }
}