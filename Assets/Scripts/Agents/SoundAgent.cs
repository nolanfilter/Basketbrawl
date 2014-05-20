using UnityEngine;
using System.Collections;

public class SoundAgent : MonoBehaviour {

	public enum SoundEffects
	{
		BallBounce = 0,
		ToBall = 1,
		GotHit = 2,
		Hit = 3,
		Jump = 4,
		CarryBall = 5,
		Score = 6,
		ToggleUp = 7,
		ToggleDown = 8,
		AdvanceScreen = 9,
		Countdown1 = 10,
		Countdown2 = 11,
		Countdown3 = 12,
		CountdownGo = 13,
		Logo = 14,
		Fanfare = 15,
		Invulernability = 16,
		Impact = 17,
	}
	
	public GameObject audioObject;
	
	public AudioClip BallBounceClip;
	public AudioClip ToBallClip;
	public AudioClip GotHitClip;
	public AudioClip HitClip;
	public AudioClip JumpClip;
	public AudioClip CarryBallClip;
	public AudioClip ScoreClip;
	public AudioClip ToggleUpClip;
	public AudioClip ToggleDownClip;
	public AudioClip AdvanceScreenClip;
	public AudioClip Countdown1Clip;
	public AudioClip Countdown2Clip;
	public AudioClip Countdown3Clip;
	public AudioClip CountdownGoClip;
	public AudioClip LogoClip;
	public AudioClip FanfareClip;
	public AudioClip InvulernabilityClip;
	public AudioClip ImpactClip;
	
	public AudioClip backgroundMusicClip;
	private AudioSource backgroundMusicSource;
	
	private float backgroundMusicVolume = 1f;
	
	private int globalVolume;	
	private string globalVolumeString = "GlobalVolume";
	
	private static SoundAgent mInstance = null;
	public static SoundAgent instance
	{
		get
		{
			return mInstance;
		}
	}
	
	void Awake()
	{
		if( mInstance != null )
		{
			Debug.LogError( string.Format( "Only one instance of SoundAgent allowed! Destroying:" + gameObject.name +", Other:" + mInstance.gameObject.name ) );
			Destroy( gameObject );
			return;
		}

		mInstance = this;
	}
	
	void Start()
	{
		if( audioObject == null )
			audioObject = Camera.main.gameObject;
		
		if( PlayerPrefs.HasKey( globalVolumeString ) )
		{
			globalVolume = PlayerPrefs.GetInt( globalVolumeString );
		}
		else
		{
			globalVolume = 1;
			updateGlobalVolumePref();	
		}
		
		backgroundMusicSource = audioObject.AddComponent<AudioSource>();
		backgroundMusicSource.Stop();
		backgroundMusicSource.loop = true;
		backgroundMusicSource.volume = backgroundMusicVolume * (float)globalVolume;
		backgroundMusicSource.clip = backgroundMusicClip;
	}
	
	public static void PlayClip( SoundEffects soundEffect )
	{
		if( instance )
			instance.internalPlayClip( soundEffect );
	}
	
	private void internalPlayClip( SoundEffects soundEffect )
	{
		if( audioObject == null )
			return;
		
		AudioSource audioSource = audioObject.AddComponent<AudioSource>();
		
		audioSource.loop = false;
		audioSource.volume = (float)globalVolume;
		
		switch( soundEffect )
		{
			case SoundEffects.BallBounce: audioSource.clip = BallBounceClip; break;
			case SoundEffects.ToBall: audioSource.clip = ToBallClip; break;
			case SoundEffects.GotHit: audioSource.clip = GotHitClip; break;
			case SoundEffects.Hit: audioSource.clip = HitClip; break;
			case SoundEffects.Jump: audioSource.clip = JumpClip; break;
			case SoundEffects.CarryBall: audioSource.clip = CarryBallClip; break;
			case SoundEffects.Score: audioSource.clip = ScoreClip; break;
			case SoundEffects.ToggleUp: audioSource.clip = ToggleUpClip; break;
			case SoundEffects.ToggleDown: audioSource.clip = ToggleDownClip; break;
			case SoundEffects.AdvanceScreen: audioSource.clip = AdvanceScreenClip; break;
			case SoundEffects.Countdown1: audioSource.clip = Countdown1Clip; break;
			case SoundEffects.Countdown2: audioSource.clip = Countdown2Clip; break;
			case SoundEffects.Countdown3: audioSource.clip = Countdown3Clip; break;
			case SoundEffects.CountdownGo: audioSource.clip = CountdownGoClip; break;
			case SoundEffects.Logo: audioSource.clip = LogoClip; break;
			case SoundEffects.Fanfare: audioSource.clip = FanfareClip; break;
			case SoundEffects.Invulernability: audioSource.clip = InvulernabilityClip; break;
			case SoundEffects.Impact: audioSource.clip = ImpactClip; break;
		}
		
		
		if( audioSource.clip == null )
		{
			Destroy( audioSource );
			return;
		}
			
		audioSource.Play();
		
		StartCoroutine( DestroyOnFinish( audioSource, audioSource.clip.length ) );
	}
	
