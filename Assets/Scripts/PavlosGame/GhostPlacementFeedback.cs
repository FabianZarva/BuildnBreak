using UnityEngine;
using PavlosGame;

namespace PavlosGame
{
    public class GhostPlacementFeedback : MonoBehaviour
    {
        public static GhostPlacementFeedback Instance { get; private set; }

        [SerializeField] private Color normal, warning, blocked;
        private Material ghostMat;

        private void Awake() => Instance = this;

        public void SetGhost(Material mat) => ghostMat = mat;

        public void UpdateColor(MaterialType type)
        {
            if (ghostMat == null) return;

            int placed = BlockLimitManager.Instance.GetPlaced(type);
            int limit = BlockLimitManager.Instance.GetLimit(type);

            ghostMat.color = (placed >= limit) ? blocked :
                             (placed >= limit - 2) ? warning : normal;
        }
    }
}
