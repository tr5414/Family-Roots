using UnityEngine;
using UnityEngine.Serialization;

public class CorkboardGUIPhoto : CorkboardGUIItem
{
    [FormerlySerializedAs("pinpoint")]
    public Transform pinPoint;
    public Transform nametagPoint;

    public MeshRenderer meshRenderer;

    public CorkboardGUIFamilyNameTag activeNameTag;

    public FamilyMemberData familyMember { get; private set; }

    public void Setup(FamilyMemberData memberData)
    {
        if (memberData.photo == null)
        {
            Debug.LogErrorFormat("Missing photo material for family member: {0}", familyMember);
        }
        else
        {
            meshRenderer.material = memberData.photo;
        }
        

        familyMember = memberData;
    }
}
