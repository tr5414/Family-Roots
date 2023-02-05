using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorkboardFamilyTreeNetwork : MonoBehaviour
{
    [SerializeField]
    private List<CorkboardGUIPhoto> allPhotos;

    [SerializeField]
    private CorkboardFamilyTreeConnection templateEmptyConnection;

    private List<CorkboardFamilyTreeConnection> activeConnections = new List<CorkboardFamilyTreeConnection>();

    public void MakeConnection(CorkboardGUIPhoto parent, CorkboardGUIPhoto child)
    {
        if (SearchForBadCases(parent, child))
        {
            return;
        }

        CorkboardFamilyTreeConnection existingChildConnection = FindConnectionWithChildPhoto(child);

        if (!existingChildConnection)
        {
            // Find a connection with the single Parent.
            CorkboardFamilyTreeConnection existingSingleParentConnection = FindConnectionWithSingleParentPhoto(parent);

            // We must create a brand new connection with both parent and child
            if (existingSingleParentConnection)
            {
                existingSingleParentConnection.TryAddChild(child);
            }
            else
            {
                CorkboardFamilyTreeConnection connect = CreateNewConnection(parent);
                connect.TryAddChild(child);
            }
        }
        else
        {
            if (existingChildConnection.Parents.Count >= 2)
            {
                // Too many parents. Bail here.
                return;
            }

            CorkboardGUIPhoto parent2 = existingChildConnection.Parents.First();
            if (parent == parent2)
            {
                return; // Nothing to do. Already connected
            }

            CorkboardFamilyTreeConnection existingDualParentConnection = FindConnectionWithDualParentPhotos(parent, parent2);

            if (existingDualParentConnection)
            {
                existingChildConnection.TryRemoveChild(child);
                existingDualParentConnection.TryAddChild(child);
            }
            else
            {
                existingChildConnection.TryRemoveChild(child);
                CorkboardFamilyTreeConnection connect = CreateNewConnection(parent, parent2);
                connect.TryAddChild(child);
            }
        }
    }

    private bool SearchForBadCases(CorkboardGUIPhoto parent, CorkboardGUIPhoto child)
    {
        // grandparent paradox
        HashSet<CorkboardGUIPhoto> explored = new HashSet<CorkboardGUIPhoto>();
        HashSet<CorkboardGUIPhoto> toExplore = new HashSet<CorkboardGUIPhoto>();
        toExplore.Add(child);
        do
        {
            CorkboardGUIPhoto nextExplore = toExplore.FirstOrDefault();
            explored.Add(nextExplore);
            toExplore.Remove(nextExplore);

            IEnumerable<CorkboardGUIPhoto> batch = activeConnections.Where(conn => conn.Parents.Contains(nextExplore))
            .SelectMany(conn => conn.Children)
            .Where(chil => !explored.Contains(chil));

            toExplore.UnionWith(batch);

            if (toExplore.Contains(parent))
            {
                return true;
            }
        }
        while (toExplore.Any());

        return false;
    }

    private CorkboardFamilyTreeConnection FindConnectionWithChildPhoto(CorkboardGUIPhoto child)
    {
        IEnumerable<CorkboardFamilyTreeConnection> connections = activeConnections.Where(conn => conn.Children.Contains(child));
        if (connections.Count() > 1)
        {
            Debug.LogError("Illegal state: child photo has multiple connections. Breaking all connections with child.");
            foreach(var killConnection in connections.ToList())
            {
                killConnection.TryRemoveChild(child);
            }
            return null;
        }

        return connections.FirstOrDefault();
    }

    private CorkboardFamilyTreeConnection FindConnectionWithSingleParentPhoto(CorkboardGUIPhoto singleParent)
    {
        IEnumerable<CorkboardFamilyTreeConnection> connections = activeConnections.Where(conn => conn.Parents.Count == 1 && conn.Parents.Contains(singleParent));
        if (connections.Count() > 1)
        {
            Debug.LogError("Illegal state: single parent photo has multiple connections.");
        }

        return connections.FirstOrDefault();
    }

    private CorkboardFamilyTreeConnection FindConnectionWithDualParentPhotos(CorkboardGUIPhoto parentOne, CorkboardGUIPhoto parentTwo)
    {
        IEnumerable<CorkboardFamilyTreeConnection> connections = activeConnections.Where(conn => conn.Parents.Contains(parentOne) && conn.Parents.Contains(parentTwo));
        if (connections.Count() > 1)
        {
            Debug.LogError("Illegal state: single parent photo has multiple connections.");
        }

        return connections.FirstOrDefault();
    }

    private CorkboardFamilyTreeConnection CreateNewConnection(CorkboardGUIPhoto parent1, CorkboardGUIPhoto parent2 = null)
    {
        CorkboardFamilyTreeConnection connection = Instantiate(templateEmptyConnection, transform);
        connection.SetParents(parent1, parent2);
        activeConnections.Add(connection);

        return connection;
    }

    public void BreakConnection(CorkboardStringConnector connector)
    {
        CorkboardFamilyTreeConnection connection = activeConnections.Where(conn => conn.GetSpawnedConnections().Contains(connector)).FirstOrDefault();

        List<CorkboardGUIPhoto> photos = new List<CorkboardGUIPhoto>();
        photos.Add(connector.parent.GetComponentInParent<CorkboardGUIPhoto>());
        photos.Add(connector.child.GetComponentInParent<CorkboardGUIPhoto>());

        photos.RemoveAll(p => p == null);
        
        if (connection.Parents.IsSupersetOf(photos)) // Oops all parents
        {
            connection.TryRemoveChildren();
        }
        else
        {
            foreach (CorkboardGUIPhoto photo in photos)
            {
                connection.TryRemoveChild(photo);
            }
        }
    }


    public void PrepareForSnips()
    {
        foreach (CorkboardFamilyTreeConnection connection in activeConnections)
        {
            foreach (var strings in connection.GetSpawnedConnections())
                strings.SetUpForSnip();
        }
    }

    public void WindDownSnips()
    {
        foreach (CorkboardFamilyTreeConnection connection in activeConnections)
        {
            foreach (var strings in connection.GetSpawnedConnections())
                strings.WindDownCollision();
        }
    }
}
