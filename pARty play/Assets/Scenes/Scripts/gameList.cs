using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

// IS CALLED GAMELIST..... BUT DOES EVERYTHING.... NEEDS REFACTORED
public class gameList : MonoBehaviour
{

    public GameObject canMain, canGamesList, canLobby, gameButtonPreFab, GameListContent, playerNamePreFab, PlayersListContent, readyToLobbyButton, nicknameInput;
    public string[] gameNames = { "Pumpkin Pick", "Quick Corners", "Spooky Stones" };

    public Text roomKeyLabel;

    public int minGamesRequired = 1;
    public List<string> gamesSelected = new List<string>();

    private List<User> players = new List<User>();

    // TODO: Change to User entity
    public string nickname;

    // public List<string> Players
    // {
    //     get
    //     {
    //         return players;
    //     }

    //     set
    //     {
    //         players = value;
    //         this.addPlayersToLobbyList();
    //     }
    // }


    public GameObject eventSystem;
    private GameDataRelay gameDataRelay;

    public string roomKeyVal = "";

    void Awake()
    {
        eventSystem = GameObject.Find("EventSystem");
        gameDataRelay = eventSystem.GetComponent<GameDataRelay>();
    }

    // Use this for initialization
    void Start()
    {
        nickname = "Matt";

        canMain.SetActive(true);
        canGamesList.SetActive(false);
        canLobby.SetActive(false);

        // Build the games selection list
        foreach (string gameName in gameNames)
        {
            GameObject temp = Instantiate(gameButtonPreFab, GameListContent.transform);
            var gameButton = temp.GetComponent<gameButton>();
            gameButton.gameName = gameName;

            temp.GetComponent<Button>().onClick.AddListener(() =>
            {

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
    public void hostGameClicked()
    {
        QueueEvent(() =>
        {
            canMain.SetActive(false);
            canGamesList.SetActive(true);
            this.gameDataRelay.createRoom();

        });
    }

    public void startGame()
    {
        this.gameDataRelay.startGame(roomKeyVal);
    }

    public void gameStarted()
    {
    }

    public void timeLeft(string time)
    {
    }

    public void gameOver()
    {
        int score = 0;
        this.gameDataRelay.gatherScores(score);
    }

    public void finalScores(object scores)
    {
    }




    public void roomKey(string key)
    {
        QueueEvent(() =>
        {
            roomKeyVal = key;
            roomKeyLabel.text = key;
            Debug.Log(key);
            // do something with this.
        });
    }

    public void partyCodeEntered()
    {

        QueueEvent(() =>
                {
                    canMain.SetActive(false);
                    // TODO: Navigate to the appropriate lobby
                    readyToLobbyButtonClicked();
                });

    }

    public void cancelGameSelectionClicked()
    {
        QueueEvent(() =>
        {

            canMain.SetActive(true);
            canGamesList.SetActive(false);
        });

    }

    public void readyToLobbyButtonClicked()
    {
        // Players.Add("Matt");
        // Players.Add("Adam");
        // Players.Add("Kyle");
        // Players.Add("RyanIsABitch");
        QueueEvent(() =>
        {
            canGamesList.SetActive(false);
            canLobby.SetActive(true);
        });

        // TODO: This needs to happen live
        // addPlayersToLobbyList();
    }


    public void addPlayerToLobber(User user)
    {
        QueueEvent(() =>
        {

            Debug.Log("Add user to lobby");
            GameObject temp = Instantiate(playerNamePreFab, PlayersListContent.transform);
            temp.GetComponent<Text>().text = user.name;

            if (user.name.Equals(nickname))
            {
                temp.GetComponent<Text>().color = Color.blue;
            }
        });
    }

    // NEED TO LISTEN ON LOBBY CHANGES
    // public void addPlayersToLobbyList() {
    //     // Clear the previous player list
    //     foreach (Transform child in PlayersListContent.transform) {
    //         Debug.Log(child.gameObject);
    //         Destroy(child.gameObject);
    //     }

    //     // Build the games selection list
    //     foreach (string player in Players) {
    //         GameObject temp = Instantiate(playerNamePreFab, PlayersListContent.transform);
    //         temp.GetComponent<Text>().text = player;

    //         if (player.Equals(nickname)) {
    //             temp.GetComponent<Text>().color = Color.blue;
    //         }
    //     }
    // }

    // public void nicknameChanged() {
    //     Players.Remove(nickname);
    //     nickname = nicknameInput.GetComponent<InputField>().text;
    //     Players.Add(nickname);

    //     addPlayersToLobbyList();
    // }

    public void cancelLobbyClicked()
    {
        QueueEvent(() =>
        {

            canLobby.SetActive(false);
            canGamesList.SetActive(true);
        });
    }

    // TODO: Load the game
    public void readyToPlayClicked()
    {

    }

    public void QueueEvent(Action action)
    {
        lock (m_queueLock)
        {
            m_queuedEvents.Add(action);
        }
    }

    void Update()
    {
        MoveQueuedEventsToExecuting();

        while (m_executingEvents.Count > 0)
        {
            Action e = m_executingEvents[0];
            m_executingEvents.RemoveAt(0);
            e();
        }
    }

    private void MoveQueuedEventsToExecuting()
    {
        lock (m_queueLock)
        {
            while (m_queuedEvents.Count > 0)
            {
                Action e = m_queuedEvents[0];
                m_executingEvents.Add(e);
                m_queuedEvents.RemoveAt(0);
            }
        }
    }

    private System.Object m_queueLock = new System.Object();
    private List<Action> m_queuedEvents = new List<Action>();
    private List<Action> m_executingEvents = new List<Action>();
}