	public static void PlayBackgroundMusic( bool reset = false )
	{
		if( instance )
			instance.internalPlayBackgroundMusic( reset );
	}
	
	public static void MuteSounds()
	{
		if( instance )
			instance.setGlobalVolume( 0 );
	}
	
	public static void UnmuteSounds()
	{
		if( instance )
			instance.setGlobalVolume( 1 );
	}
	
	public static void PauseSounds()
	{
		if( instance )
			instance.internalPauseSounds();
	}
	
	private void internalPauseSounds()
	{
		AudioSource[] audioSources = audioObject.GetComponents<AudioSource>();
		
		for( int i = 0; i < audioSources.Length; i++ )
			if( audioSources[i] != backgroundMusicSource )
				audioSources[i].Pause();
	}
	
	public static void UnpauseSounds()
	{
		if( instance )
			instance.internalUnpauseSounds();
	}
	
	private void internalUnpauseSounds()
	{
		AudioSource[] audioSources = audioObject.GetComponents<AudioSource>();
		
		for( int i = 0; i < audioSources.Length; i++ )
			if( audioSources[i] != backgroundMusicSource )
				audioSources[i].Play();
	}
	
	private void setGlobalVolume( int newVolume )
	{
		if( globalVolume == newVolume )
			return;
		
		globalVolume = newVolume;
		updateGlobalVolumePref();
		
		AudioSource[] audioSources = audioObject.GetComponents<AudioSource>();
		
		for( int i = 0; i < audioSources.Length; i++ )
			if( audioSources[i] != backgroundMusicSource )
				audioSources[i].volume = (float)globalVolume;
		
		if( backgroundMusicSource )
			backgroundMusicSource.volume = backgroundMusicVolume * (float)globalVolume;
	}
	
	public static bool isMuted()
	{
		if( instance )
			return instance.internalIsMuted();
			
		return false;
	}
	
	private bool internalIsMuted()
	{		
		return ( globalVolume == 0 );
	}
	
	private void internalPlayBackgroundMusic( bool reset )
	{
		if( backgroundMusicClip == null || backgroundMusicSource.isPlaying )
			return;
		
		if( reset )
			backgroundMusicSource.time = 0f;
			
		backgroundMusicSource.Play();
	}
	
	public static void PauseBackgroundMusic()
	{
		if( instance )
			instance.internalPauseBackgroundMusic();
	}
	
	private void internalPauseBackgroundMusic()
	{
		if( backgroundMusicClip == null || backgroundMusicSource == null || !backgroundMusicSource.isPlaying )
			return;
		
		backgroundMusicSource.Pause();
	}
	
	private void updateGlobalVolumePref()
	{
		PlayerPrefs.SetInt( globalVolumeString, globalVolume );
	}
	
	private IEnumerator DestroyOnFinish( AudioSource source, float duration )
	{
		float currentTime = 0f;
		
		do
		{
			if( source.isPlaying )
				currentTime += Time.deltaTime;
			
			yield return null;
			
		} while( currentTime < duration );
		
		Destroy( source );
	}
}
