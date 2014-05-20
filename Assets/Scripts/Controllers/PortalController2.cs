using UnityEngine;
using System.Collections;

public class PortalController2 : MonoBehaviour {

	public Vector3 newPosition;

	void OnTriggerEnter( Collider other )
	{
		if( other.gameObject.tag == "Player" || other.gameObject.tag == "Ball" )
			other.transform.position = newPosition;
	}
}
