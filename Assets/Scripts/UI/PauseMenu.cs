using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance => FindObjectOfType<PauseMenu>(true);
    public static bool IsActive { get; private set; }

    protected void OnEnable()
    {
        IsActive = true;
        PlayerControlLocking.Instance.Lock(this);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    protected void OnDisable()
    {
        IsActive = false;
        PlayerControlLocking.Instance?.Unlock(this);
    }
}
