using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;//Singleton

    public TMP_InputField nameField;//Input field for the player name

    public GameObject battleButton;//Button tobegin searching for a game
    public GameObject cancelButton;//Button to cancel searching

    private void Awake()
    {
        lobby = this;//assign Singleton
    }

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();//Connect to master photon server
        if (PlayerInfo.PI.playerName != "")//Check if the player name has already been set 
        {
            nameField.SetTextWithoutNotify(PlayerInfo.PI.playerName);//Update input field with current player name
        }
        else
        {
            int playerName = Random.Range(0, 10000);//Pick random number 

            nameField.SetTextWithoutNotify("Player " + playerName);//Set a random player name 
        }
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;//When master loads level all clients load as well
        battleButton.SetActive(true);//Show battle button
    }

    public void OnBattleButtonClicked()
    {
        battleButton.SetActive(false);//Hide battle button 
        cancelButton.SetActive(true);//Show Cancel button
        PlayerInfo.PI.playerName = nameField.text;// set the player name using the name input field 
        PhotonNetwork.JoinRandomRoom();//Joins a random room
    }
    
    /// <summary>
    /// Failed to join
    /// </summary>
    /// <param name="returnCode"></param>
    /// <param name="message"></param>
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();//Join failed create new room
    }

    void CreateRoom()
    {
        int randomRoomName = Random.Range(0, 10000);//Get a random number to add to the room name
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4 };//Set default room options
        PhotonNetwork.CreateRoom("Room" + randomRoomName.ToString(), roomOps);//Create new room
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //failed to create room probably duplicate room name 
        CreateRoom();//Retry creating room
    }

    public void OnCancelButtonClicked()
    {
        cancelButton.SetActive(false);//Hide cancel button
        battleButton.SetActive(true);//Show battle Button
        PhotonNetwork.LeaveRoom();//Leave room 
    }
}
