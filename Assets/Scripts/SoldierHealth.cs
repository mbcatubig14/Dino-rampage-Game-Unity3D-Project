using UnityEngine;
using UnityEngine.UI;
using System.Collections;


//This class handles the health of the soldier
public class SoldierHealth : MonoBehaviour {

	public int startingHealth = 1000;
	public int currentHealth;
	public Slider healthSlider;
	public Image damageImage;
	public float flashSpeed = 5f;
	public Color flashColour = new Color(1f,0f,0f,0.1f);

	Animator anim;
	SoldierMovement soldierMovement;
	bool damaged;

	void Awake () {
		anim = GetComponent<Animator> ();
		soldierMovement = GetComponent<SoldierMovement> ();
		currentHealth = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (damaged) {
			damageImage.color = flashColour;
		} else {
			damageImage.color = Color.Lerp (damageImage.color,Color.clear,flashSpeed * Time.deltaTime);		
		}
		damaged = false;
	}

	public void TakeDamage(int amount){
		bool isDead = false;
		damaged = true;
		currentHealth -= amount;
		healthSlider.value = currentHealth;

		if(currentHealth <= 0 && !isDead){
			//Death function
			anim.SetTrigger ("Die");
			soldierMovement.enabled = false;
		}
	}
}
