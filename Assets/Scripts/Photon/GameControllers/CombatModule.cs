using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class CombatModule : MonoBehaviour
{

    private PhotonView PV;
    private PlayerAvatar playerAvatar;

    public Transform fireLoc;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;

    public TMP_Text healthDisplay;

    public float fireTimer = 0.5f;
    public float fireRate = 0.5f;

    public bool ShootingTest = false;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        playerAvatar = GetComponent<PlayerAvatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)//Is this the local player
        {
            return;
        }

        fireTimer += Time.deltaTime;//Update Timer
        if (Input.GetMouseButton(0) || ShootingTest)//Check for fire input
        {
            if (fireTimer >= fireRate)//Check for fire rate 
            {
                fireTimer = 0;
                Shoot();//Fire
            }
        }
       
    }

    /// <summary>
    /// Shoots a bullet from the fire location
    /// </summary>
    public void Shoot()
    {
        if (fireLoc == null)//If fire location hasn't been setup
        {
            if (playerAvatar.avatarVisual)
                fireLoc = playerAvatar.avatarVisual.GetComponent<TankRotator>().fireLocation;//Get the origin from the tank helper class
        }
        PV.RPC("RPC_MuzzleFlash", RpcTarget.All,fireLoc.position,fireLoc.rotation);//Spawn Muzzle flash over the network
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), fireLoc.position, fireLoc.rotation);//Spawn Bullet on network
    }
    [PunRPC]
    void RPC_MuzzleFlash(Vector3 pos,Quaternion rot)
    {
        //Muzzle flash is a regular prefab without a photon view so it gets spawned through an RPC
        GameObject.Instantiate(muzzleFlashPrefab, pos, rot);
    }
}

