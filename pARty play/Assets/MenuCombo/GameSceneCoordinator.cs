using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneCoordinator : MonoBehaviour {
    public GameObject parentScene,menuScene;

	// Use this for initialization
	void Start () {
		
	}

    public void StartGame()
    {
        parentScene.SetActive(true);
        menuScene.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
