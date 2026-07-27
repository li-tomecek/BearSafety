using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : Menu<ControlsMenu>
{
    [Header("General")]
    [SerializeField] private GameObject panel;

    [Header("Buttons")]
    [SerializeField] private Button closeButton;


    private void Start()
    {
        closeButton.onClick.AddListener(() => { CloseMenu(); });
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
        closeButton.onClick.RemoveAllListeners();
    }
}
