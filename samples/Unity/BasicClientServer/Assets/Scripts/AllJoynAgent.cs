using UnityEngine;
using AllJoynUnity;

// The AllJoynAgent prefab must exist once, and only once, in your scene.
// This prefab/behavior will take care of the background loading and
// message processing required to use AllJoyn with Unity.
//
// In addition, the AllJoynAgent.cs script must execute before any other
// script which uses AllJoyn. Most Unity scripts use Start() for initialization,
// and the AllJoynAgent uses Awake(). In most cases this will ensure that
// the required code is initialized prior to any other AllJoyn code.
//
// If this is not the case in your project, go to the Unity editor menu at:
//   Edit->Project Settings->Script Execution Order
// and drag the AllJoynAgent.cs script into the execution order above 'Default Time',
// or enter the value '-100' for the execution time.
public class AllJoynAgent : MonoBehaviour
{
	// Awake() is called before any calls to Start() on any game object are made.
	void Awake()
	{
		// Output AllJoyn version information to log
		Debug.Log("AllJoyn Library version: " + AllJoyn.GetVersion());
		Debug.Log("AllJoyn Library buildInfo: " + AllJoyn.GetBuildInfo());

		// Enable callbacks on main thread only
		AllJoyn.SetMainThreadOnlyCallbacks(true);
	}

	// Update is called once per frame
	void Update()
	{
		// Pump messages from AllJoyn
		AllJoyn.TriggerCallbacks();
	}
}
