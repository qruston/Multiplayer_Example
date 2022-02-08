
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    public static PhotonRoom room;
    private PhotonView PV;

    public int multiplayerScene = 2;
    public int currentScene;

    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }

        DontDestroyOnLoad(gameObject);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    /// <summary>
    /// Called when player has joined a room
    /// </summary>
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("We are now in a room");
        StartGame();
    }

    /// <summary>
    /// Loads the Multiplayer level
    /// </summary>
    private void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    /// <summary>
    /// Creates player after the scene has finished loading
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if (currentScene == multiplayerScene)
        {
            CreatePlayer();
        }
    }

    /// <summary>
    /// Instantiates the player object
    /// </summary>
    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity,0);
        PhotonNetwork.LocalPlayer.NickName = PlayerInfo.PI.playerName;
    }
}
