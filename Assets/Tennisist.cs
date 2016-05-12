using UnityEngine;
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
        Smash = 6,
        Serve = 7
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

    protected float vRun = 5f;
    protected float vStrafe = 1.5f;
    protected float reactCooldown = 0.1f;

    protected float driveSwingTime = 0.9f;
    protected float volleySwingTime = 0.25f;
    protected float sliceSwingTime = 0.5f;
    protected float reachSwingTime = 0.25f;
    protected float serveSwingTime = 1.0f;

    protected Vector3 driveHitDistanceFH = new Vector3 (-1.4f,0, -0.8f);
    protected Vector3 driveHitDistanceBH = new Vector3(1.25f, 0, -0.9f);

    protected Vector3 volleyHitDistanceFH = new Vector3(-1.2f, 0, -0.7f);
    protected Vector3 volleyHitDistanceBH = new Vector3(0.75f, 0, -0.6f);

    protected Vector3 serveHitDistance = new Vector3(0.55f, 2.9f, 0.85f);
    protected Vector3 serveTossDistance = new Vector3(0.55f, 1f, 0.85f);
    protected Vector3 serveTossV = new Vector3(0, 7.6f, 0);

    protected float sliceHitDistance = 0.75f;
    protected float reachHitDistance = 1.5f;

    protected float groundstrokeHeightMin = 0.5f;
    protected float groundstrokeHeightMax = 1.5f;

    protected float volleyHeightMin = 0.7f;
    protected float volleyHeightMax = 2.2f;

    protected float reachHeightMin = 0.1f;

    GameObject ball;


    PlayerState state = PlayerState.Watching;
    HitType hit;
    float hitHeightK;
    bool forehand;
    float hitTimer =  7000;
    bool deuceCourt;


    Vector3 moveTo = Vector3.zero;
    Vector3 contactPoint = Vector3.zero;



	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");
	}

	// Update is called once per frame
	void Update () {

       // if (state!=PlayerState.Watching)
        //    Debug.Log("S = " + state.ToString());
        Animator animator = GetComponent<Animator>();

        hitTimer -= Time.deltaTime;
        //Debug.Log("hitTimer2 = " + hitTimer);

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
                               // moveTo.z = -13;
                             //   Time.timeScale = 0;
                            }
                            break;
                        case HitType.Slice:
                            if (hitTimer < -sliceSwingTime)
                            {
                                ball.GetComponent<Ball>().HitSlice();
                                state = PlayerState.Recover;
                                // moveTo.z = -13;
                                //   Time.timeScale = 0;
                            }
                            break;
                        case HitType.Volley:
                            if (hitTimer < -volleySwingTime)
                            {
                                Vector3 v = ball.GetComponent<Rigidbody>().velocity;
                                v.z = -v.z;
                                v.y = Mathf.Abs(v.y);
                                ball.GetComponent<Rigidbody>().velocity = v;
                                //ball.GetComponent<Ball>().Hit();
                                state = PlayerState.Recover;
                                //moveTo.z = -4;
                               // Time.timeScale = 0.1f;
                            }
                            break;
                        case HitType.Serve:
                            if (hitTimer < -serveSwingTime)
                            {
#if UNITY_EDITOR

                               // UnityEditor.EditorApplication.isPaused = true;
#endif
                               // ball.transform.position = transform.position + serveHitDistance;
                                ball.GetComponent<Ball>().HitServe(deuceCourt);
                                state = PlayerState.Recover;
                                moveTo = transform.position;
                                hitTimer = 1000;

                                //   moveTo.z = -4;
                                // Time.timeScale = 0.1f;
                            }
                            break;

                        default:
                            state = PlayerState.Watching;
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
            animator.SetFloat("HitHeight", hitHeightK);
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

        if (state == PlayerState.Run || state == PlayerState.Strafe)
        {
            Vector3 dir = moveTo - transform.position;
            dir.Normalize();
            animator.SetFloat("MoveDirectionX", -dir.x);
            animator.SetFloat("MoveDirectionY", dir.z);
        }
        else
        {
            animator.SetFloat("MoveDirectionX", 0);
            animator.SetFloat("MoveDirectionY", 0);
        }

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

        float opp = Mathf.Sign(-transform.position.z);
        float shift = Mathf.Sign(transform.position.x - hitGS.x);

        forehand = (opp*shift) < 0;

        moveV.y = 0;
        moveGS.y = 0;



        if (forehand)
        {
            moveV += volleyHitDistanceFH * opp;
            moveGS += driveHitDistanceFH * opp;
        }
        else
        {
            moveV += volleyHitDistanceBH * opp;
            moveGS += driveHitDistanceBH * opp;
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

        Debug.Log("hitGSTime =" + hitGSTime + " / " + (driveSwingTime + (dGS / vRun)));
        Debug.Log("hitVTime =" + hitVTime + " / " + (volleySwingTime + (dV / vRun)));

     //   Debug.Log("dGS =" + dGS);
      //  Debug.Log("dV =" + dV);

/*
        Debug.Log("canHitDrive = " + canHitDrive  );
        Debug.Log("canHitVolley = " + canHitVolley); 
        Debug.Log("canHitSlice = " + canHitSlice );
        Debug.Log("shouldRun = " + shouldRun);*/

        moveTo = dV > dGS ? moveGS : moveV;
        //moveTo = transform.position;
        //moveTo.y = 0;

        state = shouldRun ? PlayerState.Run : PlayerState.Strafe;
        hit = HitType.None;



        hitTimer = 100;

        if (canHitVolley && !shouldRun)
        {
            hitHeightK = (hitV.y - volleyHeightMin) / (volleyHeightMax - volleyHeightMin);
            hitTimer = hitVTime - volleySwingTime;
            hit = HitType.Volley;
            moveTo = moveV;
        }
        else if (canHitDrive)
        {
            hitHeightK = (hitGS.y - groundstrokeHeightMin) / (groundstrokeHeightMax - groundstrokeHeightMin);
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
            hitHeightK = (hitV.y - volleyHeightMin) / (volleyHeightMax - volleyHeightMin);
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

    public void Receive()
    {
        Animator animator = GetComponent<Animator>();
        animator.Rebind();
        state = PlayerState.Watching;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        moveTo = transform.position;
    }


    public void Serve(bool first, bool deuce)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        moveTo = transform.position;

        Animator animator = GetComponent<Animator>();
        animator.Rebind();
        state = PlayerState.Swing;
        hit = HitType.Serve;
        hitTimer = 0;
        animator.SetInteger("HitType", (int)hit);
        animator.SetBool("ReadyToHit", true);

        ball.transform.position = transform.position + serveTossDistance;
        ball.GetComponent<Rigidbody>().velocity = serveTossV;

        deuceCourt = deuce;
#if UNITY_EDITOR

        // UnityEditor.EditorApplication.isPaused = true;
#endif
    }
}
