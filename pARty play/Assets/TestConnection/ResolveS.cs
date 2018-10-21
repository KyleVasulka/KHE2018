using GoogleARCore.Examples.CloudAnchors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveS : MonoBehaviour {
    public GameObject AnchControl;
    private CloudAnchorController CAnchorController;

    // Use this for initialization
    void Start()
    {
        AnchControl = GameObject.Find("CloudAnchorController");

    }
    public void ResolveAnchor(AnchorPayload payload)
    {
        CAnchorController = AnchControl.GetComponent<CloudAnchorController>();
        CAnchorController._ResolveAnchorFromId(payload.CloudID);
    }
}
