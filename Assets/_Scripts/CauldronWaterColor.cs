using UnityEngine;

public class CauldronWaterColor : MonoBehaviour
{
    [Header("Renderer воды (меш/плэйн)")]
    [SerializeField] private Renderer waterRenderer;

    [SerializeField] private string colorProperty = "_Color";

    private MaterialPropertyBlock mpb;

    void Awake()
    {
        if (!waterRenderer) waterRenderer = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void SetWaterColor(Color c)
    {
        waterRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(colorProperty, c);
        waterRenderer.SetPropertyBlock(mpb);
    }
}
