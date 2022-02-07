using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    public GameObject battleButton;
    public GameObject cancelButton;

    private void Awake()
    {
        lobby = this;//assign Singleton
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();//Connect to master photon server
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("Player Has Connected to photon servers");
        PhotonNetwork.AutomaticallySyncScene = true;//When master loads level all clients load as well
        battleButton.SetActive(true);
    }

    public void OnBattleButtonClicked()
    {
        //Debug.Log("Player Has clicked the battle button");
        battleButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();//Joins a random room
    }
    
    /// <summary>
    /// Failed to join
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        //Debug.Log("tried to join room but failed, there must be no rooms");
        CreateRoom();
    }

    void CreateRoom()
    {
        //Debug.Log("Player Is Creating A Room");
        int randomRoomName = Random.Range(0, 10000);//Get a random number to add to the room name
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };//Set default room options
        PhotonNetwork.CreateRoom("Room" + randomRoomName.ToString(), roomOps);//Create new room
    }

   

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //failed to create room probably duplicate room name 
        //Debug.Log("Failed to create Room, there is probably a duplicate room name");
        CreateRoom();//Retry creating room
    }

    public void OnCancelButtonClicked()
    {
        //Debug.Log("Player Has canceld searching");
        cancelButton.SetActive(false);
        battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
