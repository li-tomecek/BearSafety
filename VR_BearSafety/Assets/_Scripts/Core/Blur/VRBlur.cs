using UnityEngine;

public class VRBlur : Singleton<VRBlur>
{
    [SerializeField] private Material blurMaterial;



    private void Start()
    {
        ClearBlur();
    }

    private void SetBlur(float amount)
    {
        if (blurMaterial == null)
        {
            Debug.LogWarning("VRBlur: Blur Material is not assigned.");
            return;
        }

        blurMaterial.SetFloat("_BlurAmount", amount);
    }

    public void ClearBlur()
    {
        SetBlur(0.0f);
    }

    public void HeavyBlur()
    {
        SetBlur(15.0f);
    }

    protected override void OnDestroy()
    {
        base.Awake();

        ClearBlur();
    }
}