using UnityEngine;
using UnityEngine.UI;


public class MainMenu : Menu<MainMenu>
{
    [Header("Game Scene String")]
    [SerializeField] private string sceneName;


    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;


    protected override void Awake()
    {
        base.Awake();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        playButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    private void StartGame()
    {
        SceneService.Instance?.LoadScene(sceneName);
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
