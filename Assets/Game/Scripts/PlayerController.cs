﻿using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : NetworkBehaviour
{
    [Header("Movement Variables")]
    [SerializeField]
    private float jumpSensitivity = 1500f;
    [SerializeField]
    private float lookSensitivity = 5f;
    [Header("First Person Camera Position")]
    [SerializeField]
    float fpCameraY = 0.45f;                 // The height off of the ground that the camera should be
    [SerializeField]
    float fpCameraX = 0f;                    // The height off of the ground that the camera should be
    [SerializeField]
    float fpCameraZ = 0.2f;                  // The height off of the ground that the camera should be
    [Header("Third Person Camera Position")]
    [SerializeField]
    float tpCameraDistance = 6f;            // Distance from the player that the camera should be
    [SerializeField]
    float tpCameraY = 7f;                   // The height off of the ground that the camera should be
    [SerializeField]
    bool isFirstPerson = true;
    [SerializeField]
    public int team;
    static int numPlayers = 0;

    Transform mainCamera;
    Vector3 tpCameraOffset;
    Vector3 fpCameraOffset;

    //[SerializeField]
    //private Image crosshair; 

    float xRotation;
    float yRotation;
    float xRotationV;
    float yRotationV;
    float lookSmoothDamp = 0.1f;
    float walkingSpeed = 6f;
    float speed;
    float sprintSpeed = 12f;

    private Rigidbody rb;

    // Use this for initialization
    void Start()
    {
        team = numPlayers % 2;
        numPlayers++;
        // if this player is not the local player...
        if (!isLocalPlayer)
        {
            // then remove this script. By removing this script all the rest of the code will not run.
            Destroy(this);
            return;
        }
        //crosshair.enabled = true;
        rb = GetComponent<Rigidbody>();

        tpCameraOffset = new Vector3(0f, tpCameraY, -tpCameraDistance);
        fpCameraOffset = new Vector3(fpCameraX, fpCameraY, fpCameraZ);

        mainCamera = Camera.main.transform;
        MoveCamera();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        xRotation -= Input.GetAxis("Mouse Y") * lookSensitivity;
        if (xRotation > 90)
        {
            xRotation = 90;
        }
        else if (xRotation < -90)
        {
            xRotation = -90;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = walkingSpeed;
        }

        yRotation += Input.GetAxis("Mouse X") * lookSensitivity;
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        transform.Translate(Input.GetAxis("Horizontal") * Time.deltaTime * speed, 0, Input.GetAxis("Vertical") * Time.deltaTime * speed);

        Vector3 jumpForce = Vector3.zero;

        if (Input.GetButton("Jump"))
        {
            jumpForce = Vector3.up * jumpSensitivity;
            if (jumpForce != Vector3.zero)
            {
                rb.AddForce(Time.fixedDeltaTime * jumpForce, ForceMode.Acceleration);
            }
        }

        // Update the camera's position/rotation
        MoveCamera();
    }

    void MoveCamera()
    {
        mainCamera.position = transform.position;
        mainCamera.rotation = transform.rotation;
        if (isFirstPerson)
        {
            fpCameraOffset = new Vector3(fpCameraX, fpCameraY, fpCameraZ);
            mainCamera.Translate(fpCameraOffset);
        }
        else
        {
            tpCameraOffset = new Vector3(0f, tpCameraY, -tpCameraDistance);
            mainCamera.Translate(tpCameraOffset);
            mainCamera.LookAt(transform);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Pick Up (Stone)"))
        {
            PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
            GameObject baseObject = GameObject.Find("Base" + (team + 1));
            baseObject.GetComponent<ResourceBank>().Add("Stone", TempController.amount);
            TempController.StartRespawnTimer();
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
        else if (other.gameObject.CompareTag("Pick Up (Wood)"))
        {
            PickUpController TempController = other.gameObject.GetComponent<PickUpController>();
            GameObject baseObject = GameObject.Find("Base" + (team + 1));
            baseObject.GetComponent<ResourceBank>().Add("Wood", TempController.amount);
            TempController.StartRespawnTimer();
            other.gameObject.GetComponent<MeshRenderer>().enabled = false;
            other.gameObject.GetComponent<BoxCollider>().enabled = false;
        }
    }
}
