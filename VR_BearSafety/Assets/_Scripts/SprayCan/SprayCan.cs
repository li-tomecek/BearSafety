using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SprayCan : GrabInteractable
{
    [Header("Visual Config")] [SerializeField]
    private float _sprayRange;

    [SerializeField] private Transform _sprayOrigin;
    [SerializeField] ParticleSystem _sprayFX;
    [SerializeField] Outline _hoverOutline;

    [Header("Spray Config")] [SerializeField]
    private float _defaultAcceptanceAngle;

    [SerializeField] private float _totalSprayDuration = 8; //typically canisters have 7-9 seconds of spray time
    [SerializeField] private LayerMask _sprayTargetLayerMask;
    //[SerializeField] private float _targetBurstDuration = 2;

    [Header("Additional Input detection")] [SerializeField]
    private InputActionReference _rightHandPress;

    [SerializeField] private InputActionReference _leftHandPress;

    private enum Hand
    {
        None,
        Left,
        Right
    };

    private Hand _heldHand = Hand.None;

    private bool _isHeld, _isUnclipped;
    private float _remainingSeconds; //how much spray left in the can based on time.

    #region Start & Update

    protected override void Start()
    {
        base.Start();
        _sprayFX?.Stop();
        _hoverOutline.enabled = false;
        _remainingSeconds = _totalSprayDuration;

        _rightHandPress.action.Enable();
        _rightHandPress.action.performed += (v) => TryButton(Hand.Right);
        _leftHandPress.action.performed += (v) => TryButton(Hand.Left);
        
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        _rightHandPress.action.Disable();
        _rightHandPress.action.performed -= (v) => TryButton(Hand.Right);
        _leftHandPress.action.performed -= (v) => TryButton(Hand.Left);
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
    #endregion

    #region Interactable Actions      
    protected override void OnGrab(SelectEnterEventArgs arg0)
    {
        if (_isHeld == true) return;

        _isHeld = true;
        CheckWhichHand(arg0.interactorObject);
    }

    protected override void OnDrop(SelectExitEventArgs arg0)
    {
        //check for anchors?
        _isHeld = false;
        OnDeactivate(null);
    }

    protected override void OnActivate(ActivateEventArgs arg0)
    {
        //Particle Effects
        if(_isUnclipped && _remainingSeconds > 0)
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
    public void CheckWhichHand(IXRSelectInteractor interactor)
    {
        if (interactor.handedness == InteractorHandedness.Left)
        {
            Debug.Log("[SprayCan] grabbed by left hand");
            _heldHand = Hand.Left;

        }
        else if (interactor.handedness == InteractorHandedness.Right)
        {
            Debug.Log("[SprayCan] grabbed by right hand");
            _heldHand = Hand.Right;

        }
        else
        {
            Debug.Log("[SprayCan] Could not determine of held by left or right hand.");
            _heldHand = Hand.None;
        }
        
    }
    
    private void TryButton(Hand hand)
    {
        if (_isHeld && _isUnclipped == false)
        {
            //unclip
            Debug.Log("[SprayCan] Unclipping trigger.");
        }
        
        else if (_isHeld && _isUnclipped && hand == _heldHand)
        {
            //Spray
            OnActivate(null);
        }
    }

    public void Unclip()
    {
        
    }

    #region Accessors / Mutators
    public bool IsHeld => _isHeld;
    public bool IsSpraying => _sprayFX.isPlaying;
    public float RemainingSprayTime => _remainingSeconds;
    public float RemainingSprayPercent => _remainingSeconds / _totalSprayDuration;
    #endregion

}
