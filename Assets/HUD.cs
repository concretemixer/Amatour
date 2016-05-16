using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    GameObject ball;
    Game game;
	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");
        game = GameObject.Find("Game").GetComponent<Game>();	
	}
	
	// Update is called once per frame
	void Update () {
        GameObject o = GameObject.Find("BallText");
        o.GetComponent<Text>().text = ball.GetComponent<Rigidbody>().velocity.magnitude.ToString("F2") + ", " + (ball.GetComponent<Ball>().Sliced ? "sliced" : "flat");

        string score = "??";

        if (Mathf.Abs(game.scoreGame[0] - game.scoreGame[1])>1.5 && Mathf.Max(game.scoreGame[1], game.scoreGame[0]) > 3.5 )
            score = (game.scoreGame[0] > game.scoreGame[1]) ? "W - L" : "L - W";
        else if (game.scoreGame[0] < 3 || game.scoreGame[1] < 3)
        {
            string[] pts = { "0", "15", "30", "40", "x", "xx" };
            score = pts[game.scoreGame[0]] + " - " + pts[game.scoreGame[1]];
        }
        else
        {
            if (game.scoreGame[1] == game.scoreGame[0])
                score = "Deuce";
            else
                score = game.scoreGame[0] > game.scoreGame[1] ? "Adv - 0" : "0 - Adv";
        }


        

        o = GameObject.Find("ScoreText");
        o.GetComponent<Text>().text = (game.scoreSet[0]+ "-"+ game.scoreSet[1]) + " | " + score + " | " +
            game.state.ToString();

        o = GameObject.Find("FpsText");
        o.GetComponent<Text>().text = (1.0f / Time.smoothDeltaTime).ToString("F2");
	}
}
