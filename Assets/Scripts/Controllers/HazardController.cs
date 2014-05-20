using UnityEngine;
using System.Collections;

public class HazardController : MonoBehaviour {

	private float spikeForce = 5f;

	void OnTriggerEnter( Collider other )
	{
		ParentController parentContoller = other.gameObject.GetComponent<ParentController>();

		if( parentContoller )
			parentContoller.GetPlayerController().ToBall( Vector3.up * spikeForce + Vector3.right * parentContoller.GetPlayerController().GetHorizontalVelocity() );
	}
}
