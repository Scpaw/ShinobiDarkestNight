using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;
    public List<GameObject> particles;
    private bool corutineOn;

    void Awake()
    {
        instance = this;

    }
    private void Start()
    {  
        List<GameObject> list = new List<GameObject>();
        if (particles.Count != 0)
        {
            foreach (GameObject effect in particles)
            {
                list.Add(Instantiate(effect, new Vector3(300, 699, 345), new Quaternion(0, 0, 0, 0)));
            }
            particles.Clear();
            particles = list;
        }
    }
    public void UseParticle(string particleName, Transform position)
    {
        StartCoroutine(StartParticle(particleName, position));
    }

    public IEnumerator StartParticle(string particleName, Transform position)
    {      
        int particleToPlay = 0;
        if (particles.Count > 0)
        {
            foreach (GameObject effect in particles)
            {
                if (effect.name == particleName)
                {
                    particleToPlay = particles.IndexOf(effect);
                }
            }          
            if (particles[particleToPlay].GetComponent<ParticleSystem>().isEmitting)
            {
                yield return new WaitUntil(() => !particles[particleToPlay].GetComponent<ParticleSystem>().isEmitting);
            }
            particles[particleToPlay].transform.position = position.position;
            particles[particleToPlay].GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForEndOfFrame();
          particles[particleToPlay].GetComponent<ParticleSystem>().Stop();
    }
    
}
