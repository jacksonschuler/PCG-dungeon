using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Use this for initialization

    Rigidbody2D body;

    Animator anim;

    SpriteRenderer rend;

    float moveSpeed = 3.0f;


	void Start () {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        rend = GetComponent<SpriteRenderer>();
	}
	

	void FixedUpdate () {



        anim.SetFloat("VerticalSpeed", Input.GetAxis("Vertical"));

        anim.SetFloat("HorizontalSpeed", Input.GetAxis("Horizontal"));

        
        if (Input.GetAxis("Horizontal") < 0)
        {
            rend.flipX = true;
        }
        else
        {
            rend.flipX = false;
        }

        print(Input.GetAxis("Horizontal"));

        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0.0f);

        body.transform.position = transform.position + movement * Time.deltaTime * moveSpeed;
    }
}
