using UnityEngine;

namespace PavlosGame
{
    public enum MaterialType { Wood, Concrete, Steel, Plastic, Aluminum }

    public class MaterialProperties : MonoBehaviour
    {
        public MaterialType materialType;
    }
}   