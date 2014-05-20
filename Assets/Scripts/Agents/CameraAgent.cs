using UnityEngine;
using System.Collections;

public class CameraAgent : MonoBehaviour {

	private float maxX;
	private float minX;
	private float maxY;
	private float minY;

	private static CameraAgent mInstance = null;
	public static CameraAgent instance
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
			Debug.LogError( string.Format( "Only one instance of CameraAgent allowed! Destroying:" + gameObject.name + ", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;
	}

	void Update()
	{
		transform.position = Vector3.MoveTowards( transform.position, PlayerAgent.GetMiddlePoint() - Vector3.forward * 10f + Vector3.up * 4f, 0.1f );

		if( transform.position.x > maxX )
			transform.position = new Vector3( maxX, transform.position.y, transform.position.z );

		if( transform.position.x < minX )
			transform.position = new Vector3( minX, transform.position.y, transform.position.z );

		if( transform.position.y > maxY )
			transform.position = new Vector3( transform.position.x, maxY, transform.position.z );
		
		if( transform.position.y < minY )
			transform.position = new Vector3( transform.position.x, minY, transform.position.z );
	}

	public static void SetBoundaries( float newMaxX, float newMinX, float newMaxY, float newMinY )
	{
		if( instance )
			instance.internalSetBoundaries( newMaxX, newMinX, newMaxY, newMinY );
	}

	private void internalSetBoundaries( float newMaxX, float newMinX, float newMaxY, float newMinY )
	{
		maxX = newMaxX;
		minX = newMinX;
		maxY = newMaxY;
		minY = newMinY;
	}
	
	public static void Enable()
	{
		if( instance )
			instance.enabled = true;
	}
	
	public static void Disable()
	{
		if( instance )
			instance.enabled = false;
	}
}
