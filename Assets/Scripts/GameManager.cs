using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    [SerializeField] int maxParticlesCount = 30;

    [SerializeField] Transform particleParent;

    [SerializeField] GameObject smashParticle;
    Stack<GameObject> freeSmashParticles = new Stack<GameObject>();

    [SerializeField] GameObject splatterParticle;
    Stack<GameObject> freeSplatterParticles = new Stack<GameObject>();

    //TODO maybe make highscore

	// Use this for initialization
	void Awake() {
        DontDestroyOnLoad(gameObject);
        for (int i = 0; i < maxParticlesCount; i++)
        {
            GameObject newSmashParticle = Instantiate(smashParticle, transform.position, Quaternion.Euler(new Vector3(90f, 0f, 0f)), particleParent);
            newSmashParticle.SetActive(false);
            freeSmashParticles.Push(newSmashParticle);

            GameObject newSplatterParticle = Instantiate(splatterParticle, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), particleParent);
            newSplatterParticle.SetActive(false);
            freeSplatterParticles.Push(newSplatterParticle);
        }
	}
	
	public GameObject GetSmashParticle(Vector3 pos)
    {
        GameObject pObj = freeSmashParticles.Pop();
        pObj.SetActive(true);
        pObj.transform.position = pos;
        ParticleSystem pSys = pObj.GetComponent<ParticleSystem>();
        pSys.Play();
        StartCoroutine(GetParticleBack(pObj, pSys.main.duration, freeSmashParticles));
        return pObj;
    }

    public GameObject GetSplatterParticle(Vector3 pos)
    {
        GameObject pObj = freeSplatterParticles.Pop();
        pObj.SetActive(true);
        pObj.transform.position = pos;
        ParticleSystem pSys = pObj.GetComponent<ParticleSystem>();
        pSys.Play();
        StartCoroutine(GetParticleBack(pObj, pSys.main.duration, freeSplatterParticles));
        return pObj;
    }

    IEnumerator GetParticleBack(GameObject ps, float seconds, Stack<GameObject> stackToPush)
    {
        yield return new WaitForSeconds(seconds);
        ps.SetActive(false);
        stackToPush.Push(ps);
    }
}
