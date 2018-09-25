using UnityEngine;
using UnityEngine.AI;

namespace Eran.IsometricTiled2Unity.Example.Demos.ITU_Nav_Prefab.Scripts
{
    public class ITU_NavActor : MonoBehaviour
    {
        public Transform navGoalTrans;

        private NavMeshAgent mAgent;

        // Use this for initialization
        private void Start()
        {
            mAgent = GetComponent<NavMeshAgent>();
            mAgent.updateRotation = false;
        }


        // Update is called once per frame
        private void Update()
        {
            mAgent.SetDestination(navGoalTrans.position);
        }
    }
}