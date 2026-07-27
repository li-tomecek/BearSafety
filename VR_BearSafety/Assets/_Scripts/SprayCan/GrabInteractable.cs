using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Mediator class btwn interactable objects and the XRGrabInteractable class.
/// Pretty useless overall, but eliminates the requirement of manually assigning actions in-editor.
/// </summary>

[RequireComponent(typeof(XRGrabInteractable))]
public abstract class GrabInteractable : MonoBehaviour
{
    protected XRGrabInteractable _xrGrabber;

    protected virtual void Start()
    {
        _xrGrabber = GetComponent<XRGrabInteractable>();
        
        //Note: can also do this manually in-editor
        _xrGrabber.activated.AddListener(OnActivate);             
        _xrGrabber.deactivated.AddListener(OnDeactivate);
       
        _xrGrabber.selectEntered.AddListener(OnGrab);
        _xrGrabber.selectExited.AddListener(OnDrop);

        _xrGrabber.hoverEntered.AddListener(OnHoverStart);
        _xrGrabber.hoverExited.AddListener(OnHoverEnd);
    }

    protected virtual void OnDestroy()
    {
        _xrGrabber.activated.RemoveAllListeners();
        _xrGrabber.deactivated.RemoveAllListeners();

        _xrGrabber.selectEntered.RemoveAllListeners();
        _xrGrabber.selectExited.RemoveAllListeners();

        _xrGrabber.hoverEntered.RemoveAllListeners();
        _xrGrabber.hoverExited.RemoveAllListeners();
    }
   
    protected virtual void OnGrab(SelectEnterEventArgs arg0){}
    protected virtual void OnDrop(SelectExitEventArgs arg0){}
    
    protected virtual void OnActivate(ActivateEventArgs arg0){}
    protected virtual void OnDeactivate(DeactivateEventArgs arg0){}

    protected virtual void OnHoverStart(HoverEnterEventArgs arg0){}
    protected virtual void OnHoverEnd(HoverExitEventArgs arg0){}

}
