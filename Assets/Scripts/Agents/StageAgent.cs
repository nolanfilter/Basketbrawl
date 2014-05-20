using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageAgent : MonoBehaviour {


	public GameObject[] stagesPrefabs;
	private GameObject currentStage;

	public bool makeHoops;

	public int level;

	public GameObject hoopPrefab;
	private List<GameObject> hoops;

	public Vector3[] hoopPositions;
	
	public static float countDownTime = 0.8f;

	private Vector4 stageCoordinates = Vector4.zero;
	
	private static StageAgent mInstance = null;
	public static StageAgent instance
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
			Debug.LogError( string.Format( "Only one instance of StageAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}

		mInstance = this;
		
		if( stagesPrefabs.Length == 0 )
		{
			Debug.LogError( "No stages in " + gameObject.name );
			enabled = false;
			return;
		}
		
		hoops = new List<GameObject>();
	}
	
	public static void CreateLevel()
	{
		if( instance )
			instance.internalCreateLevel();
	}
	
	private void internalCreateLevel()
	{
		int length = stagesPrefabs.Length;

		if( level < length )
		{
			currentStage = Instantiate( stagesPrefabs[ level ], Vector3.zero, Quaternion.identity ) as GameObject;

			switch( level )
			{
				case 0: CameraAgent.SetBoundaries( 2.3f, -2.3f, 3.5f, -0.8f ); break;
				case 1: CameraAgent.SetBoundaries( 0.6f, -0.6f, 3.25f, 0.4f ); stageCoordinates = new Vector4( -40f, 20f, -20f, 40f ); break;
			}
		}
		else
		{
			int randomInt = Random.Range( 0, length );
			
			currentStage = Instantiate( stagesPrefabs[ randomInt ], Vector3.zero, Quaternion.identity ) as GameObject;
		}
		
		currentStage.transform.parent = transform;

		if( makeHoops && hoopPrefab != null )
		{
			GameObject tempHoop;

			for( int i = 0; i < hoopPositions.Length; i++ )
			{
				tempHoop = Instantiate( hoopPrefab, hoopPositions[i], Quaternion.identity ) as GameObject;

				tempHoop.transform.parent = transform;
				
				hoops.Add( tempHoop );
			}
		}
	}
	
	public static void DestroyLevel()
	{
		if( instance )
			instance.internalDestroyLevel();
	}
	
	private void internalDestroyLevel()
	{
		Destroy( currentStage );
		
		for( int i = 0; i < hoops.Count; i++ )
			Destroy( hoops[i] );
			
		hoops.Clear();
	}

	public static Vector4 GetStageCoordinates()
	{
		if( instance )
			return instance.stageCoordinates;

		return Vector4.zero;
	}
}
