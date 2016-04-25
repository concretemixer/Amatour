using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {

    
	// Use this for initialization
	void Start () {

        Vector3 v = new Vector3(0, -4.15f, 41.5f);
        v.Normalize();
        v *= 43;

        GetComponent<Rigidbody>().velocity = v;
        lifetime = 0;
        Debug.Log("v1 = " + GetComponent<Rigidbody>().velocity.magnitude);
	}

    float lifetime = 0;

    const float G = 11f;
    const float Air = 0.015f;

    void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;


        Vector3 v = -GetComponent<Rigidbody>().velocity;
        float vLen = v.magnitude;

        v.Normalize();

        GetComponent<Rigidbody>().AddForce(new Vector3(0, -G, 0));
        GetComponent<Rigidbody>().AddForce(v * (Air * vLen * vLen));


        if (GetComponent<Rigidbody>().velocity.y>0)
            Debug.Log("posY = " + gameObject.transform.position.y);
    }    

	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Rigidbody>().isKinematic = false;
            float x = gameObject.transform.position.x;
            gameObject.transform.position = new Vector3(x, 2.9f, -11.89f);
            Time.timeScale = 0.5f;
            Start();
        }

       

	}

    void OnCollisionEnter(Collision collision) {
        return;
        Debug.Log("t = " + lifetime.ToString());
        Debug.Log("pos = " + gameObject.transform.position.z);
        Debug.Log("v2 = " + GetComponent<Rigidbody>().velocity.magnitude);
        Time.timeScale = 0;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
