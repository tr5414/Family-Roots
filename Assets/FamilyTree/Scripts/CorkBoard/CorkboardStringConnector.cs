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

    public Transform midpointTransform;

    public bool hasArrow;
    public float arrowPointOffset = 0.05f;
    public float arrowSize = 0.1f;
    public float arrowAngleDegrees = 130;

    private MeshCollider meshCollider;
    private LineRenderer line;

    private Vector3 lastParentPos;
    private Vector3 lastChildPos;

    private void Awake()
    {
        GameObject newMidPoint = new GameObject("Midpoint");
        newMidPoint.transform.parent = transform;
        midpointTransform = newMidPoint.transform;

        transform.position = Vector3.zero; //this must be at 0,0,0 because of mesh generation
    }

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.enabled = false;

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.enabled = false;

        if (!corkboard)
        {
            corkboard = CorkboardGUI.FetchActiveCorkboard();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (parent.position != lastParentPos || child.position != lastChildPos)
        {
            BuildRope();
        }

        lastParentPos = parent.position;
        lastChildPos = child.position;
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
        midpointTransform.position = midpoint;

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

    public void SetUpForSnip()
    {
        GenerateMeshForCollider();
    }

    private void GenerateMeshForCollider()
    {
        Mesh mesh = new Mesh();
        line.BakeMesh(mesh, true);

        meshCollider.sharedMesh = mesh;
        meshCollider.enabled = true;
    }

    public void WindDownCollision()
    {
        if (meshCollider)
        {
            meshCollider.sharedMesh = null;
            meshCollider.enabled = false;
        }
    }
}
