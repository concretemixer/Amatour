using UnityEngine;
using System.Collections;

public class Racket : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
            GetComponent<Rigidbody>().AddForce(new Vector3(-100, 0, 0));
        if (Input.GetKey(KeyCode.RightArrow))
            GetComponent<Rigidbody>().AddForce(new Vector3(100, 0, 0));
        if (Input.GetKey(KeyCode.UpArrow))
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 500));
        if (Input.GetKey(KeyCode.DownArrow))
            GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, -500));
    }

	// Update is called once per frame
	void Update () {
	    
	}
}
