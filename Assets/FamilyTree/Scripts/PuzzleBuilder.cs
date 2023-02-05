using System.Collections.Generic;
using UnityEngine;

public class PuzzleBuilder : MonoBehaviour
{
    public CorkboardGUI corkboard;

    public CorkboardGUIPhoto prefabPhoto;
    public CorkboardGUIFamilyNameTag prefabNameTag;

    public List<FamilyMemberData> familyMembers;

    // Start is called before the first frame update
    void Start()
    {
        if (!corkboard)
        {
            corkboard = CorkboardGUI.FetchActiveCorkboard();
        }

        List<CorkboardGUIItem> newItems = new List<CorkboardGUIItem>();

        // Begin propagating.
        foreach(FamilyMemberData fam in familyMembers)
        {
            CorkboardGUIFamilyNameTag nameTag = Instantiate(prefabNameTag, corkboard.transform);
            nameTag.Setup(fam);
            newItems.Add(nameTag);

            CorkboardGUIPhoto photo = Instantiate(prefabPhoto, corkboard.transform);
            photo.Setup(fam);
            newItems.Add(photo);
        }

        Shuffle(newItems);

        corkboard.AttachItemsToBoard(newItems);
    }

    public static void Shuffle<T>(IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }
}
