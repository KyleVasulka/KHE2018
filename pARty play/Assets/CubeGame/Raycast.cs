using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Raycast : MonoBehaviour
{

    void FixedUpdate()
    {

    }

    // Use this for initialization
    void Start()
    {

    }
    public Text txtScore;
    private int Score = 0;
    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Attempting Raycast");
            RaycastHit hit;
            Ray landingRay = new Ray(transform.position, transform.forward);

            if (Physics.Raycast(landingRay, out hit, 10f))
            {
                if (hit.collider.tag == "cube")
                {
                    //cube was hit, send info to server
                    //for now update own score
                    Score += 1;
                    txtScore.text = "Score: " + Score;
                    //for now reset cube, in future remove this
                    GameObject PlayArea = GameObject.Find("PlayArea(Clone)");
                    RandCube RandCubeScript = PlayArea.GetComponent<RandCube>();
                    RandCubeScript.ResetCube();

                }
            };
            //Do something with the touches
            Debug.Log("leaving loop");
            return;
        }
        else
        {
            return;   
        }

        Touch myTouch = Input.GetTouch(0);
        Touch[] myTouches = Input.touches;


       
        for (int i = 0; i < Input.touchCount; i++)
        {
            
        }
        
    }
}
