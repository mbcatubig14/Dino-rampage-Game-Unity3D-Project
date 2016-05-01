using UnityEngine;
using System.Collections;

public class DinoMovement : MonoBehaviour
{

	Transform soldier;
	SoldierHealth soldierHealth;
	DinoHealth dinoHealth;
	NavMeshAgent nav;

	void Awake ()
	{
		soldier = GameObject.FindGameObjectWithTag ("Player").transform;
		soldierHealth = soldier.GetComponent<SoldierHealth> ();
		dinoHealth = GetComponent<DinoHealth> ();
		nav = GetComponent<NavMeshAgent> ();
	}

	void Update ()
	{
		if (dinoHealth.currentHealth > 0 && soldierHealth.currentHealth > 0) {
			nav.SetDestination (soldier.position);
		} else {
			nav.enabled = false;
		}

	}
}
