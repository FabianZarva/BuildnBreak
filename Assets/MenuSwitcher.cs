using UnityEngine;
using UnityEngine.UI;

public class MenuSwitcher : MonoBehaviour
{
    public Button button;
    public GameObject underline;
    public GameObject pageToActivate;
    public GameObject[] otherPages;

    void Start()
    {
        button.onClick.AddListener(SwitchMenu);
    }

    void SwitchMenu()
    {
        if (pageToActivate.activeSelf)
            return;

        pageToActivate.SetActive(true);
        underline.SetActive(true);

        foreach (GameObject page in otherPages)
        {
            page.SetActive(false);
            
            Transform otherUnderline = page.transform.Find("underline");
            if (otherUnderline != null)
                otherUnderline.gameObject.SetActive(false);
        }
    }
}
