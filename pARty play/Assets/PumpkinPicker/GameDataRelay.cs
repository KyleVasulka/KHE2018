﻿// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.Networking;
// using Quobject.SocketIoClientDotNet.Client;
// using Newtonsoft.Json;
// using GoogleARCore.CrossPlatform;


// public class BaseDataBroadcast
// {
//     public string type;
// }

// [System.Serializable]
// public class JoinedRoomResult
// {

//     public string roomKey;
//     public AnchorPayload localizationData;
// }
// [System.Serializable]
// public class User
// {
//     public string uid;
//     public bool isHost;
//     public string name;
//     public string roomKey;
//     public AnchorPayload achor;
//     public bool trigger_get_anchor = false;

//     public User()
//     {
//         uid = System.Guid.NewGuid().ToString();
//         isHost = false;
//         name = "Unset";
//         roomKey = "";
//         achor = new AnchorPayload();
//     }

//     public string asJson()
//     {
//         return JsonUtility.ToJson(this);
//     }

// }
// [System.Serializable]
// public class AnchorPayload
// {
//     //public XPAnchor physicalAnchor;
//     public string CloudID;
//     public string ipAddress;
//     public int RoomKey;

//     public AnchorPayload()
//     {
//         CloudID = "";
//         ipAddress = "";
//         RoomKey = -1;
//     }

//     public string asJson()
//     {
//         return JsonUtility.ToJson(this);
//     }
// }


// // TODO: timeLeft and payload for each emit event.
// // should update accordingly. the game sequence should always have a value.
// public class GameSequence
// {
//     public User user;
//     public string joinedRoom;
//     public Socket relay;
//     public int timeLeft = 60;
//     public bool isHost = false;
//     public InputField inputRoom;

//     private readonly string wsEndpoint = "ws://party-play.herokuapp.com/";

//     public GameSequence(User usr, InputField fld)
//     {
//         user = usr;
//         inputRoom = fld;
//         SetupClient();
//     }

//     public void SetupClient()
//     {

//         // relay = IO.Socket("ws://party-play.herokuapp.com/");
//         relay = IO.Socket(this.wsEndpoint);

//         relay.On(Socket.EVENT_CONNECT, () =>
//         {
//             Debug.Log("Connected to middleman");
//         });
//     }

//     // host creates a room
//     public void hostCreatesARoom()
//     {
//         isHost = true;
//         relay.Emit("createRoom", user.asJson());
//     }

//     public void joinRoom(string room)
//     {
//         Debug.Log("Joining room with :" + room);
//         user.roomKey = room;
//         // Join specified room id
//         relay.Emit("joinRoom", user.asJson());
//     }

//     public void drop()
//     {
//         relay.Emit("leaveRoom", user.asJson());
//     }

//     public void broadcastLocalization()
//     {
//         // Possibly pass user to param
//         relay.Emit("broadcastLocalizationData", user);
//     }




//     public void broadcastData<T>(T obj) where T : BaseDataBroadcast
//     {
//         relay.Emit("broadcastData", JsonUtility.ToJson(obj));
//     }

//     public void stopGame()
//     {
//         relay.Emit("stopGame", user.asJson());
//     }

//     public void requestLocalizationData()
//     {
//         relay.Emit("requestLocalizationData", user.asJson());
//     }

//     public void setLocalizationData()
//     {
//         Debug.Log("user " + user.asJson());

//         relay.Emit("setLocalizationData", user.asJson());
//     }

//     public void setHostLocalizationRentalNumber(int roomNum)
//     {
//         user.achor.RoomKey = roomNum;
//     }

//     public void setHostLocalizationData(AnchorPayload payload)
//     {

        

//         Debug.Log("setHostLocalizationDataCalled " + payload.asJson());
//         user.achor = payload;
//         setLocalizationData();
//     }

//     public AnchorPayload GroupPayload; 
//     public void setupListeners()
//     {
//         // Invalid key specified..
//         relay.On("invalidKey", (data) => {
//             Debug.Log(data);
//         });

