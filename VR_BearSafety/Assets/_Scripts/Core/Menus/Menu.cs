using UnityEngine;
using UnityEngine.UI;

public abstract class Menu<T> : Singleton<T> where T : MonoBehaviour
{
    [SerializeField] private AudioClip clickSFX;
    [SerializeField] private Vector3 _offsetFromCamera = new Vector3(0.0f, 0.0f, -1.5f);

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
    
    protected void RecenterMenu()
    {
        gameObject.transform.position = Camera.main.transform.position + _offsetFromCamera;
    }

    public abstract void OpenMenu();
    public abstract void CloseMenu();
}
