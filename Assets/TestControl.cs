using UnityEngine;
using System.Collections;

public class TestControl : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    bool key = true;
    float lifetime=0;

	void Update () {
        lifetime += Time.deltaTime;
        Animator animator =  GetComponent<Animator>();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Ready") || animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            if (key)
            {
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    animator.SetTrigger("MakeServe");
                    key = false;
                    nailEndPos = true;
                }

                if (Input.GetKey(KeyCode.RightArrow))
                {
                    animator.SetTrigger("MakeFHVolley");
                    key = false;
                    nailEndPos = true;
                   // Time.timeScale = 0.1f;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    animator.SetTrigger("MakeBHVolley");
                    key = false;
                    nailEndPos = true;
                }
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    animator.SetTrigger("MakeBHSlice");
                    key = false;
                    nailEndPos = true;
                }
                          /*
                if (lifetime > 2)
                {
                    animator.SetTrigger("MakeFHDrive");
                    key = false;
                    nailEndPos = true;
                }           */
            }
        }
        else
            key = true;


	}

    Vector3 nailPos = Vector3.zero;
    bool nailEndPos = false;
    

    void OnAnimatorMove()
    {
        Animator animator = GetComponent<Animator>();
        
        /*
        if (animator.GetNextAnimatorStateInfo(0).IsName("Ready"))
        {
            if (animator.IsInTransition(0) && updateRootAfter)
            {
                Debug.Log("Here");
                Vector3 newPosition = transform.position;
                Vector3 root = transform.Find("Root_jnt").localPosition;
                transform.position = newPosition + root;
                updateRootAfter = false;

               // Time.timeScale = 0;
            }            
        }
          */

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Running"))
        {
            Vector3 newPosition = transform.position;
            Vector3 fwd = transform.forward;

            float speed = 2.5f;

            if (animator.IsInTransition(0))
            {
                float k = animator.GetAnimatorTransitionInfo(0).normalizedTime;

                speed *= (1 - k) * (1 - k);

                var lookPos = new Vector3(0,12,0) - transform.position;
                lookPos.y = 0;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, k*k); 
            }

            transform.position = newPosition + fwd * Time.deltaTime * speed;

        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Ready"))
        {
            if (animator.IsInTransition(0))
            {
                Vector3 newPosition = transform.position;
                if (nailEndPos)
                {

                    Vector3 diff = transform.Find("Root_jnt").localPosition - nailPos;
                    nailPos = transform.Find("Root_jnt").localPosition;
                    // Time.timeScale = 0;
                    // newPosition.x += diff.x*2;
                    diff *= transform.localScale.z;
                    newPosition.z -= diff.z;
                    newPosition.x -= diff.x;
                    // newPosition.y = 0.5f;
                    //Debug.Log(nailPos.z+" / "+transform.Find("Root_jnt").localPosition.z);
                }
                //  else
                //   newPosition.y = 0.0f;

                transform.position = newPosition;


            }
            else
            {
                nailPos = transform.Find("Root_jnt").localPosition;
            }
        }
        else {
            if (nailPos != Vector3.zero)
            {
                nailEndPos = false;
                nailPos = Vector3.zero;
            }
        }
    }
}
