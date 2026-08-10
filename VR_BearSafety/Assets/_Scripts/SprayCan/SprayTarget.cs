using UnityEngine;

public class SprayTarget : MonoBehaviour
{

    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _acceptedAngleDeviation = -1; //if -1, will take the default angle from the spray can.

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_targetTransform == null)
            _targetTransform = transform;

        gameObject.layer = LayerMask.NameToLayer("SprayTarget");
    }

    public Transform TargetTransform => _targetTransform;
    public float AcceptedAngleDeviation => _acceptedAngleDeviation;
}
