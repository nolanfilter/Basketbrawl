using UnityEngine;
using System.Collections;

public class ImprintController : MonoBehaviour {

	private InputController.PlayerNumber lastImprint = InputController.PlayerNumber.Invalid;

	private PlayerController playerController;

	void Start()
	{
		playerController = transform.parent.gameObject.GetComponent<PlayerController>();
	}

	public InputController.PlayerNumber GetLastImprint()
	{
		return lastImprint;
	}
	
	public void SetLastImprint( InputController.PlayerNumber newImprint )
	{
		lastImprint = newImprint;
	}

	public PlayerController GetPlayerController()
	{
		return playerController;
	}
}
