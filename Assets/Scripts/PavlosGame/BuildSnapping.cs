using UnityEngine;
using PavlosGame;

namespace PavlosGame
{
    public static class BuildSnapping
    {
        public static float snapRange = 1.5f;

        public static void AlignToConnector(ref Vector3 position, ref Vector3 normal, bool preferFlatSurface = true)
        {
            Collider[] nearby = Physics.OverlapSphere(position, snapRange);
            foreach (Collider col in nearby)
            {
                Connector connector = col.GetComponent<Connector>();
                if (connector != null)
                {
                    Vector3 toConnector = (connector.transform.position - position);
                    float dot = Vector3.Dot(toConnector.normalized, normal);

                    // Only snap if the connector is beneath the surface (not beside or above)
                    if (dot < -0.9f || !preferFlatSurface)
                    {
                        position = connector.transform.position;
                        normal = connector.transform.forward;
                        return;
                    }
                }
            }


            // Default grid snap
            position = new Vector3(Mathf.Round(position.x), Mathf.Round(position.y), Mathf.Round(position.z));
            normal = Vector3.up;
        }

    }
}
