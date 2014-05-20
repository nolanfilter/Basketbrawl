using UnityEngine;
using System.Collections;

public class MagmaController : MonoBehaviour {
	
	void OnTriggerEnter( Collider other )
	{
		Debug.Log("DIE!!");
		if( other.gameObject.tag == "Player" || other.gameObject.tag == "Ball" )
			Debug.Log("DIE!!");
			//KillPlayer();
	}
}
