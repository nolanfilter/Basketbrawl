using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class InputController : MonoBehaviour {
		
	public delegate void ButtonDownEventHandler( InputController.ButtonType button );
	public event ButtonDownEventHandler OnButtonDown;
	
	public delegate void ButtonHoldEventHandler( InputController.ButtonType button );
	public event ButtonHoldEventHandler OnButtonHeld;
	
	public delegate void ButtonUpEventHandler( InputController.ButtonType button );
	public event ButtonUpEventHandler OnButtonUp;
	
	public enum PlayerNumber
	{
		Player1 = 0,
		Player2 = 1,
		Player3 = 2,
		Player4 = 3,
		Invalid = 4,
	}
	public PlayerNumber currentPlayerNumber = PlayerNumber.Invalid;
	
	public enum ButtonType
	{
		Up = 0,
		Down = 1,
		Left = 2,
		Right = 3,
		Jump = 4,
		Jump2 = 5,
		Action = 6,
		Action2 = 7,
		Start = 8,
		Sel = 9,
		Invalid = 10,
	}
	
	private string verticalAxisString;
	private string horizontalAxisString;
	
	private KeyCode[] codes = new KeyCode[ Enum.GetNames( typeof( ButtonType ) ).Length - 1 ];
	
	private Array buttonTypes = Enum.GetValues( typeof( ButtonType ) );
	
	private Dictionary<ButtonType, bool> currentButtonList;
	private Dictionary<ButtonType, bool> oldButtonList;
	
	void Start()
	{
		if( currentPlayerNumber == PlayerNumber.Invalid )
		{
			Debug.LogError( "Invalid Player Number on " + gameObject.name );
			enabled = false;
			return;
		}

		int number = (int)currentPlayerNumber;

		if( number == 1 )
			for( int i = 0; i < Input.GetJoystickNames().Length; i++ )
				Debug.Log( Input.GetJoystickNames()[i] );
		
		verticalAxisString = "Vertical" + number;
		horizontalAxisString = "Horizontal" + number;
		
		//hardcoded for PS3 controller, PS4 controller, and PC
		if( Input.GetJoystickNames().Length > number )
		{	
			switch( Input.GetJoystickNames()[number] )
			{
				//PS3
				case "Sony PLAYSTATION(R)3 Controller":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button4 + 20 * number );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button6 + 20 * number );
				 	codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button7 + 20 * number );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button5 + 20 * number );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 * number );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 * number );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button14 + 20 * number );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button12 + 20 * number );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button13 + 20 * number );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button15 + 20 * number );
				} break;

				//PS4
				case "Sony Computer Entertainment Wireless Controller":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button16 + 20 * number );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button17 + 20 * number );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button18 + 20 * number );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 * number );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button9 + 20 * number );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button13 + 20 * number );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button1 + 20 * number );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 * number );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button2 + 20 * number );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 * number );
				} break;

				//SNES
				case " 2Axes 11Keys Game  Pad":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button6 + 20 * number );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button7 + 20 * number );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button10 + 20 * number );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button11 + 20 * number );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button8 + 20 * number );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button9 + 20 * number );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button2 + 20 * number );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button0 + 20 * number );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button1 + 20 * number );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button3 + 20 * number );
				} break;

				//XBOX 360
				case "":
				{
					codes[ (int)ButtonType.Up ] = (KeyCode)( (int)KeyCode.Joystick1Button5 + 20 * number );
					codes[ (int)ButtonType.Down ] = (KeyCode)( (int)KeyCode.Joystick1Button6 + 20 * number );
					codes[ (int)ButtonType.Left ] = (KeyCode)( (int)KeyCode.Joystick1Button7 + 20 * number );
					codes[ (int)ButtonType.Right ] = (KeyCode)( (int)KeyCode.Joystick1Button8 + 20 * number );
					codes[ (int)ButtonType.Sel ] = (KeyCode)( (int)KeyCode.Joystick1Button10 + 20 * number );
					codes[ (int)ButtonType.Start ] = (KeyCode)( (int)KeyCode.Joystick1Button9 + 20 * number );
					codes[ (int)ButtonType.Jump ] = (KeyCode)( (int)KeyCode.Joystick1Button16 + 20 * number );
					codes[ (int)ButtonType.Jump2 ] = (KeyCode)( (int)KeyCode.Joystick1Button19 + 20 * number );
					codes[ (int)ButtonType.Action ] = (KeyCode)( (int)KeyCode.Joystick1Button17 + 20 * number );
					codes[ (int)ButtonType.Action2 ] = (KeyCode)( (int)KeyCode.Joystick1Button18 + 20 * number );
				} break;
			}
		}
		else
		{
			if( number == 0 || number == 1 )
			{	
				codes[ (int)ButtonType.Up ] = KeyCode.W;
				codes[ (int)ButtonType.Down ] = KeyCode.S;
			 	codes[ (int)ButtonType.Left ] = KeyCode.A;
				codes[ (int)ButtonType.Right ] = KeyCode.D;
				codes[ (int)ButtonType.Sel ] = KeyCode.Q;
				codes[ (int)ButtonType.Start ] = KeyCode.E;
				codes[ (int)ButtonType.Jump ] = KeyCode.LeftShift;
				codes[ (int)ButtonType.Jump2 ] = KeyCode.LeftShift;
				codes[ (int)ButtonType.Action ] = KeyCode.LeftCommand;
				codes[ (int)ButtonType.Action2 ] = KeyCode.LeftCommand;
			}
			else if( number == 2 || number == 3 )
			{	
				codes[ (int)ButtonType.Up ] = KeyCode.UpArrow;
				codes[ (int)ButtonType.Down ] = KeyCode.DownArrow;
			 	codes[ (int)ButtonType.Left ] = KeyCode.LeftArrow;
				codes[ (int)ButtonType.Right ] = KeyCode.RightArrow;
				codes[ (int)ButtonType.Sel ] = KeyCode.Period;
				codes[ (int)ButtonType.Start ] = KeyCode.Backslash;
				codes[ (int)ButtonType.Jump ] = KeyCode.Space;
				codes[ (int)ButtonType.Jump2 ] = KeyCode.Space;
				codes[ (int)ButtonType.Action ] = KeyCode.Return;
				codes[ (int)ButtonType.Action2 ] = KeyCode.Return;
			}
		}
		//end hardcoded

		currentButtonList = new Dictionary<ButtonType, bool>();
		oldButtonList = new Dictionary<ButtonType, bool>();
		
		foreach( ButtonType button in buttonTypes )
		{
			currentButtonList.Add( button, false );
			oldButtonList.Add( button, false );
		}
	}
	
	void Update()
	{
		if( StateAgent.GetCurrentState() == StateAgent.State.CountDown || StateAgent.GetCurrentState() == StateAgent.State.PostGame )
			return;
	
		float verticalAxis = Input.GetAxisRaw( verticalAxisString );
		float horizontalAxis = Input.GetAxisRaw( horizontalAxisString );
		
		currentButtonList[ ButtonType.Up ] = verticalAxis > 0f;
		currentButtonList[ ButtonType.Right ] = horizontalAxis > 0f;
		currentButtonList[ ButtonType.Down ] = verticalAxis < 0f;
		currentButtonList[ ButtonType.Left ] = horizontalAxis < 0f;
		
		for( int i = 0; i < codes.Length; i++ )
		{
			if( i == (int)ButtonType.Up || i == (int)ButtonType.Right || i == (int)ButtonType.Down || i == (int)ButtonType.Left )
				currentButtonList[ (ButtonType)i ] = currentButtonList[ (ButtonType)i ] || Input.GetKey( codes[ i ] );
			else if( i == (int)ButtonType.Action2 )
				currentButtonList[ ButtonType.Action ] = currentButtonList[ ButtonType.Action ] || Input.GetKey( codes[ i ] );
			else if( i == (int)ButtonType.Jump2 )
				currentButtonList[ ButtonType.Jump ] = currentButtonList[ ButtonType.Jump ] || Input.GetKey( codes[ i ] );
			else
				currentButtonList[ (ButtonType)i ] = Input.GetKey( codes[ i ] );
			
		}
		
		foreach( ButtonType button in buttonTypes )
		{
			if( currentButtonList[ button ] )
			{
				if( oldButtonList[ button ] )
					SendHeldEvent( button );
				else
					SendDownEvent( button );
			}
			else
			{
				if( oldButtonList[ button ] )
					SendUpEvent( button );
			}
			
			oldButtonList[ button ] = currentButtonList[ button ];
		}
	}
	
	private void SendDownEvent( ButtonType button )
	{						
		//Debug.Log( button );

		if( OnButtonDown != null )
			OnButtonDown( button );		
	}
	
	private void SendHeldEvent( ButtonType button )
	{
		if( OnButtonHeld != null )
			OnButtonHeld( button );	
	}
	
	private void SendUpEvent( ButtonType button )
	{
		if( OnButtonUp != null )
			OnButtonUp( button );	
	}

	public Vector3 GetRawAxes()
	{
		float verticalAxis = Input.GetAxisRaw( verticalAxisString );
		float horizontalAxis = Input.GetAxisRaw( horizontalAxisString );

		Vector3 rawAxes = new Vector3( horizontalAxis, verticalAxis, 0f );

		if( rawAxes == Vector3.zero )
		{
			if( currentButtonList[ ButtonType.Up ] )
				rawAxes += Vector3.up;

			if( currentButtonList[ ButtonType.Down ] )
				rawAxes += Vector3.down;

			if( currentButtonList[ ButtonType.Left ] )
				rawAxes += Vector3.left;

			if( currentButtonList[ ButtonType.Right ] )
				rawAxes += Vector3.right;

		}

		return rawAxes;
	}
}
