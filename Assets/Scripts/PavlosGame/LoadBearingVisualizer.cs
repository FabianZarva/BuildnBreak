using UnityEngine;
using System.Collections.Generic;
using PavlosGame;

namespace PavlosGame
{
    public class LoadBearingVisualizer : MonoBehaviour
    {
        public MaterialType materialType;

        public Material outlineNone;
        public Material outlineLow;
        public Material outlineMedium;
        public Material outlineHigh;

        public float stressAbsorption = 1f;
        public Color stableColor = Color.green;
        public Color mediumStressColor = Color.yellow;
        public Color highStressColor = Color.red;
        public Material normalMaterial;
        public Material crackedMaterial;
        public Material heavilyCrackedMaterial;

        private Renderer rend;
        private Rigidbody rb;
        private FixedJoint[] joints;
        private float materialStrength;
        private float scaleFactor;
        private float currentStress = 0f;
        private bool isCracked = false;
        private bool isHeavilyCracked = false;
        private bool stressCheckReady = false;
        private bool isBlueprint = false;

        private bool isInWater = false;
        private float waterHeight = 0f;
        private float buoyancyStrength = 10f; // tweak per material later

        private static readonly Dictionary<MaterialType, float> materialStrengthValues = new()
        {
            { MaterialType.Wood, 100f },
            { MaterialType.Concrete, 300f },
            { MaterialType.Steel, 600f },
            { MaterialType.Aluminum, 250f },
            { MaterialType.Plastic, 80f }
        };

        void Start()
        {
            Debug.Log($"[{gameObject.name}] LoadBearingVisualizer Start() triggered.");

            if (GetComponent<BlueprintTag>())
            {
                Debug.Log($"{gameObject.name}: Blueprint mode � setting visual material only");
                rend = GetComponent<Renderer>() ?? GetComponentInChildren<Renderer>();
                if (rend != null && normalMaterial != null)
                    rend.sharedMaterial = normalMaterial;

                isBlueprint = true;
                return;
            }


            rend = GetComponent<Renderer>();
            rb = GetComponent<Rigidbody>();
            joints = GetComponents<FixedJoint>();

            materialStrength = materialStrengthValues[materialType];
            buoyancyStrength = materialType switch
            {
            MaterialType.Wood => 15f,
            MaterialType.Plastic => 12f,
            MaterialType.Aluminum => 8f,
            MaterialType.Concrete => 4f,
            MaterialType.Steel => 2f,
            _ => 6f
            };

                // Assign mass based on material (helps balance buoyancy vs inertia)
            if (rb != null)
                {
                 rb.mass = materialType switch
            {
            MaterialType.Wood => 2f,
            MaterialType.Plastic => 3f,
            MaterialType.Aluminum => 4f,
            MaterialType.Concrete => 8f,
            MaterialType.Steel => 12f,
            _ => 5f
        };
    }


            scaleFactor = transform.localScale.x * transform.localScale.y * transform.localScale.z;
            materialStrength *= scaleFactor;

            if (rend == null)
                rend = GetComponentInChildren<Renderer>(); // Fallback to child renderer

            if (rend != null && normalMaterial != null)
            {
                rend.sharedMaterial = normalMaterial;
                Debug.Log($"[{gameObject.name}] Assigned sharedMaterial: {normalMaterial.name}");
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Renderer or normal material was missing!");
            }

            Invoke(nameof(EnableStressCheck), 0.1f);
        }

        void EnableStressCheck() => stressCheckReady = true;

        void Update()
        {
            UpdateOutlineVisual();
            if (isBlueprint || !stressCheckReady) return;

           // UNITY 6 MOTION-BASED STRESS (reacts to shaking)
            float linear = rb.linearVelocity.magnitude;
            float angular = rb.angularVelocity.magnitude;

            float motionStress = (linear + angular * 0.5f) * 5f; // scale factor tuned
            float force = rb.mass * linear;

            float totalStress = (force + motionStress) / (joints.Length + 1);
            currentStress = Mathf.Clamp(totalStress / materialStrength, 0f, 1f);

            foreach (FixedJoint joint in joints)
            {
                if (joint != null && joint.connectedBody != null)
                {
                    var connectedBlock = joint.connectedBody.GetComponent<LoadBearingVisualizer>();
                    if (connectedBlock != null)
                    {
                        float jointStress = totalStress * 0.5f;
                        float weakestStrength = Mathf.Min(materialStrength, connectedBlock.materialStrength);
                        if (jointStress > weakestStrength * 0.5f)
                        {
                            Destroy(joint);
                            Debug.Log($"Joint between {gameObject.name} and {connectedBlock.gameObject.name} broke!");
                        }
                    }
                }
            }

            if (currentStress > 0.3f && !isHeavilyCracked)
            {
                rend.material = crackedMaterial;
                isCracked = true;
                Debug.Log($"{gameObject.name} is showing small cracks!");
            }
            else if (currentStress > 0.5f && !isHeavilyCracked)
            {
                rend.material = heavilyCrackedMaterial;
                isHeavilyCracked = true;
                Debug.Log($"{gameObject.name} has deep cracks and is close to breaking!");
            }

            /* if (currentStress >= 1f)
            {
                Destroy(gameObject);
                Debug.Log($"{gameObject.name} collapsed due to high stress!");
            } */
        }
    
