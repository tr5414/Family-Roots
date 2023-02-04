using System.Collections.Generic;
using UnityEngine;

public class CorkboardGUI : MonoBehaviour
{
    [SerializeField] private StarterAssets.StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private Vector3 preferredRelativeForwardVector = Vector3.forward;

    public float UseTimeout = 0.35f;

    private Collider[] corkboardColliders;
    private HashSet<CorkboardGUIElement> elements = new HashSet<CorkboardGUIElement>();

    private float _useTimeoutDelta = 0f;

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

        corkboardColliders = GetComponents<Collider>();

        CorkboardGUIElement[] elmList = GetComponentsInChildren<CorkboardGUIElement>();
        elements.UnionWith(elmList);

        AttachElementsToBoard();
    }

    private void AttachElementsToBoard()
    {
        if (corkboardColliders.Length == 0)
        {
            Debug.LogError("Unable to find colliders for the corkboard. Bailing out.");
            return;
        }

        foreach(CorkboardGUIElement elm in elements)
        {
            Vector3 attachPosition = Vector3.positiveInfinity;
            foreach(Collider collide in corkboardColliders)
            {
                Vector3 testAttachPosition = collide.ClosestPoint(elm.transform.position);

                if (attachPosition.sqrMagnitude > testAttachPosition.sqrMagnitude)
                    attachPosition = testAttachPosition;
            }

            elm.transform.position = attachPosition;

            elm.transform.forward = transform.rotation * preferredRelativeForwardVector;
        }
    }

    void Update()
    {
        if (starterAssetsInputs.sprint && _useTimeoutDelta <= 0f)
        {
            _useTimeoutDelta = UseTimeout;
            if (currentState == BoardState.Idle)
            {
                IdleUseCheck();
            }
            else if (currentState == BoardState.MovingItem)
            {
                currentState = BoardState.Idle;
                movingItem = null;
            }
        }
        else
        {
            _useTimeoutDelta -= Time.deltaTime;
        }

        if (currentState == BoardState.MovingItem)
        {
            RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, Camera.main.transform.forward, 100.0f);

            foreach(RaycastHit hit in hits)
            {
                if (hit.collider.transform == transform)
                {
                    Vector3 newPosition = hit.point;
                    movingItem.transform.position = newPosition;
                }
            }
        }
    }

    private void IdleUseCheck()
    {
        bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 100.0f);
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
