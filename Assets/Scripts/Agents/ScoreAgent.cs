using UnityEngine;
using System;
using System.Collections;

public class ScoreAgent : MonoBehaviour {

	public GUIStyle textStyle;
	
	private int winScore = 3;
	private int latestWinner;
	
	private int[] playerScores = new int[ Enum.GetNames( typeof( InputController.PlayerNumber ) ).Length - 1 ];
	private Rect[] playerScoreRects = new Rect[ Enum.GetNames( typeof( InputController.PlayerNumber ) ).Length - 1 ];
	
	private static ScoreAgent mInstance = null;
	public static ScoreAgent instance
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
			Debug.LogError( string.Format( "Only one instance of ScoreAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}

		mInstance = this;
	}
	
	void Start()
	{
		ResetScores();
		
		playerScoreRects[ (int)InputController.PlayerNumber.Player1 ] = new Rect( Screen.width * 0.1f, Screen.height - 50f, 0f, 0f );
		playerScoreRects[ (int)InputController.PlayerNumber.Player2 ] = new Rect( Screen.width * 0.3f, Screen.height - 50f, 0f, 0f );
		playerScoreRects[ (int)InputController.PlayerNumber.Player3 ] = new Rect( Screen.width * 0.7f, Screen.height - 50f, 0f, 0f );
		playerScoreRects[ (int)InputController.PlayerNumber.Player4 ] = new Rect( Screen.width * 0.9f, Screen.height - 50f, 0f, 0f );
	}
	
	void OnGUI()
	{
		if( StateAgent.GetCurrentState() == StateAgent.State.PreGame )
			return;
	
		int scoresDisplayed = 0;
	
		for( int i = 0; i < 4; i++ )
		{	
			if( PlayerAgent.GetPlayerActive( i ) )
			{
				textStyle.normal.textColor = PlayerAgent.GetPlayerColor( i );
	
				GUI.Label( playerScoreRects[ scoresDisplayed ], "" + playerScores[i] + "/3", textStyle );
				
				scoresDisplayed++;
			}
		}
	}
	
	public static void ResetScores()
	{
		if( instance )
			instance.internalResetScores();
	}
	
	private void internalResetScores()
	{
		for( int i = 0; i < playerScores.Length; i++ )
			playerScores[i] = 0;
			
		latestWinner = -1;
	}
	
	public static void IncrementPlayerScore( InputController.PlayerNumber player )
	{
		if( instance )
			instance.internalIncrementPlayerScore( player );
	}
	
	private void internalIncrementPlayerScore( InputController.PlayerNumber player )
	{
		playerScores[ (int)player ]++;
		
		//check for win
		if( playerScores[ (int)player ] >= winScore )
		{
			latestWinner = (int)player;
			StateAgent.ChangeState( StateAgent.State.PostGame );
		}
	}
	
	public static int GetWinner()
	{
		if( instance )
			return instance.latestWinner;
			
		return -1;
	}
}
