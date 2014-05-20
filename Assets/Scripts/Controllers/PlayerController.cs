using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public GameObject tallSpriteObject;
	public GameObject ballSpriteObject;
	public Animator spriteAnimator;

	public GameObject tallObject;
	
	public GameObject punchObject;
	private Collider punchCollider;
	
	public GameObject ballObject;
	private Collider ballCollider;
	private Rigidbody ballRigidbody;
	
	public SpriteRenderer carryImageSprite;

	public SpriteRenderer aimSprite;

	public Collider pickUpCollider;
			
	public enum State
	{
		Tall,
		Ball,
		Invalid,
	}
	public State currentState;
	
	private CharacterController controller;
	private InputController inputController;
	
	private float gravity = -9.8f;
	private Vector3 gravityVector;

	private Vector3 movementVector;

	private Vector3 lastDirectionVector;

	private float speed = 10f;

	private float jumpForce = 30f;
	private float jumpDuration = 0.5f;

	private bool useGravity;
	private bool isFacingRight;
	private bool isPunching;
	private bool isHoldingJump;
	private bool isAiming;
	private bool isThrowing;

	private float aimDuration = 1f;
	
	private float punchWindUpDuration = 0.1f;
	private float punchDuration = 0.25f;
	private float punchForce = 10f;
	private float punchWindDownDuration = 0.1f;
	private float punchLength = 1.5f;

	private float throwForce = 1000f;
	private float throwWindDownDuration = 0.5f;
	
	private float ballDuration = 7.5f;
	private float ballForceMagnitude = 50f;
	private int buttonMashCounter;
	private int maxButtonMash = 50;
	
	private float maxRigidbodyVelocity = 25f;

	private float angularVelocity = 0f;
	private float angularForce = 5f;

	private float horizontalVelocity;
	private float horizontalForce = 2f;
	
	private float invulnerabilityDuration = 2f;
	private float invulnerabilityBeginTime;
		
	void Awake()
	{
		if( tallObject == null )
		{
			Debug.LogError( "No tall object on " + gameObject.name );
			enabled = false;
			return;
		}
		
		controller = tallObject.GetComponent<CharacterController>();
		
		if( controller == null )
		{
			Debug.LogError( "No character controller on " + tallObject.name );
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
		
		if( punchObject == null )
		{
			Debug.LogError( "No punch object on " + gameObject.name );
			enabled = false;
			return;
		}
		
		punchCollider = punchObject.GetComponent<Collider>();
		
		if( punchCollider == null )
		{
			Debug.LogError( "No punch collider on " + punchObject.name );
			enabled = false;
			return;
		}
		
		if( ballObject == null )
		{
			Debug.LogError( "No ball object on " + gameObject.name );
			enabled = false;
			return;
		}
		
		ballCollider = ballObject.GetComponent<Collider>();
		
		if( ballCollider == null )
		{
			Debug.LogError( "No ball collider on " + ballObject.name );
			enabled = false;
			return;
		}
		
		ballRigidbody = ballObject.GetComponent<Rigidbody>();
		
		if( ballRigidbody == null )
		{
			Debug.LogError( "No ball rigidbody on " + ballObject.name );
			enabled = false;
			return;
		}
		
		if( carryImageSprite == null )
		{
			Debug.LogError( "No carry image sprite on " + gameObject.name );
			enabled = false;
			return;
		}

		if( aimSprite == null )
		{
			Debug.LogError( "No aim sprite on " + tallObject.name );
			enabled = false;
			return;
		}

		if( pickUpCollider == null )
		{
			Debug.LogError( "No pick up collider on " + tallObject.name );
			enabled = false;
			return;
		}

		if( tallSpriteObject == null )
		{
			Debug.LogError( "No sprite object on " + tallObject.name );
			enabled = false;
			return;
		}

		if( ballSpriteObject == null )
		{
			Debug.LogError( "No sprite object on " + ballObject.name );
			enabled = false;
			return;
		}

		if( spriteAnimator == null )
		{
			Debug.LogError( "No sprite animator on " + tallObject.name );
			enabled = false;
			return;
		}
	}
	
	void Start()
	{	
		currentState = State.Tall;
		PlayerAgent.RegisterTall( tallObject );
		
		gravityVector = transform.up * gravity;
		useGravity = true;
		
		isFacingRight = transform.position.x < 0f;

		if( !isFacingRight )
			tallObject.transform.localScale = new Vector3( -1f, 1f, 1f );

		isPunching = false;
		isHoldingJump = false;
		isAiming = false;
		isThrowing = false;

		Color playerColor = PlayerAgent.GetPlayerColor( (int)inputController.currentPlayerNumber );

		playerColor.a = 1f;
		
		tallObject.renderer.material.color = playerColor;
		punchCollider.renderer.material.color = playerColor;
		ballObject.renderer.material.color = playerColor;
		aimSprite.color = playerColor;
		
		SpriteRenderer[] spriteRenderers = tallSpriteObject.GetComponentsInChildren<SpriteRenderer>();
		
		for( int i = 0; i < spriteRenderers.Length; i++ )
			spriteRenderers[i].color = playerColor;
		
		spriteRenderers = ballSpriteObject.GetComponentsInChildren<SpriteRenderer>();
		
		for( int i = 0; i < spriteRenderers.Length; i++ )
			spriteRenderers[i].color = playerColor;

		punchCollider.enabled = false;
		
		ballObject.SetActive( false );
		
		carryImageSprite.enabled = false;

		aimSprite.enabled = false;
	}
	
	void OnEnable()
	{
		inputController.OnButtonDown += OnButtonDown;
		inputController.OnButtonHeld += OnButtonHeld;
		inputController.OnButtonUp += OnButtonUp;
	}
	
	void OnDisable()
	{
		inputController.OnButtonDown -= OnButtonDown;
		inputController.OnButtonHeld -= OnButtonHeld;
		inputController.OnButtonUp -= OnButtonUp;
	}
	
	void OnDestroy()
	{
		PlayerAgent.UnregisterTall( tallObject );
		PlayerAgent.UnregisterBall( ballObject );
	}
	
	void Update()
	{	
		applyGravity();
		
		if( tallObject.activeInHierarchy )
			controller.Move( movementVector );
		
		if( isAiming )
		{		
			if( lastDirectionVector.x > 0f )
				isFacingRight = true;
			else if( lastDirectionVector.x < 0f )
				isFacingRight = false;
		}
		else
		{
			if( movementVector.x > 0f )
				isFacingRight = true;
			else if( movementVector.x < 0f )
				isFacingRight = false;
		}

		tallObject.transform.localScale = new Vector3( ( isFacingRight ? 1f : -1f ), 1f, 1f );

		//animator
		if( currentState == State.Tall && controller.isGrounded && !isPunching )
			spriteAnimator.SetFloat( "Speed", Mathf.Abs( movementVector.x ) );
		
		movementVector = Vector3.zero;

		//lastDirectionVector = Vector3.zero;		

		isHoldingJump = false;

		if( tallObject.transform.position.z != 0f )
			tallObject.transform.position = new Vector3( tallObject.transform.position.x, tallObject.transform.position.y, 0f );

		pickUpCollider.enabled = ( currentState == State.Tall && !isPunching && !isThrowing && !isCarryingBall() && PlayerAgent.GetNumBalls() > 0 );

		if( currentState == State.Ball )
		{
			if( ballObject.transform.localScale != Vector3.one )
				ballObject.transform.localScale = Vector3.one;

			ballObject.transform.rotation *= Quaternion.AngleAxis( angularVelocity, Vector3.forward );
		}

		angularVelocity *= 0.5f;
		horizontalVelocity *= 0.5f;

		//programmatic death box
		Transform currentTransform = null;

		if( currentState == State.Tall )
			currentTransform = tallObject.transform;
		else if( currentState == State.Ball )
			currentTransform = ballObject.transform;

		if( currentTransform )
		{
			Vector4 deathBoxCoordinates = StageAgent.GetStageCoordinates();

			if( currentTransform.position.x < deathBoxCoordinates.x || currentTransform.position.y > deathBoxCoordinates.y || currentTransform.position.y < deathBoxCoordinates.z || currentTransform.position.x > deathBoxCoordinates.w )
				currentTransform.position = PlayerAgent.GetSpawnPoint( (int)inputController.currentPlayerNumber );
		}
	}
	
	private void applyGravity()
	{		
		if( useGravity && !controller.isGrounded )
			movementVector += gravityVector * Time.deltaTime;
	}
	
	private IEnumerator DoJump()
	{		
		spriteAnimator.SetBool( "Jumping", true );

		SoundAgent.PlayClip( SoundAgent.SoundEffects.Jump );

		float beginTime = Time.time;
		float currentTime;
		float lerp;

		//ascent
		do
		{
			currentTime = Time.time - beginTime;
			lerp = currentTime / jumpDuration;
			lerp = 1f - Mathf.Pow( lerp, 2f );
			
			Vector3 newMovement = Vector3.up * jumpForce * lerp * Time.deltaTime * ( isHoldingJump ? 1.5f : 1f );

			if( !isPunching && !isAiming )
				movementVector += newMovement;

			RaycastHit hitInfo;
				
			if( Physics.Raycast( tallObject.transform.position + Vector3.up * controller.height * 0.5f + Vector3.left * controller.radius * 0.5f, Vector3.up, out hitInfo, 1f ) )
				PushOtherPlayer( hitInfo, newMovement );
			else if( Physics.Raycast( tallObject.transform.position + Vector3.up * controller.height * 0.5f + Vector3.right * controller.radius * 0.5f, Vector3.up, out hitInfo, 1f ) )
				PushOtherPlayer( hitInfo, newMovement );
			
			yield return null;
			
		} while( currentTime < jumpDuration );	

		//descent
		do
		{
			if( !isPunching && !isAiming )
			{
				applyGravity();
				//applyGravity();
			}
			
			yield return null;
			
		} while( !controller.isGrounded && tallObject.activeInHierarchy );	

		spriteAnimator.SetBool( "Jumping", false );
	}

	private IEnumerator DoAim()
	{
		aimSprite.enabled = true;

		float beginTime = Time.time;


		while( isAiming ) 
		{
			if( ( Time.time - beginTime < aimDuration ) && !controller.isGrounded )
				movementVector += gravityVector * -0.75f * Time.deltaTime;
			
			lastDirectionVector = inputController.GetRawAxes().normalized;
			
			aimSprite.enabled = lastDirectionVector != Vector3.zero;
			
			aimSprite.transform.position = tallObject.transform.position + lastDirectionVector * 2.5f + Vector3.forward * -2f;
			
			aimSprite.transform.LookAt( tallObject.transform.position, Vector3.forward );
			
			yield return null;
		}

		aimSprite.enabled = false;
	}
	
	private void PushOtherPlayer( RaycastHit hitInfo, Vector3 newMovement )
	{
		PlayerController playerController = null;
				
		if( hitInfo.collider.gameObject.transform.parent != null )
			playerController = hitInfo.collider.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
				
		if( playerController )
			playerController.Push( newMovement * 0.25f );
	}
	
	private IEnumerator DoPunch()
	{	
		isPunching = true;

		spriteAnimator.SetBool( "Punching", true );
	
		if( movementVector != Vector3.zero )
			StartCoroutine( MovementOverTime( Vector3.right * ( isFacingRight ? 1f : -1f ), punchForce, punchDuration * 2.5f ) );

		yield return new WaitForSeconds( punchWindUpDuration );

		SoundAgent.PlayClip( SoundAgent.SoundEffects.Hit );

		punchObject.transform.localPosition = new Vector3( 0.75f * ( isFacingRight ? 1f : -1f ), 0f, 0f );
		punchCollider.enabled = true;
		
		float beginTime = Time.time;
		float currentTime;	
		float adjustedDuration = punchDuration - punchWindUpDuration;
		
		do
		{
			currentTime = Time.time - beginTime;
				
			RaycastHit hitInfo;
					
			Vector3 directionVector = Vector3.right * ( isFacingRight ? 1f : -1f );			
			
			if( Physics.Raycast( tallObject.transform.position + Vector3.up * controller.height * 0.25f + directionVector * controller.radius * 0.5f, directionVector, out hitInfo, punchLength ) )
				PunchOtherPlayer( hitInfo, directionVector );
			else if( Physics.Raycast( tallObject.transform.position + Vector3.down * controller.height * 0.25f + directionVector * controller.radius * 0.5f, directionVector, out hitInfo, punchLength ) )
				PunchOtherPlayer( hitInfo, directionVector );		

			if( !controller.isGrounded )
				movementVector += gravityVector * -0.75f * Time.deltaTime;

			yield return null;
			
		} while( currentTime < adjustedDuration );
				
		punchCollider.enabled = false;
		punchObject.transform.localPosition = Vector3.zero;
		
		spriteAnimator.SetBool( "Punching", false );

		yield return new WaitForSeconds( punchWindDownDuration );

		isPunching = false;
	}
	
	private void PunchOtherPlayer( RaycastHit hitInfo, Vector3 directionVector )
	{
		PlayerController playerController = null;
				
		if( hitInfo.collider.gameObject.transform.parent != null )
			playerController = hitInfo.collider.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();

		if( playerController && playerController.currentState == State.Tall )
		{
			playerController.ToBall( directionVector );

			ImprintController imprintController = playerController.ballObject.GetComponent<ImprintController>();
			
			if( imprintController )
			{
				imprintController.SetLastImprint( inputController.currentPlayerNumber );
			}
		}
	}
	
	private IEnumerator MovementOverTime( Vector3 directionVector, float force, float duration )	
	{
		float beginTime = Time.time;
		float currentTime;
		float lerp;
		
		do
		{
			currentTime = Time.time - beginTime;
			lerp = 1f - ( currentTime / duration );
			
			Vector3 newMovement = directionVector * force * lerp * Time.deltaTime;
			
			movementVector += newMovement;
			
			yield return null;
			
		} while( currentTime < duration );	
	}
	
	private IEnumerator ReturnToTall()
	{
		float beginTime = Time.time;
		
		do
		{
			if( ballRigidbody.velocity.magnitude > maxRigidbodyVelocity )
				ballRigidbody.velocity = ballRigidbody.velocity.normalized * maxRigidbodyVelocity;
						
			yield return null;
			
		} while( ( Time.time - beginTime < ballDuration ) && ( buttonMashCounter < maxButtonMash ) );
				
		ToTall();
	}

	private IEnumerator DoThrowWindDown()
	{
		isThrowing = true;

		yield return new WaitForSeconds( throwWindDownDuration );

		isThrowing = false;
	}

	private IEnumerator CheckForCarryBall()
	{
		while( true )
		{
			if( carryImageSprite.enabled && !isCarryingBall() )
				TurnOffCarryImage();

			yield return null;
		}
	}
	
	private void ThrowBall( Transform opponentball )
	{
		StopCoroutine( "CheckForCarryBall" );

		carryImageSprite.enabled = false;
		
		opponentball.parent = null;
		
		if( lastDirectionVector == Vector3.zero )
			lastDirectionVector = Vector3.right * ( isFacingRight ? 1f : -1f );
		
		Vector3 newPosition = Vector3.right * lastDirectionVector.x * 1f + Vector3.up * lastDirectionVector.y * 1.5f;
		
		RaycastHit hitInfo;
		
		if( Physics.Raycast( tallObject.transform.position + Vector3.down * controller.height * 0.5f, Vector3.down, out hitInfo, 1f ) )
		{			
			newPosition = new Vector3( newPosition.x, Mathf.Abs( newPosition.y ), 0f );
			lastDirectionVector = new Vector3( lastDirectionVector.x, Mathf.Abs( lastDirectionVector.y ), 0f );
		}
				
		if( Physics.Raycast( tallObject.transform.position + Vector3.right * controller.radius * 0.5f, Vector3.right, out hitInfo, 1f ) )
		{
			if( hitInfo.collider.tag == "Stage" || hitInfo.collider.tag == "RotatingCube" )
			{
				newPosition = new Vector3( Mathf.Abs( newPosition.x ) * -1f, newPosition.y, 0f );
				lastDirectionVector = new Vector3( Mathf.Abs( lastDirectionVector.x ) * -1f, lastDirectionVector.y, 0f );
			}
		}
		
		if( Physics.Raycast( tallObject.transform.position + Vector3.left * controller.radius * 0.5f, Vector3.left, out hitInfo, 1f ) )
		{
			if( hitInfo.collider.tag == "Stage" || hitInfo.collider.tag == "RotatingCube" )
			{
				newPosition = new Vector3( Mathf.Abs( newPosition.x ), newPosition.y, 0f );
				lastDirectionVector = new Vector3( Mathf.Abs( lastDirectionVector.x ), lastDirectionVector.y, 0f );
			}
		}

		StartCoroutine( "DoThrowWindDown" );

		opponentball.position = tallObject.transform.position + newPosition;
		opponentball.gameObject.SetActive( true );
		
		//Debug.Log( lastDirectionVector );
		//Debug.Log( newPosition );
		
		opponentball.rigidbody.velocity = Vector3.zero;
		opponentball.rigidbody.AddForce( lastDirectionVector * throwForce );
	}

	private IEnumerator DoBallOffset()
	{
		Vector2 randomVector = Random.insideUnitCircle * 0.25f;
		Vector3 beginPosition = new Vector3( randomVector.x, randomVector.y, 0f );

		ballSpriteObject.transform.localPosition = beginPosition;

		float beginTime = Time.time;
		float duration = 0.25f;

		float lerp;
		float currentTime;

		do
		{
			currentTime = Time.time - beginTime;
			lerp = currentTime / duration;

			ballSpriteObject.transform.localPosition = Vector3.Lerp( beginPosition, Vector3.zero, lerp );

			yield return null;

		} while ( currentTime < duration );
	}
	
	private bool isDirectionalButton( InputController.ButtonType button )
	{
		return ( button == InputController.ButtonType.Up || button == InputController.ButtonType.Left || button == InputController.ButtonType.Right || button == InputController.ButtonType.Down );
	}
	
	private void internalToTall()
	{
		if( currentState == State.Tall )
			return;
				
		StopCoroutine( "ReturnToTall" );

		Vector3 velocity = ballRigidbody.velocity;

		ballObject.transform.parent = transform;
		ballObject.SetActive( false );
		PlayerAgent.TurnOffCarryImages();
		
		currentState = State.Tall;
		
		PlayerAgent.RegisterTall( tallObject );

		tallObject.transform.position = ballObject.transform.position + Vector3.up * 0.55f;

		invulnerabilityBeginTime = Time.time;

		isPunching = false;
		useGravity = true;
		tallObject.SetActive( true );

		StartCoroutine( MovementOverTime( velocity.normalized, velocity.magnitude, 0.75f ) );
	}

	private void internalToBall( Vector3 direction )
	{


		if( currentState == State.Ball )
		{
			Push( direction );
			return;
		}
		
		if( Time.time - invulnerabilityBeginTime < invulnerabilityDuration )
		{
			return;
		}

		SoundAgent.PlayClip( SoundAgent.SoundEffects.GotHit );

		horizontalVelocity = movementVector.x;

		StopCoroutine( "DoPunch" );
		punchCollider.enabled = false;
		punchObject.transform.localPosition = Vector3.zero;

		Transform opponentball = tallObject.transform.FindChild( "Ball" );

		if( opponentball )
			ThrowBall( opponentball );

		PlayerAgent.UnregisterTall( tallObject );
		
		currentState = State.Ball;
		
		tallObject.SetActive( false );
		ballObject.transform.position = tallObject.transform.position;
		ballObject.SetActive( true );
		ballRigidbody.AddForce( direction * ballForceMagnitude );
		
		buttonMashCounter = 0;

		SoundAgent.PlayClip( SoundAgent.SoundEffects.ToBall );

		StartCoroutine( "ReturnToTall" );
	}

	private bool isCarryingBall()
	{
		Transform opponentball = tallObject.transform.FindChild( "Ball" );
		
		return ( opponentball != null );
	}

	//event handlers
	private void OnButtonDown( InputController.ButtonType button )
	{	
		if( currentState == State.Ball )
		{
			buttonMashCounter++;

			StopCoroutine( "DoBallOffset" );
			StartCoroutine( "DoBallOffset" );
		}

		switch( button )
		{
			case InputController.ButtonType.Left:
			{	
				if( !isAiming )
					movementVector += Vector3.left * speed * Time.deltaTime;

				angularVelocity += angularForce;
				horizontalVelocity -= horizontalForce;
			} break;
			
			case InputController.ButtonType.Right: 
			{
				if( !isAiming )
					movementVector += Vector3.right * speed * Time.deltaTime;	

				angularVelocity -= angularForce;
				horizontalVelocity += horizontalForce;
			} break;
			
			case InputController.ButtonType.Jump: 
			{
				if( currentState == State.Tall && controller.isGrounded && !isAiming && !isPunching )
				{
					StartCoroutine( "DoJump" );
				}
			} break;
			
			case InputController.ButtonType.Action:
			{
				switch( currentState )
				{
					case State.Tall:
					{
						Transform opponentball = tallObject.transform.FindChild( "Ball" );
					
						if( opponentball )
						{				
							isAiming = true;
							StartCoroutine( "DoAim" );
						}
						else
						{
							if( !isPunching && currentState == State.Tall )
								StartCoroutine( "DoPunch" );
						}
					} break;

					case State.Ball:
					{
						//if( ballRigidbody.velocity.magnitude < 1f )
						//	ToTall();
					} break;

				}
				
			} break;
		}
	}
	
	private void OnButtonHeld( InputController.ButtonType button )
	{	
		switch( button )
		{
			case InputController.ButtonType.Left:
			{	
				if( !isAiming )
					movementVector += Vector3.left * speed * Time.deltaTime;	

				angularVelocity += angularForce;
				horizontalVelocity -= horizontalForce;

			} break;
			
			case InputController.ButtonType.Right: 
			{
				if( !isAiming )
					movementVector += Vector3.right * speed * Time.deltaTime;		

				angularVelocity -= angularForce;
				horizontalVelocity += horizontalForce;
			} break;
			
			case InputController.ButtonType.Jump: 
			{
				if( currentState == State.Tall && controller.isGrounded && !isAiming && !isPunching )
				{	
					StartCoroutine( "DoJump" );
				}
				else
				{
					isHoldingJump = true;
				}
			} break;
		}
	}
	
	private void OnButtonUp( InputController.ButtonType button )
	{
		switch( button )
		{
			case InputController.ButtonType.Action:
			{
				Transform opponentball = tallObject.transform.FindChild( "Ball" );
				
				if( opponentball && isAiming )
				{				
					ThrowBall( opponentball );
				}

				isAiming = false;
				
			} break;
		}
	}
	//end event handlers
	
	//public functions
	public void Push( Vector3 direction )
	{		
		if( currentState == State.Tall )
			movementVector += direction;
		else if( currentState == State.Ball )
			ballRigidbody.AddForce( direction * ballForceMagnitude );
	}
	
	public void ToBall( Vector3 direction )
	{		
		internalToBall( direction );
	}
	
	public void CarryBall( GameObject ball )
	{		
		if( isCarryingBall() )
			return;

		carryImageSprite.color = ball.renderer.material.color;

		ball.rigidbody.velocity = Vector3.zero;
		ball.SetActive( false );

		ball.transform.parent = tallObject.transform;
		ball.transform.localPosition = Vector3.up * 1.5f;
		
		carryImageSprite.enabled = true;
		
		ImprintController imprintController = ball.GetComponent<ImprintController>();
		
		if( imprintController )
		{
			imprintController.SetLastImprint( inputController.currentPlayerNumber );
		}

		SoundAgent.PlayClip( SoundAgent.SoundEffects.CarryBall );

		StartCoroutine( "CheckForCarryBall" );
	}
	
	public void TurnOffCarryImage()
	{		
		if( tallObject.transform.FindChild( "Ball" ) == null )
		{
			carryImageSprite.enabled = false;
			isAiming = false;
		}
		//else
		//	Debug.Log( "did not turn off carry image" );
	}

	public void ToTall()
	{
		internalToTall();
	}

	public float GetHorizontalVelocity()
	{
		if( currentState == State.Tall )
			return horizontalVelocity;

		return 0f;
	}
}
