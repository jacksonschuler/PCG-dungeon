using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Use this for initialization

    Rigidbody2D body;
    float moveSpeed = 5.0f;


	void Start () {
        body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
    //basic movement using WASD
	void FixedUpdate () {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        body.transform.position = transform.position + movement * Time.deltaTime * moveSpeed;
    }
}
