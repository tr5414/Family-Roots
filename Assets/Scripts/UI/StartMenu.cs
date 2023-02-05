using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private string initialGameScene;

    public void StartGame()
    {
        SceneManager.LoadScene(initialGameScene, LoadSceneMode.Single);
    }
}
