using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json;

[System.Serializable]
public class RoomKey {
	public string key;
}

// potentially derivable UI abstraction.
//  need to use for main thread UI updates.
// NOTE: for UI updates, and an example on the socket client,
// see: https://github.com/floatinghotpot/socket.io-unity/blob/master/Demo/SocketIOScript.cs
public class GameUI {
}

public class GameDataRelay : MonoBehaviour {
	public string joinedRoom = "";
	public Socket relay;
	public bool isHost = false;
	public Text dataEntry;
	public Text txtRoom;
	public InputField inputRoom;


	void SetupClient() {
		relay = IO.Socket("https://party-play.herokuapp.com");
		relay.On(Socket.EVENT_CONNECT, () =>
		{
			Debug.Log("Connected to middleman");
		});				
	}

	void Awake () {
		SetupClient();
	}

	// host creates a room
	public void createRoom() {
		isHost = true;
		relay.Emit("createRoom");
	}

	public void joinRoom() {
		// Join specified room id
		relay.Emit("joinRoom", inputRoom.text);
		Debug.Log(inputRoom.text);
		// dataEntry.text = "Room joined";
	}

	void Start () {
		// When anyone joins the room output room Id
		relay.On("joinedRoom", (data) => {
			string str = data.ToString();
			RoomKey id = JsonUtility.FromJson<RoomKey>(str);
			Debug.Log(id.key);
		});
	}

	public void BtnOnClick(){//button was click, send our id
		relay.Emit(joinedRoom, "ID was clicked");
	}

}
