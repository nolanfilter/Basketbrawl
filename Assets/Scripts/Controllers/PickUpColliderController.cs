using UnityEngine;
using System.Collections;

public class PickUpColliderController : MonoBehaviour {

	private PlayerController playerController;

	void Start()
	{
		playerController = transform.parent.parent.gameObject.GetComponent<PlayerController>();

		if( playerController == null )
		{
			enabled = false;
			return;
		}
	}

	private void OnTriggerEnter( Collider other)
	{
		if( other.tag == "Ball" )
			if( playerController != null )
				playerController.CarryBall( other.gameObject );
	}
}
