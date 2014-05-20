using UnityEngine;
using System.Collections;

public class ParentController : MonoBehaviour {

	private PlayerController playerController = null;

	void Awake()
	{
		Transform parent = transform.parent;

		while( parent != null && playerController == null )
		{
			playerController = parent.gameObject.GetComponent<PlayerController>();

			parent = parent.parent;
		}
	}

	public PlayerController GetPlayerController()
	{
		return playerController;
	}
}
