using UnityEngine;
using System.Collections;

public abstract class FollowTarget : MonoBehaviour
{
	
	[SerializeField] public Transform target;
	[SerializeField] private bool autoTargetPlayer = true;

	public void FindTargetPlayer ()
	{
		if (target == null) {
			GameObject targetObject = GameObject.FindGameObjectWithTag ("Player");

			if (targetObject) {
				SetTarget (targetObject.transform);
			}
		}
	}

	public virtual void SetTarget (Transform newTransform)
	{
		target = newTransform;
	}

	public Transform Target{ get { return this.target; } }

	virtual protected void Start ()
	{
		if (autoTargetPlayer) {
			FindTargetPlayer ();
		}
	}

	protected abstract void Follow (float deltaTime);

	void FixedUpdate ()
	{
		if (autoTargetPlayer && (target == null || !target.gameObject.activeSelf)) {
			FindTargetPlayer ();
		}

		if (target != null && (target.GetComponent<Rigidbody> () != null && !target.GetComponent<Rigidbody> ().isKinematic)) {
			Follow (Time.deltaTime);
		}
	}
}
