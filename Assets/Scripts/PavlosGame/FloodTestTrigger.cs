using UnityEngine;

namespace PavlosGame
{
    public class FloodTestTrigger : MonoBehaviour
    {
        public Flood floodSystem;
        public KeyCode triggerKey = KeyCode.F;

        void Update()
        {
            if (Input.GetKeyDown(triggerKey) && floodSystem != null)
            {
                Debug.Log("[FloodTestTrigger] Flood started!");
                floodSystem.StartFlood();
            }
        }
    }
}
