using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAgent : MonoBehaviour {

	public GameObject playerTogglePrefab;
	public GameObject playerPrefab;

	public Color player1Color = Color.magenta;
	public Color player2Color = Color.cyan;
	public Color player3Color = Color.green;
	public Color player4Color = Color.yellow;

	private List<GameObject> ballObjects;
	private List<GameObject> tallObjects;

	private List<GameObject> toggles;
	private List<GameObject> players;
	
	private Vector3[] fourPlayerTogglePoints;
	private Vector3[] fourPlayerSpawnPoints;
	
	private int numPlayers;
	private bool[] activePlayers;
		
	private static PlayerAgent mInstance = null;
	public static PlayerAgent instance
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
			Debug.LogError( string.Format( "Only one instance of PlayerAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}

		mInstance = this;
		
		ballObjects = new List<GameObject>();
		tallObjects = new List<GameObject>();

		toggles = new List<GameObject>();
		players = new List<GameObject>();
		
		activePlayers = new bool[4];
	}

	void Start()
	{
		fourPlayerTogglePoints = new Vector3[] { new Vector3( -12.5f, -2.5f, -1f ), 
												 new Vector3( -6.5f, -2.5f, -1f ), 
												 new Vector3( 6.5f, -2.5f, -1f ),
												 new Vector3( 12.5f, -2.5f, -1f ) };
	
		fourPlayerSpawnPoints = new Vector3[] { new Vector3( -6f, 2f, 0f ), 
												new Vector3( 6f, 2f, 0f ), 
												new Vector3( -10f, 2f, 0f ),
												new Vector3( 10f, 2f, 0f ) };
												
		ResetPlayers();
	}

	public static void CreateToggles( bool createAll, bool setWinner = false, bool setActive = false )
	{
		if( instance )
			instance.internalCreateToggles( createAll, setWinner, setActive );
	}
	
	private void internalCreateToggles( bool createAll, bool setWinner, bool setActive )
	{
		GameObject temp;
		InputController inputController;
		
		int togglesCreated = 0;
		
		for( int i = 0; i < 4; i++ )
		{
			if( createAll || activePlayers[i] )
			{
				temp = Instantiate( playerTogglePrefab, fourPlayerTogglePoints[ togglesCreated ], Quaternion.identity ) as GameObject;
				
				togglesCreated++;
				
				inputController = temp.GetComponent<InputController>();
				
				if( inputController )
					inputController.currentPlayerNumber = (InputController.PlayerNumber)i;
				
				toggles.Add( temp );
				
				if( ( setActive && activePlayers[i] ) || ( setWinner && i == ScoreAgent.GetWinner() ) )
				{			
					ToggleController toggleController = temp.GetComponent<ToggleController>();
					
					if( toggleController )
					{
						toggleController.setIsCurrentlyTall( true, false );

						if( setWinner && i == ScoreAgent.GetWinner() )
							toggleController.enableCrown();
					}
				}
			}
		}
	}
	
	public static void DestroyAllToggles()
	{
		if( instance )
			instance.internalDestroyAllToggles();
	}
	
	private void internalDestroyAllToggles()
	{
		for( int i = 0; i < toggles.Count; i++ )
			Destroy( toggles[i] );
		
		toggles.Clear();
	}

	public static void CreatePlayers( bool createAll )
	{
		if( instance )
			instance.internalCreatePlayers( createAll );
	}

	private void internalCreatePlayers( bool createAll )
	{
		StartCoroutine( "DoCreatePlayers", createAll );
	}
	
	private IEnumerator DoCreatePlayers( bool createAll )
	{
		GameObject temp;
		InputController inputController;
		
		int playersCreated = 0;
		
		for( int i = 0; i < 4; i++ )
		{
			if( createAll || activePlayers[i] )
			{
				temp = Instantiate( playerPrefab, fourPlayerSpawnPoints[ playersCreated ], Quaternion.identity ) as GameObject;
				
				playersCreated++;
				
				inputController = temp.GetComponent<InputController>();
				
				if( inputController )
					inputController.currentPlayerNumber = (InputController.PlayerNumber)i;
							
				players.Add( temp );
				
				yield return new WaitForSeconds( StageAgent.countDownTime );
			}
		}
	}

	public static void DestroyAllPlayers()
	{
		if( instance )
			instance.internalDestroyAllPlayers();
	}
	
	private void internalDestroyAllPlayers()
	{
		for( int i = 0; i < players.Count; i++ )
			Destroy( players[i] );
			
		players.Clear();
	}

	public static void RegisterBall( GameObject go )
	{
		if( instance )
			instance.internalRegisterBall( go );
	}
	
	private void internalRegisterBall( GameObject go )
	{
		if( !ballObjects.Contains( go ) )
			ballObjects.Add( go );
	}
	
	public static void UnregisterBall( GameObject go )
	{
		if( instance )
			instance.internalUnregisterBall( go );
	}
	
	private void internalUnregisterBall( GameObject go )
	{		
		if( ballObjects.Contains( go ) )
			ballObjects.Remove( go );		
	}
	
	public static void RegisterTall( GameObject go )
	{
		if( instance )
			instance.internalRegisterTall( go );
	}
	
	private void internalRegisterTall( GameObject go )
	{
		if( !tallObjects.Contains( go ) )
			tallObjects.Add( go );
	}
	
	public static void UnregisterTall( GameObject go )
	{
		if( instance )
			instance.internalUnregisterTall( go );
	}
	
	private void internalUnregisterTall( GameObject go )
	{
		if( tallObjects.Contains( go ) )
			tallObjects.Remove( go );
	}
	
	public static void TurnOffCarryImages()
	{
		if( instance )
			instance.internalTurnOffCarryImages();
	}
	
	private void internalTurnOffCarryImages()
	{
		for( int i = 0; i < tallObjects.Count; i++ )
		{			
			PlayerController playerController = tallObjects[i].transform.parent.gameObject.GetComponent<PlayerController>();
						
			if( playerController != null )
				playerController.TurnOffCarryImage();
		}
	}

	public static Vector3 GetMiddlePoint()
	{
		if( instance )
			return instance.internalGetMiddlePoint();

		return Vector3.zero;
	}

	private Vector3 internalGetMiddlePoint()
	{
		Vector3 centroid = Vector3.zero;

		float numPoints = 0f;

		for( int i = 0; i < tallObjects.Count; i++ )
		{
			centroid += tallObjects[i].transform.position;
			numPoints += 1f;
		}

		for( int j = 0; j < ballObjects.Count; j++ )
		{
			centroid += ballObjects[j].transform.position;
			numPoints += 1f;
		}

		centroid += new Vector3( 0f, 4f, 0f );
		numPoints += 1f;

		if( numPoints != 0f )
			centroid /= numPoints;

		return centroid;
	}

	public static int GetNumBalls()
	{
		if( instance )
			return instance.internalGetNumBalls();

		return -1;
	}

	private int internalGetNumBalls()
	{
		return ballObjects.Count;
	}

	public static Vector3 GetSpawnPoint( int playerNumber )
	{
		if( instance )
			return instance.fourPlayerSpawnPoints[ playerNumber ];

		return Vector3.zero;
	}

	public static Color GetPlayerColor( int playerNumber )
	{
		if( instance )
			return instance.internalGetPlayerColor( playerNumber );

		return Color.grey;
	}

	private Color internalGetPlayerColor( int playerNumber )
	{
		switch( playerNumber ) 
		{
			case 0: return player1Color;
			case 1: return player2Color;
			case 2: return player3Color;
			case 3: return player4Color;
		}

		return Color.grey;
	}
	
	public static void SetPlayerActive( int playerNumber )
	{
		if( instance )
		{
			instance.activePlayers[ playerNumber ] = true;
			instance.numPlayers++;
		}
	}
	
	public static void SetPlayerInactive( int playerNumber )
	{
		if( instance )
		{
			instance.activePlayers[ playerNumber ] = false;
			instance.numPlayers--;
		}
	}
	
	public static int GetNumPlayers()
	{
		if( instance )
			return instance.numPlayers;
			
		return 0;
	}
	
	public static void ResetPlayers()
	{
		if( instance )
			instance.internalResetPlayers();
	}
	
	private void internalResetPlayers()
	{
		for( int i = 0; i < activePlayers.Length; i++ )
			activePlayers[i] = false;
	
		numPlayers = 0;
	}
	
	public static bool GetPlayerActive( int playerNumber )
	{
		if( instance )
			return instance.activePlayers[ playerNumber ];
			
		return false;
	}
}
