using UnityEngine;
using System.Collections;

public class DebugAgent : MonoBehaviour {

	private static DebugAgent mInstance = null;
	public static DebugAgent instance
	{
		get
		{
			return mInstance;
		}
	}
	
	void Awake()
	{
		if( mInstance != null )
		{
			Debug.LogError( string.Format( "Only one instance of DebugAgent allowed! Destroying:" + gameObject.name + ", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}
	
	void Update()
	{
		if( Input.GetKey( KeyCode.B ) && Input.GetKey( KeyCode.F ) && Input.GetKey( KeyCode.O ) && Input.GetKey( KeyCode.D ) && StateAgent.GetCurrentState() == StateAgent.State.Playing )
			StateAgent.ChangeState( StateAgent.State.PostGame );
	}
}
