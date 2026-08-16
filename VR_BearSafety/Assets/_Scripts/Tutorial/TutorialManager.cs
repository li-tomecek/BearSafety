using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private List<TutorialPopup> _tutorialPopups;
    private Queue<TutorialPopup> _tutorialQueue;
    private TutorialStep _currentStep;

    [SerializeField] private GameObject _tutorialGameObject;
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _textBox;
    
    public void Start()
    {
        _tutorialQueue =  new Queue<TutorialPopup>(_tutorialPopups);
        TutorialEvent.ActionPerformed += CheckForNextStep;

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

        _textBox.text = pageContents.Dequeue();
        _nextButton.onClick.RemoveAllListeners();

        if (pageContents.Count <= 0)
        {
            //Close the popup
            _nextButton.onClick.AddListener(() => _tutorialGameObject.SetActive(false));
        }
        else
        {
            //"next" button will take you to next page, if there are multiple
            _nextButton.onClick.AddListener(() => DisplayNextPage(pageContents));
        }
    }

    public void ForceDisplayTutorial(TutorialPopup popup)
    {
        DisplayNextPage(new Queue<string>(popup.TutorialPages));
    }
}

public enum TutorialStep
{
    None,
    GrabCan,
    Unclip, 
    BurstSpray
}
