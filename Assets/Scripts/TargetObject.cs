using UnityEngine;

public class TargetObject : MonoBehaviour
{
    public Material normalMaterial;
    public Material hoverMaterial;
    public Material selectedMaterial;

    private Renderer objectRenderer;
    private bool isSelected = false;

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.material = normalMaterial;
    }

    public void SetHover(bool hover)
    {
        if (isSelected) return;

        objectRenderer.material = hover ? hoverMaterial : normalMaterial;
    }

    public void Select()
    {
        isSelected = true;
        objectRenderer.material = selectedMaterial;
    }

    public void ResetTarget()
    {
        isSelected = false;
        objectRenderer.material = normalMaterial;
    }
}