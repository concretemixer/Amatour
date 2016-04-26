using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {


	// Use this for initialization
	void Start () {


        Vector3 v = new Vector3(-0.35f, 0.2f, 1);
        lifetime = 0;
        v.Normalize();

            v *= 28;
            GetComponent<Rigidbody>().velocity = v;

        Debug.Log("v1 = " + GetComponent<Rigidbody>().velocity.magnitude);

       
	}

    Vector3 f = Vector3.zero;

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

        if (transform.position.y > 0)
        {
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -G, 0));
            GetComponent<Rigidbody>().AddForce(v * (Air * vLen * vLen));
        }

     
        GetComponent<Rigidbody>().AddForce(f);
        f = Vector3.zero;
    }    

	// Update is called once per frame
    bool keyB = false;
    bool keyC = false;

	void Update () {

        if (Input.GetKey(KeyCode.A))
        {
            GetComponent<Rigidbody>().isKinematic = false;
            float x = gameObject.transform.position.x;
            gameObject.transform.position = new Vector3(4.09f, 1.3f, -11.89f);
            Time.timeScale = 1f;
            Start();
        }
        if (Input.GetKey(KeyCode.B))
        {
            if (!keyB)
            {
                float speed = GetComponent<Rigidbody>().velocity.magnitude;
                Vector3 v;
                if (transform.position.z>0)
                    v = new Vector3(0, 0,-11.89f) - transform.position;
                else
                    v = new Vector3(0, 0, 11.89f) - transform.position;
                v.Normalize();
                v.y = 0.8f;
                v.Normalize();

                GetComponent<Rigidbody>().velocity = v * speed * 0.6f;

                keyB = true;
            }
        }
        else
            keyB = false;

        if (Input.GetKey(KeyCode.C))
        {
            if (!keyC)
            {
                float speed = GetComponent<Rigidbody>().velocity.magnitude;
                Vector3 v;
                if (transform.position.z > 0)
                    v = new Vector3(0, 1.3f, -11.89f) - transform.position;
                else
                    v = new Vector3(0, 0, 11.89f) - transform.position;
                v.Normalize();
                v.y = 0.6f;
                v.Normalize();

                GetComponent<Rigidbody>().velocity = v * speed * 1.5f;

                keyC = true;
            }
        }
        else
            keyC = false;
       

	}

    void OnTriggerEnter(Collider col) {

        if (col.gameObject.tag == "Clay")
        {
            Vector3 v = GetComponent<Rigidbody>().velocity;
            if (v.y < 0)
            {
                Debug.Log("v2 = " + GetComponent<Rigidbody>().velocity.magnitude);
                v.y = -v.y * 0.75f;
                v.x = v.x * 0.8f;
                v.z = v.z * 0.8f;
                GetComponent<Rigidbody>().velocity = v;
            }
        }
        return;
        Debug.Log("t = " + lifetime.ToString());
        Debug.Log("pos = " + gameObject.transform.position.z);
        Debug.Log("v2 = " + GetComponent<Rigidbody>().velocity.magnitude);
        Time.timeScale = 0;
        GetComponent<Rigidbody>().isKinematic = true;
    }
}
