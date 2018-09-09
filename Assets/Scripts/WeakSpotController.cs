using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	// Use this for initialization
	void Awake() {
        energy = maxEnergy;
        handLeft = GameObject.FindGameObjectWithTag("HandLeft").GetComponent<HandLeftController>();
        handLeft.OnSpecialSmashEnd += ResetRageMeter;
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
        if (energy <= 0 && energyOnEnemies <= 0)
        {
            GameManager.Instance.Dead();
            //TODO play fancy dying animation, making the hands fall down of weakness and then the world collapses
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
            RageMeter += stealAmount * 2;
        }
        if(RageMeter > maxRage)
        {
            RageMeter = maxRage;
        }
        energy -= stealAmount;
        energyOnEnemies += stealAmount;
        //TODO play animation or alter particle effect to show player, energy has been stolen
        return stealAmount;
    }

    public void EnergyLostForever(int energyLost)
    {
        energyOnEnemies -= energyLost;
    }

    public void RegainEnergy(int energyToRegain)
    {
        energy += energyToRegain;
        energyOnEnemies -= energyToRegain;
        if(energy > maxEnergy)
        {
            energy = maxEnergy;
        }
        //TODO play animation or alter particle effect to show player, energy has been regained
    }

    void ResetRageMeter()
    {
        RageMeter = 0;
    }
}
