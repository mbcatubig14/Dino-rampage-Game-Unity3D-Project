using UnityEngine;
using System.Collections;

public class DinoAttack : MonoBehaviour
{

	public float timeBetweenAttacks = 0.5f; // The time in seconds between each attack.
	public int attackDamage = 10;  // The amount of health taken away per attack.

	Animator anim;                              // Reference to the animator component.
	GameObject soldierController;                          // Reference to the player GameObject.
	SoldierHealth soldierHealth;                  // Reference to the player's health.
	DinoHealth dinoHealth;                    // Reference to this enemy's health.
	bool soldierInRange;                         // Whether player is within the trigger collider and can be attacked.
	float timer;                                // Timer for counting up to the next attack.

	void Awake ()
	{
		soldierController = GameObject.FindGameObjectWithTag ("Player");
		soldierHealth = soldierController.GetComponent<SoldierHealth>();
		dinoHealth = GetComponent<DinoHealth> ();
		anim = GetComponent<Animator> ();
	}

	void OnTriggerEnter (Collider other)
	{
		// If the entering collider is the soldier...
		if(other.gameObject == soldierController)
		{
			// ... the soldier is in range.
			soldierInRange = true;
		}
	}

	void OnTriggerExit (Collider other)
	{
		// If the exiting collider is the soldier...
		if(other.gameObject == soldierController)
		{
			// ... the soldier is no longer in range.
			soldierInRange = false;
		}
	}

	void Update ()
	{
		timer += Time.deltaTime;
		if (timer >= timeBetweenAttacks && soldierInRange && dinoHealth.currentHealth >0) {
			//Attack the player
			timer = 0f;// Reset the timer.
			if (soldierHealth.currentHealth > 0) {// If the soldier has health to lose...
				soldierHealth.TakeDamage (attackDamage); // ... damage the soldier.
				anim.SetTrigger ("Attack");
			}
		}
		 
		if (soldierHealth.currentHealth <= 0) { // If the soldier has zero or less health...
			anim.SetTrigger ("PlayerDead"); // ... tell the animator the soldier is dead.
		}
	}
}
