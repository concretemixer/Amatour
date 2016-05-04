﻿using UnityEngine;
using System.Collections;



public class Tennisist : MonoBehaviour {

    enum HitType
    {
        None = 0,
        Drive = 1,
        Slice = 2,
        Volley = 3,
        Reach = 4,
        Block = 5,
        Smash = 6
    }

    enum PlayerState
    {
        Recover,
        Watching,
        Approach,
        Strafe,
        Run,
        React,
        GiveUp,
        Swing,
        Hit
    }

    protected float vRun = 2.5f;
    protected float vStrafe = 1;
    protected float reactCooldown = 0.1f;

    protected float driveSwingTime = 0.9f;
    protected float volleySwingTime = 0.66f;
    protected float sliceSwingTime = 0.9f;
    protected float reachSwingTime = 0.25f;

    protected Vector3 driveHitDistanceFH = new Vector3 (-1.5f,0, -0.8f);
    protected Vector3 driveHitDistanceBH = new Vector3(1.25f, 0, -0.9f);

    protected Vector3 volleyHitDistanceFH = new Vector3(-1.2f, 0, -0.8f);
    protected Vector3 volleyHitDistanceBH = new Vector3(1.2f, 0, -0.8f);

    protected float sliceHitDistance = 0.75f;
    protected float reachHitDistance = 1.5f;

    protected float groundstrokeHeightMin = 0.5f;
    protected float groundstrokeHeightMax = 1.5f;

    protected float volleyHeightMin = 0.5f;
    protected float volleyHeightMax = 2.5f;

    protected float reachHeightMin = 0.1f;

    GameObject ball;


    PlayerState state = PlayerState.Watching;
    HitType hit;
    bool forehand;
    float hitTimer =  10000;

