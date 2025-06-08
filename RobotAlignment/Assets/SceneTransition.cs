using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       Invoke("LoadNextScene", 30f); // Call function after 30 seconds
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Icon Scene"); // Replace with your scene name
    }
}
