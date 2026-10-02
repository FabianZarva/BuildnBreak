using UnityEditor;
using PavlosGame;

namespace PavlosGame.Editor
{
    [CustomEditor(typeof(Fracture))]
    [CanEditMultipleObjects]
    public class FractureEditor : UnityEditor.Editor
    {
        // Empty editor required for custom property drawers to work properly
    }
}
