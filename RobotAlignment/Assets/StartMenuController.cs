using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public string sceneToLoad; // The name of the scene to load when the game starts
    public void OnStartClicked()
    {
        // Load the game scene when the "Start" button is clicked
        SceneManager.LoadScene(sceneToLoad);
    }
}
