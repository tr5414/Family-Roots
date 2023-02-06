using UnityEngine;

public class CorkboardGUIFamilyNameTag : CorkboardGUIItem
{
    public TMPro.TextMeshPro nameText;

    private CorkboardGUIPhoto attachedToPhoto;

    private CorkboardGUIPhoto hoveringPhoto;

    private CorkboardGUI corkboard;

    public FamilyMemberData familyMember { get; private set; }

    private const string ERROR_FULLNAME = "ERROR_";

    public AudioSource audioOnPickup;
    public AudioSource audioOnPlace;

    private void Start()
    {
        corkboard = CorkboardGUI.FetchActiveCorkboard();
    }

    public void Setup(FamilyMemberData memberData)
    {
        if (string.IsNullOrWhiteSpace(memberData.fullName))
        {
            Debug.LogErrorFormat("Missing name for family member data: {0}", memberData.name);
            nameText.text = ERROR_FULLNAME + memberData.name;
        }
        else
        {
            nameText.text = memberData.fullName;
        }

        familyMember = memberData;
    }

    public void CheckToAttach()
    {
        if (hoveringPhoto)
        {
            attachedToPhoto = hoveringPhoto;
            attachedToPhoto.activeNameTag = this;
            transform.position = attachedToPhoto.nametagPoint.position;
            transform.parent = attachedToPhoto.transform;
            audioOnPlace?.Play();
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
        audioOnPickup?.Play();
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
