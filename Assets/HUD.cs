using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    GameObject ball;
    GameObject game;
	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");
        game = GameObject.Find("Game");	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject o = GameObject.Find("BallText");
        o.GetComponent<Text>().text = ball.GetComponent<Rigidbody>().velocity.magnitude.ToString("F2") + ", " + (ball.GetComponent<Ball>().Sliced ? "sliced" : "flat");

        o = GameObject.Find("ScoreText");
        o.GetComponent<Text>().text = game.GetComponent<Game>().scoreGame[0].ToString() + "-" + game.GetComponent<Game>().scoreGame[1].ToString() + " | " +
            game.GetComponent<Game>().state.ToString();

	}
}
