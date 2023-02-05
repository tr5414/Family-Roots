using UnityEngine;

public class InteractableIteractor : MonoBehaviour
{
    private Interactable lastInteractableTargeted;

    public GameObject reticle;

    private void Update()
    {
        bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 200);
        if (isHit)
        {
            var hitTransform = hit.collider.transform;
            var currentInteractableTargeted = hitTransform.GetComponent<Interactable>();
            if (currentInteractableTargeted == null)
            {
                currentInteractableTargeted = hitTransform.GetComponentInParent<Interactable>();
            }

            if (currentInteractableTargeted != lastInteractableTargeted)
            {
                if (currentInteractableTargeted != null)
                {
                    currentInteractableTargeted.BeingLookedAt();
                }
                if (lastInteractableTargeted != null)
                {
                    lastInteractableTargeted.NotBeingLookedAt();
                }
            }

            lastInteractableTargeted = currentInteractableTargeted;

            if (currentInteractableTargeted != null)
            {
                reticle.transform.position = hit.point;
                if (FindObjectOfType<StarterAssets.StarterAssetsInputs>(true).usePrimary)
                {
                    currentInteractableTargeted.Interact();
                }
            }
            reticle.SetActive(currentInteractableTargeted != null);
        }
    }
}
