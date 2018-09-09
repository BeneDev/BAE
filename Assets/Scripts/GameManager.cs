using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

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
