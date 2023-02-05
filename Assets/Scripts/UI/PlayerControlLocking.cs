﻿using StarterAssets;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlLocking : MonoBehaviour
{
    public static PlayerControlLocking Instance => FindObjectOfType<PlayerControlLocking>(true);

    private HashSet<object> lockingObjects = new HashSet<object>();

    public void Lock(object key)
    {
        lockingObjects.Add(key);

        Set(true);

    }

    public void Unlock(object key)
    {
        lockingObjects.Remove(key);
        if(lockingObjects.Count == 0)
        {
            Set(false);
        }
    }

    private void Set(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.None : CursorLockMode.Locked;
        FindObjectOfType<StarterAssetsInputs>().cursorLocked = !locked;
        FindObjectOfType<FirstPersonController>().enabled = !locked;

    }
}