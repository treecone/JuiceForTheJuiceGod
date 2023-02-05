using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.ParticleSystem;

public class JS_JuiceParticle : MonoBehaviour
{

    //https://www.reddit.com/r/Unity3D/comments/50dayf/make_particles_track_to_object/

    private ParticleSystem p;
    private ParticleSystem.Particle[] particles;
    private Transform Target;
    Transform thisTransform;


    void Start()
    {
        Target = GameObject.Find("Player").transform.GetChild(0);
        p = GetComponent<ParticleSystem>();
    }


    void Update()
    {
        if (Target == null)
            return;

        particles = new ParticleSystem.Particle[p.particleCount];

        p.GetParticles(particles);

        for (int i = 0; i < particles.GetUpperBound(0); i++)
        {

            float ForceToAdd = (particles[i].startLifetime - particles[i].remainingLifetime) * (10 * Vector3.Distance(Target.position, particles[i].position));

            //Debug.DrawRay (particles [i].position, (Target.position - particles [i].position).normalized * (ForceToAdd/10));


            particles[i].velocity = (Target.position - particles[i].position).normalized * ForceToAdd;

            //particles [i].position = Vector3.Lerp (particles [i].position, Target.position, Time.deltaTime / 2.0f);

        }

        p.SetParticles(particles, particles.Length);

    }
}
