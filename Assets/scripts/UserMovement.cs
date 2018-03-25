using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserMovement : MonoBehaviour {

    public Transform centergaze;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float speed = 1.3f;
        Vector2 joy = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        transform.position += centergaze.forward * joy.y * speed;
	}
}
