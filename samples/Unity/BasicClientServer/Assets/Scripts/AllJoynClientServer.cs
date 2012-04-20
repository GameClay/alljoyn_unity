using UnityEngine;
using AllJoynUnity;
using basic_clientserver;

public class AllJoynClientServer : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		Debug.Log("Starting up AllJoyn service and client");
		basicServer = new BasicServer();
		basicClient = new BasicClient();

		basicClient.Connect();
	}

	// Update is called once per frame
	void Update()
	{
		//System.GC.Collect();

		if(basicClient.Connected/* && !gotReply*/)
		{
			string reply = basicClient.CallRemoteMethod();
			//Debug.Log("BasicClient.CallRemoteMethod returned '" + reply + "'");
			gotReply = true;
		}
	}

	BasicServer basicServer;
	BasicClient basicClient;
	bool gotReply = false;
}
