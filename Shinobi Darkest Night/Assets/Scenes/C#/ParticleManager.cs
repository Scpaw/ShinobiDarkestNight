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
    public void UseParticle(string particleName, Vector3 position, Vector3 rotation)
    {
        StartCoroutine(StartParticle(particleName, position,rotation));
    }

    public IEnumerator StartParticle(string particleName, Vector3 position, Vector3 rotation)
    {      
        int particleToPlay = 0;
        if (particles.Count > 0)
        {
            foreach (GameObject effect in particles)
            {

                if (effect.name.ToString() == (particleName + "(Clone)").ToString())
                {
                    particleToPlay = particles.IndexOf(effect);
                }
            }          
            if (particles[particleToPlay].GetComponent<ParticleSystem>().isEmitting)
            {
                yield return new WaitUntil(() => !particles[particleToPlay].GetComponent<ParticleSystem>().isEmitting);
            }
            particles[particleToPlay].transform.position = position;
            particles[particleToPlay].transform.rotation = Quaternion.Euler(rotation);
            particles[particleToPlay].GetComponent<ParticleSystem>().Play();
        }
        yield return new WaitForEndOfFrame();
          particles[particleToPlay].GetComponent<ParticleSystem>().Stop();
    }
    
}
