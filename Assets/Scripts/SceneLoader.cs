using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public bool joystickEnabled = false;

    void Start()
    {
        SceneManager.LoadScene("Netcode");
    }
}
