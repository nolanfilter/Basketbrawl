using UnityEngine;
using System.Collections;

public class ToggleController : MonoBehaviour {
	
	public GameObject tallObject;
	public SpriteRenderer ballSprite;
	public SpriteRenderer crownSprite;

	public GUIStyle textStyle;

	private SpriteRenderer[] tallSprites;
	
	private InputController inputController;
	
	private bool isCurrentlyTall;

	private float beginTime;
	private float toggleBuffer = 0.5f;
	
	void Awake()
	{
		if( tallObject == null )
		{
			Debug.LogError( "No tall object on " + gameObject.name );
			enabled = false;
			return;
		}
		
		if( ballSprite == null )
		{
			Debug.LogError( "No ball sprite on " + gameObject.name );
			enabled = false;
			return;
		}

		if( crownSprite == null )
		{
			Debug.LogError( "No crown sprite on " + gameObject.name );
			enabled = false;
			return;
		}

		
		inputController = GetComponent<InputController>();
		
		if( inputController == null )
		{
			Debug.LogError( "No input controller on " + gameObject.name );
			enabled = false;
			return;
		}

		crownSprite.enabled = false;

		tallSprites = tallObject.GetComponentsInChildren<SpriteRenderer>();
		
		setIsCurrentlyTall( false, false );
	}
	
	void Start()
	{
		Color color = PlayerAgent.GetPlayerColor( (int)inputController.currentPlayerNumber );
				
		for( int i = 0; i < tallSprites.Length; i++ )
			tallSprites[i].color = color;
		
		ballSprite.color = color;
		textStyle.normal.textColor = color;
	}
	
	void OnEnable()
	{
		inputController.OnButtonDown += OnButtonDown;
	}
	
	void OnDisable()
	{
		inputController.OnButtonDown -= OnButtonDown;
	}

	void OnGUI()
	{
		if( StateAgent.GetCurrentState() == StateAgent.State.PreGame )
		{
			if( isCurrentlyTall )
			{
				Vector3 screenPosition = Camera.main.WorldToScreenPoint( transform.position + transform.up * 1.5f );
				GUI.Label( new Rect( screenPosition.x, Screen.height - screenPosition.y, 0f, 0f ), "READY", textStyle );
			}
			else
			{
				Vector3 screenPosition = Camera.main.WorldToScreenPoint( transform.position - transform.up * 2f );
				GUI.Label( new Rect( screenPosition.x, Screen.height - screenPosition.y, 0f, 0f ), "NOT READY", textStyle );
			}
		}
	}

	public void setIsCurrentlyTall( bool newIsCurrentlyTall, bool playSound = true )
	{			
		isCurrentlyTall = newIsCurrentlyTall;
		
		tallObject.SetActiveRecursively( isCurrentlyTall );
		ballSprite.enabled = !isCurrentlyTall;

		if( playSound )
		{
			if( isCurrentlyTall )
				SoundAgent.PlayClip( SoundAgent.SoundEffects.ToggleUp );
			else
				SoundAgent.PlayClip( SoundAgent.SoundEffects.ToggleDown );
		}
	}

	public void enableCrown()
	{	
		crownSprite.enabled = true;
		
		Animator animator = tallObject.GetComponent<Animator>();
		
		if( animator )
			animator.SetBool( "Winner", true );
	}
	
	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{	
		if( StateAgent.GetCurrentState() == StateAgent.State.PreGame )
		{
			if( StateAgent.GetChangePending() )
				return;

			if( button != InputController.ButtonType.Sel && button != InputController.ButtonType.Start )
			{
				if( !StateAgent.GetCanToggle() || Time.time - beginTime < toggleBuffer )
					return;

				beginTime = Time.time;

				setIsCurrentlyTall( !isCurrentlyTall );
				
				if( isCurrentlyTall )
					PlayerAgent.SetPlayerActive( (int)inputController.currentPlayerNumber );
				else
					PlayerAgent.SetPlayerInactive( (int)inputController.currentPlayerNumber );
			}
			else
			{
				if( PlayerAgent.GetNumPlayers() > 1 )
					StateAgent.ChangeState( StateAgent.State.CountDown );
			}
		}
		else if( StateAgent.GetCurrentState() == StateAgent.State.WinScreen )
			StateAgent.ChangeState( StateAgent.State.PreGame );
	}
}
