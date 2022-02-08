using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class CombatModule : MonoBehaviour
{

    private PhotonView PV;
    private PlayerAvatar avatarSetup;
    public Transform rayOrigin;
    public GameObject bulletPrefab;
    public GameObject muzzleFlashPrefab;

    public TMP_Text healthDisplay;

    public float fireTimer = 0.5f;
    public float fireRate = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        avatarSetup = GetComponent<PlayerAvatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)
        {
            return;
        }

        fireTimer += Time.deltaTime;//Update Timer
        if (Input.GetMouseButton(0))
        {
            if (fireTimer >= fireRate)
            {
                fireTimer = 0;
                Shoot();
            }
        }
       
    }

    public void Shoot()
    {
        if (rayOrigin == null)//If Ray Origin hasn't been setup
        {
            if (avatarSetup.myCharacter)
                rayOrigin = avatarSetup.myCharacter.GetComponent<TankRotator>().fireLocation;//Get the origin from the tank helper class
        }
        PV.RPC("RPC_MuzzleFlash", RpcTarget.All,rayOrigin.position,rayOrigin.rotation);//Spawn Muzzle flash over the network
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), rayOrigin.position, rayOrigin.rotation);//Spawn Bullet on network
    }
    [PunRPC]
    void RPC_MuzzleFlash(Vector3 pos,Quaternion rot)
    {
        //Muzzle flash is a regular prefab without a photon view so it gets spawned through an RPC
        GameObject.Instantiate(muzzleFlashPrefab, pos, rot);
    }
}

