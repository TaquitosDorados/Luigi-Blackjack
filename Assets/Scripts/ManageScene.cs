using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManageScene : MonoBehaviour
{
    public void SigEscena(string queScene)
    {
        SceneManager.LoadScene(queScene);
    }
}
