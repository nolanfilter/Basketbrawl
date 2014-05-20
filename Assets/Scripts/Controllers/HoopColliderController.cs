using UnityEngine;
using System.Collections;

public class HoopColliderController : MonoBehaviour {

	public delegate void HoopColliderEventHandler( GameObject ball );
	public event HoopColliderEventHandler OnCollisionWithBall;
	
	void OnTriggerEnter( Collider other ) 
	{
		if( OnCollisionWithBall != null )
			OnCollisionWithBall( other.gameObject );	
	}
	
	void OnTriggerStay( Collider other ) 
	{
		if( OnCollisionWithBall != null )
			OnCollisionWithBall( other.gameObject );	
	}
}
