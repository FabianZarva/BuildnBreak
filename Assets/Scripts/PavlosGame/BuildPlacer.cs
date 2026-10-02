using UnityEngine;
using PavlosGame;

namespace PavlosGame
{
    public static class BuildPlacer
    {
        public static GameObject prefabToPlace;
        public static GameObject currentGhost;

        public static void Place(Vector3 position, Quaternion rotation)
        {
            GameObject prefab = BuildInputHandler.Instance.GetCurrentBlockPrefab();
            if (prefab == null)
            {
                Debug.LogWarning("No prefab set to place!");
                return;
            }

            // Create instance
            GameObject block = Object.Instantiate(prefab);

            // Get height from the prefab's Renderer (fallback to scale if needed)
            float blockHeight = 1f;
            Renderer renderer = block.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                blockHeight = renderer.bounds.size.y;
            }
            else
            {
                blockHeight = block.transform.localScale.y;
            }

            // Apply upward offset so the block sits ON the surface
            Vector3 adjustedPosition = position;

            block.transform.position = adjustedPosition;
            block.transform.rotation = rotation;

            /*   // Add components if missing
                 if (!block.GetComponent<Rigidbody>()) block.AddComponent<Rigidbody>();
                 if (!block.GetComponent<Collider>()) block.AddComponent<BoxCollider>();
                 if (!block.GetComponent<MaterialProperties>()) block.AddComponent<MaterialProperties>();
                 if (!block.GetComponent<LoadBearingVisualizer>()) block.AddComponent<LoadBearingVisualizer>();
                 if (!block.GetComponent<AutoJointManager>()) block.AddComponent<AutoJointManager>();

                 // Fix for placement problem
                 Rigidbody rb = block.GetComponent<Rigidbody>();
                 if (rb == null) rb = block.AddComponent<Rigidbody>();
                 rb.linearVelocity = Vector3.zero;
                 rb.angularVelocity = Vector3.zero;
            */

            // Only required components during blueprint mode
            if (!block.GetComponent<Collider>()) block.AddComponent<BoxCollider>();
            if (!block.GetComponent<MaterialProperties>()) block.AddComponent<MaterialProperties>();

            // Add and tag AutoJointManager but don't initialize yet
            AutoJointManager joints = block.AddComponent<AutoJointManager>();
            joints.MarkAsBlueprint();

            block.AddComponent<BlueprintTag>();

            // Assign material type to visualizer (will affect stress calc after finalization)
            var visualizer = block.GetComponent<LoadBearingVisualizer>();
            if (visualizer != null)
            {
                visualizer.materialType = BuildInputHandler.Instance.currentMaterial;
            }

            Debug.Log("Block placed at: " + adjustedPosition);
            BlockLimitManager.Instance.RegisterPlacement(BuildInputHandler.Instance.currentMaterial);

        }

        // Direct ghost-based placement
        public static void PlaceBlock()
        {
            if (currentGhost == null)
            {
                Debug.LogWarning("No ghost available to place.");
                return;
            }

            Place(currentGhost.transform.position, currentGhost.transform.rotation);
        }
    }
}
