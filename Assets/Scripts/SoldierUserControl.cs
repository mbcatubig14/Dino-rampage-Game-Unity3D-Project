using UnityEngine;
using System.Collections;

public class SoldierUserControl : MonoBehaviour
{

	public bool walkByDefault = false;
	//If we want to walk by default

	private SoldierMovement soldierMove;
	//reference to the soldier movement script
	private Transform cam;
	//reference to the camera object transform
	private Vector3 camForward,move;
	//stores the forward vector of the cam

	public bool aim;
	//if we are aiming
	public float aimingWeight;
	//the aiming weight

	public bool lookInCameraDirection;
	// if we want the soldier to look at the same direction as the camera
	Vector3 lookPos;
	//the looking position

	Animator anim;

	public ParticleSystem particleSys;

	public Transform spine;
	//the bone where we rotate the body of the soldier from
	//The Z/x/y values, doesn't really matter the values here since we ovveride them depending on the weapon
	public float aimingZ = 0;
	public float aimingX = 0;
	public float aimingY = 60.0f;
	//The point in the ray we do from the camera, basically how far the character looks
	public float point = 30;

	public int damagePerShot = 20;
	// The damage inflicted by each bullet.
	     
	void Start ()
	{
		if (Camera.main != null) {
			cam = Camera.main.transform;
		}
		soldierMove = GetComponent<SoldierMovement> ();

		anim = GetComponent<Animator> ();

	}

	float horizontal;
	float vertical;

	void FixedUpdate ()
	{
		//our connection with the variables and our Input
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");
		//if we are not aiming
		if (!aim) {
			if (cam != null) { //if there is a camera
				//Take the forward vector of the camera (from its transform) and 
				// eliminate the y component
				// scale the camera forward with the mask (1, 0, 1) to eliminate y and normalize it
				camForward = Vector3.Scale (cam.forward, new Vector3 (1, 0, 1)).normalized;

				//move input front/backward = forward direction of the camera * user input amount (vertical)
				//move input left/right = right direction of the camera * user input amount (horizontal)
				move = vertical * camForward + horizontal * cam.right;
			} else {
				//if there is not a camera, use the global forward (+z) and right (+x)
				move = vertical * Vector3.forward + horizontal * Vector3.right;
			}
		} else { //but if we are aiming
			//we pass a zero to the move input
			move = Vector3.zero;

			//we make the character look where the camera is looking
			Vector3 dir = lookPos - transform.position;
			dir.y = 0;
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (dir), 20 * Time.deltaTime);

			//and we directly manipulate the animator
			//this works because we've set up from our other script
			//to take every movement in the animator and convert it to a force to be applied to the rigidbody
			anim.SetFloat ("Forward", vertical);
			anim.SetFloat ("Turn", horizontal);
		}

		if (move.magnitude > 1) //Make sure that the movement is normalized
			move.Normalize ();

		bool walkToggle = Input.GetKey (KeyCode.LeftShift) || aim; //check for walking input or aiming input

		//the walk multiplier determines if the character is running or walking
		//if walkByDefault is set and walkToggle is pressed
		float walkMultiplier = 1;

		if (walkByDefault) {
			if (walkToggle) {
				walkMultiplier = 1;
			} else {
				walkMultiplier = 0.5f;
			}
		} else {
			if (walkToggle) {
				walkMultiplier = 0.5f;
			} else {
				walkMultiplier = 1;
			}
		}

		//look position depends on if the character look towards the camera or not
		lookPos = lookInCameraDirection && cam != null ? transform.position + cam.forward * 100 : transform.position + transform.forward * 100;

		//apply the multiplier to our move input
		move *= walkMultiplier;

		//pass it to move function from our character movement script
		soldierMove.Move (move, aim, lookPos);
	}

	void Update ()
	{
		aim = Input.GetMouseButton (1);

		if (aim) {
			if (Input.GetButton ("Fire1")) {
				anim.SetTrigger ("Fire");
				particleSys.Emit (1);
				transform.GetComponentInChildren<AudioSource> ().Play ();
				ShootRay ();
			}
		}
	}

	public GameObject bulletPrefab;
	public Transform bulletSpawn;

	//Shoots a ray everytime it shoots
	void ShootRay ()
	{
		
		//find the center of the screen
		float x = Screen.width / 2;
		float y = Screen.height / 2;

		//and make a ray from it
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (x, y, 0));
		RaycastHit hit;

		//Instantiate the bullet prefab that has a line render and store it in a variable
		GameObject go = Instantiate (bulletPrefab, transform.position, Quaternion.identity) as GameObject;
		LineRenderer line = go.GetComponent<LineRenderer> ();

		//the first position of or "bullet" will be the bullet spawn point
		//of our active weapon, converted from local to world position
		Vector3 startPos = bulletSpawn.TransformPoint (Vector3.zero);
		Vector3 endPos = Vector3.zero;

		//bit shift a layer mask
		int mask = ~(1 << 8);

		//so that raycast collides with all the colliders in all the layers, except the one masked
		if (Physics.Raycast (ray, out hit, Mathf.Infinity, mask)) {
			//find the distance between the bullet spawn position and the hit.point
			float distance = Vector3.Distance (bulletSpawn.transform.position, hit.point);

			//and raycast everything in that direction and for that distance
			RaycastHit[] hits = Physics.RaycastAll (startPos, hit.point - startPos, distance);

			//and for every hit
			foreach (RaycastHit hit2 in hits) {
				if (hit2.transform.GetComponent<Rigidbody> ()) {
					//then apply the appropriate force at the correct direction
					Vector3 direction = hit2.transform.position - transform.position;
					direction = direction.normalized;
					hit2.transform.GetComponent<Rigidbody> ().AddForce (direction * 200);
				}
			}

			// Try and find an EnemyHealth script on the gameobject hit.
			DinoHealth enemyHealth = hit.collider.GetComponent <DinoHealth> ();

			// If the EnemyHealth component exist...
			if (enemyHealth != null) {
				// ... the enemy should take damage.
				enemyHealth.TakeDamage (damagePerShot, hit.point);
			}

			//the end position of our bullet is the hit.point
			endPos = hit.point;
		} else { //else if the raycast didn't hit anything
			//the end position will be a far away point upon the ray
			endPos = ray.GetPoint (100);
		}

		//set up the positions to the line renderer
		line.SetPosition (0, startPos);
		line.SetPosition (1, endPos);
	}

	void LateUpdate ()
	{
		//aim = Input.GetMouseButton (1);
		//our aiming weight smoothly becomes 0 or 1 depending if we are aiming or not, 
		aimingWeight = Mathf.MoveTowards (aimingWeight, (aim) ? 1.0f : 0.0f, Time.deltaTime * 5);

		//the normal and aiming state of the camera, basically how much close to the player it is
		Vector3 normalState = new Vector3 (0, 0, -2f);
		Vector3 aimingState = new Vector3 (0, 0, -0.5f);

		//and that is lerped depending on t = aimigweight
		Vector3 pos = Vector3.Lerp (normalState, aimingState, aimingWeight);
		cam.transform.localPosition = pos;

		if (aim) { //if we aim
			//pass the new rotation to the IK bone
			Vector3 eulerAngleOffset = Vector3.zero;
			eulerAngleOffset = new Vector3 (aimingX, aimingY, aimingZ);

			//do a ray from the center of the camera and forward
			Ray ray = new Ray (cam.position, cam.forward);

			//find where the character should look
			Vector3 lookPosition = ray.GetPoint (point);

			//and apply the rotation to the bone
			spine.LookAt (lookPosition);
			spine.Rotate (eulerAngleOffset, Space.Self);
		}
	}
}
