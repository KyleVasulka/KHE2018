using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
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
    public string roomKey;

    public User()
    {
        uid = System.Guid.NewGuid().ToString();
        isHost = false;
        name = "Unset";
        roomKey = "";
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
	private readonly string httpEndpoint = "http://localhost:8080/";
	// private readonly string httpEndpoint = "https://party-play.herokuapp.com/";
	private readonly string wsEndpoint = "ws://localhost:8080/";
	// private readonly string wsEndpoint = "ws://party-play.herokuapp.com/";

    public string joinedRoom = "";
    public Socket relay;
    public bool isHost = false;
    public Text dataEntry;
    public Text txtRoom;
    public InputField inputRoom;
    public User user;

    void SetupClient()
    {
        Debug.Log("Set up cliet");

        // relay = IO.Socket("ws://party-play.herokuapp.com/");
        relay = IO.Socket(this.wsEndpoint);

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
		//StartCoroutine(CreateRoom());
     relay.Emit("createRoom", user.asJson());
    }

    public void joinRoom()
    {
        user.roomKey = inputRoom.text;
        // Join specified room id
        relay.Emit("joinRoom", user.asJson());
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
            user.roomKey = id.key;
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


	IEnumerator CreateRoom() {
		Debug.Log(user.uid);
        // UnityWebRequest uwr = UnityWebRequest.Post(httpEndpoint + "createRoom", user.ToString());
    var request = new UnityWebRequest(httpEndpoint + "createRoom", "POST");

    byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(user.ToString());
    request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

		// uwr.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();
 
        if(request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
        }
        else {
            // Show results as text
            Debug.Log(request.downloadHandler.text);
        }
    }

}
