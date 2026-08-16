using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


[DefaultExecutionOrder(-50)]
public class PauseMenu : Menu<PauseMenu>
{
    [Header("Pause Settings")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Image pauseBackground;

    public event Action OnPauseInputEvent;

    private Color _blackTransparent = new Color(0.0f, 0.0f, 0.0f, 0.4f);

    [Header("Buttons")]
    [SerializeField] private Button resumeGame;
    [SerializeField] private Button openSettingsMenu;
    [SerializeField] private Button returnToMainMenu;

    [SerializeField] private InputActionReference _menuButtonAction;

    protected override void Awake()
    {
        base.Awake();

        pauseBackground.color = _blackTransparent;

        resumeGame.onClick.AddListener(ResumeClicked);
        openSettingsMenu.onClick.AddListener(OpenSettingsMenu);
        returnToMainMenu.onClick.AddListener(ReturnToMainMenu);
        _menuButtonAction.action.Enable();
        _menuButtonAction.action.performed += (v) => FireOnPauseInputEvent();
        
        CloseMenu();
    }

    public void FireOnPauseInputEvent()
    {
        OnPauseInputEvent?.Invoke();

        if (pauseMenu.activeInHierarchy) CloseMenu();
        else OpenMenu();
    }

    private void ResumeClicked()
    {
        OnPauseInputEvent?.Invoke();
        if (pauseMenu.activeInHierarchy) CloseMenu();
    }

    public override void OpenMenu()
    {
        RecenterMenu();
        pauseMenu.SetActive(true);
    }

    public override void CloseMenu()
    {
        pauseMenu.SetActive(false);
    }

    private void OpenSettingsMenu()
    {
        SettingsMenu.Instance?.OpenMenu();
    }

    private void ReturnToMainMenu()
    {
        ConfirmationWindow.Instance.OnConfirmed += SendToMainMenu;
        ConfirmationWindow.Instance.OnDeclined += ClearConfirmationWindowSubscriptions;
        ConfirmationWindow.Instance?.OpenMenu();
    }

    private void ClearConfirmationWindowSubscriptions()
    {
        ConfirmationWindow.Instance.OnConfirmed -= SendToMainMenu;
        ConfirmationWindow.Instance.OnDeclined -= ClearConfirmationWindowSubscriptions;
    }

    private void SendToMainMenu()
    {
        CloseMenu();

        ClearConfirmationWindowSubscriptions();

        SceneService.Instance?.LoadScene("MainMenu");
    }

    protected override void OnDestroy()
    {
        _menuButtonAction.action.performed -= (v) => FireOnPauseInputEvent();
        _menuButtonAction.action.Disable();
        base.OnDestroy();
    }
}
