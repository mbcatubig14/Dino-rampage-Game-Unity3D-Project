using UnityEngine;

public class DinoManager : MonoBehaviour
{
	public SoldierHealth soldierHealth;       // Reference to the soldier's heatlh.
	public GameObject dino;                // The allosaurus' prefab to be spawned.
	public float spawnTime = 3f;            // How long between each spawn.
	public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.


	void Start ()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		InvokeRepeating ("Spawn", spawnTime, spawnTime);
	}


	void Spawn ()
	{
		// If the soldier has no health left...
		if(soldierHealth.currentHealth <= 0f)
		{
			// ... exit the function.
			return;
		}

		// Find a random index between zero and one less than the number of spawn points.
		int spawnPointIndex = Random.Range (0, spawnPoints.Length);

		// Create an instance of the allosaurus prefab at the randomly selected spawn point's position and rotation.
		Instantiate (dino, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation);
	}
}