//Nestor Saavedra
//11/16/2017

using UnityEngine;
using System.Collections;


//Still really unsure about this particular one

public class TrafficController : MonoBehaviour {
	public float baseTrafficSpeedMultiplier = 2.0f; //
	public GameController gameController;

	private Driver[] drivers;
	private int[][] driversAndVehicles; //Array being int is placeholder for now
	private bool gameActive = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if (this.gameActive) {

		}
	
	}

	#region From GameController
	public void decreaseTrafficSpeed(uint pickupLevel) {
		//TO-DO: Make this actually do something
	}

	public void increaseTrafficSpeed(uint pickupLevel) {
		//TO-DO: Make this actually do something

	}

	public void resetTrafficSpeed() {
		//TO-DO: Make this actually do something
	}

	//Turns traffic spawning on/off
	public void enableTrafficSpawning(bool spawnTraffic) {
		this.gameActive = spawnTraffic;
	}

	#endregion
}
