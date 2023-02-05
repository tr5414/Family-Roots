public class CorkboardGUINametag : CorkboardGUIItem
{
    public TMPro.TextMeshPro nameText;

    public void Setup(string name)
    {
        nameText.text = name;
    }
}
