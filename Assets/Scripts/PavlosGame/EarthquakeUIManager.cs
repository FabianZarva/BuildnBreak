using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace PavlosGame
{
    public class EarthquakeUIManager : MonoBehaviour
    {
        public TMP_Text powerText;
        public TMP_Text timerText;

        private float totalDuration = 0f;
        private float elapsedTime = 0f;
        private bool isActive = false;

        public void StartDisplay(float duration)
        {
            totalDuration = duration;
            elapsedTime = 0f;
            isActive = true;
        }

        public void StopDisplay()
        {
            isActive = false;
            powerText.text = "Power: 0%";
            timerText.text = "Time Left: 0.0s";
        }

        void Update()
        {
            if (!isActive) return;

            elapsedTime += Time.deltaTime;

            // Calculate power percentage using sinusoidal curve (same as waveFactor)
            float progress = Mathf.Clamp01(elapsedTime / totalDuration);
            float waveFactor = Mathf.Sin(progress * Mathf.PI);
            float powerPercent = waveFactor * 100f;

            powerText.text = $"Power: {powerPercent:F0}%";
            timerText.text = $"Time Left: {(totalDuration - elapsedTime):F1}s";

            if (elapsedTime >= totalDuration)
            {
                StopDisplay();
            }
        }
    }
}