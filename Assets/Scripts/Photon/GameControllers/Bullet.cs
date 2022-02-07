using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PhotonView PV;
    public float speed = 10;
    public float timer = 0;
    public float lifetime = 10;
    public GameObject hitPrefab;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PV.IsMine)//Is not local
        {
            return;
        }

        transform.position += transform.forward * speed * Time.deltaTime;//Move Bullet forward

        timer += Time.deltaTime;
        if (timer >= lifetime)//Lifetime timer to destroy bullet if it stays in scene too long
        {
            timer = 0;
            PhotonNetwork.Destroy(gameObject);//Destroy on network
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerAvatar>())//Check if the hit was agains a player
        {
            other.GetComponent<PlayerAvatar>().DealDamage(10);//Deal Damage to player
        }

        if (PV.IsMine)//Check if the Photon view is local
        {
            PV.RPC("RPC_HitEffect", RpcTarget.All, transform.position);//There was a hit, Spawn Hit effect across the network
            StartCoroutine(WaitAndDestroy());
        }
    }

    private IEnumerator WaitAndDestroy()
    {
        yield return new WaitForSeconds(0.1f);
        PhotonNetwork.Destroy(gameObject);//Destroy this object on the network
    }

    [PunRPC]
    public void RPC_HitEffect(Vector3 pos)
    {
        GameObject.Instantiate(hitPrefab, pos,Quaternion.identity);
    }

}