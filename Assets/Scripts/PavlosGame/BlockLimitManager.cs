using System.Collections.Generic;
using UnityEngine;
using PavlosGame;

namespace PavlosGame
{
    public class BlockLimitManager : MonoBehaviour
    {
        public static BlockLimitManager Instance { get; private set; }

        [SerializeField] private MaterialLimitConfig levelConfig;
        private readonly Dictionary<MaterialType, int> placedCounts = new();

        private void Awake()
        {
            Instance = this;
            foreach (MaterialType type in System.Enum.GetValues(typeof(MaterialType)))
                placedCounts[type] = 0;
        }

        public void RegisterPlacement(MaterialType type) => placedCounts[type]++;

        public int GetPlaced(MaterialType type) =>
            placedCounts.TryGetValue(type, out var count) ? count : 0;

        public int GetLimit(MaterialType type) =>
            levelConfig?.GetLimit(type) ?? 0;

        public bool IsLimitReached(MaterialType type) =>
            GetPlaced(type) >= GetLimit(type);
    }
}
