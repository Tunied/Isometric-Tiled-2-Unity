using System.Collections.Generic;
using UnityEngine;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_Nav_Prefab.Scripts
{
    public class ITU_SoringManger : MonoBehaviour
    {
        public GameObject actorGo;
        public GameObject decoRootGo;

        private List<SpriteRenderer> mAllNeedSortObjectList;

        // Use this for initialization
        private void Start()
        {
            mAllNeedSortObjectList = new List<SpriteRenderer>();
            var actorSp = actorGo.GetComponentInChildren<SpriteRenderer>();
            mAllNeedSortObjectList.Add(actorSp);

            var decoArray = decoRootGo.GetComponentsInChildren<SpriteRenderer>();
            foreach (var spriteRenderer in decoArray)
            {
                mAllNeedSortObjectList.Add(spriteRenderer);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            mAllNeedSortObjectList.Sort((a, b) =>
            {
                var posA = a.transform.position;
                var posB = b.transform.position;
                return posA.z <= posB.z ? 1 : -1;
            });

            var index = 0;
            mAllNeedSortObjectList.ForEach(x =>
            {
                x.sortingOrder = index;
                index++;
            });
        }
    }
}