using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;

public class PlayerAvatar : MonoBehaviour, IInRoomCallbacks
{
    private PhotonView PV;

    public int playerIndex;//Index of the player on the network
    public GameObject avatarVisual;//Visual Avatar of the player
    public int initialHealth;//Health of the player

    public Image healthBar;//Health bar for the player
    public TMP_Text playerName;//Name of the player on the ui

    public Camera myCamera;//Camera for the player

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();//Get Photon View component

        //Find what index this player is 
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            if (PhotonNetwork.PlayerList[i] != null && PhotonNetwork.PlayerList[i] == PV.Owner)
            {
                playerIndex = i;//Set the Player Number to the character value for adding the tank model later
            }
        }


        if (PV.IsMine)//Is local
        {
           

            object[] data = new object[] { playerIndex };
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            //Raise event to add player to the tank game manager 
            PhotonNetwork.RaiseEvent(TankGameManager.PLAYER_ADD_EVENT, data, raiseEventOptions, SendOptions.SendReliable);

            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, playerIndex);//Spawn Player tank based on found index

            //Initialize Custom Properties
            Hashtable hash = new Hashtable();
            hash.Add("Health", initialHealth);
            hash.Add("isDead", false);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);//Set the initial health custom properties
        }
        else
        {
            Destroy(myCamera.gameObject);//If this is not the local player then we need to get rid of the camera 
        }

        playerName.text = PV.Owner.NickName;

        PhotonNetwork.AddCallbackTarget(this);

        StartCoroutine(PostCharacterAdded());//Called wherever this object is to hook up the observables added by the new tank model
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    /// <summary>
    /// Waits for 0.5 seconds and then tells the Photon view to find new observables
    /// Gets past the Photon view not refreshing itself when adding observables
    /// </summary>
    /// <returns></returns>
    private IEnumerator PostCharacterAdded()
    {
        yield return new WaitForSeconds(0.5f);
        PV.FindObservables(true);
    }

    /// <summary>
    /// Spawns the players Tank model given the players index
    /// </summary>
    /// <param name="characterIndex"></param>
    [PunRPC]
    void RPC_AddCharacter(int characterIndex)
    {
        //Spawn the Tank model based on the player Index
        avatarVisual = Instantiate(PlayerInfo.PI.PlayerTanks[characterIndex],
            transform.position,transform.rotation,transform);
    }

    /// <summary>
    /// Sets the rotation of the Tank Base to the given rotation
    /// </summary>
    /// <param name="Rotation"></param>
    public void RotateTankBase(float Rotation)
    {
        if(avatarVisual)
            avatarVisual.GetComponent<TankRotator>().tankBody.localEulerAngles = new Vector3(0,Rotation, 0);
    }

    /// <summary>
    /// Sets the rotation of the tank turret to the given rotation
    /// </summary>
    /// <param name="Rotation"></param>
    public void RotateTurret(float Rotation)
    {
        if (avatarVisual)
            avatarVisual.GetComponent<TankRotator>().tankTurret.localEulerAngles = new Vector3(0, Rotation, 0);
    }

    /// <summary>
    /// Calls the DMG RPC 
    /// </summary>
    /// <param name="dmg"></param>
    public void DealDamage(int dmg)
    {
        if (!(bool)PV.Owner.CustomProperties["isDead"])
        {
            //playerHealth -= dmg;//Damage player health
            int health = (int)PV.Owner.CustomProperties["Health"] - dmg;
            Hashtable hash = new Hashtable();//Create a new hash table
            hash.Add("Health", health);//Add player health to has table
            PV.Owner.SetCustomProperties(hash);//Set the Health custom properties

            if (health <= 0)//This player is dead 
            {
                object[] data = new object[] { playerIndex };
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
                PhotonNetwork.RaiseEvent(TankGameManager.PLAYER_DEAD_EVENT, data, raiseEventOptions, SendOptions.SendReliable);

                hash = new Hashtable();//Create a new hash table
                hash.Add("isDead", true);//Add isDead
                PV.Owner.SetCustomProperties(hash);//Set the isDead Custom Property

            }
        }
    }

    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("Health"))
            {
                healthBar.fillAmount = (float)((int)targetPlayer.CustomProperties["Health"]) / 100.0f;//Set health bar
                if (targetPlayer.IsLocal)//Check if the player is local for the functionality that should only happen on the local client
                {
                    GameSetup.GS.healthDisplay.text = ((int)targetPlayer.CustomProperties["Health"]).ToString();//Set health text

                    if ((int)targetPlayer.CustomProperties["Health"] <= 0)//Check if the player is dead
                    {
                        GetComponent<PlayerMovement>().enabled = false;//Disable movement 
                        GetComponent<CombatModule>().enabled = false;//Disable Combat
                    }
                }
            }
        }
    }


    #region Unused Interface Implimentations 

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
    }
    #endregion
}
