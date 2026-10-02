using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PavlosGame
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Rigidbody))]
    public class Fracture : MonoBehaviour
    {
        public TriggerOptions triggerOptions;
        public FractureOptions fractureOptions;
        public RefractureOptions refractureOptions;
        public CallbackOptions callbackOptions;

        [HideInInspector]
        public int currentRefractureCount = 0;

        private GameObject fragmentRoot;

        private static readonly Dictionary<MaterialType, float> fractureThresholds = new()
        {
            { MaterialType.Wood, 200f },
            { MaterialType.Concrete, 400f },
            { MaterialType.Steel, 1200f },
            { MaterialType.Aluminum, 600f },
            { MaterialType.Plastic, 150f }
        };

        private static readonly Dictionary<MaterialType, int> fracturePieces = new()
        {
            { MaterialType.Wood, 6 },
            { MaterialType.Concrete, 12 },
            { MaterialType.Steel, 2 },
            { MaterialType.Aluminum, 4 },
            { MaterialType.Plastic, 8 }
        };

        private static readonly Dictionary<MaterialType, int> maxRefractureCounts = new()
        {
            { MaterialType.Wood, 2 },
            { MaterialType.Concrete, 2 },
            { MaterialType.Steel, 1 },
            { MaterialType.Aluminum, 1 },
            { MaterialType.Plastic, 3 }
        };

        void OnCollisionEnter(Collision collision)
        {
            if (triggerOptions.triggerType != TriggerType.Collision || collision.contactCount == 0) return;

            var contact = collision.contacts[0];
            float collisionForce = collision.impulse.magnitude / Time.fixedDeltaTime;

            bool tagAllowed = triggerOptions.IsTagAllowed(contact.otherCollider.gameObject.tag);
            MaterialType matType = GetMaterialType();
            float materialThreshold = fractureThresholds[matType];

            if (collisionForce > materialThreshold && (!triggerOptions.filterCollisionsByTag || tagAllowed))
            {
                callbackOptions.CallOnFracture(contact.otherCollider, gameObject, contact.point);
                ComputeFracture();
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            if (triggerOptions.triggerType != TriggerType.Trigger) return;

            bool tagAllowed = triggerOptions.IsTagAllowed(collider.gameObject.tag);

            if (triggerOptions.filterCollisionsByTag && tagAllowed)
            {
                callbackOptions.CallOnFracture(collider, gameObject, transform.position);
                ComputeFracture();
            }
        }

        void Update()
        {
            if (triggerOptions.triggerType == TriggerType.Keyboard && Input.GetKeyDown(triggerOptions.triggerKey))
            {
                callbackOptions.CallOnFracture(null, gameObject, transform.position);
                ComputeFracture();
            }
        }

        public void CauseFracture()
        {
            callbackOptions.CallOnFracture(null, gameObject, transform.position);
            ComputeFracture();
        }

        private void ComputeFracture()
        {
            var mesh = GetComponent<MeshFilter>().sharedMesh;
            if (mesh == null) return;

            MaterialType matType = GetMaterialType();

            if (fragmentRoot == null)
            {
                fragmentRoot = new GameObject($"{name}Fragments");
                fragmentRoot.transform.SetParent(transform.parent);
                fragmentRoot.transform.position = transform.position;
                fragmentRoot.transform.rotation = transform.rotation;
                fragmentRoot.transform.localScale = Vector3.one;
            }

            if (fracturePieces.TryGetValue(matType, out int fragmentCount))
            {
                fractureOptions.fragmentCount = fragmentCount;
            }

            var fragmentTemplate = CreateFragmentTemplate(matType);

            if (fractureOptions.asynchronous)
            {
                StartCoroutine(Fragmenter.FractureAsync(
                    gameObject, fractureOptions, fragmentTemplate, fragmentRoot.transform,
                    () =>
                    {
                        Destroy(fragmentTemplate);
                        gameObject.SetActive(false);
                        if (ShouldInvokeCallback()) callbackOptions.onCompleted?.Invoke();
                    }));
            }
            else
            {
                Fragmenter.Fracture(gameObject, fractureOptions, fragmentTemplate, fragmentRoot.transform);
                Destroy(fragmentTemplate);
                gameObject.SetActive(false);
                if (ShouldInvokeCallback()) callbackOptions.onCompleted?.Invoke();
            }
        }

        private GameObject CreateFragmentTemplate(MaterialType matType)
        {
            GameObject obj = new GameObject("Fragment");
            obj.tag = tag;

            obj.AddComponent<MeshFilter>();

            var meshRenderer = obj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterials = new Material[2]
            {
                GetComponent<MeshRenderer>().sharedMaterial,
                fractureOptions.insideMaterial
            };

            var thisCollider = GetComponent<Collider>();
            var fragmentCollider = obj.AddComponent<MeshCollider>();
            fragmentCollider.convex = true;
            fragmentCollider.sharedMaterial = thisCollider.sharedMaterial;
            fragmentCollider.isTrigger = thisCollider.isTrigger;

            var thisRigidBody = GetComponent<Rigidbody>();
            var fragmentRigidBody = obj.AddComponent<Rigidbody>();
            fragmentRigidBody.linearVelocity = thisRigidBody.linearVelocity;
            fragmentRigidBody.angularVelocity = thisRigidBody.angularVelocity;
            fragmentRigidBody.linearDamping = thisRigidBody.linearDamping;
            fragmentRigidBody.angularDamping = thisRigidBody.angularDamping;
            fragmentRigidBody.useGravity = thisRigidBody.useGravity;

            if (refractureOptions.enableRefracturing &&
                currentRefractureCount < maxRefractureCounts[matType])
            {
                CopyFractureComponent(obj);
            }

            return obj;
        }

        private void CopyFractureComponent(GameObject obj)
        {
            var fractureComponent = obj.AddComponent<Fracture>();

            fractureComponent.triggerOptions = triggerOptions;
            fractureComponent.fractureOptions = fractureOptions;
            fractureComponent.refractureOptions = refractureOptions;
            fractureComponent.callbackOptions = callbackOptions;
            fractureComponent.currentRefractureCount = currentRefractureCount + 1;
            fractureComponent.fragmentRoot = fragmentRoot;
        }

        private bool ShouldInvokeCallback()
        {
            return currentRefractureCount == 0 ||
                   (currentRefractureCount > 0 && refractureOptions.invokeCallbacks);
        }

        private MaterialType GetMaterialType()
        {
            if (TryGetComponent<MaterialProperties>(out var matProp))
                return matProp.materialType;

            return MaterialType.Wood; // fallback
        }
    }
}
