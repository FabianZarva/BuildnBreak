using UnityEngine;
using PavlosGame;

namespace PavlosGame
{
    public class GhostPlacer : MonoBehaviour
    {
        public static GhostPlacer Instance;
        public GameObject currentGhost;

        [SerializeField] private Color ghostNormalColor = new Color(1f, 1f, 1f, 0.4f);
        [SerializeField] private Color ghostWarningColor = new Color(1f, 1f, 0.2f, 0.4f);
        [SerializeField] private Color ghostBlockedColor = new Color(1f, 0.2f, 0.2f, 0.4f);

        private Material ghostMat;


        void Awake()
        {
            Instance = this;
        }

        public void UpdateGhost(Vector3 position, Vector3 normal)
        {
            if (BuildModeController.IsBlocked)
            {
                Debug.Log("[GhostPlacer] Building is blocked due to disaster.");
            return;
            }

            if (currentGhost == null)
            {
                GameObject ghostPrefab = BuildInputHandler.Instance.GetCurrentGhostPrefab();
                currentGhost = Instantiate(ghostPrefab);
                Renderer rend = currentGhost.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                ghostMat = rend.material; // Cache the ghost material for tinting
                }

                BuildPlacer.currentGhost = currentGhost;

                foreach (Collider col in currentGhost.GetComponentsInChildren<Collider>())
                {
                    col.isTrigger = true;
                }

                currentGhost.SetActive(true);
            }

            var handler = BuildInputHandler.Instance;
            if (handler != null)
            {
                var visualizer = currentGhost.GetComponent<LoadBearingVisualizer>();
                if (visualizer == null)
                {
                    visualizer = currentGhost.AddComponent<LoadBearingVisualizer>();
                }

                visualizer.materialType = handler.currentMaterial;
            }
            if (handler != null && ghostMat != null)
            {       
                MaterialType type = handler.currentMaterial;
                int placed = BlockLimitManager.Instance.GetPlaced(type);
                int limit = BlockLimitManager.Instance.GetLimit(type);

                if (placed >= limit)
                ghostMat.color = ghostBlockedColor;
                else if (placed >= limit - 2)
                ghostMat.color = ghostWarningColor;
                else
                ghostMat.color = ghostNormalColor;
}

            // --- POSITIONING LOGIC STARTS HERE ---
            Vector3 preSnap = position;
            BuildSnapping.AlignToConnector(ref position, ref normal, preferFlatSurface: true);

            // Determine if the position was modified by snapping
            bool snappedToConnector = (Vector3.Distance(preSnap, position) > 0.01f);

            // Only apply vertical lift if NOT snapped and surface is flat
            if (!snappedToConnector && Vector3.Angle(normal, Vector3.up) < 5f)
            {
                Renderer rend = currentGhost.GetComponentInChildren<Renderer>();
                if (rend != null)
                {
                    float blockHeight = rend.bounds.size.y;
                    float ghostPivotYOffset = rend.bounds.center.y - currentGhost.transform.position.y;
                    position.y += (blockHeight / 2f) - ghostPivotYOffset;
                }
            }


            Debug.Log($"[GhostPlacer] Ghost placed at {position}, snappedToConnector: {snappedToConnector}, normal: {normal}");

            currentGhost.transform.position = position;
            currentGhost.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        }

        public void TryPlaceBlock()
        {
            if (BuildModeController.IsBlocked)
            {
                Debug.Log("[GhostPlacer] Building is blocked due to disaster.");
                return;
            }

            if (currentGhost == null)
            {
                Debug.LogWarning("Tried to place a block, but no ghost exists!");
                return;
            }

            Debug.Log("Trying to place ghost at " + currentGhost.transform.position);

            var handler = BuildInputHandler.Instance;
            MaterialType type = handler.currentMaterial;
            int placed = BlockLimitManager.Instance.GetPlaced(type);

            int limit = BlockLimitManager.Instance.GetLimit(type);
            if (placed >= limit)
            {
                Debug.LogWarning($"Cannot place more blocks of type {type}. Limit reached ({placed}/{limit})");
                return;
            }

            if (BuildValidator.CanPlace(currentGhost.transform.position, currentGhost.transform.up))
            {
                BuildPlacer.Place(currentGhost.transform.position, currentGhost.transform.rotation);
            }
            else
            {
                Debug.Log("BuildValidator rejected the placement.");
            }

        }

        public void ResetGhost()
        {
            if (currentGhost != null)
            {
                Destroy(currentGhost);
                currentGhost = null;
            }
            Debug.Log("[GhostPlacer] Ghost was reset.");
        }

    }
}
