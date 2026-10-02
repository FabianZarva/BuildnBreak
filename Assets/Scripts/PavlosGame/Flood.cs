using System.Collections;
using UnityEngine;

namespace PavlosGame
{
    public class Flood : MonoBehaviour
    {
        public static Flood Instance { get; private set; }


        public GameObject rainPrefab;
        public GameObject waterPrefab;

        public float floodDuration = 15f;
        public float maxFloodHeight = 5f;

        private GameObject activeRain;
        private GameObject activeWater;

        private bool isFlooding = false;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F) && !isFlooding)
            {
                StartFlood();
            }
        }

        public void StartFlood()
        {
            if (!isFlooding)
                StartCoroutine(StartFloodSequence());
        }

        IEnumerator StartFloodSequence()
        {
            isFlooding = true;
            BuildModeController.Instance.Block("flood");


            // Step 1: Start rain
            activeRain = Instantiate(rainPrefab);

            // Step 2: Wait before flood begins
            yield return new WaitForSeconds(4f);
            

            // Step 3: Spawn water
            activeWater = Instantiate(waterPrefab);
            activeWater.transform.position = new Vector3(0, -1f, 0); // Start hidden
            Vector3 startScale = activeWater.transform.localScale;
            Vector3 endScale = new Vector3(startScale.x, maxFloodHeight, startScale.z);

            float riseTime = floodDuration;
            float fallTime = floodDuration;
            float timer = 0f;

            // RISING PHASE
            while (timer < riseTime)
            {
                timer += Time.deltaTime;
                float t = timer / riseTime;

                float newY = Mathf.Lerp(-1f, maxFloodHeight / 2f, t);
                float newHeight = Mathf.Lerp(startScale.y, maxFloodHeight, t);
                float floodTopY = newY + newHeight / 2f;

                activeWater.transform.position = new Vector3(0, newY, 0);
                activeWater.transform.localScale = new Vector3(startScale.x, newHeight, startScale.z);

                if (floodTopY > 0.5f)
                    UpdateFloodDamage(floodTopY);

                yield return null;
            }

            // STAY PEAKED BRIEFLY
            yield return new WaitForSeconds(1f);
            if (activeRain) Destroy(activeRain);

            // FALLING PHASE
            timer = 0f;
            while (timer < fallTime)
            {
                timer += Time.deltaTime;
                float t = timer / fallTime;

                float newY = Mathf.Lerp(maxFloodHeight / 2f, -1f, t);
                float newHeight = Mathf.Lerp(maxFloodHeight, startScale.y, t);
                float floodTopY = newY + newHeight / 2f;

                activeWater.transform.position = new Vector3(0, newY, 0);
                activeWater.transform.localScale = new Vector3(startScale.x, newHeight, startScale.z);

                if (floodTopY > 0.5f)
                    UpdateFloodDamage(floodTopY);

                yield return null;
            }

            // Clean up
            if (activeRain) Destroy(activeRain);
            if (activeWater) Destroy(activeWater);
            BuildModeController.Instance.UnBlock();
            isFlooding = false;
        }

        void UpdateFloodDamage(float floodTopY)
        {
            var visuals = Object.FindObjectsByType<LoadBearingVisualizer>(FindObjectsSortMode.None);

            foreach (var vis in visuals)
            {
                float objY = vis.transform.position.y;
                Debug.Log($"Checking object {vis.name} at height {objY}, floodTopY = {floodTopY}");

                if (objY < floodTopY)
                {
                    float damageFactor = 1f - Mathf.Clamp01((objY + 0.5f) / floodTopY);
                    float multiplier = GetFloodStressMultiplier(vis.materialType, damageFactor);
                    vis.ApplyExternalStress(multiplier);
                }

                vis.UpdateBuoyancy(floodTopY);
            }
        }

        float GetFloodStressMultiplier(MaterialType mat, float factor)
        {
            return mat switch
            {
                MaterialType.Wood => 0.6f * factor,
                MaterialType.Plastic => 0.4f * factor,
                MaterialType.Aluminum => 0.3f * factor,
                MaterialType.Concrete => 0.1f * factor,
                MaterialType.Steel => 0.05f * factor,
                _ => 0.2f * factor
            };
        }
    }
}
