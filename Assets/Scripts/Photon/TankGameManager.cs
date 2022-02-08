using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;

public class TankGameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
    public static TankGameManager TGM;//Singleton
    private PhotonView PV;//Photon view for this object

    public const byte PLAYER_ADD_EVENT = 0;//Event ID for Add Player 
    public const byte PLAYER_DEAD_EVENT = 1;//Event ID for Player Dead

    public GameObject gameoverUI;//UI for when the game is over
    public TMP_Text winnerText;//Text for displaying the winner
    public List<bool> PlayerStatus = new List<bool>();//List of bools representing the status of each player(whether they are alive or dead)

    public void Awake()
    {
        //Initialize instance
        if (TankGameManager.TGM == null)
        {
            TankGameManager.TGM = this;
        }
        else
        {
            if (TankGameManager.TGM != this)
            {
                Destroy(TankGameManager.TGM.gameObject);
                TankGameManager.TGM = this;
            }
        }
    }

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);//Hook into the Photon callbacks
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);//remove Photon callbacks
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();//Get the Photon View
    }

    public void LeaveGame()
    {
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);//Destroy player objects in the room
        PhotonNetwork.LeaveRoom();//Leave the room
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel(1);//Load menu level
    }

    public void OnEvent(EventData photonEvent)
    {
        
        if (photonEvent.Code == PLAYER_ADD_EVENT)//Add Player Raise Event called
        {
            object[] data = (object[])photonEvent.CustomData;//Retrieve data from event
            int index = (int)data[0];//Get the player index

            while (PlayerStatus.Count <= index)//Adds new elements to the list while the list has less then the given player index
            {
                PlayerStatus.Add(true);//Add new status to the list
            }
        }

        if (photonEvent.Code == PLAYER_DEAD_EVENT)//Player Dead event called
        {
            object[] data = (object[])photonEvent.CustomData;//Retrieve data from event 
            int index = (int)data[0];

            PlayerStatus[index] = false;//Player is dead so set the status to false 

            int livingPlayerCount = 0;//Initialize living player count 
            int WinnerIndex = 0;
            for (int i = 0; i < PlayerStatus.Count; i++)// status in PlayerStatus)
            {
                if (PlayerStatus[i] == true)//Player is alive
                {
                    livingPlayerCount++;//Up Living player count 
                    WinnerIndex = i;//Set the winner index, this is used when only one living player is left
                }
            }

            if (livingPlayerCount <= 1)//only one player left 
            {
                //Show Game Over Screen
                gameoverUI.SetActive(true);
                
                winnerText.text = PhotonNetwork.PlayerList[WinnerIndex].NickName + " Wins!";//Display the nickname of the living player
                if (PhotonNetwork.IsMasterClient)//check if this client is the master client
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;//Set the room to closed so no new players join
                }
            }
        }
    }
}
