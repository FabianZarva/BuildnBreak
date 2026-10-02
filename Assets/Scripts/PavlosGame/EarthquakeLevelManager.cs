using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using OpenFracture.Runtime;
using PavlosGame;
using TMPro;
using UnityEngine.SceneManagement;

public class EarthquakeLevelManager : MonoBehaviour
{
    public float buildTime = 120f;
    public float unlockTime = 60f;
    public float winHeight = 5f;

    public GameObject[] unlockableBlocks;
    public Earthquake earthquake;
    public Transform goalZoneMarker;
    public TMP_Text timerText;
    public GameObject winUI;
    public GameObject loseUI;

    private float timer;
    private bool isBuilding = true;

    void Start()
    {
        timer = buildTime;
        winUI.SetActive(false);
        loseUI.SetActive(false);
        StartCoroutine(LevelFlow());
    }

    void Update()
    {
        if (isBuilding)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString() + "s";
        }
    }

    IEnumerator LevelFlow()
    {
        yield return new WaitForSeconds(2f); // intro delay

        // BUILD PHASE
        isBuilding = true;

        // Unlock blocks halfway
        yield return new WaitForSeconds(unlockTime);
        foreach (GameObject block in unlockableBlocks)
            block.SetActive(true); // or mark as buildable in your UI

        yield return new WaitForSeconds(buildTime - unlockTime);

        // TIME UP
        isBuilding = false;
        Earthquake.Instance.TriggerEarthquake(); // use your quake trigger

        yield return new WaitForSeconds(5f); // let the quake shake things

        // EVALUATE
        if (CheckWinCondition())
            winUI.SetActive(true);
        else
            loseUI.SetActive(true);
    }

    bool CheckWinCondition()
    {
        foreach (var block in GameObject.FindGameObjectsWithTag("PlacedBlock"))
        {
            if (block.transform.position.y > winHeight)
                return true;
        }
        return false;
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
