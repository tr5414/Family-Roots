using UnityEngine;
using UnityEngine.Events;

public class UnityEventInteractable : Interactable
{
    public UnityEvent OnInteract;

    public override void Interact()
    {
        Debug.Log($"Interacted! {name}");
        OnInteract.Invoke();
    }
}
