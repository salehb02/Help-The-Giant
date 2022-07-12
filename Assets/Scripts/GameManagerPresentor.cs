using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameManagerPresentor : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenuPanel;
    public Button startButton;

    [Header("Lose Panel")]
    public GameObject losePanel;
    public TextMeshProUGUI loseTitle;
    public Button restartButton;
    private Vector3 _initLoseTitleScale;

    [Header("Win Panel")]
    [Space(2)]
    public GameObject winPanel;
    public TextMeshProUGUI winTitle;
    public Button nextLevelButton;
    private Vector3 _initWinTitleScale;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = GetComponent<GameManager>();
        _initWinTitleScale = winTitle.transform.localScale;
        _initLoseTitleScale = loseTitle.transform.localScale;

        startButton.onClick.AddListener(_gameManager.StartGame);
        restartButton.onClick.AddListener(_gameManager.RestartLevel);
        nextLevelButton.onClick.AddListener(_gameManager.NextLevel);

        SetMainMenuActivation(true);
        SetWinPanelActivation(false);
        SetLosePanelActivation(false);
    }

    public void SetMainMenuActivation(bool active)
    {
        mainMenuPanel.SetActive(active);
    }

    public void SetWinPanelActivation(bool active)
    {
        if(!active)
        {
            winPanel.SetActive(false);
            return;
        }

        winPanel.SetActive(true);
        nextLevelButton.gameObject.SetActive(false);
        winTitle.transform.localScale = Vector3.zero;
        winTitle.transform.DOScale(_initWinTitleScale, 1f).OnComplete(() =>
        {
            nextLevelButton.gameObject.SetActive(true);
        });
    }

    public void SetLosePanelActivation(bool active)
    {
        if (!active)
        {
            losePanel.SetActive(false);
            return;
        }

        losePanel.SetActive(true);
        restartButton.gameObject.SetActive(false);
        loseTitle.transform.localScale = Vector3.zero;
        loseTitle.transform.DOScale(_initLoseTitleScale, 1f).OnComplete(() =>
        {
            restartButton.gameObject.SetActive(true);
        });
    }
}