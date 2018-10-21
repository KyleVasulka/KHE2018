using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostScript : MonoBehaviour {
    public GameObject EventSystem;
    private GameDataRelay gameDataRelay;


    // Use this for initialization
    void Start () {
        gameDataRelay = EventSystem.GetComponent<GameDataRelay>();


    }

    public void onHosting()
    {

        gameDataRelay.createRoom();

    }

    // Update is called once per frame
    void Update () {
		
	}
}
