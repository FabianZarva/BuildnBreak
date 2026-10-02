using UnityEngine;
using System.Collections.Generic;

namespace PavlosGame{
[CreateAssetMenu(fileName = "MaterialLimitConfig", menuName = "GameConfigs/MaterialLimitConfig")]
public class MaterialLimitConfig : ScriptableObject
{
    public List<MaterialLimit> materialLimits;

    [System.Serializable]
    public struct MaterialLimit
    {
        public MaterialType type;
        public int limit;
    }

    public int GetLimit(MaterialType type)
    {
        foreach (var item in materialLimits)
        {
            if (item.type == type) return item.limit;
        }
        return 0;
    }
}
}