using UnityEngine;
using UnityEngine.Events;

public class UseToInteract : MonoBehaviour
{
    private StarterAssets.StarterAssetsInputs starterAssetsInputs;

    public UnityEvent OnUsePrimaryClick;

    bool lastUsePrimary;

    // Start is called before the first frame update
    void Start()
    {
        if (starterAssetsInputs == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            starterAssetsInputs = playerObj.GetComponent<StarterAssets.StarterAssetsInputs>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (lastUsePrimary != starterAssetsInputs.usePrimary)
        {
            if (starterAssetsInputs.usePrimary)
            {
                OnUsePrimaryClick.Invoke();
            }
        }

        lastUsePrimary = starterAssetsInputs.usePrimary;
    }
}
