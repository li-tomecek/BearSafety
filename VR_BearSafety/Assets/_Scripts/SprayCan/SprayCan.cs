using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using DG.Tweening;
using UnityEngine.UI;

public class SprayCan : GrabInteractable
{
    private static SprayCan Instance;
    
    [Header("Visual Config")] [SerializeField]
    private float _sprayRange;
    [SerializeField] private Transform _sprayOrigin;
    [SerializeField] ParticleSystem _sprayFX;   //pressurized steam effect
    [SerializeField] ParticleSystem _trailFX;     //Burst where the spray actually hits the target (if hitting a target)
    [SerializeField] Outline _hoverOutline;
    [SerializeField] Image _sprayCapacity;

    [Header("Spray Config")] [SerializeField]
    private float _defaultAcceptanceAngle;
    [SerializeField] private float _totalSprayDuration = 8; //typically canisters have 7-9 seconds of spray time
    [SerializeField] private LayerMask _sprayTargetLayerMask;
    //[SerializeField] private float _targetBurstDuration = 2;

    [Header("Reload Config")]
    [Tooltip("Set to -1 to NEVER have it reload")]
    [SerializeField] private float _timeUntilSprayReload = 3.0f;
    [SerializeField] private float _timeToReload = 3.0f;
    private float _timeUntilLastSpray = 0.0f;

    [Header("Unclip Config")] 
    [SerializeField] private GameObject _clipGameObject;
    [SerializeField] private Transform _clipTargetPosition;
    private Transform _defaultClipPosition;

    [Header("Bear Config")]
    [SerializeField] private float _requiredHitDuration = 4.0f;
    
    [Header("Additional Input detection")]
    [SerializeField] private InputActionReference _rightHandPress;
    [SerializeField] private InputActionReference _leftHandPress;

    [SerializeField] private float _tutorialRequiredSprayTime = 4f;
    private float _totalTimeHit;

    private AudioSource _audioSource;


    private enum Hand
    {
        None,
        Left,
        Right
    };

    private Hand _heldHand = Hand.None;

    private bool _isHeld, _isUnclipped;
    private float _remainingSeconds; //how much spray left in the can based on time.

    public void Reset()
    {
        _sprayFX?.Stop();
        _trailFX.Stop();
        _clipGameObject.SetActive(true);
        _clipGameObject.transform.SetPositionAndRotation(_defaultClipPosition.position, _defaultClipPosition.rotation);
        _isHeld = false;
        _isUnclipped = false;
        _totalTimeHit = 0;

        _hoverOutline.enabled = false;
        UpdateRemainingSprayTime(_totalSprayDuration);
    }
    
    #region Start & Update
    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    protected override void Start()
    {
        base.Start();
        _sprayFX?.Stop();
        _trailFX?.Stop();
        
        _rightHandPress.action.Enable();
        _rightHandPress.action.performed += (v) => TryButton(Hand.Right);
        _leftHandPress.action.performed += (v) => TryButton(Hand.Left);
        
        _rightHandPress.action.canceled += (v) => TryRelease(Hand.Right);
        _leftHandPress.action.canceled += (v) => TryRelease(Hand.Left);

        _defaultClipPosition = _clipGameObject.transform;
        Reset();

        _audioSource = GetComponent<AudioSource>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _rightHandPress.action.Disable();
        _rightHandPress.action.performed -= (v) => TryButton(Hand.Right);
        _leftHandPress.action.performed -= (v) => TryButton(Hand.Left);
        
        _rightHandPress.action.canceled -= (v) => TryRelease(Hand.Right);
        _leftHandPress.action.canceled -= (v) => TryRelease(Hand.Left);
    }

    void Update()
    {
        Spray();
        ReloadSpray();
    }

