using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;       


    private Vector3 offset;         

   
    void Start()
    {

        offset = transform.position - player.transform.position;
    }

    //Allows for the camera to follow the player from a top down position
    void LateUpdate()
    {

        transform.position = player.transform.position + offset;
    }
}
