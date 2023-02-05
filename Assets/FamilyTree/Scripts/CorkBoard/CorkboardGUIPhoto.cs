using UnityEngine;
using UnityEngine.Serialization;

public class CorkboardGUIPhoto : CorkboardGUIItem
{
    [FormerlySerializedAs("pinpoint")]
    public Transform pinPoint;
    public Transform nametagPoint;

    public MeshRenderer meshRenderer;

    public void Setup(FamilyMemberData memberData)
    {
        meshRenderer.material = memberData.photo;
    }
}
