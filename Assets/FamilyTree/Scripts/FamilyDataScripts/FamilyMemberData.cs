using UnityEngine;

[CreateAssetMenu(fileName = "FamilyMember", menuName = "FamilyTree/FamilyMember", order =66)]
public class FamilyMemberData : ScriptableObject
{
    public string fullName;

    public Material photo;

    public FamilyMemberData[] children;
}
