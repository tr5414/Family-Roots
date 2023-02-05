using UnityEngine;
using UnityEngine.UI;

public class ReticuleCanvas : MonoBehaviour
{
    public Image basicReticule;
    public Image scissorsReticule;

    public void Start()
    {
        basicReticule.enabled = true;
        scissorsReticule.enabled = false;
    }


    public void SetScissorsMode(bool scissors)
    {
        //basicReticule.enabled = !scissors;
        scissorsReticule.enabled = scissors;
    }
}
