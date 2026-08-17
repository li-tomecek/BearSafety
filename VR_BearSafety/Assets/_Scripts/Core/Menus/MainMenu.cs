using UnityEngine;
using UnityEngine.UI;


public class MainMenu : Menu<MainMenu>
{
    [Header("Game Scene String")]
    [SerializeField] private string tutorialSceneName;
    [SerializeField] private string practiceSceneName;


    [Header("Buttons")]
    [SerializeField] private Button tutorialButton;
    [SerializeField] private Button practiceButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;


    protected override void Awake()
    {
        base.Awake();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        tutorialButton.onClick.AddListener(StartTutorial);
        practiceButton.onClick.AddListener(StartPractice);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartTutorial()
    {
        SceneService.Instance?.LoadScene(tutorialSceneName);
    }

    private void StartPractice()
    {
        SceneService.Instance?.LoadScene(practiceSceneName);
    }

    private void OpenSettings()
    {
        SettingsMenu.Instance.OpenMenu();
    }

    private void QuitGame()
    {
        Application.Quit();
    }


    public override void OpenMenu()
    {
    }

    public override void CloseMenu()
    {
    }
}
