using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;

[System.Serializable]
public class RoomKey
{
    public string key;
}

public class User
{
    public string uid;
    public bool isHost;
    public string name;
    public string currentRoomKey;

    public User()
    {
        uid = System.Guid.NewGuid().ToString();
        isHost = false;
        name = "Unset";
        currentRoomKey = "";
    }


    public string asJson()
    {
        return JsonUtility.ToJson(this);
    }

}

// potentially derivable UI abstraction.
//  need to use for main thread UI updates.
// NOTE: for UI updates, and an example on the socket client,
// see: https://github.com/floatinghotpot/socket.io-unity/blob/master/Demo/SocketIOScript.cs
public class GameUI
{
}

public class GameDataRelay : MonoBehaviour
{
    public string joinedRoom = "";
    public Socket relay;
    public bool isHost = false;
    public Text dataEntry;
    public Text txtRoom;
    public InputField inputRoom;
    public User user;

    void SetupClient()
    {
        Debug.Log("Set up client	");

        // relay = IO.Socket("ws://party-play.herokuapp.com/");
        relay = IO.Socket("ws://localhost:8080/");

        Debug.Log(relay);

        relay.On(Socket.EVENT_CONNECT, () =>
        {
            Debug.Log("Connected to middleman");
        });
    }

    void Awake()
    {
        user = new User();
        SetupClient();
    }

    // host creates a room
    public void createRoom()
    {
        Debug.Log("Create Room");
        isHost = true;
        relay.Emit("createRoom", user.asJson());
    }

    public void joinRoom()
    {
        user.currentRoomKey = inputRoom.text;
        // Join specified room id
        relay.Emit("joinRoom", user);
        Debug.Log(inputRoom.text);
        // dataEntry.text = "Room joined";
    }

    public void drop()
    {
		Debug.Log("Leave game");
        relay.Emit("leaveRoom", user.asJson());
    }

    void Start()
    {
        // When anyone joins the room output room Id
        relay.On("joinedRoom", (data) =>
        {
            string str = data.ToString();
            RoomKey id = JsonUtility.FromJson<RoomKey>(str);
            user.currentRoomKey = id.key;
            Debug.Log(id.key);
            Debug.Log(user.uid);
        });

        // This is emmited from the channel of the room
        relay.On("newMemberJoined", (data) =>
         {
             User newUser = JsonUtility.FromJson<User>(data.ToString());
             if (user.uid == newUser.uid)
             {
                 Debug.Log("I Joined!");
             }
             else
             {
                 Debug.Log("New person joined!");
                 Debug.Log(newUser.uid);
             }
         });


        // This is emmited from the channel of the room
        relay.On("memberDropped", (data) =>
         {
             User deadUser = JsonUtility.FromJson<User>(data.ToString());
             Debug.Log("Someone left!");
             Debug.Log(deadUser.uid);

         });

    }

    public void BtnOnClick()
    {//button was click, send our id
        relay.Emit(joinedRoom, "ID was clicked");
    }

}
