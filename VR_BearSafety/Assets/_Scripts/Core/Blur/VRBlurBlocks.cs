using UnityEngine;

[RequireComponent(typeof(Collider))]
public class VRBlurBlocks : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VRBlur.Instance?.HeavyBlur();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VRBlur.Instance?.ClearBlur();
        }
    }

}
