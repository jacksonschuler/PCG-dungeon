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
	void Update () {
        if (Input.GetKey(KeyCode.D))
        {
            body.AddForce(Vector2.right * moveSpeed);
        }

        if (Input.GetKey(KeyCode.A))
        {
            body.AddForce(Vector2.left * moveSpeed);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            body.AddForce(Vector2.up * moveSpeed);
        }
        if (Input.GetKey(KeyCode.S)) {
            body.AddForce(Vector2.down * moveSpeed);
        }
    }
}
