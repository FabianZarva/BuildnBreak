using UnityEngine;
using System.Collections;
using PavlosGame;

namespace PavlosGame
{
    public class Earthquake : MonoBehaviour
    {
        
        [Range(1.0f, 10.0f)]
        public float richterMagnitude = 5.0f;
        public float shakeDuration = 4f;
        public float tremorInterval = 0.15f;
        public float shakeVariation = 0.3f;
        public float verticalShakeFactor = 0.2f;
        public int numberOfAftershocks = 2;
        public float aftershockDelayMin = 2f;
        public float aftershockDelayMax = 5f;

        private bool isShaking = false;

        public static Earthquake Instance;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject); // optional: prevent duplicates
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isShaking)
            {
                StartCoroutine(TriggerEarthquakeWithAftershocks());
            }
        }

        IEnumerator TriggerEarthquakeWithAftershocks()
        {
            BuildModeController.Instance.Block("earthquake");

            yield return StartCoroutine(StartEarthquake(richterMagnitude, shakeDuration));

            for (int i = 0; i < numberOfAftershocks; i++)
            {
                float delay = Random.Range(aftershockDelayMin, aftershockDelayMax);
                yield return new WaitForSeconds(delay);

                float reducedMagnitude = richterMagnitude - Random.Range(0.5f, 1.5f);
                float reducedDuration = shakeDuration * 0.5f;
                yield return StartCoroutine(StartEarthquake(Mathf.Clamp(reducedMagnitude, 2f, 9f), reducedDuration));
            }

            isShaking = false;
            BuildModeController.SetBlocked(false);

        }
public IEnumerator StartEarthquake(float magnitude, float duration)
{
    Object.FindAnyObjectByType<EarthquakeUIManager>()?.StartDisplay(duration);

    isShaking = true;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        float force = ConvertRichterToForce(magnitude, elapsedTime / duration);
        var allRigidbodies = Object.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);

        foreach (var rb in allRigidbodies)
        {
            if (!rb.isKinematic)
            {
                // Shake direction per object
                Vector3 shake = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-0.2f, 0.2f),
                    Random.Range(-1f, 1f)
                ).normalized;

                // Scale shake by force and block mass
                Vector3 impulse = shake * force / Mathf.Max(rb.mass, 1f);

                // Apply as velocity change (non-deprecated)
                rb.linearVelocity += impulse;
            }
        }

        elapsedTime += tremorInterval;
        yield return new WaitForSeconds(tremorInterval);
    }

    isShaking = false;
}



        float ConvertRichterToForce(float magnitude, float progress)
        {
            float baseForce = 1.5f;
            float scaledMagnitude = Mathf.Pow(2.0f, magnitude - 3.0f);
            float waveFactor = Mathf.Sin(progress * Mathf.PI);

            return Mathf.Clamp(baseForce * scaledMagnitude * waveFactor, 0f, 10f);
        }

        Vector3 GetRealisticShakeDirection()
        {
            float horizontal = Random.Range(-1f, 1f);
            float depth = Random.Range(-1f, 1f);
            float vertical = Random.Range(-verticalShakeFactor, verticalShakeFactor);
            return new Vector3(horizontal, vertical, depth).normalized;
        }

        public void TriggerEarthquake()
        {
            if (!isShaking)
                StartCoroutine(TriggerEarthquakeWithAftershocks());
        }
    }
}