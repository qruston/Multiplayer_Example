using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameSetup : MonoBehaviour
{

    public static GameSetup GS;

    public TMP_Text healthDisplay;

    public Transform[] spawnPoints;

    private void OnEnable()
    {
        if (GameSetup.GS == null)
        {
            GameSetup.GS = this;
        }
    }
}
