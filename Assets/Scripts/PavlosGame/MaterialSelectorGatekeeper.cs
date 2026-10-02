using UnityEngine;
using UnityEngine.UI;
using PavlosGame;

namespace PavlosGame
{
    public class MaterialSelectorGatekeeper : MonoBehaviour
    {
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color blockedColor = new Color(1f, 0.2f, 0.2f, 0.5f);

        public void UpdateSlotVisual(GameObject slotObject, MaterialType type)
        {
            var img = slotObject.GetComponent<Image>();
            var btn = slotObject.GetComponent<Button>();

            int placed = BlockLimitManager.Instance.GetPlaced(type);
            int limit = BlockLimitManager.Instance.GetLimit(type);

            bool isBlocked = (limit <= 0 || placed >= limit);

            if (img != null)
                img.color = isBlocked ? blockedColor : normalColor;
            if (btn != null)
                btn.interactable = !isBlocked;
        }
    }
}