using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExitGames.Client.Photon;

public class TankGameManager : MonoBehaviourPunCallbacks, IInRoomCallbacks, IOnEventCallback
{
    public static TankGameManager TGM;
    private PhotonView PV;

    public const byte PLAYER_ADD_EVENT = 0;
    public const byte PLAYER_DEAD_EVENT = 1;

    public GameObject gameoverUI;
    public TMP_Text winnerText;
    public List<bool> PlayerStatus = new List<bool>();

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
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
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
        
        if (photonEvent.Code == PLAYER_ADD_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            int index = (int)data[0];

            while (PlayerStatus.Count <= index)//Adds new elements to the list while the list has less then the given player index
            {
                PlayerStatus.Add(true);
            }
        }

        if (photonEvent.Code == PLAYER_DEAD_EVENT)
        {
            object[] data = (object[])photonEvent.CustomData;
            int index = (int)data[0];

            PlayerStatus[index] = false;

            int livingPlayerCount = 0;
            int WinnerIndex = 0;
            for (int i = 0; i < PlayerStatus.Count; i++)// status in PlayerStatus)
            {
                if (PlayerStatus[i] == true)
                {
                    livingPlayerCount++;
                    WinnerIndex = i;
                }
            }

            if (livingPlayerCount <= 1)//One player left 
            {
                //Show Game Over Screen
                gameoverUI.SetActive(true);
                
                winnerText.text = PhotonNetwork.PlayerList[WinnerIndex].NickName + " Wins!";
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.CurrentRoom.IsOpen = false;//Set the room to closed so no new players join
                }
            }
        }
    }
}
