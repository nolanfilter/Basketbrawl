using UnityEngine;
using System.Collections;

public class ConfettiController : MonoBehaviour {

	private ParticleSystem ps;

	void Awake()
	{
		ps = GetComponent<ParticleSystem>();

		if( ps == null )
		{
			enabled = false;
			return;
		}
	}

	void Start()
	{
		StartCoroutine( "RandomlyColorParticles" );
		Destroy( gameObject, ps.duration );
	}

	private IEnumerator RandomlyColorParticles()
	{
		yield return new WaitForEndOfFrame();

		ParticleSystem.Particle[] particles = new ParticleSystem.Particle[ ps.particleCount ];

		int length = ps.GetParticles( particles );

		for( int i = 0; i < length; i++ )
			particles[i].color = new Color( Random.value, Random.value, Random.value, 1f );

		ps.SetParticles( particles, length );
	}
}
