using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PavlosGame
{
    public class AutoJointManager : MonoBehaviour
    {
        public float jointRange = 1.1f;
        public float breakForce = 10000f;
        public float breakTorque = 10000f;

        private bool isBlueprint = false;

        public void MarkAsBlueprint()
        {
            isBlueprint = true;
        }

        public void InitializeJoints()
        {
            if (isBlueprint)
            {
                Debug.Log($"{gameObject.name}: Skipping joint setup (blueprint mode)");
                return;
            }

            Debug.Log($"{gameObject.name}: Initializing joints...");
            StartCoroutine(DelayedJointSetup());
        }

        IEnumerator DelayedJointSetup()
        {
            yield return null;

            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    CreateJoints(child.gameObject);
                }
            }

            if (transform.childCount == 0)
            {
                if (TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    CreateJoints(gameObject);
                }
            }
        }

        void CreateJoints(GameObject obj)
        {
            Collider[] colliders = Physics.OverlapSphere(obj.transform.position, jointRange);

            foreach (Collider col in colliders)
            {
                if (col.gameObject != obj && col.attachedRigidbody != null)
                {
                    FixedJoint joint = obj.AddComponent<FixedJoint>();
                    joint.connectedBody = col.attachedRigidbody;
                    joint.breakForce = breakForce;
                    joint.breakTorque = breakTorque;
                }
            }
        }
    }
}