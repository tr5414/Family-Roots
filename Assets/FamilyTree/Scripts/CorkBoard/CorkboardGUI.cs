using System.Collections.Generic;
using UnityEngine;

public class CorkboardGUI : MonoBehaviour
{
    [SerializeField] private CorkboardFamilyTreeNetwork familyNetwork;

    [SerializeField] private Vector3 preferredRelativeForwardVector = Vector3.forward;

    [SerializeField] private Collider corkboardCollider;

    [SerializeField] private float raycastRange = 10f;

    private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    private HashSet<CorkboardGUIItem> allItems = new HashSet<CorkboardGUIItem>();

    private bool lastUsePrimaryState = false;
    private bool lastUseSecondaryState = false;

    enum BoardState
    {
        Idle,
        MovingPhoto,
        MakingConnection,
        BreakingConnection,
        MovingNameTag
    }

    private BoardState currentState = BoardState.Idle;

    private CorkboardGUIPhoto movingPhoto = null;

    [SerializeField] private CorkboardStringConnector moveableConnection;
    [SerializeField] private Transform moveableConnectionChild = null;

    private CorkboardGUINameTag movingNameTag = null;

    private ReticuleCanvas reticule;

    public static CorkboardGUI FetchActiveCorkboard()
    {
        CorkboardGUI corkboard = null;

        GameObject corkboardObj = GameObject.FindGameObjectWithTag("Corkboard");
        if (corkboardObj)
            corkboard = corkboardObj.GetComponent<CorkboardGUI>();

        if (!corkboard)
        {
            Debug.LogError("No live corkboard could be found for use.");
        }

        return corkboard;
    }

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

        reticule = FindObjectOfType<ReticuleCanvas>();

        moveableConnection.corkboard = this;
        moveableConnection.gameObject.SetActive(false);
        moveableConnection.child = moveableConnectionChild;

        CorkboardGUIItem[] elmList = GetComponentsInChildren<CorkboardGUIItem>();

        AttachItemsToBoard(elmList);
    }

    public Vector3 GetCorkboardForwardVector()
    {
        return transform.rotation * preferredRelativeForwardVector;
    }

    public void AttachItemsToBoard(IEnumerable<CorkboardGUIItem> items)
    {
        if (corkboardCollider == null)
        {
            Debug.LogError("Unable to find corkboardCollider. Bailing out.");
            return;
        }

        // TODO Can we do better with transforms?
        foreach(CorkboardGUIItem itm in items)
        {
            // Random is too sloppy
            // itm.transform.position += new Vector3(Random.Range(-1, 1), Random.Range(-0.25f, 0.25f), 0);

            Vector3 attachPosition = corkboardCollider.ClosestPoint(itm.transform.position);

            itm.transform.position = attachPosition;
            itm.transform.forward = GetCorkboardForwardVector();
        }

        allItems.UnionWith(items);
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
            else if (currentState == BoardState.MovingPhoto)
            {
                MovingItemUseToggleCheck(usePrimary, useSecondary);
            }
            else if (currentState == BoardState.MakingConnection)
            {
                MakingConnectionUseToggleCheck(usePrimary, useSecondary);
            }
            else if (currentState == BoardState.BreakingConnection)
            {
                BreakingConnectionsUseToggleCheck(usePrimary, useSecondary);
            }
            else if (currentState == BoardState.MovingNameTag)
            {
                MovingNameTagUseToggleCheck(usePrimary, useSecondary);
            }

            lastUsePrimaryState = starterAssetsInputs.usePrimary;
            lastUseSecondaryState = starterAssetsInputs.useSecondary;
        }

        if (currentState == BoardState.MovingPhoto)
        {
            MovingPhotoUpdate();
        }
        else if (currentState == BoardState.MakingConnection)
        {
            MakingConnectionUpdate();
        }
        else if (currentState == BoardState.MovingNameTag)
        {
            MovingNameTagUpdate();
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
                CorkboardGUIItem hitItem = hit.collider.GetComponent<CorkboardGUIItem>();

                if (hitItem && allItems.Contains(hitItem))
                {
                    CorkboardGUIPhoto hitPhoto;
                    if (hitPhoto = hitItem as CorkboardGUIPhoto)
                    {
                        movingPhoto = hitPhoto;
                        currentState = BoardState.MovingPhoto;
                        return;
                    }

                    if (hitItem as CorkboardGUIScissors)
                    {
                        reticule.SetScissorsMode(true);
                        currentState = BoardState.BreakingConnection;
                        familyNetwork.PrepareForSnips();
                        return;
                    }

                    CorkboardGUINameTag hitNameTag;
                    if (hitNameTag = hitItem as CorkboardGUINameTag)
                    {
                        movingNameTag = hitNameTag;
                        movingNameTag.Detach();
                        currentState = BoardState.MovingNameTag;
                        return;
                    }
                }
            }
        }
        else if (useSecondary)
        {
            bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, raycastRange);
            if (isHit)
            {
                var hitItem = hit.collider.GetComponent<CorkboardGUIPhoto>(); // Only connect photos

                if (hitItem && allItems.Contains(hitItem))
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
            movingPhoto = null;
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

                if (hitItem && moveableConnection.parent != hitItem.transform && allItems.Contains(hitItem))
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

    private void BreakingConnectionsUseToggleCheck(bool usePrimary, bool useSecondary)
    {
        if (useSecondary)
        {
            currentState = BoardState.Idle;
            reticule.SetScissorsMode(false);
            familyNetwork.WindDownSnips();
        }
        else if (usePrimary)
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, raycastRange);

            foreach (RaycastHit hit in hits)
            {
                CorkboardStringConnector stringConn = hit.transform.GetComponent<CorkboardStringConnector>();
                if (stringConn)
                {
                    familyNetwork.BreakConnection(stringConn);

                    currentState = BoardState.Idle;
                    reticule.SetScissorsMode(false);
                    familyNetwork.WindDownSnips();
                    return;
                }
            }
        }
    }

    private void MovingNameTagUseToggleCheck(bool usePrimary, bool useSecondary)
    {
        if (!usePrimary || useSecondary)
        {
            currentState = BoardState.Idle;
            movingNameTag.CheckToAttach();
            movingNameTag = null;
        }
    }

    private void MovingPhotoUpdate()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, raycastRange);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == corkboardCollider)
            {
                Vector3 newPosition = hit.point;
                movingPhoto.transform.position = newPosition;
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

    private void MovingNameTagUpdate()
    {
        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, raycastRange);

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == corkboardCollider)
            {
                Vector3 newPosition = hit.point;
                movingNameTag.transform.position = newPosition;
            }
        }
    }
}
