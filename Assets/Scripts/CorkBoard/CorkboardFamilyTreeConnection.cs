using System.Collections.Generic;
using UnityEngine;

public class CorkboardFamilyTreeConnection : MonoBehaviour
{
    [SerializeField]
    private List<CorkboardGUIPhoto> parents = new List<CorkboardGUIPhoto>();
    [SerializeField]
    private List<CorkboardGUIPhoto> children = new List<CorkboardGUIPhoto>();

    [SerializeField]
    private CorkboardGUI corkboard;

    public CorkboardStringConnector parentChildTemplate;
    public CorkboardStringConnector spouseTemplate;

    private List<CorkboardStringConnector> spawnedConnections = new List<CorkboardStringConnector>();

    // Start is called before the first frame update
    void Start()
    {
        if (corkboard == null)
        {
            Debug.LogError("Missing corkboard");
        }

        UpdateRelationship();
    }

    /*
    public bool TryAddChild(CorkboardGUIPhoto child)
    {

    }

    public bool TryAddChild(CorkboardGUIPhoto parent)
    {
        if (parent1 != null && parent2 != null)
        {
            Debug.LogErrorFormat("Cannot add third parent to connection. Parent={0}", parent);
            return false;
        }
    }
    */

    public void UpdateRelationship()
    {
        DestroyConnections();

        if (parents.Count == 0 || children.Count == 0)
        {
            return;
        }

        if (parents.Count > 2)
        {
            Debug.LogErrorFormat("{0} connection has three parents. Error?!", name);
        }

        // Single parent direct connects all children.
        if (parents.Count == 1)
        {
            CorkboardGUIPhoto singleParent = parents[0];
            foreach (CorkboardGUIPhoto child in children)
            {
                CorkboardStringConnector childConnector = Instantiate(parentChildTemplate);
                childConnector.corkboard = corkboard;
                childConnector.parent = singleParent.pinpoint.transform;
                childConnector.child = child.pinpoint.transform;

                spawnedConnections.Add(childConnector);
            }
        }

        else if (parents.Count == 2)
        {
            CorkboardStringConnector spouseConnector = Instantiate(spouseTemplate);
            spouseConnector.corkboard = corkboard;
            spouseConnector.parent = parents[0].pinpoint.transform;
            spouseConnector.child = parents[1].pinpoint.transform;

            spawnedConnections.Add(spouseConnector);

            
            foreach (CorkboardGUIPhoto child in children)
            {
                CorkboardStringConnector childConnector = Instantiate(parentChildTemplate);
                childConnector.corkboard = corkboard;
                childConnector.parent = spouseConnector.transform; // Use the midpoint on the spouseconnector to get children from.
                childConnector.child = child.pinpoint.transform;

                spawnedConnections.Add(childConnector);
            }
        }

    }

    void DestroyConnections()
    {
        foreach(var conn in spawnedConnections)
        {
            if (conn)
                Destroy(conn.gameObject);
        }
        spawnedConnections.Clear();
    }

    private void OnDestroy()
    {
        DestroyConnections();
    }
}
