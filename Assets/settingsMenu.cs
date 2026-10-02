using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;  // Slider for adjusting the volume
    public Button returnButton;  // Button to return to the main menu

    private void Start()
    {
        // Load saved volume from PlayerPrefs or set it to 1 (max) by default
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);

        // Add listeners to the slider and return button
        volumeSlider.onValueChanged.AddListener(UpdateVolume);
        returnButton.onClick.AddListener(ReturnToMainMenu);
    }

    // Function to update the game's volume
    void UpdateVolume(float volume)
    {
        AudioListener.volume = volume;  // Set the global volume
        PlayerPrefs.SetFloat("Volume", volume);  // Save the volume setting
    }

    // Function to return to the main menu
    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Replace with the actual name of your main menu scene
    }
}
