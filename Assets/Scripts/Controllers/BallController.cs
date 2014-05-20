using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {
	
	private float ballInvulnerabilityDuration = 0.25f;

	void OnEnable()
	{
		StartCoroutine( "DoBallInvulernability" );
	}
	
	void OnDisable()
	{		
		StopCoroutine( "DoBallInvulernability" );
		PlayerAgent.UnregisterBall( gameObject );
	}
	
	private IEnumerator DoBallInvulernability()
	{
		rigidbody.useGravity = false;
		
		yield return new WaitForSeconds( ballInvulnerabilityDuration );
		
		rigidbody.useGravity = true;
		
		PlayerAgent.RegisterBall( gameObject );
	}
}
