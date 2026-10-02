using UnityEngine;
using PavlosGame;
using System.Collections.Generic;



namespace PavlosGame
{
    public class BuildInputHandler : MonoBehaviour
    {
        [Header("Level Material Limits (Configurable)")]
        public MaterialLimitConfig levelMaterialLimitConfig;
        


        public KeyCode toggleKey = KeyCode.B;
        public LayerMask placementLayer;
        public static bool IsBuildModeActive { get; private set; }

        public static BuildInputHandler Instance { get; private set; }

        [Header("Ghost Prefabs (by Material Type)")]
        public GameObject ghostWood;
        public GameObject ghostConcrete;
        public GameObject ghostSteel;
        public GameObject ghostAluminum;
        public GameObject ghostPlastic;

        [Header("Block Prefabs (by Material Type)")]
        public GameObject blockWood;
        public GameObject blockConcrete;
        public GameObject blockSteel;
        public GameObject blockAluminum;
        public GameObject blockPlastic;

        [Header("UI Reference")]
        public MaterialSelectorUI materialSelector;

        public MaterialType currentMaterial = MaterialType.Concrete;

        public MaterialType selectedMaterial;
        public void SetMaterial(MaterialType type)
        {
            

        selectedMaterial = type;
        currentMaterial = type;
        }



        void Awake()
        {
            Instance = this;
            

            if (materialSelector != null) ;
                //materialSelector.OnMaterialSelected += UpdateMaterialType;
        }

        void Update()
        {
            // Material switching
            if (Input.GetKeyDown(KeyCode.Alpha1)) currentMaterial = MaterialType.Wood;
            if (Input.GetKeyDown(KeyCode.Alpha2)) currentMaterial = MaterialType.Concrete;
            if (Input.GetKeyDown(KeyCode.Alpha3)) currentMaterial = MaterialType.Steel;
            if (Input.GetKeyDown(KeyCode.Alpha4)) currentMaterial = MaterialType.Aluminum;
            if (Input.GetKeyDown(KeyCode.Alpha5)) currentMaterial = MaterialType.Plastic;

            if (Input.GetKeyDown(toggleKey))
            {
                IsBuildModeActive = !IsBuildModeActive;

                if (!IsBuildModeActive)
                {
                    FinalizeBlueprintBlocks();

                    if (GhostPlacer.Instance != null)
                    {
                        GhostPlacer.Instance.ResetGhost(); // Fully destroy + null
                    }
                }

                else
                {
                    if (GhostPlacer.Instance != null)
                    {
                        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, placementLayer))
                        {

                            Debug.Log($"[Raycast] Hit: {hit.collider.gameObject.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                            if (Vector3.Angle(hit.normal, Vector3.up) <= 80f)
                            {
                                GhostPlacer.Instance.UpdateGhost(hit.point, hit.normal);
                            }
                        }
                        else
                        {
                            if (GhostPlacer.Instance != null)
                            {
                                GhostPlacer.Instance.ResetGhost(); // kill broken ghost if any

                                ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f));
                                if (Physics.Raycast(ray, out hit, Mathf.Infinity, placementLayer))
                                {
                                    Debug.Log($"[Raycast] Hit: {hit.collider.gameObject.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                                    if (Vector3.Angle(hit.normal, Vector3.up) <= 80f)
                                    {
                                        GhostPlacer.Instance.UpdateGhost(hit.point, hit.normal);
                                    }
                                }
                            }
                        }

                    }
                }
            }

            if (!IsBuildModeActive || BuildModeController.IsBlocked) return;


            Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseRay, out RaycastHit mouseHit, Mathf.Infinity, placementLayer))
            {
                if (Vector3.Angle(mouseHit.normal, Vector3.up) <= 80f)
                {
                    // Fix: only update or spawn ghost if it's missing or inactive
                    var ghostPlacer = GhostPlacer.Instance;

                    if (ghostPlacer.currentGhost == null || !ghostPlacer.currentGhost.activeSelf)
                    {
                        ghostPlacer.UpdateGhost(mouseHit.point, mouseHit.normal);
                    }
                    else
                    {
                        ghostPlacer.UpdateGhost(mouseHit.point, mouseHit.normal);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        ghostPlacer.TryPlaceBlock();
                    }
                }
            }


        }

        public GameObject GetCurrentGhostPrefab() => currentMaterial switch
        {
            MaterialType.Wood => ghostWood,
            MaterialType.Concrete => ghostConcrete,
            MaterialType.Steel => ghostSteel,
            MaterialType.Aluminum => ghostAluminum,
            MaterialType.Plastic => ghostPlastic,
            _ => ghostConcrete
        };

        public GameObject GetCurrentBlockPrefab() => currentMaterial switch
        {
            MaterialType.Wood => blockWood,
            MaterialType.Concrete => blockConcrete,
            MaterialType.Steel => blockSteel,
            MaterialType.Aluminum => blockAluminum,
            MaterialType.Plastic => blockPlastic,
            _ => blockConcrete
        };

        void FinalizeBlueprintBlocks()
        {
            var blueprintBlocks = Object.FindObjectsByType<BlueprintTag>(FindObjectsSortMode.None);

            foreach (var tag in blueprintBlocks)
            {
                GameObject go = tag.gameObject;
                Object.Destroy(tag);

                go.tag = "PlacedBlock";

                int placedLayer = LayerMask.NameToLayer("PlacedBlock");
                if (placedLayer != -1)
                {
                    go.layer = placedLayer;
                }
                else
                {
                    Debug.LogWarning("Layer 'PlacedBlock' not found. Please add it in Project Settings > Tags and Layers.");
                }

                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb == null) rb = go.AddComponent<Rigidbody>();

                LoadBearingVisualizer visualizer = go.GetComponent<LoadBearingVisualizer>();
                if (visualizer == null)
                {
                    visualizer = go.AddComponent<LoadBearingVisualizer>();
                    visualizer.enabled = true;
                }
                visualizer.Invoke("EnableStressCheck", 0.1f);

                AutoJointManager joints = go.GetComponent<AutoJointManager>();
                if (joints == null)
                {
                    joints = go.AddComponent<AutoJointManager>();
                    joints.enabled = true;
                }
                joints.InitializeJoints();
            }

            Debug.Log("All blueprint blocks finalized.");
        }

        private void UpdateMaterialType(MaterialType selected)
        {
            currentMaterial = selected;
            Debug.Log($"[BuildInputHandler] Material set via UI to: {selected}");
        }

    }
}
