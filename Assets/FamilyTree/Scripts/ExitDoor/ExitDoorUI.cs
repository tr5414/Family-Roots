using UnityEngine;
using UnityEngine.UI;

public class ExitDoorUI : MonoBehaviour
{
    public Canvas canvasObject;

    public GameObject exitQuestionContainer;

    public Button exitQuestionConfirmButton;
    public Button exitQuestionCancelButton;

    public GameObject fadeOutContainer;

    public GameObject resultsContainer;

    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    private PlayerControlLocking controlLock;

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

        exitQuestionCancelButton.onClick.AddListener(Hide);
        controlLock = FindObjectOfType<PlayerControlLocking>();
    }
    public void PromptWithQuestion()
    {
        exitQuestionContainer.SetActive(true);
        fadeOutContainer.SetActive(false);
        resultsContainer.SetActive(false);

        controlLock.Lock(this);
        canvasObject.gameObject.SetActive(true);
    }

    public void PromptWithResults()
    {
        // TODO
    }

    public void Hide()
    {
        exitQuestionContainer.SetActive(false);
        fadeOutContainer.SetActive(false);
        resultsContainer.SetActive(false);

        controlLock.Unlock(this);
        canvasObject.gameObject.SetActive(false);
    }

    private void EvaluateResults()
    {
        // TODO
    }
}
