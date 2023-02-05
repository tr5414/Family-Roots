using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private Renderer render;
    private Material regMaterial;
    public Material HighlightMaterial;

    public void Awake()
    {
        render = GetComponent<Renderer>();
        regMaterial = render.material;

    }
    public virtual void Interact()
    {
        
    }

    public void BeingLookedAt()
    {
        ChangeMaterial(HighlightMaterial);
    }
    public void NotBeingLookedAt()
    {
        ChangeMaterial(regMaterial);
    }
    public void ChangeMaterial(Material mat)
    {
        render.material = mat;
    }
}
