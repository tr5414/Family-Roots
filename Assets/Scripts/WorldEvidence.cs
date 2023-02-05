using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EvidenceReference))]
public class WorldEvidence : MonoBehaviour
{
    private EvidenceReference evidenceReference;
    [SerializeField] private GameObject closeUp;

    protected void Awake()
    {
        evidenceReference = GetComponent<EvidenceReference>();
    }

    public void OnInteract()
    {
        EvidenceCloseup.Instance.Show(closeUp);
    }
}
