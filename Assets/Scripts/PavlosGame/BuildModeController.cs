using UnityEngine;
using PavlosGame;

namespace PavlosGame
{
    public class BuildModeController : MonoBehaviour
    {
        public static BuildModeController Instance { get; private set; }
        private static bool _isBlocked = false;
        public static bool IsBlocked => _isBlocked;

        public static void SetBlocked(bool blocked)
        {
            if (blocked)
                Instance.Block("external event");
            else
                Instance.UnBlock();
        }

        private void Awake() => Instance = this;

       

        public void Block(string reason)
        {
            _isBlocked = true;
            GhostPlacer.Instance?.ResetGhost();
            Debug.Log($"[BuildModeController] Build blocked due to {reason}");
        }

        public void UnBlock()
        {
            _isBlocked = false;
            Debug.Log("[BuildModeController] Building re-enabled");
        }

        public bool IsBuildAllowed() => !_isBlocked;
    }
}
