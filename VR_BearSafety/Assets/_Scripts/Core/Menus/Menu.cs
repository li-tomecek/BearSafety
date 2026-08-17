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
        var cam = Camera.main;

        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        gameObject.transform.position =
            cam.transform.position + cameraForward * _offsetFromCamera.z;

        gameObject.transform.rotation = Quaternion.LookRotation(cameraForward);
    }

    public abstract void OpenMenu();
    public abstract void CloseMenu();
}
