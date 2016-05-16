using UnityEngine;
using System.Collections;



public class Player : MonoBehaviour {

    enum PlayerState
    {
        Recover,
        Watching,
        Approach,
        Move,
        React,
        GiveUp,
        Swing,
        Hit
    }

    PlayerState state;
    float reactFreeze = 0.1f;
    float swingTimer = 0;
    float hitCooldown = 0;

    GameObject ball;

    Vector3 moveTo = Vector3.zero;
    Vector3 contactPoint = Vector3.zero;



	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");
        state = PlayerState.Recover;
	}

	// Update is called once per frame
	void Update () {

        switch (state)
        {
            case PlayerState.Recover:
                {
                    if (moveTo == Vector3.zero)
                    {
                        moveTo = new Vector3(0, 0, 11.89f + 1);
                        Vector3 v = moveTo - transform.position;
                        v.Normalize();
                        GetComponent<Rigidbody>().velocity = v * 5;
                    }
                    else
                    {
                        if ((transform.position - moveTo).magnitude < 0.1)
                        {
                            state = PlayerState.Watching;
                            GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }
                        if (ball.GetComponent<Rigidbody>().velocity.z > 0)
                        {
                            reactFreeze = 0.1f;
                            state = PlayerState.React;                            
                            GetComponent<Rigidbody>().velocity = Vector3.zero;
                        }
                    }
                }
                break;
            case PlayerState.Watching:
                if (ball.GetComponent<Rigidbody>().velocity.z > 0)
                {
                    reactFreeze = 0.2f;
                    state = PlayerState.React;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;

            case PlayerState.React:
                
                if (reactFreeze > 0)
                {
                    reactFreeze -= Time.deltaTime;
                }
                else
                {
                   
                    state = PlayerState.Move;
                    moveTo = Vector3.zero;
                    GetComponent<Rigidbody>().velocity = Vector3.zero;
                }
                break;
            case PlayerState.GiveUp:
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                break;
            case PlayerState.Move:
                {
                    if (moveTo == Vector3.zero)
                    {
                        CountHitPoint();
                        Vector3 v = moveTo - transform.position;
                        v.Normalize();
                        GetComponent<Rigidbody>().velocity = v * 5;
                    }
                    else if ((transform.position - moveTo).magnitude < 0.1)
                    {
                        state = PlayerState.Swing;
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                        swingTimer = 0;
                    }

                    if (ball.transform.position.z > contactPoint.z)
                        state = PlayerState.GiveUp;
                }
                break;
            case PlayerState.Swing:
                swingTimer += Time.deltaTime;
                if (ball.GetComponent<Rigidbody>().velocity.z > 0)
                {
                    if (ball.transform.position.z > contactPoint.z)
                    {
                        Debug.Log("Hit: " + swingTimer);
                        ball.GetComponent<Ball>().Hit(Vector3.zero);
                        state = PlayerState.Hit;
                        hitCooldown = 0.2f;
                    }
                }
                break;
            case PlayerState.Hit:
                hitCooldown -= Time.deltaTime;
                if (hitCooldown < 0)
                {
                    state = PlayerState.Recover;
                    moveTo = Vector3.zero;
                }
                break;
        }

        /*
        if (state == PlayerState.Move || state == PlayerState.Swing || state == PlayerState.Hit)
        {
            if (ball.GetComponent<Rigidbody>().velocity.z < 0)
            {
                state = PlayerState.Recover;
                moveTo = Vector3.zero;
            }
        }
         */
	}

    void CountHitPoint()
    {
       

        Vector3 v = new Vector3(ball.GetComponent<Rigidbody>().velocity.x, 0, ball.GetComponent<Rigidbody>().velocity.z);
        v.Normalize();
        Ray r = new Ray(
            new Vector3(ball.transform.position.x, 0, ball.transform.position.z),
            v);

        Vector3 c = Vector3.Cross(r.direction, transform.position - r.origin);
        Vector3 dir = Quaternion.AngleAxis(-Mathf.Sign(c.y)*90, Vector3.up) * v;

        moveTo = transform.position + dir * (c.magnitude-1);
        contactPoint = transform.position + dir * c.magnitude;        
    }

}
