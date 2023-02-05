using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorkboardFamilyTreeNetwork : MonoBehaviour
{
    [SerializeField]
    private CorkboardFamilyTreeConnection templateEmptyConnection;

    private List<CorkboardFamilyTreeConnection> activeConnections = new List<CorkboardFamilyTreeConnection>();

    private HashSet<CorkboardGUIPhoto> familyMemberPhotos = new HashSet<CorkboardGUIPhoto>();
    private List<FamilyMemberData> allfamilyMembers = new List<FamilyMemberData>();

    public void Start()
    {
        PuzzleBuilder pb = FindObjectOfType<PuzzleBuilder>();

        SetupWithFamilyData(pb.familyMembers);
    }

    public void SetupWithFamilyData(IEnumerable<FamilyMemberData> data)
    {
        allfamilyMembers.AddRange(data);
    }

    public void AddFamilyPhoto(CorkboardGUIPhoto photo)
    {
        familyMemberPhotos.Add(photo);
    }
    
    public void MakeConnection(CorkboardGUIPhoto parent, CorkboardGUIPhoto child)
    {
        familyMemberPhotos.Add(parent);
        familyMemberPhotos.Add(child);

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

    public bool GradeConnections(out int totalChildConnections, out int totalNameConnections, out int childErrors, out int nameErrors)
    {
        // Start with name judgements
        totalNameConnections = allfamilyMembers.Count;
        nameErrors = 0;

        foreach (var famfam in allfamilyMembers)
        {
            bool correctName = familyMemberPhotos.Where(photo => photo.familyMember == famfam)
                .Select(photo => photo.activeNameTag)
                .Where(nameTag => nameTag && nameTag.familyMember == famfam)
                .Any();

            if (!correctName)
                nameErrors++;
        }

        //Do children connection judgements
        totalChildConnections = allfamilyMembers.SelectMany(mem => mem.children).Count();
        childErrors = 0;

        foreach (var famfam in allfamilyMembers)
        {
            CorkboardGUIPhoto parentPhoto = familyMemberPhotos.Where(photo => photo.familyMember == famfam).FirstOrDefault();

            foreach (var child in famfam.children)
            {
                CorkboardGUIPhoto childPhoto = familyMemberPhotos.Where(photo => photo.familyMember == child).FirstOrDefault();
                if (!parentPhoto || !childPhoto)
                {
                    childErrors++;
                    continue;
                }

                bool hasChild = activeConnections.Where(conn => conn.Parents.Contains(parentPhoto))
                    .Where(conn => conn.Children.Contains(childPhoto))
                    .Any();

                if (!hasChild)
                    childErrors++;
            }
        }

        return (childErrors == 0) && (nameErrors == 0);
    }

    public void PrintGrade()
    {
        bool success = GradeConnections(out int totalChildConnections, out int totalNameConnections, out int childErrors, out int nameErrors);

        Debug.LogFormat("You have {0} the test.", (success ? "Passed" : "Failed"));

        Debug.LogFormat("Child connections: {0}/{1}", totalChildConnections-childErrors, totalChildConnections);
        Debug.LogFormat("Name connections: {0}/{1}", totalNameConnections - nameErrors, totalNameConnections);
    }
}
