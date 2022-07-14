using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public const string MonsterLayer = "Monster";
    public const string PlayerLayer = "Player";
    public const string CurrentLevel = "CURRENT_LEVEL";

    public bool IsPaused { get; private set; }

    private GameManagerPresentor _presentor;



    private void Awake()
    {
        if (SceneManager.GetActiveScene().buildIndex != PlayerPrefs.GetInt(CurrentLevel, 0))
            SceneManager.LoadScene(PlayerPrefs.GetInt(CurrentLevel, 0));
    }

    private void Start()
    {
        IsPaused = true;
        _presentor = GetComponent<GameManagerPresentor>();
    }

    public void StartGame()
    {
        IsPaused = false;
        _presentor.SetMainMenuActivation(false);
    }

    public void WinGame()
    {
        IsPaused = true;
        _presentor.SetWinPanelActivation(true);
    }

    public void LoseGame()
    {
        IsPaused = true;
        _presentor.SetLosePanelActivation(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLevel()
    {
        var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        if (currentLevelIndex < SceneManager.sceneCountInBuildSettings - 1)
            currentLevelIndex++;
        else
            currentLevelIndex = UnityEngine.Random.Range(0, SceneManager.sceneCountInBuildSettings);

        PlayerPrefs.SetInt(CurrentLevel, currentLevelIndex);
        SceneManager.LoadScene(currentLevelIndex);
    }
}