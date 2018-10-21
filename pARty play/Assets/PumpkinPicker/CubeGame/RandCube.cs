using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandCube : MonoBehaviour {
    public GameObject CubePrefab;


	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ResetCube()
    {
        GameObject plane = GameObject.Find("PlayArea(Clone)");
        Destroy(GameObject.FindGameObjectWithTag("cube"));//remove current cube

        var cube = Instantiate(CubePrefab);
        cube.transform.parent = plane.transform;
        cube.transform.localPosition = new Vector3(Random.Range(-2.5f,2.5f), .5f, Random.Range(-2.5f, 2.5f));

    }
}
