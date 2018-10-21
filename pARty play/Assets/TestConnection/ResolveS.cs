using GoogleARCore.Examples.CloudAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResolveS : MonoBehaviour {
    public GameObject AnchControl;
    private CloudAnchorController CAnchorController;
    public GameDataRelay gameDataRelay;
    public Text Roomtext;

    // Use this for initialization
    void Start()
    {
        //AnchControl = GameObject.Find("CloudAnchorController");

    }
    public void ResolveAnchor(AnchorPayload payload)
    {
        CAnchorController = AnchControl.GetComponent<CloudAnchorController>();
        CAnchorController._ResolveAnchorFromId(payload.CloudID);

        
    }
    private AnchorPayload ourPayload;
    public void Update()
    {
        if (gameDataRelay.getUser().trigger_get_anchor)
        {//update the AnchorPayload
            gameDataRelay.getUser().trigger_get_anchor = false;
            AnchorPayload ourPayload = gameDataRelay.getAnchorPayload(); 
            ResolveAnchor(ourPayload);

        }
    }

    public void Join()
    {
        //get partyID
        Debug.Log("attempting to join room");
        gameDataRelay.joinRoom(Roomtext.text);
    }
}
