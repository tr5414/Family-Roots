using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Interactable : MonoBehaviour
{
    //private Renderer[] renders;
    //private Material[] regMaterial;
    //public Material HighlightMaterial;

    public void Awake()
    {
        //renders = GetComponentsInChildren<Renderer>();
        //regMaterial = new Material[renders.Length];
        //for (int i = 0; i < renders.Length; i++)
        //{
        //    regMaterial[i] = renders[i].material;
        //}
    }
    public virtual void Interact()
    {
        
    }

    public void BeingLookedAt()
    {
        //for (int i = 0; i < renders.Length; i++)
        //{
        //    renders[i].material = HighlightMaterial;
        //}
    }
    public void NotBeingLookedAt()
    {
        //for (int i = 0; i < renders.Length; i++)
        //{
        //    renders[i].material = HighlightMaterial;
        //}
    }
}
