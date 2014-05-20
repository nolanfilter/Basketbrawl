using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StateAgent : MonoBehaviour {

	public GUIStyle textStyle;
	public GUIStyle subtitleStyle;
	
	public Renderer movieRenderer;
	public Renderer backgroundRenderer;
	public Renderer flashRenderer;

	public GameObject confettiPrefab;

	private List<GameObject> confettiObjects;
	
	private string displayString;
	private Rect textRect;

	private string subtitleString;
	private Rect subtitleRect;

	private string creditsString;
	private Rect creditsRect;

	private bool canToggleInPreGame;
	private bool canChangeFromWinScreen;
	private bool changePending;

	public enum State
	{
		Intro,
		PreGame,
		CountDown,
		Playing,
		PostGame,
		WinScreen,
		Invalid,
	}
	private State currentState = State.Invalid;

	private static StateAgent mInstance = null;
	public static StateAgent instance
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
			Debug.LogError( string.Format( "Only one instance of StateAgent allowed! Destroying:" + gameObject.name + ", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}
		
		mInstance = this;

		confettiObjects = new List<GameObject>();
	}
	
	void Start()
	{
		textRect = new Rect( 0f, 0f, Screen.width, Screen.height );
		subtitleRect = new Rect( 0f, 0f, Screen.width, Screen.height );
		creditsRect = new Rect( 0f, Screen.height * 0.9f, Screen.width, Screen.height * 0.1f );
	
		canChangeFromWinScreen = true;
		changePending = false;

		movieRenderer.enabled = false;
		backgroundRenderer.enabled = false;
		flashRenderer.enabled = false;
	
		ChangeState( State.Intro );
	}
	
	void OnGUI()
	{
		GUI.Label( textRect, displayString, textStyle );

		if( PlayerAgent.GetNumPlayers() > 1 )
			GUI.Label( subtitleRect, subtitleString, subtitleStyle );

		GUI.Label( creditsRect, creditsString, subtitleStyle );
	}

	public static void ChangeState( State newState )
	{
		if( instance )
			instance.internalChangeState (newState);
	}

	private void internalChangeState( State newState )
	{
		if( currentState == newState || !canChangeFromWinScreen )
			return;
			
		switch( newState )
		{
			case State.Intro:
			{
				SoundAgent.PauseBackgroundMusic();

				StartCoroutine( "DoIntro" );

			} break;

			case State.PreGame:
			{
				if( currentState == State.WinScreen )
				{
					SoundAgent.PlayClip( SoundAgent.SoundEffects.AdvanceScreen );

					StopCoroutine( "DoConfetti" );

					for( int i = 0; i < confettiObjects.Count; i++ )
						Destroy( confettiObjects[i] );

					confettiObjects.Clear();
					
					creditsString = "Vanessa Briceno    Nolan Filter    Bruce Lan";

					StartCoroutine( "DoPreGame" );
				}
				else
					canToggleInPreGame = true;

				backgroundRenderer.enabled = true;
				SoundAgent.PauseBackgroundMusic();

				CameraAgent.Disable();
				Camera.main.transform.position = Vector3.forward * -10f;
			
				PlayerAgent.DestroyAllToggles();
				PlayerAgent.CreateToggles( true, false, true );

				subtitleString = "PRESS START";
			
			} break;
			
			case State.CountDown:
			{
			ScoreAgent.ResetScores();

				subtitleString = "";
				creditsString = "";

				SoundAgent.PlayClip( SoundAgent.SoundEffects.AdvanceScreen );

				CameraAgent.Enable();			
			
				StageAgent.CreateLevel();
			
				PlayerAgent.DestroyAllToggles();
				PlayerAgent.CreatePlayers( false );
				
				StartCoroutine( "DoCountDown" );
				
			} break;
			
			case State.Playing:
			{
				backgroundRenderer.enabled = false;
				SoundAgent.PlayBackgroundMusic( true );
				
			} break;
			
			case State.PostGame:
			{
				SoundAgent.PauseBackgroundMusic();
				SoundAgent.PlayClip( SoundAgent.SoundEffects.Fanfare );

				StartCoroutine( "DoPostGame" );
				
			} break;
			
			case State.WinScreen:
			{
				CameraAgent.Disable();
				Camera.main.transform.position = Vector3.forward * -10f;
			
			
				PlayerAgent.DestroyAllPlayers();
				PlayerAgent.CreateToggles( false, true );
				
				StageAgent.DestroyLevel();
				
				StartCoroutine( "DoWinScreen" );
				StartCoroutine( "DoConfetti" );
				
			} break;
		}

		currentState = newState;

		StartCoroutine( "DoChangePending" );
	}
	
	public static State GetCurrentState()
	{
		if( instance )
			return instance.currentState;
			
		return State.Invalid;
	}

	public static bool GetChangePending()
	{
		if( instance )
			return instance.changePending;

		return true;
	}

	public static bool GetCanToggle()
	{
		if( instance )
			return instance.canToggleInPreGame;

		return true;
	}
	
	private IEnumerator DoCountDown()
	{
		yield return new WaitForSeconds( StageAgent.countDownTime * 0.5f );
		
		displayString = "3";
		SoundAgent.PlayClip( SoundAgent.SoundEffects.Countdown3 );
		
		yield return new WaitForSeconds( StageAgent.countDownTime * 1f );
		
		displayString = "2";
		SoundAgent.PlayClip( SoundAgent.SoundEffects.Countdown2 );
		
		yield return new WaitForSeconds( StageAgent.countDownTime * 1f );
		
		displayString = "1";
		SoundAgent.PlayClip( SoundAgent.SoundEffects.Countdown1 );
		
		yield return new WaitForSeconds( StageAgent.countDownTime * 1f );
		
		displayString = "GO!";
		SoundAgent.PlayClip( SoundAgent.SoundEffects.CountdownGo );
		
		yield return new WaitForSeconds( StageAgent.countDownTime * 1f );
		
		displayString = "";
		
		ChangeState( State.Playing );
	}

	private IEnumerator DoIntro()
	{
		yield return new WaitForSeconds( 1f );

		SoundAgent.PlayClip( SoundAgent.SoundEffects.Logo );

		movieRenderer.enabled = true;
		MovieTexture movTexture = movieRenderer.material.mainTexture as MovieTexture;
		
		if( movTexture )
			movTexture.Play();

		yield return new WaitForSeconds( 0.75f );
		
		SoundAgent.PlayClip( SoundAgent.SoundEffects.Impact );
		
		yield return new WaitForSeconds( 0.15f );

		flashRenderer.material.color = Color.white;
		flashRenderer.enabled = true;
		backgroundRenderer.enabled = true;
		movieRenderer.enabled = false;
		
		if( movTexture )
			movTexture.Stop();

		float beginTime = Time.time;
		float duration = 0.5f;

		float currentTime;
		float lerp;

		Color endColor = new Color( 1f, 1f, 1f, 0f );

		do
		{
			currentTime = Time.time - beginTime;
			lerp = currentTime / duration;

			flashRenderer.material.color = Color.Lerp( Color.white, endColor, lerp );

			yield return null;

		} while( currentTime < duration );

		flashRenderer.renderer.enabled = false;

		ChangeState( State.PreGame );
	}

	private IEnumerator DoPreGame()
	{
		canToggleInPreGame = false;

		yield return new WaitForSeconds( 1f );

		canToggleInPreGame = true;
	}

	private IEnumerator DoPostGame()
	{
		displayString = "GAME OVER!";
	
		yield return new WaitForSeconds( 5f );
		
		displayString = "";
		
		ChangeState( State.WinScreen );
	}
	
	private IEnumerator DoWinScreen()
	{
		canChangeFromWinScreen = false;
		
		yield return new WaitForSeconds( 3f );
		
		canChangeFromWinScreen = true;		

		subtitleString = "PLAY AGAIN?";
	}

	private IEnumerator DoConfetti()
	{
		GameObject temp;

		while( currentState == State.PostGame || currentState == State.WinScreen )
		{
			temp = Instantiate( confettiPrefab, Random.insideUnitSphere * 7.5f, Quaternion.identity ) as GameObject;

			confettiObjects.Add( temp );

			yield return new WaitForSeconds( 0.75f );
		}
	}

	private IEnumerator DoChangePending()
	{
		changePending = true;

		yield return null;

		changePending = false;
	}
}
