using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SprayCan : GrabInteractable
{
    [Header("Visual Config")]
    [SerializeField] private float _sprayRange;
    [SerializeField] private Transform _sprayOrigin;
    [SerializeField] ParticleSystem _sprayFX;
    [SerializeField] Outline _hoverOutline;

    [Header("Spray Config")]
    [SerializeField] private float _defaultAcceptanceAngle;
    [SerializeField] private float _totalSprayDuration = 8; //typically cannisters have 7-9 seconds of spray time
    [SerializeField] private LayerMask _sprayTargetLayerMask;
    //[SerializeField] private float _targetBurstDuration = 2;
    
    private bool _isHeld;
    private float _remainingSeconds;    //how much spray left in the can based on time.

    protected override void Start()
    {
        base.Start();
        _sprayFX?.Stop();
        _hoverOutline.enabled = false;
        _remainingSeconds = _totalSprayDuration;
    }

    void Update()
    {
        if((_isHeld && _sprayFX.isPlaying) == false) return;
    
        _remainingSeconds -= Time.deltaTime;
        if(_remainingSeconds <= 0)      //Check for remaining spray time
        {
            _remainingSeconds = 0;
            _sprayFX.Stop();
        } 
        else                            //Check for spray targets (bear)
        {
            Ray ray = new Ray(_sprayOrigin.position, _sprayOrigin.forward);
            
            Debug.DrawRay(ray.origin, ray.direction * _sprayRange, Color.red, 1f);
            if (Physics.Raycast(ray, out RaycastHit hit, _sprayRange, _sprayTargetLayerMask)) 
            {
                SprayTarget target = hit.collider.GetComponentInParent<SprayTarget>();
                if(target != null)
                {
                    float accuracy = GetAccuracy(target.TargetTransform, target.AcceptedAngleDeviation);
                    Debug.Log($"Spray Target Hit: {target.name} | Accuracy: {accuracy}");
                }
            }
        }

    }

    public float GetAccuracy(Transform target, float acceptanceAngle = -1)
    {
        Vector3 referenceVec = target.position - _sprayOrigin.position;
        float distanceToTarget = referenceVec.magnitude;

        if(distanceToTarget > _sprayRange) return 0;

        Vector3 actualVec = _sprayOrigin.forward * distanceToTarget;

        //Get the angular difference between the actual spray vector and the reference (accurate) one
        //angle = ArcCos((ref * actual)/(||ref||*||actual||))
        float angle = Vector3.Dot(referenceVec, actualVec);
        angle = Mathf.Acos(angle / (distanceToTarget * distanceToTarget)) * Mathf.Rad2Deg;

        if(acceptanceAngle < 0) acceptanceAngle = _defaultAcceptanceAngle;
        
        if(angle > acceptanceAngle) return 0;

        return 1 - (angle/acceptanceAngle);

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
        if(_remainingSeconds > 0)
            _sprayFX?.Play();
    }

    protected override void OnDeactivate(DeactivateEventArgs arg0)
    {
        _sprayFX?.Stop();
    }

    protected override void OnHoverStart(HoverEnterEventArgs arg0)
    {
        if(!_isHeld)
            _hoverOutline.enabled = true;
    }
    
    protected override void OnHoverEnd(HoverExitEventArgs arg0)
    {
        _hoverOutline.enabled = false;
    }
    #endregion

    #region Accessors / Mutators
    public bool IsHeld => _isHeld;
    public bool IsSpraying => _sprayFX.isPlaying;
    #endregion

}
