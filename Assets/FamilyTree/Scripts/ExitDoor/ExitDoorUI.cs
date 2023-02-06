using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitDoorUI : MonoBehaviour
{
    public Canvas canvasObject;

    public GameObject exitQuestionContainer;

    public Button exitQuestionConfirmButton;
    public Button exitQuestionCancelButton;

    public GameObject fadeOutContainer;

    public GameObject resultsContainer;
    public ResultsScreenPrompt resultsScreen;

    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    private PlayerControlLocking controlLock;

    private bool isMidPrompt = false;

    void Awake()
    {
        canvasObject.gameObject.SetActive(false);
    }
    void Start()
    {
        if (starterAssetsInputs == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            starterAssetsInputs = playerObj.GetComponent<StarterAssets.StarterAssetsInputs>();
        }

        controlLock = FindObjectOfType<PlayerControlLocking>();

        exitQuestionCancelButton.onClick.AddListener(Hide);
        exitQuestionConfirmButton.onClick.AddListener(PromptWithResults);
    }
    public void PromptWithQuestion()
    {
        if (isMidPrompt)
            return;

        exitQuestionContainer.SetActive(true);
        fadeOutContainer.SetActive(false);
        resultsContainer.SetActive(false);

        controlLock.Lock(this);
        canvasObject.gameObject.SetActive(true);

        isMidPrompt = true;
    }

    public void PromptWithResults()
    {
        //if (isMidPrompt)
        //    return;

        resultsScreen.EvaluateResults();
        exitQuestionContainer.SetActive(false);
        fadeOutContainer.SetActive(true);
        resultsContainer.SetActive(true);

        //controlLock.Lock(this);
        canvasObject.gameObject.SetActive(true);

        isMidPrompt = true;
    }

    public void Hide()
    {
        exitQuestionContainer.SetActive(false);
        fadeOutContainer.SetActive(false);
        resultsContainer.SetActive(false);

        controlLock.Unlock(this);
        canvasObject.gameObject.SetActive(false);

        isMidPrompt = false;
    }
}
