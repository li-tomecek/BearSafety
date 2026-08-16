using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TutorialPopup", menuName = "Scriptable Objects/TutorialPopup")]
public class TutorialPopup : ScriptableObject
{
    [SerializeField] public List<string> TutorialPages;
    [SerializeField] public TutorialStep Step;
}
