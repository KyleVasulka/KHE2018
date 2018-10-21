using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyCode : MonoBehaviour
{

    public Text text;
    public InputField input;
	public GameObject eventSystem;
    private GameDataRelay gameDataRelay;

	public gameList gameList;

    void Awake() {
		eventSystem = GameObject.Find("EventSystem");
		gameDataRelay = eventSystem.GetComponent<GameDataRelay>();
	}
	
	// Use this for initialization
    void Start()
    {
		
        this.input = GetComponent<InputField>();
		this.input.characterLimit = 4;
        this.input.onValueChanged.AddListener((item) =>
        {
            if (item.Length == 4)
            {

                gameDataRelay.joinRoom(item.ToLower());
				gameList.partyCodeEntered();
            }
        });

    }

    // Update is called once per frame
    void Update()
    {

    }
}
