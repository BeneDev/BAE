using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePowerup : MonoBehaviour {

    public Sprite Icon
    {
        get
        {
            return icon;
        }
    }

    [SerializeField] protected float duration = 5f;
    [SerializeField] protected float timeUntilActive = 1.5f;
    protected MeshRenderer rend;
    [SerializeField] protected GameObject particle;

    [Space]
    
    [SerializeField] float yMovement;
    [Range(0, 1)] float multiplier = 0f;
    [Range(0, 20)] [SerializeField] float period = 2f;
    protected Vector3 startPos;

    protected HandLeftController handLeft;
    protected HandRightController handRight;
    
    protected SphereCollider coll;

    [SerializeField] AudioSource powerupSound;

    [SerializeField] protected Sprite icon;

    private void Awake()
    {
        coll = GetComponent<SphereCollider>();
        rend = GetComponent<MeshRenderer>();
        Invoke("EnableCollider", timeUntilActive);
        startPos = transform.position;
    }

    private void Update()
    {
        //prevent dividing by 0
        if (period <= Mathf.Epsilon) // Epsilon is the tiniest value a float can have so dont compare floats to zero (too inconsistent), but compare them to epsilon and never as "==" rather than "<="
        {
            return;
        }

        float cycles = Time.time / period; // determines how far into the sin wave we are

        const float tau = Mathf.PI * 2; // about 6.28
        float rawSinWave = Mathf.Sin(cycles * tau); // goes from -1 to +1

        multiplier = rawSinWave / 2f + 0.5f; // goes from 0 to +1

        transform.position = startPos + new Vector3(0f, yMovement * multiplier, 0f); // adds the current movement to the object's transform
    }

    protected void EnableCollider()
    {
        coll.enabled = true;
    }

    protected virtual void Use()
    {
        if(!powerupSound.isPlaying)
        {
            powerupSound.Play();
        }
        Destroy(rend);
        Destroy(coll);
        Destroy(particle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "HandLeft")
        {
            handLeft = other.gameObject.GetComponent<HandLeftController>();
            if(handLeft.CanKill)
            {
                Use();
            }
        }
        else if(other.gameObject.tag == "HandRight")
        {
            handRight = other.gameObject.GetComponent<HandRightController>();
            if(handRight.CanKill)
            {
                Use();
            }
        }
    }
}
