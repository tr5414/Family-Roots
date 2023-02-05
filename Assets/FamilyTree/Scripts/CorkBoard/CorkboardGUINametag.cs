public class CorkboardGUINameTag : CorkboardGUIItem
{
    public TMPro.TextMeshPro nameText;

    public void Setup(FamilyMemberData memberData)
    {
        nameText.text = memberData.fullName;
    }
}
