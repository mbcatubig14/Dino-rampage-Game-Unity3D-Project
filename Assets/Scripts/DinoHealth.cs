using UnityEngine;
using System.Collections;

//Handles the Dinosaur's Health
public class DinoHealth : MonoBehaviour {

	public int startingHealth = 100;
	public int currentHealth;
	public float sinkSpeed = 2.5f;
	public int scoreValue = 10;

	Animator anim;
	CapsuleCollider capsuleCollider;
	bool isDead;
	bool isSinking;

	void Awake () {
		anim = GetComponent<Animator> ();
		capsuleCollider = GetComponent<CapsuleCollider> ();
		currentHealth = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if(isSinking){
			transform.Translate (-Vector3.up * sinkSpeed * Time.deltaTime);
		}
	}

	//Used by the Soldier when shooting the reduces the health of the dinosaur until it dies
	public void TakeDamage(int amount, Vector3 hitPoint){
		if (isDead)
			return;
		currentHealth -= amount;

		if(currentHealth <=0){

			//Death Part
			isDead = true;

			capsuleCollider.isTrigger = true;
			anim.SetTrigger ("Dead");
		}
	}
		

	//Used by the Dinosaur's Animation Event when dying
	public void StartSinking(){
		GetComponent<NavMeshAgent> ().enabled = false;
		GetComponent<Rigidbody> ().isKinematic = true;
		isSinking = true;

		ScoreManager.score += scoreValue;

		Destroy (gameObject, 2f);
	}
}
