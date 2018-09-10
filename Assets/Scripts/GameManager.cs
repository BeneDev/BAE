﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public bool IsPSInput
    {
        get
        {
            return isPSInput;
        }
    }

    [Header("Canvases"), SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup gameplayUI;
    [SerializeField] CanvasGroup endScreen;

    [SerializeField] MonoBehaviour[] scriptsToEnableToPlay;

    [SerializeField] int maxParticlesCount = 30;

    [SerializeField] Transform particleParent;

    [SerializeField] GameObject smashParticle;
    Stack<GameObject> freeSmashParticles = new Stack<GameObject>();

    [SerializeField] GameObject splatterParticle;
    Stack<GameObject> freeSplatterParticles = new Stack<GameObject>();

    [SerializeField] GameObject littleEnergyParticle;
    Stack<GameObject> freeLittleEnergies = new Stack<GameObject>();

    [SerializeField] bool isPSInput = false;

    //TODO maybe make highscore

	// Use this for initialization
	void Awake()
    {
        if (!particleParent)
        {
            
        }
        for (int i = 0; i < maxParticlesCount; i++)
        {
            GameObject newSmashParticle = Instantiate(smashParticle, transform.position, Quaternion.Euler(new Vector3(90f, 0f, 0f)), particleParent);
            newSmashParticle.SetActive(false);
            freeSmashParticles.Push(newSmashParticle);

            GameObject newSplatterParticle = Instantiate(splatterParticle, transform.position, Quaternion.Euler(new Vector3(-90f, 0f, 0f)), particleParent);
            newSplatterParticle.SetActive(false);
            freeSplatterParticles.Push(newSplatterParticle);

            GameObject newLittleEnergy = Instantiate(littleEnergyParticle, transform.position, Quaternion.Euler(Vector3.zero), particleParent);
            newLittleEnergy.SetActive(false);
            freeLittleEnergies.Push(newLittleEnergy);
        }
	}

    private void Start()
    {
        foreach (MonoBehaviour script in scriptsToEnableToPlay)
        {
            script.enabled = false;
        }
    }

    public void PlayGame()
    {
        foreach (MonoBehaviour script in scriptsToEnableToPlay)
        {
            script.enabled = true;
        }
        StartCoroutine(FadeCanvas(mainMenu, 0f, 1f));
        StartCoroutine(FadeCanvas(gameplayUI, 1f, 1f, 1f));
    }

    public void Dead()
    {
        StartCoroutine(FadeCanvas(endScreen, 1f, 1f));
        StartCoroutine(FadeCanvas(gameplayUI, 0f, 1f));
        foreach (MonoBehaviour script in scriptsToEnableToPlay)
        {
            script.enabled = false;
        }
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float fadeTo, float duration, float secondsToWait = 0f)
    {
        yield return new WaitForSeconds(secondsToWait);
        if(fadeTo > 0f)
        {
            canvas.gameObject.SetActive(true);
        }
        else
        {
            canvas.interactable = false;
        }
        float startFadeValue = canvas.alpha;
        float progress;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            if (fadeTo > startFadeValue)
            {
                progress = t / duration;
            }
            else
            {
                progress = 1 - t / duration;
            }
            canvas.alpha = progress;
            yield return new WaitForEndOfFrame();
        }
        canvas.alpha = fadeTo;
        if (fadeTo <= 0f)
        {
            canvas.gameObject.SetActive(false);
        }
        else
        {
            canvas.interactable = true;
        }
        yield break;
    }

    public GameObject GetLittleEnergy(GameObject objectToFollow)
    {
        ParticleSystem ps = freeLittleEnergies.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.SetActive(true);
        ps.Play();
        if(objectToFollow.GetComponent<EnemyController>())
        {
            StartCoroutine(GetFollowingParticleSystemBack(2.5f, ps.gameObject, freeLittleEnergies, objectToFollow.GetComponent<EnemyController>(), WeakSpotController.Instance.transform.position));
        }
        return ps.gameObject;
    }

    IEnumerator GetFollowingParticleSystemBack(float durationAfterDeath, GameObject ps, Stack<GameObject> stackToPush, EnemyController enemyToFollow, Vector3 fadeTo)
    {
        while(enemyToFollow.HasEnergy > 0)
        {
            if(enemyToFollow)
            {
                ps.transform.position = enemyToFollow.gameObject.transform.position + Vector3.up * 1.1f;
            }
            else
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        if(!enemyToFollow)
        {
            Vector3 startPos = ps.transform.position;
            for (float t = 0f; t < durationAfterDeath; t += Time.deltaTime)
            {
                ps.transform.position = startPos + (fadeTo - startPos) * (t / durationAfterDeath);
                yield return new WaitForEndOfFrame();
            }
        }
        ps.SetActive(false);
        stackToPush.Push(ps);
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
