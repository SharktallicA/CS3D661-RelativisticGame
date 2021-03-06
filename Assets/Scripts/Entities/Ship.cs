﻿using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Enumeration os possible speeds the engine can support.
/// </summary>
public enum SpeedFactor { Reverse = -16, Zero = 0, One = 256, Two = 32, Three = 4, Full = 1 };

/// <summary>
/// Body-subclass for scene ship objects.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
class Ship : Body
{
    /// <summary>
    /// Maximum speed of ship when at SpeedFactor.Full.
    /// </summary>
    [SerializeField]
    private float maxSpeed = 250f;

    /// <summary>
    /// Allows the ship part rotation speed to be multiplied.
    /// </summary>
    [SerializeField]
    private float rotationModifier = 24f;

    /// <summary>
    /// Speed factor that the ship is set to use.
    /// </summary>
    private SpeedFactor speedFactor = SpeedFactor.Zero;

    /// <summary>
    /// Distance in which asteroid's mass are counted as yours.
    /// </summary>
    [SerializeField]
    private float collectiveMassDist = 2f;

    /// <summary>
    /// Internal reference to relativistics UI display.
    /// </summary>
    private GameObject relativisticsMetre;

    /// <summary>
    /// Internal reference to all parts of the ship that can rotate.
    /// </summary>
    private Rotator[] rotatables;

    /// <summary>
    /// Calculated collection of mass from nearby objects;
    /// </summary>
    private float localisedMass = 0f;

    private void Start()
    {
        // Get relativistics UI reference
        relativisticsMetre = GameObject.FindGameObjectWithTag("Relativisticsmetre");
        // Find all rotatable ship parts
        rotatables = FindObjectsOfType<Rotator>();
    }

    private void Update()
    {
        // Gradually accelerate/decelerate the ship to the desired speed
        speed = Mathf.Lerp(speed, GetSpeedTarget(), 0.01f);

        // Ensure all rotatable ship parts turn at the proper speed
        for (int i = 0; i < rotatables.Length; i++)
            rotatables[i].SetSpeed(speed * rotationModifier);

        // Update all relativistic UI display readouts
        if (relativisticsMetre)
        {
            relativisticsMetre.transform.GetChild(0).GetComponent<Text>().text = GetSpeedKMS() + "km/s";
            relativisticsMetre.transform.GetChild(1).GetComponent<Text>().text = GetGrav() + "N";
            relativisticsMetre.transform.GetChild(2).GetComponent<Text>().text = GetMass() * 1000 + "kg";
            relativisticsMetre.transform.GetChild(3).GetComponent<Text>().text = GetLength() * 100 + "m";
        }
    }

    /// <summary>
    /// Allows this Ship to be destroyed externally.
    /// </summary>
    /// <param name="cause">Message of destruction cause for debugging.</param>
    public override void DestroyBody(string cause = "nothing")
    {
        GameObject.FindGameObjectsWithTag("MainCamera")[0].SetActive(false);
        GameObject.FindGameObjectsWithTag("MainCanvas")[0].GetComponent<CanvasGroup>().blocksRaycasts = GameObject.FindGameObjectsWithTag("MainCanvas")[0].GetComponent<CanvasGroup>().interactable = false;
        GameObject.FindGameObjectsWithTag("MainCanvas")[0].GetComponent<CanvasGroup>().alpha = 0;
        GameObject.FindGameObjectsWithTag("DeathCanvas")[0].GetComponent<CanvasGroup>().blocksRaycasts = GameObject.FindGameObjectsWithTag("DeathCanvas")[0].GetComponent<CanvasGroup>().interactable = true;
        GameObject.FindGameObjectsWithTag("DeathCanvas")[0].GetComponent<CanvasGroup>().alpha = 1;

        base.DestroyBody();
    }

    ///// <summary>
    ///// Returns the object's current mass along with localised
    ///// mass from nearby asteroids.
    ///// </summary>
    //public override float GetMass()
    //{
    //    localisedMass = rb.mass;
    //    Collider[] nearby = Physics.OverlapSphere(transform.position, Mathf.Pow(collectiveMassDist, 2));
        
    //    for (int i = 0; i < nearby.Length; i++)
    //    {
    //        if (nearby[i].transform.GetComponent<Asteroid>())
    //            localisedMass += nearby[i].transform.parent.GetComponent<Asteroid>().GetMass();
    //    }

    //    return localisedMass;
    //}

    /// <summary>
    /// Returns ship's target speed (as SpeedFactor setting).
    /// </summary>
    public SpeedFactor GetSpeedFactor() { return speedFactor; }

    /// <summary>
    /// Returns ship's target speed (as calculated decimal).
    /// </summary>
    public float GetSpeedTarget()
    {
        if (speedFactor == SpeedFactor.Zero) return 0f;
        else return maxSpeed / (int)speedFactor;
    }

    /// <summary>
    /// Accepts and sets the ship's SpeedFactor setting.
    /// </summary>
    /// <param name="nSpeed">New SpeedFactor setting.</param>
    public void SetSpeedFactor(SpeedFactor nSpeed)
    {
        speedFactor = nSpeed;
        DampenInertia();
    }

    /// <summary>
    /// Allows the collective mass to be visisble in inspector.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 1);
        Gizmos.DrawSphere(transform.position, collectiveMassDist);
    }
}