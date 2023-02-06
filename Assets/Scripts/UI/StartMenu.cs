using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private string initialGameScene;

    public void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(initialGameScene, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}
