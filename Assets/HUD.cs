using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {

    GameObject ball;
	// Use this for initialization
	void Start () {
        ball = GameObject.Find("Ball");	
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<Text>().text = ball.GetComponent<Rigidbody>().velocity.magnitude.ToString("F2") + ", " + (ball.GetComponent<Ball>().Sliced ? "sliced" : "flat");
	}
}
