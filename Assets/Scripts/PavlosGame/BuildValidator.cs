using UnityEngine;

public static class BuildValidator
{
    public static bool CanPlace(Vector3 position, Vector3 normal)
    {
        if (Vector3.Angle(normal, Vector3.up) > 80f)
        {
            Debug.Log("Placement rejected: surface too steep.");
            return false;
        }

        // Small vertical offset to keep box above the surface you're placing on
        Vector3 offset = new Vector3(0, 0.25f, 0);  // tweak height based on your cube scale
        Vector3 halfExtents = new Vector3(0.45f, 0.4f, 0.45f);
        Collider[] hits = Physics.OverlapBox(position + offset, halfExtents);

        foreach (Collider c in hits)
        {
            if (!c.isTrigger)
            {
                Debug.Log("Placement rejected: overlapping with '" + c.gameObject.name + "'");
                return false;
            }
        }

        Debug.Log("Placement allowed.");
        return true;
    }
}
