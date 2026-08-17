using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<TutorialPopup> _tutorialPopups = new();
    [SerializeField] private bool _openFirstOnStartup;
    private Queue<TutorialPopup> _tutorialQueue;
    private TutorialStep _currentStep;

    [Header("Popup")]
    [SerializeField] private GameObject _tutorialGameObject;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _buttonText;
    [SerializeField] private TextMeshProUGUI _textBox;
    [SerializeField] private float _charactersPerSecond = 12f;
    [SerializeField] private Vector3 _offsetFromCamera = new();
    
    public void Start()
    {
        _tutorialQueue =  new Queue<TutorialPopup>(_tutorialPopups);
        TutorialEvent.ActionPerformed += CheckForNextStep;
        
        if (_openFirstOnStartup && _tutorialPopups.Count > 0)
        {
            var nextPopup = _tutorialQueue.Dequeue();
            _currentStep = nextPopup.Step;
            RecenterPage();
            DisplayNextPage(new Queue<string>(nextPopup.TutorialPages));
        }
        else
        {
            _tutorialGameObject.SetActive(false);
        }

    }
    public void OnDestroy()
    {
        TutorialEvent.ActionPerformed -= CheckForNextStep;
    }

    public void CheckForNextStep(TutorialStep completedStep)
    {
        if (_tutorialQueue.Count > 0 && completedStep == _currentStep)
        {
            var nextPopup = _tutorialQueue.Dequeue();
            _currentStep = nextPopup.Step;
            RecenterPage();
            DisplayNextPage(new Queue<string>(nextPopup.TutorialPages));
        }
    }

    public void DisplayNextPage(Queue<string> pageContents)
    {
        if (pageContents.Count <= 0)
        {
            Debug.LogWarning("No popup pages available");
            return;
        }
        
        _tutorialGameObject.SetActive(true);

        //_textBox.text = pageContents.Dequeue();
        TypeText(pageContents.Dequeue());
        _nextButton.onClick.RemoveAllListeners();

        if (pageContents.Count <= 0)
        {
            //Close the popup
            _buttonText.text = "Got it";
            _nextButton.onClick.AddListener(() => _tutorialGameObject.SetActive(false));
        }
        else
        {
            //"next" button will take you to next page, if there are multiple
            _buttonText.text = "Next";
            _nextButton.onClick.AddListener(() => DisplayNextPage(pageContents));
        }
    }

    public void ForceDisplayTutorial(TutorialPopup popup)
    {
        RecenterPage();
        DisplayNextPage(new Queue<string>(popup.TutorialPages));
    }

    public void RecenterPage()
    {
        var cam = Camera.main;

        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0f;
        cameraForward.Normalize();

        _tutorialGameObject.transform.position =
            cam.transform.position + cameraForward * _offsetFromCamera.z;

        _tutorialGameObject.transform.rotation = Quaternion.LookRotation(cameraForward);
    }
    
    
    public void TypeText(string message)
    {
        _textBox.DOKill();
        _textBox.text = message;
        _textBox.maxVisibleCharacters = 0;

        float duration = message.Length / _charactersPerSecond;

        DOTween.To(
            () => _textBox.maxVisibleCharacters,
            x => _textBox.maxVisibleCharacters = x,
            message.Length,
            duration
        ).SetEase(Ease.Linear);
    }
}

public enum TutorialStep
{
    None,
    GrabCan,
    Unclip, 
    BurstSpray
}
