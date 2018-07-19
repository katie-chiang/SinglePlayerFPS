using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ParticleBulletSystem
{
public class ExamplePlayerScript : CaptainsMessPlayer
{
	public Image image;
	public Text nameField;
	public Text readyField;
	public Text rollResultField;
	public Text totalPointsField;

	[SyncVar]
	public Color myColour;

	// Simple game states for a dice-rolling game

	[SyncVar]
	public int rollResult;

	[SyncVar]
	public int totalPoints;

	[SyncVar]
	public bool locationSynced;

	public GameObject spherePrefab;

	private byte[] savedBytes;

	private bool locationSent;

	//syncvar means value changed on server will get automatically updated on clients
	[SyncVar]
	public int playerHealth = 5;

	public override void OnStartLocalPlayer()
	{
		base.OnStartLocalPlayer();

		if(isServer){
			//set authority of particle system to server

		}
		// Send custom player info
		// This is an example of sending additional information to the server that might be needed in the lobby (eg. colour, player image, personal settings, etc.)
		//start off health with 5 and decrement everytime player gets hit
		CmdInitHealth();
		myColour = UnityEngine.Random.ColorHSV(0,1,1,1,1,1);
		CmdSetCustomPlayerInfo(myColour);
	}

	[Command]
	void CmdInitHealth(){
		playerHealth = 5;
	}



	[Command]
	public void CmdSetCustomPlayerInfo(Color aColour)
	{
		myColour = aColour;
	}

	[Command]
	public void CmdRollDie()
	{
		rollResult = UnityEngine.Random.Range(1, 7);
	}


	[Command]
	public void CmdPlayAgain()
	{
		ExampleGameSession.instance.PlayAgain();
	}

	public override void OnClientEnterLobby()
	{
		base.OnClientEnterLobby();

		// Brief delay to let SyncVars propagate
		Invoke("ShowPlayer", 0.5f);
	}

	public override void OnClientReady(bool readyState)
	{
		if (readyState)
		{
			readyField.text = "READY!";
			readyField.color = Color.green;
		}
		else
		{
			readyField.text = "not ready";
			readyField.color = Color.red;
		}
	}

	void ShowPlayer()
	{
		//transform.SetParent(GameObject.Find("Canvas/PlayerContainer").transform, false);

		// image.color = myColour;	
		nameField.text = deviceName;
		readyField.gameObject.SetActive(true);

		// rollResultField.gameObject.SetActive(false);
		// totalPointsField.gameObject.SetActive(false);

		OnClientReady(IsReady());
	}

	int countTouch = 0;
	Vector3 prevPosition = Vector3.zero;
	Vector3 touchPoint = Vector3.zero;

	//keep a list of curves that the player drew
	//all the players have a different list because lists aren't networked yet
	public List<GameObject> curvesDrew;

	GameObject currentBezier = null;

	//detect touch of player here
	public void Update()
	{
		string synced = locationSynced ? "SYNC" : "NO";
		// totalPointsField.text = "Points: " + totalPoints.ToString() + synced;
		// if (rollResult > 0) {
		// 	rollResultField.text = "Roll: " + rollResult.ToString();
		// } else {
		// 	rollResultField.text = "";
		// }


		//check mouse click to fire missile
		ExampleGameSession gameSession = ExampleGameSession.instance;
		if(isLocalPlayer && gameSession && gameSession.gameState == GameState.WaitingForRolls){
			
			if(Input.GetMouseButtonDown(0)){

				Debug.Log("mouse clicked");							
				Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 point = raycast.GetPoint(0.1f);

				//fire missile across all clients
				CmdFireMissile(point);
				
			}else if(Input.GetMouseButtonDown(1)){
				//if the user right click, then finished this bezier
				//checking the networked movement of the transforms
				Debug.Log("move");
				transform.position = transform.position + Vector3.right;
			}
		}
	}

	[Command]
	public void CmdFireMissile(Vector3 position){
		
		RpcFireMissile(position);
				
	}


	[ClientRpc]
	public void RpcFireMissile(Vector3 position){
		
		ParticleManager.manager.Emit_OneShot(1, transform, 2, 1);

	}

	//triggered when a particle hit a gameobject
	//so need to take health off the gameobject that was hit
	//When OnParticleCollision is invoked from a script attached to a GameObject with a Collider, 
	//the GameObject parameter represents the ParticleSystem. 
	//When OnParticleCollision is invoked from a script attached to a ParticleSystem, 
	//the GameObject parameter represents a GameObject with an attached Collider struck by the ParticleSystem.
	public void OnParticleCollision(GameObject other)
	{	
		//tell server to decrement the health of the player
		//Debug.Log("HIT COLLISION ENTER!!!!!!!!!");
		Debug.Log("HIT COLLISION IN PLAYER-> the object hit is: " + other.name);

		if(other.name == "CubePlayer"){
			//decrement player health by one for the player hit
			other.GetComponent<ExamplePlayerScript>().playerHealth--;
		}

	}
	


	[Command]
	public void CmdMakeSphere(Vector3 position)
	{	
		GameObject obj = (GameObject)Instantiate(spherePrefab, position, Quaternion.identity);
		NetworkServer.SpawnWithClientAuthority(obj, connectionToClient);
		NetworkInstanceId id = obj.GetComponent<NetworkIdentity>().netId;
		Debug.Log("network id: " + id.ToString());
		Debug.Log("INSTANTIATED!!!!!!!!!!!!");
		//dont set colour for now
		//RpcSetSphereColor (sphere, myColour.r, myColour.g, myColour.b);
		RpcgetReferenceToObject(id);
		//RpcTesting(id);
		//RpcTestingWithAuthority(id);
	}

	[ClientRpc]
	void RpcTesting(NetworkInstanceId id){

		//this is always true??
		//always reassigning currentBezier?
		//The FindLocalObject() function is called on a client to transform a netId received 
		//from a server to a local game object
		//netID is the same across all machines
		if(ClientScene.FindLocalObject(id) != null){
			currentBezier = ClientScene.FindLocalObject(id);
			Debug.Log("ID in test: " + currentBezier.GetComponent<NetworkIdentity>().netId.ToString());
			Debug.Log("FOUND LOCAL OBJECT");

		}
	}

		[ClientRpc]
	void RpcTestingWithAuthority(NetworkInstanceId id){
			
		if(hasAuthority){
		currentBezier = ClientScene.FindLocalObject(id);
		Debug.Log("ID in test: " + currentBezier.GetComponent<NetworkIdentity>().netId.ToString());
		Debug.Log("HAS AUTHORITY");
		}
		
	}

	//command and client rpc parameters are limited to the same ones as SyncVar
	//primitives or unity defined types or structs
	[ClientRpc]
	void RpcgetReferenceToObject(NetworkInstanceId id){
		//only update currentBezier if it's local player
		Debug.Log("id: " + id.ToString() + " is local? " + isLocalPlayer.ToString());
		if(isLocalPlayer){
			currentBezier = ClientScene.FindLocalObject(id);
			Debug.Log("assigned, IS LOCAL");
		}
		//if not, then don't do anything to currentBezier
	}

	[ClientRpc]
	public void RpcOnStartedGame()
	{
		//readyField.gameObject.SetActive(false);

		//rollResultField.gameObject.SetActive(true);
		//totalPointsField.gameObject.SetActive(true);
	}

	//this local bool is for drawing one bezier curve for now
	bool canStartDraw = true;
	//the gui that is displayed on the user's screen
	void OnGUI()
	{
		if (isLocalPlayer)
		{
			GUILayout.BeginArea(new Rect(0, Screen.height * 0.8f, Screen.width, 100));
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();

			ExampleGameSession gameSession = ExampleGameSession.instance;
			if (gameSession)
			{
				if (gameSession.gameState == GameState.Lobby ||
					gameSession.gameState == GameState.Countdown)
				{
					if (GUILayout.Button(IsReady() ? "Not ready" : "Ready", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
					{
						if (IsReady()) {
							SendNotReadyToBeginMessage();
						} else {
							SendReadyToBeginMessage();
						}
					}
				}
				else if (gameSession.gameState == GameState.WaitingForRolls)
				{

					
				}
				else if (gameSession.gameState == GameState.GameOver)
				{
					if (isServer)
					{
						if (GUILayout.Button("Play Again", GUILayout.Width(Screen.width * 0.3f), GUILayout.Height(100)))
						{
							CmdPlayAgain();
						}
					}
				}
			}

			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
    	}
	}
}
}
