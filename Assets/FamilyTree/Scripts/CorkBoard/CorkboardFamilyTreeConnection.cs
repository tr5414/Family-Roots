using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorkboardFamilyTreeConnection : MonoBehaviour
{
    public HashSet<CorkboardGUIPhoto> Parents { get; private set; } = new HashSet<CorkboardGUIPhoto>();

    public HashSet<CorkboardGUIPhoto> Children { get; private set; } = new HashSet<CorkboardGUIPhoto>();

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
            GameObject corkboardObj = GameObject.FindGameObjectWithTag("Corkboard");
            if (corkboardObj == null)
            {
                Debug.LogError("Missing corkboard");
            }
            else
            {
                corkboard = corkboardObj.GetComponent<CorkboardGUI>();
            }
        }

        UpdateRelationship();
    }

    public bool TryRemoveChild(CorkboardGUIPhoto child)
    {
        bool removed = Children.Remove(child);
        if (removed)
            UpdateRelationship();
        return removed;
    }

    public bool TryAddChild(CorkboardGUIPhoto child)
    {
        if (Children.Contains(child))
        {
            return false;
        }
        Children.Add(child);
        UpdateRelationship();
        return true;
    }

    public bool SetParents(CorkboardGUIPhoto parent1, CorkboardGUIPhoto parent2)
    {
        if (Parents.Count != 0)
        {
            Debug.LogError("Tried to set parents when parents already set!");
            return false;
        }

        Parents.Add(parent1);
        if (parent2 != null)
            Parents.Add(parent2);

        UpdateRelationship();
        return true;
    }

    public void UpdateRelationship()
    {
        DestroyConnections();

        if (Parents.Count == 0 || Children.Count == 0)
        {
            return;
        }

        if (Parents.Count > 2)
        {
            Debug.LogErrorFormat("{0} connection has three parents. Error?!", name);
        }

        // Single parent direct connects all children.
        if (Parents.Count == 1)
        {
            CorkboardGUIPhoto singleParent = Parents.First();
            foreach (CorkboardGUIPhoto child in Children)
            {
                CorkboardStringConnector childConnector = Instantiate(parentChildTemplate);
                childConnector.corkboard = corkboard;
                childConnector.parent = singleParent.pinpoint.transform;
                childConnector.child = child.pinpoint.transform;

                spawnedConnections.Add(childConnector);
            }
        }

        else if (Parents.Count == 2)
        {
            CorkboardStringConnector spouseConnector = Instantiate(spouseTemplate);
            spouseConnector.corkboard = corkboard;
            CorkboardGUIPhoto[] parentList = Parents.ToArray();
            spouseConnector.parent = parentList[0].pinpoint.transform;
            spouseConnector.child = parentList[1].pinpoint.transform;

            spawnedConnections.Add(spouseConnector);

            
            foreach (CorkboardGUIPhoto child in Children)
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
