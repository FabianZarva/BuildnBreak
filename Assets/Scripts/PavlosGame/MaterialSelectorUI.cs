using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PavlosGame;

public class MaterialSelectorUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> slotObjects;
    [SerializeField] private List<MaterialType> slotTypes;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color blockedColor = new Color(1f, 0.2f, 0.2f, 0.5f);

    private BuildInputHandler buildInputHandler;
    private int currentIndex = -1;

    void Start()
    {
        buildInputHandler = Object.FindAnyObjectByType<BuildInputHandler>();
        UpdateSlotVisuals();
        AutoSelectFirstAvailable();
    }

    void Update()
    {
        UpdateSlotVisuals();

        for (int i = 0; i < slotObjects.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                TrySelectMaterial(i);
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        TrySelectMaterial(index);
    }

    private void TrySelectMaterial(int index)
    {
    if (buildInputHandler == null) return;

    MaterialType selectedType = slotTypes[index];
        int placed = BlockLimitManager.Instance.GetPlaced(selectedType);
        int limit = BlockLimitManager.Instance.GetLimit(selectedType);

    if (limit <= 0 || placed >= limit)
    {
        Debug.LogWarning($"[MaterialSelectorUI] Cannot select {selectedType}. Limit reached or disabled.");

        // Force refresh to current valid slot (don't fake highlight Aluminum)
        if (currentIndex >= 0)
        {
            HighlightSlot(currentIndex);
            buildInputHandler.SetMaterial(slotTypes[currentIndex]);
        }
        else
        {
            AutoSelectFirstAvailable(); // nothing previously selected
        }
        return;
    }

    SelectMaterial(index);
    }

    private void SelectMaterial(int index)
    {
        if (buildInputHandler == null) return;

        currentIndex = index;
        buildInputHandler.SetMaterial(slotTypes[index]);
        HighlightSlot(index);
    }

    private void HighlightSlot(int selectedIndex)
    {
        for (int i = 0; i < slotObjects.Count; i++)
        {
            Transform highlight = slotObjects[i].transform.Find("highlight");
            if (highlight != null)
            {
                highlight.gameObject.SetActive(i == selectedIndex);
            }
        }
    }

    private void UpdateSlotVisuals()
    {
        if (buildInputHandler == null) return;

        for (int i = 0; i < slotTypes.Count; i++)
        {
            MaterialType type = slotTypes[i];
            int placed = BlockLimitManager.Instance.GetPlaced(type);
            int limit = BlockLimitManager.Instance.GetLimit(type);

            GameObject slot = slotObjects[i];
            Image img = slot.GetComponent<Image>();
            Button btn = slot.GetComponent<Button>();

            bool isBlocked = (limit <= 0 || placed >= limit);

            if (img != null)
                img.color = isBlocked ? blockedColor : normalColor;

            if (btn != null)
                btn.interactable = !isBlocked;
        }
    }

    private void AutoSelectFirstAvailable()
    {
        for (int i = 0; i < slotTypes.Count; i++)
        {
            MaterialType type = slotTypes[i];

            int placed = BlockLimitManager.Instance.GetPlaced(type);
            int limit = BlockLimitManager.Instance.GetLimit(type);

            if (limit > 0 && placed < limit)
            {
                SelectMaterial(i);
                return;
            }
        }

        Debug.LogWarning("[MaterialSelectorUI] No available materials to auto-select.");
    }
}
