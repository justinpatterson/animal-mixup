using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuController : MonoBehaviour
{
    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene(1);
    }
}