    Vector3 moveTo = Vector3.zero;
    Vector3 contactPoint = Vector3.zero;



	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");
	}

	// Update is called once per frame
	void Update () {
        Animator animator = GetComponent<Animator>();

        hitTimer -= Time.deltaTime;

        switch (state)
        {
            case PlayerState.Run:
                {
                    Vector3 v = moveTo - transform.position;
                    v.Normalize();
                    GetComponent<Rigidbody>().velocity = v * vRun;

                    animator.SetInteger("MoveType", 2);
                                           
                }
                break;
            case PlayerState.Strafe:
                {
                    Vector3 v = moveTo - transform.position;
                    v.Normalize();
                    GetComponent<Rigidbody>().velocity = v * vStrafe;
                    animator.SetInteger("MoveType", 1);

                }
                break;
            case PlayerState.Recover:
                {
                    Vector3 v = moveTo - transform.position;
                    v.Normalize();
                    GetComponent<Rigidbody>().velocity = v * vStrafe;
                    //animator.SetInteger("MoveType", 1);
                    if ((transform.position - moveTo).magnitude < 0.1)
                    {
                        state = PlayerState.Watching;
                        GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }
                }
                break;

            case PlayerState.Swing:
                {
                    switch (hit)
                    {
                        case HitType.Drive:
                            if (hitTimer < -driveSwingTime)
                            {
                                ball.GetComponent<Ball>().Hit();
                                state = PlayerState.Recover;
                                moveTo.z = -13;
                            }
                            break;
                        case HitType.Volley:
                            if (hitTimer < -volleySwingTime)
                            {
                                ball.GetComponent<Ball>().Hit();
                                state = PlayerState.Recover;
                                moveTo.z = -4;
                            }
                            break;
                    }
                }
                break;
        }

        animator.SetFloat("HitTimer", hitTimer);

//        Debug.Log("ht = " + hitTimer);
        if (hitTimer < 0)
        {
            
            //Debug.Log("R = " + animator.GetBool("ReadyToHit"));
           // Time.timeScale = 0.1f;
        }


        if (state == PlayerState.Run || state == PlayerState.Strafe)
        {
            animator.SetBool("Forehand", forehand);
            animator.SetInteger("HitType", (int)hit);
            if ((transform.position - moveTo).magnitude < 0.1)
            {
                
                animator.SetBool("ReadyToHit", true);        
                //animator.Rebind();
                state = PlayerState.Swing;
                GetComponent<Rigidbody>().velocity = Vector3.zero;                
            }
            else
                animator.SetBool("ReadyToHit", false);        
        }
        else
            animator.SetInteger("MoveType", 0);
    
	}

    public void OnBallHit()
    {
        state = PlayerState.React;
        ball.GetComponent<Ball>().CountTrajectory();
        Invoke("OnBallHitReact", reactCooldown);
    }

    protected void OnBallHitReact()
    {
        Vector3 bounce, hitV, hitGS;
        float hitVTime, hitGSTime;

        bounce = GetBouncePoint();
        GetGroundstrokeHitPoint(out hitGS, out hitGSTime);
        GetVolleyHitPoint(out hitV, out hitVTime);

       // GameObject.Find("Bounce").transform.position = bounce;
        GameObject.Find("HitGS").transform.position = hitGS;
        GameObject.Find("HitV").transform.position = hitV;

        Vector3 moveV = hitV, moveGS = hitGS;


        float shift = Mathf.Sign(transform.position.x - hitGS.x);
        forehand = shift < 0;

        moveV.y = 0;
        moveGS.y = 0;



        if (forehand)
        {
            moveV += volleyHitDistanceFH;
            moveGS += driveHitDistanceFH;
        }
        else
        {
            moveV += volleyHitDistanceBH;
            moveGS += driveHitDistanceBH;
        }
        

        GameObject.Find("MoveGS").transform.position = moveGS;
        GameObject.Find("MoveV").transform.position = moveV;

        float dV = Vector3.Distance(transform.position, moveV);
        float dGS = Vector3.Distance(transform.position, moveGS);

        bool canHitDrive = false;
        bool canHitVolley = false;
        bool canHitSlice = false;
        bool shouldRun = false;

        canHitDrive = driveSwingTime + (dGS / vStrafe) < hitGSTime;
        canHitVolley = volleySwingTime + (dV / vStrafe) < hitVTime;
        canHitSlice = sliceSwingTime + (dGS / vStrafe) < hitGSTime;

        if (!canHitDrive && !canHitVolley)
        {
            canHitDrive = driveSwingTime + (dGS / vRun) < hitGSTime;
            canHitVolley = volleySwingTime + (dV / vRun) < hitVTime;
            if (canHitDrive || canHitVolley)
            {
                shouldRun = true;
            }
            else
            {
                if (!canHitSlice)
                {
                    canHitSlice = sliceSwingTime + (dGS / vRun) < hitGSTime;
                    shouldRun = true;
                }
            }

        }

        Debug.Log("hitGSTime =" + hitGSTime);
        Debug.Log("hitVTime =" + hitVTime);

        Debug.Log("dGS =" + dGS);
        Debug.Log("dV =" + dV);


        Debug.Log("canHitDrive = " + canHitDrive  );
        Debug.Log("canHitVolley = " + canHitVolley); 
        Debug.Log("canHitSlice = " + canHitSlice );
        Debug.Log("shouldRun = " + shouldRun);

        moveTo = dV > dGS ? moveGS : moveV;

        state = shouldRun ? PlayerState.Run : PlayerState.Strafe;
        hit = HitType.None;

        hitTimer = 1000;

        if (canHitVolley && !shouldRun)
        {
            hitTimer = hitVTime - volleySwingTime;
            hit = HitType.Volley;
            moveTo = moveV;
        }
        else if (canHitDrive)
        {
            hit = HitType.Drive;
            hitTimer = hitGSTime - driveSwingTime;
        }
        else if (canHitSlice)
        {
            hit = HitType.Slice;
            hitTimer = hitGSTime - sliceSwingTime;
        }
        else if (canHitVolley)
        {
            hit = HitType.Volley;
            hitTimer = hitVTime - volleySwingTime;
            moveTo = moveV;
        }

        
    }

    private void GetGroundstrokeHitPoint(out Vector3 result, out float timing)
    {
        Ball _ball = ball.GetComponent<Ball>();

        result = Vector3.zero;
        timing = float.MaxValue;
        float d = float.MaxValue;

        bool ground = false;
        int t = 0;
        foreach (var point in _ball.Trajectory)
        {
            t++;

            if (_ball.Velocity.z > 0 && point.z < 0.5)
                continue;

            if (point.y <= 0)
                ground = true;

            if (ground)
            {

                if (point.y > groundstrokeHeightMax || point.y < groundstrokeHeightMin)
                    continue;

                float _d = Vector3.Distance(transform.position, point);
                if (d > _d)
                {
                    result = point;
                    timing = (_ball.TrajectoryTime + Time.fixedDeltaTime * (t - 1)) - Time.time;
                    d = _d;
                }
            }
        }
    }

    private void GetVolleyHitPoint(out Vector3 result, out float timing)
    {
        Ball _ball = ball.GetComponent<Ball>();

        result = Vector3.zero;
        timing = float.MaxValue;
        float d = float.MaxValue;

        int t = 0;
        foreach (var point in _ball.Trajectory)
        {
            t++;
            if (_ball.Velocity.z > 0 && point.z < 0.5)
                continue;

            if (point.y <= 0)
                break;

            if (point.y > volleyHeightMax || point.y < volleyHeightMin)
                continue;

            float _d = Vector3.Distance(transform.position, point);
            if (d > _d)
            {
                timing = (_ball.TrajectoryTime + Time.fixedDeltaTime * (t - 1))-Time.time;
                result = point;
                d = _d;
            }
        }

    }

    private Vector3 GetBouncePoint()
    {
        Ball _ball = ball.GetComponent<Ball>();

        Vector3 result = Vector3.zero;


        foreach (var point in _ball.Trajectory)
        {
            if (point.y <= 0)
            {
                result = point;
                result.y = 0;
                break;
            }
            
        }

        return result;
    }

    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();
    }
}