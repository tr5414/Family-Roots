using UnityEngine;
using UnityEngine.Serialization;

public class CorkboardGUIPhoto : CorkboardGUIItem
{
    [FormerlySerializedAs("pinpoint")]
    public Transform pinPoint;
    public Transform nametagPoint;

    public MeshRenderer meshRenderer;

    public CorkboardGUINameTag activeNameTag;

    public void Setup(FamilyMemberData memberData)
    {
        meshRenderer.material = memberData.photo;
    }
}
