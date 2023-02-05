using System.Collections.Generic;
using UnityEngine;

public class CorkboardGUI : MonoBehaviour
{

    [SerializeField] private CorkboardFamilyTreeNetwork familyNetwork;

    [SerializeField] private Vector3 preferredRelativeForwardVector = Vector3.forward;

    [SerializeField] private Collider corkboardCollider;

    [SerializeField] private float raycastRange = 10f;

    private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    private HashSet<CorkboardGUIItem> elements = new HashSet<CorkboardGUIItem>();

    private bool lastUsePrimaryState = false;
    private bool lastUseSecondaryState = false;

    enum BoardState
    {
        Idle,
        MovingItem,
        MakingConnection
    }

    private BoardState currentState = BoardState.Idle;

    [SerializeField] private CorkboardStringConnector moveableConnection;

    [SerializeField] private Transform moveableConnectionChild = null;

    private CorkboardGUIItem movingItem = null;

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

        if (familyNetwork == null)
        {
            Debug.LogError("Missing family network. Cannot make connections.");
        }

        moveableConnection.corkboard = this;
        moveableConnection.gameObject.SetActive(false);
        moveableConnection.child = moveableConnectionChild;

        CorkboardGUIItem[] elmList = GetComponentsInChildren<CorkboardGUIItem>();
        elements.UnionWith(elmList);

        AttachElementsToBoard();
    }

    public Vector3 GetCorkboardForwardVector()
    {
        return transform.rotation * preferredRelativeForwardVector;
    }

    private void AttachElementsToBoard()
    {
        if (corkboardCollider == null)
        {
            Debug.LogError("Unable to find corkboardCollider. Bailing out.");
            return;
        }

        foreach(CorkboardGUIItem elm in elements)
        {
            Vector3 attachPosition = Vector3.positiveInfinity;
            {
                Vector3 testAttachPosition = corkboardCollider.ClosestPoint(elm.transform.position);

                if (attachPosition.sqrMagnitude > testAttachPosition.sqrMagnitude)
                    attachPosition = testAttachPosition;
            }

            elm.transform.position = attachPosition;
            elm.transform.forward = GetCorkboardForwardVector();
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
            else if (currentState == BoardState.MovingItem)
            {
                MovingItemUseToggleCheck(usePrimary, useSecondary);
            }
            else if (currentState == BoardState.MakingConnection)
            {
                MakingConnectionUseToggleCheck(usePrimary, useSecondary);
            }

            lastUsePrimaryState = starterAssetsInputs.usePrimary;
            lastUseSecondaryState = starterAssetsInputs.useSecondary;
        }

        if (currentState == BoardState.MovingItem)
        {
            MovingItemUpdate();
        }
        else if (currentState == BoardState.MakingConnection)
        {
            MakingConnectionUpdate();
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
                var hitItem = hit.collider.GetComponent<CorkboardGUIItem>();

                if (hitItem && elements.Contains(hitItem))
                {
                    movingItem = hitItem;
                    currentState = BoardState.MovingItem;
                }
            }
        }
        else if (useSecondary)
        {
            bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, raycastRange);
            if (isHit)
            {
                var hitItem = hit.collider.GetComponent<CorkboardGUIPhoto>(); // Only connect photos

                if (hitItem && elements.Contains(hitItem))
                {
                    currentState = BoardState.MakingConnection;
                    moveableConnection.parent = hitItem.transform;
                    moveableConnection.gameObject.SetActive(true);
                    moveableConnectionChild.position = hit.point;
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

    private void MakingConnectionUseToggleCheck(bool usePrimary, bool useSecondary)
    {
        if (usePrimary)
        {
            currentState = BoardState.Idle;
            moveableConnection.gameObject.SetActive(false);
            return;
        }

        if (!useSecondary)
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, raycastRange);

            foreach (RaycastHit hit in hits)
            {
                var hitItem = hit.collider.GetComponent<CorkboardGUIPhoto>(); // Only connect photos

                if (hitItem && moveableConnection.parent != hitItem.transform && elements.Contains(hitItem))
                {
                    CorkboardGUIPhoto parent = moveableConnection.parent.GetComponent<CorkboardGUIPhoto>();
                    CorkboardGUIPhoto child = hitItem.GetComponent<CorkboardGUIPhoto>();
                    familyNetwork.MakeConnection(parent, child);
                }
            }

            currentState = BoardState.Idle;
            moveableConnection.gameObject.SetActive(false);
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

    private void MakingConnectionUpdate()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, raycastRange);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == corkboardCollider)
            {
                Vector3 newPosition = hit.point;
                moveableConnectionChild.position = newPosition;
            }
        }
    }
}
