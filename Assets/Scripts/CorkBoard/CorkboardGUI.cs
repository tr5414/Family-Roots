using System.Collections.Generic;
using UnityEngine;

public class CorkboardGUI : MonoBehaviour
{
    [SerializeField] private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private Vector3 preferredRelativeForwardVector = Vector3.forward;

    [SerializeField] private Collider corkboardCollider;

    [SerializeField] private float raycastRange = 10f;

    //public float UseTimeout = 0.35f;

    private HashSet<CorkboardGUIElement> elements = new HashSet<CorkboardGUIElement>();

    private float _useTimeoutDelta = 0f;

    private bool lastUsePrimaryState = false;
    private bool lastUseSecondaryState = false;

    enum BoardState
    {
        Idle,
        MovingItem
    }

    private BoardState currentState = BoardState.Idle;

    private CorkboardGUIElement movingItem = null;

    void Start()
    {
        if (starterAssetsInputs == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            starterAssetsInputs = playerObj.GetComponent<StarterAssets.StarterAssetsInputs>();
        }

        if (corkboardCollider == null)
        {
            Debug.LogWarning("Corkboard is missing collider. Searching for a replacement. This could cause issues.");
            corkboardCollider = GetComponent<Collider>();
        }


        CorkboardGUIElement[] elmList = GetComponentsInChildren<CorkboardGUIElement>();
        elements.UnionWith(elmList);

        AttachElementsToBoard();
    }

    private void AttachElementsToBoard()
    {
        if (corkboardCollider == null)
        {
            Debug.LogError("Unable to find corkboardCollider. Bailing out.");
            return;
        }

        foreach(CorkboardGUIElement elm in elements)
        {
            Vector3 attachPosition = Vector3.positiveInfinity;
            //foreach(Collider collide in corkboardColliders)
            {
                Vector3 testAttachPosition = corkboardCollider.ClosestPoint(elm.transform.position);

                if (attachPosition.sqrMagnitude > testAttachPosition.sqrMagnitude)
                    attachPosition = testAttachPosition;
            }

            elm.transform.position = attachPosition;

            elm.transform.forward = transform.rotation * preferredRelativeForwardVector;
        }
    }

    void Update()
    {
        if (lastUsePrimaryState != starterAssetsInputs.usePrimary // if primary toggle
            || lastUseSecondaryState != starterAssetsInputs.useSecondary) // if secondary toggle
        {
            bool usePrimary = starterAssetsInputs.usePrimary;
            bool useSecondary = starterAssetsInputs.useSecondary;

            if (currentState == BoardState.Idle)
            {
                IdleUseToggleCheck(usePrimary, useSecondary);
            }

            if (currentState == BoardState.MovingItem)
            {
                MovingItemUseToggleCheck(usePrimary, useSecondary);
            }

            lastUsePrimaryState = starterAssetsInputs.usePrimary;
            lastUseSecondaryState = starterAssetsInputs.useSecondary;
        }

        if (currentState == BoardState.MovingItem)
        {
            MovingItemUpdate();
        }
    }

    private void IdleUseToggleCheck(bool usePrimary, bool useSecondary)
    {
        if (!usePrimary && !useSecondary)
        {
            return;
        }

        if (usePrimary)
        {
            bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, raycastRange);
            if (isHit)
            {
                var hitItem = hit.collider.GetComponent<CorkboardGUIElement>();

                if (hitItem && elements.Contains(hitItem))
                {
                    movingItem = hitItem;
                    currentState = BoardState.MovingItem;
                }
            }
        }
    }

    private void MovingItemUseToggleCheck(bool usePrimary, bool useSecondary)
    {
        if (!usePrimary)
        {
            currentState = BoardState.Idle;
            movingItem = null;
        }
    }

    private void MovingItemUpdate()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, raycastRange);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == corkboardCollider)
            {
                Vector3 newPosition = hit.point;
                movingItem.transform.position = newPosition;
            }
        }
    }
}