//         // When anyone joins the room output room Id
//         relay.On("joinedRoom", (data) =>
//         {
//             string str = data.ToString();
//             JoinedRoomResult joinResult = JsonUtility.FromJson<JoinedRoomResult>(str);
//             user.roomKey = joinResult.roomKey;

//             if(user.achor.RoomKey == -1 && joinResult.localizationData != null)
//             {
//                 user.achor = joinResult.localizationData;
//                 GroupPayload = joinResult.localizationData;
//                     user.trigger_get_anchor = true;
//                     // user.achor = anchorPayload;

//             }
//             if (user.isHost)
//             {
//                 //construct payload
//                 //user.roomKey;
                
//                 //geterate ip
//                 //generate dictionary

//                 //setLocalizationData();
//             }


//         });

//         // This is emmited from the channel of the room
//         relay.On("newMemberJoined", (data) =>
//         {
//             User newUser = JsonUtility.FromJson<User>(data.ToString());
//             if (user.uid == newUser.uid)
//             {
//                 Debug.Log("I Joined!");
//             }
//             else
//             {
//                 Debug.Log("New person joined!");
//                 Debug.Log(newUser.uid);
//             }
//         });


//         // This is emmited from the channel of the room
//         relay.On("memberDropped", (data) =>
//         {
//             User deadUser = JsonUtility.FromJson<User>(data.ToString());
//             Debug.Log("Someone left!");
//             Debug.Log(deadUser.uid);

//         });
        
//         relay.On("broadcastLocalizationData", (data) =>
//         {
//             Debug.Log("Data: " + data.ToString() + "User.UID = " + user.uid);
//             AnchorPayload anchorPayload = JsonUtility.FromJson<AnchorPayload>(data.ToString());
//             GroupPayload = anchorPayload;
//             if (user.achor.RoomKey == -1)
//             {
//                 user.trigger_get_anchor = true;
//             // user.achor = anchorPayload;
                
//             }
            

//         });


//         relay.On("broadcastData", (data) =>
//         {
//             BaseDataBroadcast payload = JsonUtility.FromJson<BaseDataBroadcast>(data.ToString());

//             switch (payload.type)
//             {
//                 case "something":
//                     break;
//             }
//         });


//     }


// }

// // potentially derivable UI abstraction.
// //  need to use for main thread UI updates.
// // NOTE: for UI updates, and an example on the socket client,
// // see: https://github.com/floatinghotpot/socket.io-unity/blob/master/Demo/SocketIOScript.cs
// public class GameUI
// {
// }

// public class GameDataRelay : MonoBehaviour
// {
//     // private readonly string httpEndpoint = "https://party-play.herokuapp.com/";
//     // private readonly string wsEndpoint = "ws://party-play.herokuapp.com/";
//     public Text dataEntry;
//     public Text txtRoom;
//     public GameSequence seq;
//     public InputField inputRoom;
    


//     void Awake()
//     {
//         User user = new User();
//         seq = new GameSequence(user, inputRoom);
//     }


//     void Start()
//     {
//         seq.setupListeners();
//     }

//     // ui update loop
//     void Update()
//     {

//     }
//     public AnchorPayload getAnchorPayload()
//     {
//         return seq.GroupPayload;
//     }

//     public void createRoom()
//     {
//         seq.hostCreatesARoom();
//     }

//     public User getUser()
//     {
//         return seq.user;
//     }

//     public void leaveRoom()
//     {
//         seq.drop();
//     }

//     public void joinRoom(string room)
//     {
//         seq.joinRoom(room);
//     }

//     public void endGame()
//     {
//         seq.stopGame();
//     }

//     public void requestLocalizationData()
//     {
//         seq.requestLocalizationData();
//     }

//     public void setLocalizationData()
//     {
//         seq.setLocalizationData();
//     }

// }
