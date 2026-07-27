using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationWindow : Menu<ConfirmationWindow>
{
    [Header("General")]
    [SerializeField] private GameObject panel;

    [Header("Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button declineButton;

    public event Action OnConfirmed;
    public event Action OnDeclined;


    private void Start()
    {
        confirmButton.onClick.AddListener(() => { CloseMenu(); OnConfirmed?.Invoke(); });
        declineButton.onClick.AddListener(() => { CloseMenu(); OnDeclined?.Invoke(); });
    }

    private void OnEnable()
    {
        PauseMenu.Instance.OnPauseInputEvent += CloseMenu;
    }

    private void OnDisable()
    {
        PauseMenu.Instance.OnPauseInputEvent -= CloseMenu;
    }

    public override void OpenMenu()
    {
        panel.SetActive(true);
    }

    public override void CloseMenu()
    {
        panel.SetActive(false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        confirmButton.onClick.RemoveAllListeners();
        declineButton.onClick.RemoveAllListeners();
    }
}
