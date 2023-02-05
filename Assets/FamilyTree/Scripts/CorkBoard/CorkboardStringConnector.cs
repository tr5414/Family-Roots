using UnityEngine;

/// <summary>
/// A single piece of string that has no corelation to anything and only knows transforms.
/// Usually belongs to something else to know how this should connect.
/// 
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class CorkboardStringConnector : MonoBehaviour
{
    public CorkboardGUI corkboard;

    public Transform parent;
    public Transform child;

    public bool hasArrow;
    public float arrowPointOffset = 0.05f;
    public float arrowSize = 0.1f;
    public float arrowAngleDegrees = 130;


    private LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;

        if (!corkboard)
        {
            GameObject corkboardObj = GameObject.FindGameObjectWithTag("Corkboard");
            corkboard = corkboardObj.GetComponent<CorkboardGUI>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 midpoint = (child.position + parent.position) / 2;
        transform.position = midpoint; // Put me at the midpoint, so connections can be made off me.

        BuildRope();

    }

    void BuildRope()
    {
        int positions = 3;
        if (hasArrow)
        {
            positions += 4;
        }
        line.positionCount = positions;

        Vector3 midpoint = (child.position + parent.position) / 2;

        line.SetPosition(0, parent.position);
        line.SetPosition(1, midpoint);
        line.SetPosition(2, child.position);

        if (hasArrow)
        {
            Vector3 stringForwardVector = (child.position - parent.position).normalized;

            Vector3 upVector = corkboard.GetCorkboardForwardVector();

            Vector3 arrowLeftVector = Quaternion.AngleAxis(arrowAngleDegrees, upVector) * stringForwardVector;
            arrowLeftVector *= arrowSize;
            Vector3 arrowRightVector = Quaternion.AngleAxis(-arrowAngleDegrees, upVector) * stringForwardVector;
            arrowRightVector *= arrowSize;

            line.SetPosition(3, child.position - arrowPointOffset * stringForwardVector);
            line.SetPosition(4, child.position + arrowLeftVector - arrowPointOffset*stringForwardVector);
            line.SetPosition(5, child.position + arrowRightVector - arrowPointOffset*stringForwardVector);
            line.SetPosition(6, child.position - arrowPointOffset * stringForwardVector);
        }

        line.enabled = true;
    }
}
