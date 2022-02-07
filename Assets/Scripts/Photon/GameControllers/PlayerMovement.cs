using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private PhotonView PV;
    private CharacterController myCC;
    private PlayerAvatar AS;
    public float movementSpeed;
    public float rotationSpeed;
    float baseRotation = 0;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        myCC = GetComponent<CharacterController>();
        AS = GetComponent<PlayerAvatar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            BasicMovement();
            BasicRotation();
        }
    }

    void BasicMovement()
    {
        baseRotation = 0;
        Vector3 MovementDirection = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            MovementDirection += transform.forward;
            myCC.Move(transform.forward * Time.deltaTime * movementSpeed); 
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MovementDirection += -transform.forward;
            myCC.Move(-transform.forward * Time.deltaTime * movementSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            MovementDirection += -transform.right;
            myCC.Move(-transform.right * Time.deltaTime * movementSpeed);
        } else if (Input.GetKey(KeyCode.D))
        {
            MovementDirection += transform.right;
            myCC.Move(transform.right * Time.deltaTime * movementSpeed);
        }

        baseRotation = Vector3.Angle(transform.forward, MovementDirection);
        if (MovementDirection.x < 0)
        {
            baseRotation = 360 - baseRotation;
        }

        AS.RotateTankBase(baseRotation);
    }

    void BasicRotation()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, 1000))
        {
            float angle = Vector3.Angle(transform.forward, hit.point - transform.position);
            if (hit.point.x < transform.position.x)
            {
                angle = 360 - angle;
            }
            Debug.Log("Angle: " + angle);
            AS.RotateTurret(angle);
        }
        
    }
}
