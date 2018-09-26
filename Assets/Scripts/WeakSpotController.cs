using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class WeakSpotController : Singleton<WeakSpotController> {

    public int EnergyCoun
    {
        get
        {
            return energy;
        }
    }

    public int RageMeter
    {
        get
        {
            return rageMeter;
        }
        private set
        {
            rageMeter = value;
            if(OnRageMeterChanged != null)
            {
                OnRageMeterChanged(rageMeter, maxRage);
            }
        }
    }

    public int MaxRage
    {
        get
        {
            return maxRage;
        }
    }

    public System.Action<int, int> OnRageMeterChanged;

    [SerializeField] int maxEnergy = 100;
    int energy;
    int energyOnEnemies;

    [SerializeField] int maxRage = 100;
    int rageMeter = 0;

    [SerializeField] GameObject energyEffect;

    HandLeftController handLeft;

    [SerializeField] PostProcessingProfile postProfile;

    [SerializeField] float normalSaturation = 1.25f;
    

	// Use this for initialization
	void Awake()
    {
        energy = maxEnergy;
        handLeft = GameObject.FindGameObjectWithTag("HandLeft").GetComponent<HandLeftController>();
        handLeft.OnSpecialSmashStarted += ResetRageMeter;
        var colorGrading = postProfile.colorGrading.settings;
        colorGrading.basic.saturation = normalSaturation;
        postProfile.colorGrading.settings = colorGrading;
    }

    private void Start()
    {
        if (OnRageMeterChanged != null)
        {
            OnRageMeterChanged(rageMeter, maxRage);
        }
    }

    private void Update()
    {
        if (energy <= 0 && energyOnEnemies <= 0 && !GameManager.Instance.IsDead)
        {
            GameManager.Instance.Dead();
        }
        if(energyEffect)
        {
            Vector3 scale = new Vector3(energy * 0.01f, energy * 0.01f, energy * 0.01f);
            energyEffect.transform.localScale = scale;
            for(int i = 0; i < energyEffect.transform.childCount; i++)
            {
                energyEffect.transform.GetChild(i).transform.localScale = scale;
            }
        }
    }

    void RecalculateEnergyOnEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        energyOnEnemies = 0;
        foreach (var enemy in enemies)
        {
            energyOnEnemies += enemy.GetComponent<EnemyController>().HasEnergy;
        }
    }

    public int LoseEnergy(int energyToSteal)
    {
        int stealAmount;
        if(energyToSteal > energy)
        {
            stealAmount = energy;
        }
        else
        {
            stealAmount = energyToSteal;
        }
        if(RageMeter < maxRage)
        {
            RageMeter += stealAmount;
        }
        if(RageMeter > maxRage)
        {
            RageMeter = maxRage;
        }
        energy -= stealAmount;
        // Change post processing saturation when low energy
        if(energy <= maxEnergy * 0.3333f)
        {
            var colorGrading = postProfile.colorGrading.settings;
            colorGrading.basic.saturation = normalSaturation * (energy / (maxEnergy * 0.333333f));
            postProfile.colorGrading.settings = colorGrading;
        }
        WaveSpawner.Instance.IncreaseBirdSpawnChance(2);
        return stealAmount;
    }

    public void EnergyLostForever(int energyLost)
    {
        WaveSpawner.Instance.IncreaseBirdSpawnChance(3);
    }

    public void GainRage(int rageToGain)
    {
        RageMeter += rageToGain;
    }

    public void RegainEnergy(int energyToRegain)
    {
        energy += energyToRegain;
    }
    public void GainEnergy(int energyToGain)
    {
        energy += energyToGain;
    }

    void ResetRageMeter()
    {
        RageMeter = 0;
    }
}
