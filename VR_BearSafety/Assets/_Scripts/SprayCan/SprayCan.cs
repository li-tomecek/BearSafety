using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SprayCan : GrabInteractable
{
    [SerializeField] private float _sprayRange;
    [SerializeField] private float _acceptanceAngle;
    [SerializeField] private Transform _sprayOrigin;
    [SerializeField] ParticleSystem _sprayFX;

    private bool _isHeld;

    public float GetAccuracy(Transform target)
    {
        Vector3 referenceVec = target.position - _sprayOrigin.position;
        float distanceToTarget = referenceVec.magnitude;

        if(distanceToTarget > _sprayRange) return 0;

        Vector3 actualVec = _sprayOrigin.forward * distanceToTarget;

        //Get the angular difference between the actual spray vector and the reference (accurate) one
        //angle = ArcCos((ref * actual)/(||ref||*||actual||))
        float angle = Vector3.Dot(referenceVec, actualVec);
        angle = Mathf.Acos(angle / distanceToTarget * distanceToTarget);

        if(angle > _acceptanceAngle) return 0;

        return 1 - (angle/_acceptanceAngle);

    }

    #region Interactable Actions      
    //TODO: if there are other interactable objects in the sim, make an abstract class with these
    protected override void OnGrab(SelectEnterEventArgs arg0)
    {
        _isHeld = true;
    }

    protected override void OnDrop(SelectExitEventArgs arg0)
    {
        //check for anchors?
        _isHeld = false;
    }

    protected override void OnActivate(ActivateEventArgs arg0)
    {
        //Particle Effects
        _sprayFX?.Play();
    }

    protected override void OnDeactivate(DeactivateEventArgs arg0)
    {
        
    }

    protected override void OnHoverStart(HoverEnterEventArgs arg0)
    {
        //highlight?
    }
    
    protected override void OnHoverEnd(HoverExitEventArgs arg0)
    {
        //end highlight
    }
    #endregion

    #region Accessors / Mutators
    public bool IsHeld => _isHeld;
    #endregion

}
