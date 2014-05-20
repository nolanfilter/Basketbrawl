using UnityEngine;
using System.Collections;

public class POWController : MonoBehaviour {

	void OnCollisionEnter( Collision other )
	{

		if( other.gameObject.tag == "Ball" )
			Debug.Log("Earthquake!!");
			//Earthquake();
	}
}