    #region UpdateFunctions
    private void Spray()
    {
        if ((_isHeld && _sprayFX.isPlaying) == false) { _audioSource.Stop(); return; }

        UpdateRemainingSprayTime(_remainingSeconds - Time.deltaTime);
        if (_remainingSeconds <= 0)      //Check for remaining spray time
        {
            _audioSource.Stop();
            _sprayFX.Stop();
            _trailFX.Stop();
        }
        else                            //Check for spray targets (bear)
        {
            if (!_audioSource.isPlaying) { _audioSource.Play(); }

            Ray ray = new Ray(_sprayOrigin.position, _sprayOrigin.forward);

            Debug.DrawRay(ray.origin, ray.direction * _sprayRange, Color.red, 1f);
            if (Physics.Raycast(ray, out RaycastHit hit, _sprayRange, _sprayTargetLayerMask))
            {
                SprayTarget target = hit.collider.GetComponentInParent<SprayTarget>();
                if (target != null)
                {
                    float accuracy = GetAccuracy(target.TargetTransform, target.AcceptedAngleDeviation);
                    //Debug.Log($"Spray Target Hit: {target.name} | Accuracy: {accuracy}");
                    SetTrailPositionAndSize(
                        Mathf.Min((target.TargetTransform.position - _sprayOrigin.position).magnitude, _sprayRange), 
                        4f);
                    if (accuracy > 0.0f)
                    {
                        _totalTimeHit += Time.deltaTime;
                        if(_totalTimeHit >= _tutorialRequiredSprayTime)
                                TutorialEvent.ReportAction(TutorialStep.BurstSpray);
                    }

                    if (target.TryGetComponent(out BearController bear))
                    {
                        bear.TakeDamage((bear.MaxHealth / _requiredHitDuration) * Time.deltaTime);
                    }
                }
                else
                {
                    SetTrailPositionAndSize(_sprayRange,1.7f);
                }
            }
            else
            {
                SetTrailPositionAndSize(_sprayRange,1.7f);
            }
        }
    }

    private void ReloadSpray()
    {
        if (_timeUntilSprayReload == -1.0f) return;

        if (_sprayFX.isPlaying)
        {
            _timeUntilLastSpray = 0.0f;
            return;
        }

        _timeUntilLastSpray += Time.deltaTime;

        if (_timeUntilLastSpray < _timeUntilSprayReload) return;

        float refillRate = _totalSprayDuration / _timeToReload;
        UpdateRemainingSprayTime(_remainingSeconds + refillRate * Time.deltaTime);
    }
    #endregion

    #endregion

    #region Interactable Actions      
    protected override void OnGrab(SelectEnterEventArgs arg0)
    {
        if (_isHeld == true) return;

        _isHeld = true;
        CheckWhichHand(arg0.interactorObject);
        TutorialEvent.ReportAction(TutorialStep.GrabCan);
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
        if (_isUnclipped && _remainingSeconds > 0)
        {
            _sprayFX?.Play();
            _trailFX?.Play();
        }
    }

    protected override void OnDeactivate(DeactivateEventArgs arg0)
    {
        //_sprayFX?.Stop();
        TryRelease(Hand.None);
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
        if (_isHeld && _isUnclipped == false && (hand == _heldHand) || (_heldHand == Hand.None))
        {
            //unclip
            Unclip();
        }
        
        else if (_isHeld && _isUnclipped && hand == _heldHand)
        {
            //Spray
            OnActivate(null);
        }
    }

    private void TryRelease(Hand hand)
    {
        if (hand == Hand.None || hand == _heldHand)
        {
            _sprayFX?.Stop();
            _trailFX.Stop();
        }
    }

    public void Unclip()
    {
        Debug.Log("[SprayCan] Unclipping trigger.");
        _clipGameObject.transform.DOMove(_clipTargetPosition.position, 0.7f)
            .OnComplete(() => { 
                _isUnclipped = true; 
                _clipGameObject.SetActive(false);
            });
        
        TutorialEvent.ReportAction(TutorialStep.Unclip);
    }

    private void UpdateRemainingSprayTime(float newAmount)
    {
        _remainingSeconds = newAmount;

        _remainingSeconds = Mathf.Clamp(_remainingSeconds, 0.0f, _totalSprayDuration);

        _sprayCapacity.fillAmount = RemainingSprayPercent;
    }

    private void SetTrailPositionAndSize(float trailOffset, float size)
    {
        var main = _trailFX.main;
        main.startSize = size;
        
        _trailFX.transform.position = _sprayOrigin.position + _sprayOrigin.forward * trailOffset;
        
    }

    #region Accessors / Mutators
    public bool IsHeld => _isHeld;
    public bool IsSpraying => _sprayFX.isPlaying;
    public float RemainingSprayTime => _remainingSeconds;
    public float RemainingSprayPercent => _remainingSeconds / _totalSprayDuration;
    #endregion

}
