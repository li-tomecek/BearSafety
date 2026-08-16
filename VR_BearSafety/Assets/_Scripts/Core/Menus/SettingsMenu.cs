using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : Menu<SettingsMenu>
{
    [Header("Settings UI")]
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private GameObject[] panels;
    [SerializeField] private Button[] backButtons;
    private GameObject _activePanel = null;

    #region Volume Vars
    [Header("Volume Settings")]
    [SerializeField] private TextMeshProUGUI masterVolumeText;
    [SerializeField] private Slider masterVolumeSlider;

    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private Slider musicVolumeSlider;

    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private Slider sfxVolumeSlider;

    private float _currentMasterVolume;
    private float _currentMusicVolume;
    private float _currentSFXVolume;
    #endregion

    private RectTransform rectTransform;
    private Vector3 startPosition;
    private Coroutine currentMoveCoroutine;


    protected override void Awake()
    {
        base.Awake();

        foreach (Button button in backButtons)
        {
            button.onClick.AddListener(SaveSettings);
            button.onClick.AddListener(CloseMenu);
        }

        foreach (GameObject panel in panels)
        {
            panel.SetActive(false);
        }

        rectTransform = settingsMenu.GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;

        masterVolumeSlider.onValueChanged.AddListener((value) => UpdateVolume(value, AudioMixerKeys.MasterVolumeKey, ref _currentMasterVolume, ref masterVolumeText));
        musicVolumeSlider.onValueChanged.AddListener((value) => UpdateVolume(value, AudioMixerKeys.MusicVolumeKey, ref _currentMusicVolume, ref musicVolumeText));
        sfxVolumeSlider.onValueChanged.AddListener((value) => UpdateVolume(value, AudioMixerKeys.SFXVolumeKey, ref _currentSFXVolume, ref sfxVolumeText));
        
        CloseMenu();
    }

    private void Start()
    {
        LoadSettings();
    }

    private void OnEnable()
    {
        PauseMenu.Instance.OnPauseInputEvent += CloseMenu;
        PauseMenu.Instance.OnPauseInputEvent += RecenterMenu;
        
    }

    private void OnDisable()
    {
        PauseMenu.Instance.OnPauseInputEvent -= CloseMenu;
        PauseMenu.Instance.OnPauseInputEvent -= RecenterMenu;
    }

    #region Save/Load
    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(AudioMixerKeys.MasterVolumeKey, _currentMasterVolume);
        PlayerPrefs.SetFloat(AudioMixerKeys.MusicVolumeKey, _currentMusicVolume);
        PlayerPrefs.SetFloat(AudioMixerKeys.SFXVolumeKey, _currentSFXVolume);
        PlayerPrefs.Save();
    }

    private void LoadSettings()
    {
        _currentMasterVolume = PlayerPrefs.GetFloat(AudioMixerKeys.MasterVolumeKey, 100.0f);
        _currentMusicVolume = PlayerPrefs.GetFloat(AudioMixerKeys.MusicVolumeKey, 100.0f);
        _currentSFXVolume = PlayerPrefs.GetFloat(AudioMixerKeys.SFXVolumeKey, 100.0f);

        masterVolumeSlider.value = _currentMasterVolume;
        musicVolumeSlider.value = _currentMusicVolume;
        sfxVolumeSlider.value = _currentSFXVolume;

        UpdateVolume(_currentMasterVolume, AudioMixerKeys.MasterVolumeKey, ref _currentMasterVolume, ref masterVolumeText);
        UpdateVolume(_currentMusicVolume, AudioMixerKeys.MusicVolumeKey, ref _currentMusicVolume, ref musicVolumeText);
        UpdateVolume(_currentSFXVolume, AudioMixerKeys.SFXVolumeKey, ref _currentSFXVolume, ref sfxVolumeText);
    }
    #endregion

    public override void OpenMenu()
    {
        settingsMenu.SetActive(true);
    }

    public void OpenPanel(int index)
    {
        if (_activePanel != null) return;

        if (index >= 0 && index < panels.Length)
        {
            panels[index].SetActive(true);
            _activePanel = panels[index];
        }
    }

    public override void CloseMenu()
    {
        if (_activePanel != null)
        {
            _activePanel.SetActive(false);
            _activePanel = null;
            return;
        }

        settingsMenu.SetActive(false);
    }

    private void CloseAll()
    {
        if (_activePanel != null)
        {
            _activePanel.SetActive(false);
            _activePanel = null;
        }

        settingsMenu.SetActive(false);
    }

    private void UpdateVolume(float value, string keyName, ref float currentValue, ref TextMeshProUGUI text)
    {
        currentValue = value;
        text.text = keyName + ": " + (int)(currentValue * 100.0f);

        AudioManager.Instance?.SetMixerVolume(keyName, currentValue);
    }
}
