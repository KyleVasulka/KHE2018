using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// IS CALLED GAMELIST..... BUT DOES EVERYTHING.... NEEDS REFACTORED
public class gameList : MonoBehaviour {

    public GameObject canMain, canGamesList, canLobby, gameButtonPreFab, GameListContent, playerNamePreFab, PlayersListContent, readyToLobbyButton, nicknameInput;
    public string[] gameNames = { "Pumpkin Pick", "Quick Corners", "Spooky Stones" };

    public int minGamesRequired = 3;
    public List<string> gamesSelected = new List<string>();

    private List<string> players = new List<string>();

    // TODO: Change to User entity
    public string nickname;

    public List<string> Players
    {
        get
        {
            return players;
        }

        set
        {
            players = value;
            this.addPlayersToLobbyList();
        }
    }

    // Use this for initialization
	void Start () {
        nickname = "Matt";

        canMain.SetActive(true);
        canGamesList.SetActive(false);
        canLobby.SetActive(false);

        // Build the games selection list
        foreach (string gameName in gameNames) {
            GameObject temp = Instantiate(gameButtonPreFab, GameListContent.transform);
            var gameButton = temp.GetComponent<gameButton>();
            gameButton.gameName = gameName;

            temp.GetComponent<Button>().onClick.AddListener(() => {
                
               //  var Button = temp.GetComponent<Button>();
                gameButton.isActive = !gameButton.isActive;

                if (gameButton.isActive) gamesSelected.Add(gameButton.gameName);
                else gamesSelected.Remove(gameButton.gameName);

                gameButton.check.SetActive(gameButton.isActive);

                readyToLobbyButton.SetActive(gamesSelected.Count == minGamesRequired);
            });

            Text btntext = temp.GetComponentInChildren<Text>();
            btntext.text = gameName;
        }
	}

    // This is now Host Party
    public void hostGameClicked() {
        canMain.SetActive(false);
        canGamesList.SetActive(true);
    }

    public void partyCodeEntered() {
        canMain.SetActive(false);
        // TODO: Navigate to the appropriate lobby
        readyToLobbyButtonClicked();
    }

    public void cancelGameSelectionClicked() {
        canMain.SetActive(true);
        canGamesList.SetActive(false);
    }

    public void readyToLobbyButtonClicked() {
        Players.Add("Matt");
        Players.Add("Adam");
        Players.Add("Kyle");
        Players.Add("RyanIsABitch");

        canGamesList.SetActive(false);
        canLobby.SetActive(true);

        // TODO: This needs to happen live
        addPlayersToLobbyList();
    }

    // NEED TO LISTEN ON LOBBY CHANGES
    public void addPlayersToLobbyList() {
        // Clear the previous player list
        foreach (Transform child in PlayersListContent.transform) {
            Debug.Log(child.gameObject);
            Destroy(child.gameObject);
        }
                       
        // Build the games selection list
        foreach (string player in Players) {
            GameObject temp = Instantiate(playerNamePreFab, PlayersListContent.transform);
            temp.GetComponent<Text>().text = player;

            if (player.Equals(nickname)) {
                temp.GetComponent<Text>().color = Color.blue;
            }
        }
    }

    public void nicknameChanged() {
        Players.Remove(nickname);
        nickname = nicknameInput.GetComponent<InputField>().text;
        Players.Add(nickname);

        addPlayersToLobbyList();
    }

    public void cancelLobbyClicked() {
        canLobby.SetActive(false);
        canGamesList.SetActive(true);
    }

    // TODO: Load the game
    public void readyToPlayClicked() {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
