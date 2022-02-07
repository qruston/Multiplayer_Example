using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour
{
    public int LevelToLoad = 1;
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene(LevelToLoad);
    }
}
