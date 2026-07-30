using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject homepagePanel;
    [SerializeField] private GameObject settingsPanel;

    // Call this when pressing "Play" button
    public void PlayGame()
    {
        SceneManager.LoadScene("TutorialView"); //Goes to the tutorial scene
    }

    // Call this when pressing "Settings" button
    public void OpenSettings()
    {
        homepagePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    // Call this when pressing "Back" button inside Settings
    public void OpenHomepage()
    {
        settingsPanel.SetActive(false);
        homepagePanel.SetActive(true);
    }

    // Call this when pressing "Quit" button
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit"); // Shows in Unity Editor
    }
}