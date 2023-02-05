using UnityEngine;

public class CorkboardGUINameTag : CorkboardGUIItem
{
    public TMPro.TextMeshPro nameText;

    private CorkboardGUIPhoto attachedToPhoto;

    private CorkboardGUIPhoto hoveringPhoto;

    private CorkboardGUI corkboard;

    private void Start()
    {
        corkboard = CorkboardGUI.FetchActiveCorkboard();
    }

    public void Setup(FamilyMemberData memberData)
    {
        nameText.text = memberData.fullName;
    }

    public void CheckToAttach()
    {
        if (hoveringPhoto)
        {
            attachedToPhoto = hoveringPhoto;
            attachedToPhoto.activeNameTag = this;
            transform.position = attachedToPhoto.nametagPoint.position;
            transform.parent = attachedToPhoto.transform;
        }
    }

    public void Detach()
    {
        if (attachedToPhoto)
        {
            attachedToPhoto.activeNameTag = null;
            attachedToPhoto = null;
        }
        transform.parent = corkboard.transform;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CorkboardGUIPhoto photo = collision.collider.GetComponent<CorkboardGUIPhoto>();

        if (photo && !attachedToPhoto && !(photo.activeNameTag))
        {
            Debug.Log("Blarghle!" + photo);
            hoveringPhoto = photo;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (hoveringPhoto && hoveringPhoto.transform == collision.collider.transform)
        {
            Debug.Log("Gargle!");
            hoveringPhoto = null;
        }

    }
}