       void UpdateOutlineVisual()
    {
    if (rend == null) return;

    if (isHeavilyCracked && heavilyCrackedMaterial != null)
    {
        rend.material = heavilyCrackedMaterial;
    }
    else if (isCracked && crackedMaterial != null)
    {
        rend.material = crackedMaterial;
    }
    else
    {
        if (currentStress > 0.4f)
            rend.material = outlineHigh;
        else if (currentStress > 0.2f)
            rend.material = outlineMedium;
        else if (currentStress > 0.05f)
            rend.material = outlineLow;
        else
            rend.material = outlineNone;
    }
    }


    public void ApplyExternalStress(float additionalStress)
    {
    if (!stressCheckReady || isBlueprint) return;

    if (currentStress >= 1f) return; // Already dead

    currentStress += additionalStress * Time.deltaTime; // Smoother accumulation
    currentStress = Mathf.Clamp01(currentStress);

    if (currentStress > 0.5f && currentStress <= 0.8f && !isCracked)
    {
        rend.material = crackedMaterial;
        isCracked = true;
        Debug.Log($"{gameObject.name} is showing small cracks from flood stress!");
    }
    else if (currentStress > 0.8f && !isHeavilyCracked)
    {
        rend.material = heavilyCrackedMaterial;
        isHeavilyCracked = true;
        Debug.Log($"{gameObject.name} has deep flood-induced cracks!");
    }

    if (currentStress >= 1f)
    {
        Destroy(gameObject);
        Debug.Log($"{gameObject.name} collapsed from flood damage!");
    }
}


public void UpdateBuoyancy(float currentWaterHeight)
{
    waterHeight = currentWaterHeight;
    float objectBottom = transform.position.y - (transform.localScale.y / 2f);
    isInWater = objectBottom < waterHeight;

    if (rb == null) return;

    if (isInWater)
    {
        float submersionDepth = Mathf.Clamp01((waterHeight - objectBottom) / transform.localScale.y);

        // Core upward buoyant force
        float forceMagnitude = buoyancyStrength * submersionDepth;
        rb.AddForce(Vector3.up * forceMagnitude, ForceMode.Acceleration);

        // Water drag for damping motion (Unity 6 style)
        rb.linearDamping = Mathf.Lerp(1f, 4f, submersionDepth);
        rb.angularDamping = Mathf.Lerp(1f, 3f, submersionDepth);

        // Light materials get surface drift
        if (materialType == MaterialType.Wood || materialType == MaterialType.Plastic || materialType == MaterialType.Aluminum)
        {
            Vector3 flowDirection = new Vector3(1f, 0f, 0.5f).normalized;
            float flowSpeed = Mathf.Lerp(0.5f, 2f, submersionDepth);
            Vector3 randomTurbulence = new Vector3(
                Mathf.PerlinNoise(Time.time * 0.5f, transform.position.x),
                0f,
                Mathf.PerlinNoise(transform.position.z, Time.time * 0.5f)
            ) - Vector3.one * 0.5f;

            randomTurbulence *= 0.5f;
            Vector3 totalFlow = (flowDirection + randomTurbulence) * flowSpeed;
            rb.AddForce(totalFlow, ForceMode.Acceleration);
        }

        // Heavy materials get wobble/torque to simulate instability
        if (materialType == MaterialType.Steel || materialType == MaterialType.Concrete)
        {
            Vector3 tilt = new Vector3(
                Mathf.PerlinNoise(Time.time, transform.position.x) - 0.5f,
                0f,
                Mathf.PerlinNoise(transform.position.z, Time.time) - 0.5f
            );
            rb.AddTorque(tilt * 0.3f, ForceMode.Acceleration);
        }

        // Material-specific joint breakdown logic (adds drama!)
        float baseChance = materialType switch
        {
            MaterialType.Wood => 0.05f,
            MaterialType.Plastic => 0.03f,
            MaterialType.Aluminum => 0.015f,
            MaterialType.Concrete => 0.01f,
            MaterialType.Steel => 0.008f,
            _ => 0.02f
        };

        foreach (var joint in joints)
        {
            if (joint != null)
            {
                float chance = Random.value;
                if (chance < submersionDepth * baseChance)
                {
                    Destroy(joint);
                    Debug.Log($"{gameObject.name} joint failed underwater.");
                }
            }
        }
    }
    else
    {
        // Out of water = reset damping
        rb.linearDamping = 0.2f;
        rb.angularDamping = 0.05f;
    }
}




    // Change color based on stress
    /*   if (currentStress > 0.8f)
           rend.material.color = highStressColor;
       else if (currentStress > 0.5f)
           rend.material.color = mediumStressColor;
       else
           rend.material.color = stableColor;
    */
    }
}
