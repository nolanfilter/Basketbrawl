using UnityEngine;
using System.Collections;

public class HackyModeController : MonoBehaviour {

	void Update()
	{
		if( Input.GetKeyDown( KeyCode.Space ) )
		{
			if( StateAgent.GetCurrentState() == StateAgent.State.PreGame )
				StateAgent.ChangeState( StateAgent.State.CountDown );
			else if( StateAgent.GetCurrentState() == StateAgent.State.PostGame )
				StateAgent.ChangeState( StateAgent.State.PreGame );
		}
	}
}
