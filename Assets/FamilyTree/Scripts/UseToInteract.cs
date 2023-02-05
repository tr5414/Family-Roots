using UnityEngine;
using UnityEngine.Events;

public class UseToInteract : MonoBehaviour
{
    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    public UnityEvent OnUsePrimaryClick;

    bool lastUsePrimary;

    [SerializeField]
    private float raycastRange = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if (starterAssetsInputs == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            starterAssetsInputs = playerObj.GetComponent<StarterAssets.StarterAssetsInputs>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lastUsePrimary != starterAssetsInputs.usePrimary)
        {
            if (starterAssetsInputs.usePrimary)
            {
                RaycastCheck();
            }
        }

        lastUsePrimary = starterAssetsInputs.usePrimary;
    }

    private void RaycastCheck()
    {
        bool isHit = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, raycastRange);
        if (isHit)
        {
            var hitTransform = hit.collider.transform;

            if (hitTransform == transform)
            {
                OnUsePrimaryClick.Invoke();
            }
        }
    }
}
