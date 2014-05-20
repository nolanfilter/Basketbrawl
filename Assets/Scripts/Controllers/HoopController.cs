using UnityEngine;
using System.Collections;

public class HoopController : MonoBehaviour {

	public GameObject confettiPrefab;

	public GameObject bottomRung;
	public GameObject topRung;
	
	public HoopColliderController leftCollider;
	public HoopColliderController rightCollider;

	private BoxCollider leftBoxCollider;
	private BoxCollider rightBoxCollider;

	private Animator animator;

	private float lastHitTime;
	private InputController.PlayerNumber lastImprintLeft = InputController.PlayerNumber.Invalid;
	private InputController.PlayerNumber lastImprintRight = InputController.PlayerNumber.Invalid;
	private float shotClockDuration = 1f;

	private float rechargeDuration = 5f;
	
	void Start()
	{
		if( bottomRung == null )
		{
			Debug.LogError( "No bottom rung object on " + gameObject.name );
			enabled = false;
			return;
		}
		
		if( topRung == null )
		{
			Debug.LogError( "No top rung object on " + gameObject.name );
			enabled = false;
			return;
		}
		
		if( leftCollider == null )
		{
			Debug.LogError( "No left collider object on " + gameObject.name );
			enabled = false;
			return;
		}

		leftBoxCollider = leftCollider.gameObject.GetComponent<BoxCollider>();

		if( leftBoxCollider == null )
		{
			Debug.LogError( "No box collider object on " + leftCollider.gameObject.name );
			enabled = false;
			return;
		}
		
		if( rightCollider == null )
		{
			Debug.LogError( "No right collider object on " + gameObject.name );
			enabled = false;
			return;
		}

		rightBoxCollider = rightCollider.gameObject.GetComponent<BoxCollider>();
		
		if( rightBoxCollider == null )
		{
			Debug.LogError( "No box collider object on " + rightCollider.gameObject.name );
			enabled = false;
			return;
		}

		animator = GetComponent<Animator>();

		if( animator == null )
		{
			Debug.LogError( "No animator object on " + gameObject.name );
			enabled = false;
			return;
		}
	}
	
	void OnEnable()
	{
		if( leftCollider )
			leftCollider.OnCollisionWithBall += OnCollisionWithLeft;
		
		if( rightCollider )
			rightCollider.OnCollisionWithBall += OnCollisionWithRight;
	}
	
	void OnDisable()
	{
		if( leftCollider )
			leftCollider.OnCollisionWithBall -= OnCollisionWithLeft;
		
		if( rightCollider )
			rightCollider.OnCollisionWithBall -= OnCollisionWithRight;
	}
	
	private void OnCollisionWithLeft( GameObject ball )
	{
		//Debug.Log( "Left collider hit at " + Time.time );
		
		EvaluateHit( ball, true );
	}
	
	private void OnCollisionWithRight( GameObject ball )
	{
		//Debug.Log( "Right collider hit at " + Time.time );
		
		EvaluateHit( ball, false );
	}
	
	private void EvaluateHit( GameObject ball, bool isLeft )
	{
		InputController.PlayerNumber imprint = InputController.PlayerNumber.Invalid;
		
		ImprintController imprintController = ball.GetComponent<ImprintController>();
		
		if( imprintController )
			imprint = imprintController.GetLastImprint();
		
		if( isLeft )
		{				
			lastImprintLeft = imprint;
			
			if( lastImprintRight == InputController.PlayerNumber.Invalid )
			{
				StartCoroutine( "DoShotClock" );
			}
			else
			{
				if( lastImprintLeft == lastImprintRight )
				{
					Score( imprint, imprintController );
				}
			}
		}
		else
		{			
			lastImprintRight = imprint;
			
			if( lastImprintLeft == InputController.PlayerNumber.Invalid )
			{
				StartCoroutine( "DoShotClock" );
			}
			else
			{
				if( lastImprintLeft == lastImprintRight )
				{
					Score( imprint, imprintController );
				}
			}
		}
	}

	private void Score( InputController.PlayerNumber imprint, ImprintController imprintController )
	{
		if( confettiPrefab )
			Instantiate( confettiPrefab, transform.position, Quaternion.identity );

		ScoreAgent.IncrementPlayerScore( imprint );
		imprintController.GetPlayerController().ToTall();
		StartCoroutine( "DoRecharge" );
		ResetImprints();

		SoundAgent.PlayClip( SoundAgent.SoundEffects.Score );
	}
	
	private IEnumerator DoShotClock()
	{
		yield return new WaitForSeconds( shotClockDuration );
		
		ResetImprints();
	}
	
	private void ResetImprints()
	{
		lastImprintLeft = InputController.PlayerNumber.Invalid;
		lastImprintRight = InputController.PlayerNumber.Invalid;
	}

	private IEnumerator DoRecharge()
	{
		leftBoxCollider.enabled = false;
		rightBoxCollider.enabled = false;

		animator.SetBool( "Score", true );

		yield return new WaitForSeconds( rechargeDuration );

		animator.SetBool( "Score", false );

		leftBoxCollider.enabled = true;
		rightBoxCollider.enabled = true;
	}
}
