using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject[] highlights;  // References to just the Highlight objects

    public enum MaterialType { Wood, Concrete, Steel, Plastic, Aluminum }
    public MaterialType selectedMaterial;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad1)) SelectMaterial(0);
        if (Input.GetKeyDown(KeyCode.Keypad2)) SelectMaterial(1);
        if (Input.GetKeyDown(KeyCode.Keypad3)) SelectMaterial(2);
        if (Input.GetKeyDown(KeyCode.Keypad4)) SelectMaterial(3);
        if (Input.GetKeyDown(KeyCode.Keypad5)) SelectMaterial(4);
    }

    void SelectMaterial(int index)
    {
        for (int i = 0; i < highlights.Length; i++)
        {
            highlights[i].SetActive(i == index); // Only activate the selected one
        }

        selectedMaterial = (MaterialType)index;
        Debug.Log("Selected Material: " + selectedMaterial);
    }
}
