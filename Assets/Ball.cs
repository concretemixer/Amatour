using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour {


	// Use this for initialization
	void Start () {


        Vector3 v = Quaternion.Euler(-35.28f,0, 0) * Vector3.forward;
        lifetime = 0;
        v.Normalize();

        v *= 15;
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
        else
        {
            Vector3 v2 = GetComponent<Rigidbody>().velocity;
            if (v2.y < 0)
            {
                Debug.Log("z = " + transform.position.z);
                v2.y = -v2.y * 0.75f;
                v2.x = v2.x * 0.8f;
                v2.z = v2.z * 0.8f;
                GetComponent<Rigidbody>().velocity = v2;
            }
        }

     
        GetComponent<Rigidbody>().AddForce(f);
        f = Vector3.zero;
    }    

	// Update is called once per frame
    bool keyB = false;
    bool keyC = false;

	void Update () {

        if (Input.GetKey(KeyCode.B))
        {
            if (!keyB)
            {
                float speed = GetComponent<Rigidbody>().velocity.magnitude;

                Vector3 v;
                if (transform.position.z>0)
                    v = new Vector3(Random.Range(-4,4), 0,-Random.Range(6.4f,11.89f)) - transform.position;
                else
                    v = new Vector3(Random.Range(-4, 4), 0, Random.Range(6.4f, 11.89f)) - transform.position;
               
                v.y = 0.0f;

                float angle;

                CountAngle(speed * 0.99f, transform.position.y, v.magnitude, v.magnitude / 2, out angle);
                v.Normalize();

                v.y = Mathf.Tan( Mathf.PI * angle / 180.0f);
                v.Normalize();

                GetComponent<Rigidbody>().velocity = v * speed * 0.99f;

                keyB = true;
            }
        }
        else
            keyB = false;

        if (Input.GetKey(KeyCode.C))
        {
            if (!keyC)
            {
                Hit();
               
                keyC = true;
            }
        }
        else
            keyC = false;
       

	}

    public void Hit()
    {
        float speed = GetComponent<Rigidbody>().velocity.magnitude;

        Vector3 v;
        if (transform.position.z > 0)
            v = new Vector3(Random.Range(-4, 4), 0, -Random.Range(6.4f, 11.89f)) - transform.position;
        else
            v = new Vector3(Random.Range(-4, 4), 0, Random.Range(6.4f, 11.89f)) - transform.position;


        v.y = 0.0f;

        float angle;

        float dNet = Mathf.Abs(transform.position.z / v.z) * v.magnitude;

        if (CountAngle(speed * 0.6f + 20, transform.position.y, v.magnitude, dNet, out angle))
        {
            v.Normalize();

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);
            v.Normalize();

            GetComponent<Rigidbody>().velocity = v * (speed * 0.6f + 20);
        }
        else
        {
            CountAngle(speed * 0.6f + 10, transform.position.y, v.magnitude, dNet, out angle);
            v.Normalize();

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);
            v.Normalize();

            GetComponent<Rigidbody>().velocity = v * (speed * 0.6f + 10);
        }

    }

    void OnTriggerEnter(Collider col) {
      
    }


    bool CountAngle(float s, float y, float d, float dNet, out float _angle)
    {
        Debug.Log("D => " + d);
        float angleMax = 45;
        float angleMin = -45;
        _angle = 0;
        Vector3 pos = Vector3.zero;
        do
        {
            float angle = (angleMax + angleMin) * 0.5f;

            if (Mathf.Abs(angleMax - angleMin) < 0.5)
                break;

            Vector3 v = Quaternion.Euler(-angle, 0, 0) * Vector3.forward;
            v *= s;

            pos = new Vector3(0, y, 0);

            bool netOk = false;
            while (pos.z < d * 2 && pos.y > 0)
            {
                float vLen = v.magnitude;
                Vector3 v1 = -v;
                v1.Normalize();
                Vector3 force = new Vector3(0, -G, 0) + (v1 * (Air * vLen * vLen));
                v += force * Time.fixedDeltaTime;
                pos += v * Time.fixedDeltaTime;

                
                
                if (pos.z > dNet && !netOk) {
                    if (pos.y > 1.2)
                        netOk = true;
                    else                    
                        break;
                    
                }
            }

            Debug.Log(angle.ToString() + " => " + pos.z);

            if (netOk)
            {
                if (pos.z > d)
                    angleMax = angle;
                if (pos.z < d)
                    angleMin = angle;
            }
            else
                angleMin = angle;

            _angle = angle;

        } while (Mathf.Abs(pos.z - d) > 0.1);

        return Mathf.Abs(pos.z - d) <= 0.5;
     }
}
