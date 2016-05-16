using UnityEngine;
using System.Collections;



public class TennisistHuman : Tennisist {

	// Update is called once per frame
	void Update () {

        if (state == PlayerState.Recover)
        {
            GameObject.Find("MoveTo").transform.position = moveTo;
        }
        else
        {
            GameObject.Find("MoveTo").transform.position = new Vector3(0, -1, 0);
        }


        if (state == PlayerState.Recover || state == PlayerState.Watching)  {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {            
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100, 1 << LayerMask.NameToLayer("Ground")))
                {
                    if (hit.point.z < 0)
                    {
                        moveTo = hit.point;
                        moveTo.y = 0;
                        state = PlayerState.Recover;
                    }
                }
            }            
        }
        base.Update();
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
