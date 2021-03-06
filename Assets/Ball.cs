﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Ball : MonoBehaviour {


	// Use this for initialization
	void Start () {


        Vector3 v = Quaternion.Euler(-35.28f,0, 0) * Vector3.forward;
        lifetime = 0;
        v.Normalize();

        v *= 15;
        //GetComponent<Rigidbody>().velocity = v;

        Debug.Log("v1 = " + GetComponent<Rigidbody>().velocity.magnitude);

       
	}

    Vector3 f = Vector3.zero;

    float lifetime = 0;

    const float G = 11f;
    const float Air = 0.015f;

    bool sliced = false;

    public bool Sliced
    {
        get { return sliced; }        
    }

    Vector3 GetReboundV(Vector3 v)
    {
        Vector3 v2 = new Vector3();
        v2.y = -v.y * 0.75f;
        v2.x = v.x * 0.75f;
        v2.z = v.z * 0.75f;
        return v2;
    }

    void FixedUpdate()
    {
        if (Time.timeScale == 0)
            return;


        Vector3 v = -GetComponent<Rigidbody>().velocity;
        float vLen = v.magnitude;

        v.Normalize();

        if (transform.position.y > 0)
        {
            float _G = sliced ? G * 0.5f : G;
            GetComponent<Rigidbody>().AddForce(new Vector3(0, -_G, 0));
            GetComponent<Rigidbody>().AddForce(v * (Air * vLen * vLen));

        }
        else
        {
            Vector3 v2 = GetComponent<Rigidbody>().velocity;
            if (v2.y < 0)
            {
              //  Debug.Log("z = " + transform.position.z);                
                GetComponent<Rigidbody>().velocity = GetReboundV(v2);
                gameObject.GetComponentInParent<Game>().onBallHitGround(transform.position);
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
                transform.position = new Vector3(1, 0.5f, 14);

                Vector3 v = new Vector3(0, 0.5f, -1);
                v.Normalize();

                GetComponent<Rigidbody>().velocity = v * 18f;

                foreach (var o in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if (Mathf.Sign(o.transform.position.z) != Mathf.Sign(transform.position.z))
                        o.GetComponent<Tennisist>().OnBallHit();
                }

                keyB = true;
            }
        }
        else
            keyB = false;

        if (Input.GetKey(KeyCode.C))
        {
            if (!keyC)
            {
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.position = new Vector3(Random.Range(-4, 4), 1, -13);
                Hit(Vector3.zero);               
                keyC = true;

            }
        }
        else
            keyC = false;
       

	}

    public void Hit(Vector3 target)
    {
        sliced = false;
        float speed = GetComponent<Rigidbody>().velocity.magnitude;

        Vector3 v;

        if (target == Vector3.zero)
        {
            if (transform.position.z > 0)
                v = new Vector3(Random.Range(-4.5f, 4.5f), 0, -Random.Range(5.4f, 11.0f)) - transform.position;
            else
                v = new Vector3(Random.Range(-4.5f, 4.5f), 0, Random.Range(5.4f, 11.0f)) - transform.position;
        }
        else
        {
            Debug.Log("AIM");
            v = target - transform.position;
        }


        v.y = 0.0f;

        float angle;

        float dNet = Mathf.Abs(transform.position.z / v.z) * v.magnitude;

        if (CountAngle(speed * 0.1f + 20, transform.position.y, v.magnitude, dNet, out angle))
        {
            v.Normalize();

            angle += Random.Range(-10.0f, 10.0f);
            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);
            angle = Random.Range(-10.0f, 10.0f);
            v = Quaternion.AngleAxis(angle, Vector3.up) * v;

            v.Normalize();
            
            GetComponent<Rigidbody>().velocity = v * (speed * 0.1f + 20);
        }
            
        else
        {
            CountAngle(speed * 0.1f + 10, transform.position.y, v.magnitude, dNet, out angle);
            v.Normalize();

            angle += Random.Range(-10.0f, 10.0f);

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);

            angle = Random.Range(-10.0f, 10.0f);
            v = Quaternion.AngleAxis(angle, Vector3.up) * v;

            v.Normalize();


            GetComponent<Rigidbody>().velocity = v * (speed * 0.1f + 15);
        }
            

        gameObject.GetComponentInParent<Game>().onBallHit(transform.position);
    }


    public void HitSlice(Vector3 target)
    {
        sliced = true;
        float speed = GetComponent<Rigidbody>().velocity.magnitude;

        Vector3 v;
        if (target == Vector3.zero)
        {
            if (transform.position.z > 0)
                v = new Vector3(Random.Range(-4, 4), 0, -Random.Range(6.4f, 11.0f)) - transform.position;
            else
                v = new Vector3(Random.Range(-4, 4), 0, Random.Range(6.4f, 11.0f)) - transform.position;
        }
        else
        {
            Debug.Log("AIM");
            v = target - transform.position;
        }


        v.y = 0.0f;

        float angle;

        float dNet = Mathf.Abs(transform.position.z / v.z) * v.magnitude;

        if (CountAngle(speed * 0.1f + 15, transform.position.y, v.magnitude, dNet, out angle))
        {
            v.Normalize();

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);

            angle = Random.Range(-3.0f, 3.0f);
            v = Quaternion.AngleAxis(angle, Vector3.up) * v;

            v.Normalize();

            GetComponent<Rigidbody>().velocity = v * (speed * 0.1f + 15);
        }
        else
        {
            angle = 45;
            v.Normalize();

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);
            angle = Random.Range(-3.0f, 3.0f);
            v = Quaternion.AngleAxis(angle, Vector3.up) * v;

            v.Normalize();

            GetComponent<Rigidbody>().velocity = v * (speed * 0.1f + 15);
        }

        gameObject.GetComponentInParent<Game>().onBallHit(transform.position);      
    }


    public void HitServe(bool deuce)
    {
        sliced = false;
        float speed = GetComponent<Rigidbody>().velocity.magnitude;

        Vector3 v;
        if (transform.position.z > 0)
            v = new Vector3((deuce ? 1 : -1)*Random.Range(1, 4), 0, -Random.Range(5.4f, 6.4f)) - transform.position;
        else
            v = new Vector3((deuce ? -1 : 1) * Random.Range(1, 4), 0, Random.Range(5.4f, 6.4f)) - transform.position;


        v.y = 0.0f;

        float angle;

        float dNet = Mathf.Abs(transform.position.z / v.z) * v.magnitude;

        if (CountAngle(speed * 0.1f + 30, transform.position.y, v.magnitude, dNet, out angle))
        {
            v.Normalize();

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);
            v.Normalize();

            GetComponent<Rigidbody>().velocity = v * (speed * 0.1f + 30);
        }
        else
        {
            CountAngle(speed * 0.1f + 10, transform.position.y, v.magnitude, dNet, out angle);
            v.Normalize();

            v.y = Mathf.Tan(Mathf.PI * angle / 180.0f);
            v.Normalize();

            GetComponent<Rigidbody>().velocity = v * (speed * 0.1f + 10);
        }

        gameObject.GetComponentInParent<Game>().onBallHit(transform.position);      

    }
    void OnTriggerEnter(Collider col) {
      
    }


    bool CountAngle(float s, float y, float d, float dNet, out float _angle)
    {
        //Debug.Log("D => " + d);
        float angleMax = 45;
        float angleMin = -45;
        _angle = 0;
        Vector3 pos = Vector3.zero;

        float _G = sliced ? G * 0.5f : G;

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
                Vector3 force = new Vector3(0, -_G, 0) + (v1 * (Air * vLen * vLen));
                v += force * Time.fixedDeltaTime;
                pos += v * Time.fixedDeltaTime;

                
                
                if (pos.z > dNet && !netOk) {
                    if (pos.y > 1.2)
                        netOk = true;
                    else                    
                        break;
                    
                }
            }

          //  Debug.Log(angle.ToString() + " => " + pos.z);

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

    public List<Vector3> Trajectory
    {
        get { return trajectory; }
    }


    public Vector3 Velocity
    {
        get { return GetComponent<Rigidbody>().velocity; }
    }

    private List<Vector3> trajectory = new List<Vector3>();
    private float trajectoryTime;
    public float TrajectoryTime
    {
        get { return trajectoryTime; }        
    }

    public void CountTrajectory()
    {
        trajectory.Clear();
        trajectoryTime = Time.time;
        Vector3 pos = transform.position;
        Vector3 v = GetComponent<Rigidbody>().velocity;

        float _G = sliced ? G * 0.5f : G;

        int ground = 0;
        while (ground <= 1 || trajectory.Count<100)
        {
            Vector3 v1 = -v;
            v1.Normalize();
            float vLen = v.magnitude;
            Vector3 force = new Vector3(0, -_G, 0) + (v1 * (Air * vLen * vLen));

            v += force * Time.fixedDeltaTime;
            pos += v * Time.fixedDeltaTime;


            if (pos.y < 0 && v.y < 0)
            {
                ground++;
                v = GetReboundV(v);
            }

            trajectory.Add(pos);
        }
    }

}
