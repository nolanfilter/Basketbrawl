using UnityEngine;
using System.Collections;

public class DeathBoxAgent : MonoBehaviour {

	private static DeathBoxAgent mInstance = null;
	public static DeathBoxAgent instance
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
			Debug.LogError( string.Format( "Only one instance of DeathBoxAgent allowed! Destroying:" + gameObject.name + ", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}


}
