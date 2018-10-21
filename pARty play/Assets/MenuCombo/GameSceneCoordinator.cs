using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneCoordinator : MonoBehaviour
{
    public GameObject parentScene, menuScene;

    // Use this for initialization
    void Start()
    {

    }

    public void StartGame()
    {
        parentScene.SetActive(true);
        menuScene.SetActive(false);
    }
    public int requestScore()
    {
        GameObject temp = GameObject.Find("TxtScore");
        Text text = temp.GetComponent<Text>();

        return int.Parse(text.text.Split(':')[1].Trim());
    }

    public void EndGame()
    {
        parentScene.SetActive(false);
        menuScene.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
