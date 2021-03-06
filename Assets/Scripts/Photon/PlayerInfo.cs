using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo PI;

    public int mySelectedColour;//old player selection variable 
    public string playerName;//Current player name for the local player

    public GameObject[] PlayerTanks;//List of player tank prefabs


    private void OnEnable()
    {
        //Setup Singleton
        if (PlayerInfo.PI == null)
        {
            PlayerInfo.PI = this;
        }
        else
        {
            if (PlayerInfo.PI != this)
            {
                Destroy(PlayerInfo.PI.gameObject);
                PlayerInfo.PI = this;
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (PlayerPrefs.HasKey("MyCharacter"))
        //{
        //    mySelectedColour = PlayerPrefs.GetInt("MyCharacter");
        //}
        //else
        //{
        //    mySelectedColour = 0;
        //    PlayerPrefs.SetInt("MyCharacter", mySelectedColour);
        //}
    }

}
