using StarterAssets;
using System.Collections;
using UnityEngine;


public class EvidenceCloseup : MonoBehaviour
{
    public static EvidenceCloseup Instance => FindObjectOfType<EvidenceCloseup>(true);

    [SerializeField] private Transform evidenceAnchor;
    [SerializeField] private Transform evidenceAnchorX;
    [SerializeField] private Transform evidenceAnchorY;

    GameObject evidenceInstance;

    private StarterAssetsInputs starterAssetsInputs;

    public void Show(GameObject prefab)
    {
        if (evidenceInstance != null)
        {
            Destroy(evidenceInstance);
        }
        evidenceAnchorX.localRotation = Quaternion.identity;
        evidenceAnchorY.localRotation = Quaternion.identity;
        evidenceInstance = Instantiate(prefab, evidenceAnchor);
        evidenceInstance.transform.localScale = Vector3.one/2f;
        evidenceInstance.transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
    }

    protected void OnEnable()
    {
        PlayerControlLocking.Instance?.Lock(this);
    }

    // Update is called once per frame
    protected void Update()
    {
        if (starterAssetsInputs == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            starterAssetsInputs = playerObj.GetComponent<StarterAssets.StarterAssetsInputs>();
        }
        if (starterAssetsInputs != null)
        {
            var delta = starterAssetsInputs.look;
            //evidenceAnchorX.localEulerAngles = new Vector3(Mathf.Clamp(evidenceAnchorX.localEulerAngles.x + delta.y, -90, 90), 0, 0);
            evidenceAnchorY.localEulerAngles = new Vector3(0, evidenceAnchorY.localEulerAngles.y + delta.x*10, 0);
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        if (evidenceInstance != null)
        {
            Destroy(evidenceInstance);
        }
    }

    protected void OnDisable()
    {
        PlayerControlLocking.Instance?.Unlock(this);
        if (evidenceInstance != null)
        {
            Destroy(evidenceInstance);
        }
    }
}
