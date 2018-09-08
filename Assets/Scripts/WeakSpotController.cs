using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakSpotController : MonoBehaviour {

    [SerializeField] int maxEnergy = 100;
    int energy;

	// Use this for initialization
	void Awake() {
        energy = maxEnergy;
	}

    public void LoseEnergy(int energyToSteal)
    {
        energy -= energyToSteal;
        //TODO play animation or alter particle effect to show player, energy has been stolen
        if(energy <= 0)
        {
            print("dead");
            //TODO play fancy dying animation, making the hands fall down of weakness and then the world collapses
        }
    }
}
