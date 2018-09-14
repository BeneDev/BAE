using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : Singleton<GameManager> {

    public bool IsPSInput
    {
        get
        {
            return isPSInput;
        }
        set
        {
            isPSInput = value;
        }
    }

    public bool IsPaused
    {
        get
        {
            return isPaused;
        }
        set
        {
            isPaused = value;
        }
    }

    public System.Action OnGameStarted;
    public System.Action OnResumeToMainMenu;

    [Header("Canvases"), SerializeField] CanvasGroup mainMenu;
    [SerializeField] CanvasGroup gameplayUI;
    [SerializeField] CanvasGroup endScreen;
    [SerializeField] CanvasGroup pauseMenu;
    [SerializeField] CanvasGroup optionsMenu;

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

    [SerializeField] GameObject environment;
    Animator environmentAnim;

    bool isPaused = false;

    bool optionsOutOfPauseMenu = false;

	// Use this for initialization
	void Awake()
    {
        if(environment)
        {
            environmentAnim = environment.GetComponent<Animator>();
        }
        if (!particleParent)
        {
            //particleParent = Instantiate(GameObject.empty)
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
        pauseMenu.GetComponent<PauseMenuController>().OnContinue += Continue;
    }

    private void Update()
    {
        if(!GameManager.Instance.IsPSInput)
        {
            if (pauseMenu.alpha < 1f && Input.GetButtonDown("Pause"))
            {
                StartCoroutine(FadeCanvas(pauseMenu, 1f, 1f));
                //Time.timeScale = 0f;
                GameManager.Instance.IsPaused = true;
            }
            if (pauseMenu.alpha > 0f && Input.GetButtonDown("Pause"))
            {
                Continue();
            }
        }
        else
        {
            if (pauseMenu.alpha < 1f && Input.GetButtonDown("PSPause"))
            {
                StartCoroutine(FadeCanvas(pauseMenu, 1f, 1f));
                //Time.timeScale = 0f;
                GameManager.Instance.IsPaused = true;
            }
            if (pauseMenu.alpha > 0f && Input.GetButtonDown("PSPause"))
            {
                Continue();
            }
        }
        if(Input.GetButtonDown("Cancel"))
        {
            if(pauseMenu.alpha > 0f)
            {
                Continue();
            }
            else if(optionsMenu.alpha > 0f)
            {
                FadeOptionsOut();
            }
        }
    }

    public void FadeOutSound(AudioSource aS, float duration)
    {
        StartCoroutine(FadeSoundOut(aS, duration));
    }

    IEnumerator FadeSoundOut(AudioSource aS, float duration)
    {
        float startVol = aS.volume;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            aS.volume = startVol * (1 - (t / duration));
            yield return new WaitForEndOfFrame();
        }
        aS.volume = 0f;
        aS.Stop();
    }

    public void FadeInSound(AudioSource aS, float targetVol, float duration)
    {
        StartCoroutine(FadeSoundIn(aS, targetVol, duration));
    }

    IEnumerator FadeSoundIn(AudioSource aS, float targetVol, float duration)
    {
        aS.Play();
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            aS.volume = targetVol * (t / duration);
            yield return new WaitForEndOfFrame();
        }
        aS.volume = targetVol;
    }

    void Continue()
    {
        Time.timeScale = 1f;
        StartCoroutine(FadeCanvas(pauseMenu, 0f, 1f));
        GameManager.Instance.IsPaused = false;
    }

    public void FadeOptionsIn()
    {
        StartCoroutine(FadeCanvas(optionsMenu, 1f, 1f));
        if(pauseMenu.alpha > 0f)
        {
            StartCoroutine(FadeCanvas(pauseMenu, 0f, 1f));
            optionsOutOfPauseMenu = true;
        }
        else
        {
            optionsOutOfPauseMenu = false;
        }
    }

    public void FadeOptionsOut()
    {
        if(optionsOutOfPauseMenu)
        {
            StartCoroutine(FadeCanvas(pauseMenu, 1f, 1f));
        }
        else if(OnResumeToMainMenu != null)
        {
            OnResumeToMainMenu();
        }
        StartCoroutine(FadeCanvas(optionsMenu, 0f, 1f));
    }

    public void PlayGame()
    {
        foreach (MonoBehaviour script in scriptsToEnableToPlay)
        {
            script.enabled = true;
        }
        StartCoroutine(FadeCanvas(mainMenu, 0f, 1f));
        StartCoroutine(FadeCanvas(gameplayUI, 1f, 1f, 1f));
        if(OnGameStarted != null)
        {
            OnGameStarted();
        }
    }

    public void Dead()
    {
        StartCoroutine(FadeCanvas(endScreen, 1f, 1f));
        StartCoroutine(FadeCanvas(gameplayUI, 0f, 1f));
        foreach (MonoBehaviour script in scriptsToEnableToPlay)
        {
            script.enabled = false;
        }
        if(pauseMenu.alpha > 0f)
        {
            Continue();
        }
        Invoke("PlayEnvironmentBreakDown", 1f);
    }

    void PlayEnvironmentBreakDown()
    {
        if (environmentAnim)
        {
            environmentAnim.enabled = true;
            environmentAnim.SetTrigger("BreakDown");
        }
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach(GameObject enemy in enemies)
        {
            Destroy(enemy.GetComponent<NavMeshAgent>());
            enemy.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    IEnumerator FadeCanvas(CanvasGroup canvas, float fadeTo, float duration, float secondsToWait = 0f)
    {
        yield return new WaitForSecondsRealtime(secondsToWait);
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

    public GameObject GetLittleEnergy(GameObject objectToFollow, Vector3 offset)
    {
        ParticleSystem ps = freeLittleEnergies.Pop().GetComponent<ParticleSystem>();
        ps.gameObject.SetActive(true);
        ps.Play();
        if(objectToFollow.GetComponent<EnemyController>())
        {
            StartCoroutine(GetFollowingParticleSystemBack(2.5f, ps.gameObject, freeLittleEnergies, objectToFollow.GetComponent<EnemyController>(), WeakSpotController.Instance.transform.position, offset));
        }
        else
        {
            StartCoroutine(GetFollowingParticleSystemBack(0.5f, ps.gameObject, freeLittleEnergies, objectToFollow, WeakSpotController.Instance.transform.position, offset));
        }
        return ps.gameObject;
    }

    IEnumerator GetFollowingParticleSystemBack(float durationAfterDeath, GameObject ps, Stack<GameObject> stackToPush, EnemyController enemyToFollow, Vector3 fadeTo, Vector3 offset)
    {
        while(enemyToFollow.HasEnergy > 0)
        {
            if(enemyToFollow)
            {
                ps.transform.position = enemyToFollow.gameObject.transform.position + offset;
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

    IEnumerator GetFollowingParticleSystemBack(float durationAfterDeath, GameObject ps, Stack<GameObject> stackToPush, GameObject objectToFollow, Vector3 fadeTo, Vector3 offset)
    {
        while (objectToFollow)
        {
            ps.transform.position = objectToFollow.gameObject.transform.position + offset;
            if (objectToFollow.GetComponent<BirdController>().IsFinished)
            {
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        Vector3 startScale = ps.transform.localScale;
        for (float t = 0f; t < durationAfterDeath; t += Time.deltaTime)
        {
            ps.transform.localScale = startScale - (startScale * (t / durationAfterDeath));
            yield return new WaitForEndOfFrame();
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
