using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public enum InGameState
    {
        None,
        FirstServe,
        SecondServe,
        Playing,
        PointWonServer,
        PointLostServer
    }

    public int[] scoreGame = new int[2];
    int[] scoreSet = new int[2];
    int[] scoreTotal = new int[2];
    

    Tennisist[] players;
    Ball ball;

    public InGameState state=InGameState.None;

    int idxServing = 0;

	// Use this for initialization
	void Start () {
        players = GetComponentsInChildren<Tennisist>();
        ball = GetComponentInChildren<Ball>();
	}

    void StartPoint()
    {
        state = InGameState.FirstServe;

        bool adCourt = (scoreGame[0] + scoreGame[1]) % 2 != 0;

        Vector3 posServe = (adCourt ? GameObject.Find("ServeAd") : GameObject.Find("ServeDeuce")).transform.position;
        Vector3 posReturn = (adCourt ? GameObject.Find("ReturnAd") : GameObject.Find("ReturnDeuce")).transform.position;

        if (idxServing == 0)
        {           
            players[0].transform.position = posServe*-1;
            players[1].transform.position = posReturn*-1;
        }
        else
        {         
            players[1].transform.position = posServe;
            players[0].transform.position = posReturn;
        }

    }

    public void onNewPoint()
    {
        StartPoint();

        players[idxServing].Serve(state == InGameState.FirstServe, (scoreGame[0] + scoreGame[1]) % 2 == 0);
        players[1 - idxServing].Receive();
    }

	// Update is called once per frame
	void Update () {
	
	}

    Vector3 hitPosition;
    Vector3 bouncePosition;

    public void onBallHitGround(Vector3 pos)
    {
        if (state == InGameState.Playing)
        {
            if (Mathf.Sign(pos.z) == Mathf.Sign(bouncePosition.z))
            {
                if (Mathf.Sign(players[idxServing].transform.position.z) == Mathf.Sign(bouncePosition.z))
                    state = InGameState.PointLostServer;
                else
                    state = InGameState.PointWonServer;
            }
            else
            {
                bool ballIn = true;
                if (Mathf.Abs(pos.x) > 4.1f)
                    ballIn = false;
                if (Mathf.Abs(pos.z) > 11.87f)
                    ballIn = false;

                if (!ballIn)
                {
                    if (Mathf.Sign(players[idxServing].transform.position.z) == Mathf.Sign(hitPosition.z))
                        state = InGameState.PointLostServer;
                    else
                        state = InGameState.PointWonServer;
                }
            }
            bouncePosition = pos;
        }
        else if (state == InGameState.FirstServe || state == InGameState.SecondServe)
        {
            bool adCourt = (scoreGame[0] + scoreGame[1]) % 2 != 0;
            bool ballIn = true;
            if (Mathf.Abs(pos.x) > 4.1f)
                ballIn = false;
            if (Mathf.Abs(pos.z) > 6.4f)
                ballIn = false;
            if (adCourt && pos.x < 0)
                ballIn = false;
            if (!adCourt && pos.x > 0)
                ballIn = false;

            if (ballIn)
            {
                state = InGameState.Playing;
                bouncePosition = pos;
            }
            else
            {
                if (state == InGameState.FirstServe)
                {
                    state = InGameState.SecondServe;
                    players[idxServing].Serve(state == InGameState.FirstServe, (scoreGame[0] + scoreGame[1]) % 2 == 0);
                    players[1 - idxServing].Receive();
                }
                else
                {
                    state = InGameState.PointLostServer;                   
                }
            }
        }


        if (state == InGameState.PointLostServer)
        {
            scoreGame[1 - idxServing]++;
            state = InGameState.None;


        }
        if (state == InGameState.PointWonServer)
        {
            scoreGame[idxServing]++;
            state = InGameState.None;
        }
    }

    public void onBallHit(Vector3 pos)
    {
        hitPosition = pos;
        foreach(var o in players)
            if (Mathf.Sign(o.transform.position.z) != Mathf.Sign(ball.transform.position.z))              
                o.OnBallHit();
    }
}
