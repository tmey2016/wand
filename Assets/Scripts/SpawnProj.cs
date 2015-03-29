using UnityEngine;
using System.Collections;
using Vuforia;

public class SpawnProj : MonoBehaviour 
{

	RaycastHit hit;
	public GameObject projectile;
	public Transform spawnPosition;
	[HideInInspector]
	public int currentProjectile = 0;
	Vector3 viewportDestination = new Vector3(0.5f, 0.9f, 100);
	public GameObject lockTarget;

	public void FireProjectile()
	{
		GameObject projSpawn = Instantiate (projectile, spawnPosition.position, Quaternion.identity) as GameObject;
		Vector3 destination = Camera.main.ViewportToWorldPoint(viewportDestination);
		Vector3 projPath = destination - spawnPosition.position;
		lockTarget.GetComponent<ImageTargetBehaviour> ().enabled = false;
		projSpawn.rigidbody.AddForce ((projPath/projPath.magnitude) * -20000);
	}
	void Update()
	{

	}
}
