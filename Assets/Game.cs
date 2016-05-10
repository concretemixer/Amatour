using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

    public enum InGameState
    {
        FirstServe,
        SecondServe,
        Playing,
        PointWon
    }

    int[] scoreGame = new int[2];
    int[] scoreSet = new int[2];
    int[] scoreTotal = new int[2];
    

    Tennisist[] players;
    Ball ball;

    InGameState state;

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

        players[idxServing].Serve(state == InGameState.FirstServe);
    }

	// Update is called once per frame
	void Update () {
	
	}
}
