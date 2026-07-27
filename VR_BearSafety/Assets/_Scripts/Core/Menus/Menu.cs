using UnityEngine;
using UnityEngine.UI;

public abstract class Menu<T> : Singleton<T> where T : MonoBehaviour
{
    [SerializeField] private AudioClip clickSFX;

    private Button[] _menuButtons;


    protected override void Awake()
    {
        base.Awake();

        _menuButtons = GetComponentsInChildren<Button>(true);

        foreach (Button button in _menuButtons)
        {
            button.onClick.AddListener(PlayClickSFX);
        }
    }

    private void PlayClickSFX()
    {
        AudioManager.Instance?.PlaySound(clickSFX);
    }

    public abstract void OpenMenu();
    public abstract void CloseMenu();
}
