using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ResultsScreenPrompt : MonoBehaviour
{
    public TMPro.TextMeshProUGUI resultTitle;

    public TMPro.TextMeshProUGUI textDetails;

    public TMPro.TextMeshProUGUI nameCountText;

    public TMPro.TextMeshProUGUI childCountText;

    public Button confirmButton;

    public string failureTitle;

    [TextArea]
    public string failureDescription;

    public string successTitle;

    [TextArea]
    public string successDescription;

    public string maybeTitle;

    [TextArea]
    public string maybeDescription;

    private int totalNameConnections;
    private int totalChildConnections;

    private int nameErrors;
    private int childErrors;


    public void EvaluateResults()
    {
        CorkboardFamilyTreeNetwork familyTreeNetwork = FindObjectOfType<CorkboardFamilyTreeNetwork>();

        familyTreeNetwork.GradeConnections(out totalChildConnections, out totalNameConnections, out childErrors, out nameErrors);

        PrintResults();

        if (childErrors > 2)
        {
            FailCase();
        }
        else if (childErrors <= 1 && nameErrors < 3)
        {
            SuccessCase();
        }
        else
        {
            MediumCase();
        }

        confirmButton.onClick.AddListener(Exit);
    }
    private void PrintResults()
    {
        nameCountText.text = $"Names: {totalNameConnections - nameErrors} / {totalNameConnections}";
        childCountText.text = $"Relations: {totalChildConnections - childErrors} / {totalChildConnections}";
    }

    private void FailCase()
    {
        resultTitle.text = failureTitle;
        textDetails.text = failureDescription;
    }

    private void MediumCase()
    {
        resultTitle.text = maybeTitle;
        textDetails.text = maybeDescription;
    }

    private void SuccessCase()
    {
        resultTitle.text = successTitle;
        textDetails.text = successDescription;
    }



    public void Exit()
    {
        Application.Quit();
    }

}
