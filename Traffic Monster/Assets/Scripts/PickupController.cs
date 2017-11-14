//Nestor Saavedra
//11/16/2017

using UnityEngine;
using System.Collections;

//When a pickup is collected, use activatePickup()

[System.Serializable]
public class PickupSpawnBoundary
{
	public float xMin, xMax, zMin, zMax;
	public GameObject ground;
}


//Check if spawn location is too close

public class PickupController : MonoBehaviour
{

	private float testFrameLimit = 0;


	public GameController gameController;
	public GameObject pickupSpawnList;

	public float spawnInterval;

	[System.Serializable]
	public class PickupProbabilities
	{
		//This works by having an array of PickupType with a defined size, with the idea being that the size is 100 / probArrayBlockValue.
		public float pickupSpawnProbability = 0.2f;
		public PickupController.PickupType[] probabilityArray = new PickupController.PickupType[25];
		public uint probArrayBlockValue = 4;


		public uint getPickupProbability (PickupController.PickupType pickup)
		{
			uint probability = 0;
			foreach (PickupController.PickupType probCount in this.probabilityArray) {
				if ((int)probCount == (int)pickup) {
					probability += probArrayBlockValue;
				}
			}
			return probability;

		}



	}

	public PickupProbabilities pickupProbabilities;

	[System.Serializable]
	public class PickupObjects
	{
		public GameObject decreaseTrafficSpeed;
		public GameObject increaseTrafficSpeed;
		public GameObject increasePlayerSpeed;
		public GameObject decreasePlayerSpeed;
		public GameObject scoreMultiplier;
		public GameObject carDestroyer;

	}

	public PickupObjects pickupObjects;


	public enum PickupType
	{
		DecreaseTrafficSpeed = 0,
		IncreaseTrafficSpeed = 1,
		IncreasePlayerSpeed = 2,
		DecreasePlayerSpeed = 3,
		ScoreMultiplier = 4,
		CarDestroyer = 5,

	};


	//Array containing the current levels, the array index corresponds to the value of each enum type
	private uint[] pickupLevels = new uint[6];
	//uint because levels never go below 0


	//Pickups should not spawn outside of this boundary.
	public PickupSpawnBoundary boundary;


	private bool gameActive = false;



	//initialization
	void Start ()
	{
		//Fills pickupLevels array for first use
		for (int i = 0; i <= 5; i++) {
			this.pickupLevels [i] = 0;
		}
		InvokeRepeating ("spawnPickup", this.spawnInterval, this.spawnInterval);
	}
	
	// Update once per frame
	void Update ()
	{
		
	}


	void spawnPickup ()
	{
		//Want X and Z to be random, with Y being the Y position of the GROUND object + an interval that makes it not clip through the ground
		//Generate random X and Z from the SpawnBoundary information
		//Then feed this information to the second parameter in the instantiate method
		//This should allow pickups to spawn around the playfield correctly.
		if (this.gameActive) {
			if (this.shouldSpawnPickup ()) {
				PickupType pickupType = this.getRandomPickupType ();
				Transform parent = this.pickupSpawnList.transform;
				Vector3 spawnLocation = new Vector3 (
					                       Random.Range (this.boundary.xMin, this.boundary.xMax),
					                       this.boundary.ground.transform.position.y + 0.47f,
					                       Random.Range (this.boundary.zMin, this.boundary.zMax)
				                       );
				switch (pickupType) {
				case PickupType.CarDestroyer:
					Instantiate (this.pickupObjects.carDestroyer, spawnLocation, Quaternion.identity, parent);
					break;
				case PickupType.DecreasePlayerSpeed:
					Instantiate (this.pickupObjects.decreasePlayerSpeed, spawnLocation, Quaternion.identity, parent);
					break;
				case PickupType.DecreaseTrafficSpeed:
					Instantiate (this.pickupObjects.decreaseTrafficSpeed, spawnLocation, Quaternion.identity, parent);
					break;
				case PickupType.IncreasePlayerSpeed:
					Instantiate (this.pickupObjects.increasePlayerSpeed, spawnLocation, Quaternion.identity, parent);
					break;
				case PickupType.IncreaseTrafficSpeed:
					Instantiate (this.pickupObjects.increaseTrafficSpeed, spawnLocation, Quaternion.identity, parent);
					break;
				case PickupType.ScoreMultiplier:
					Instantiate (this.pickupObjects.scoreMultiplier, spawnLocation, Quaternion.identity, parent);
					break;
				}
				Debug.LogWarning ("Don't forget to delete this part below");
				this.activatePickup (pickupType);
			}
		}
	}

	private bool shouldSpawnPickup ()
	{
		return (Random.Range (0, 100) <= this.pickupProbabilities.pickupSpawnProbability * 100 );// ? true: false;
	}

	private void increasePickupLevel (PickupType pickup)
	{
		this.pickupLevels [(int)pickup] += 1;
		//Notify GameController about the change
		this.gameController.activatePickup (pickup, this.pickupLevels [(int)pickup]);
	}

	private void resetPickupLevel (PickupType pickup)
	{
		this.pickupLevels [(int)pickup] = 0;
		this.gameController.activatePickup (pickup, this.pickupLevels [(int)pickup]);
	}

	public void resetAllPickupLevels ()
	{ //Is public only so GameController can reset pickup levels for a new game
		//Start at 0, iterate to highest number in enum

		this.pickupLevels [(int)PickupType.CarDestroyer] = 0;
		this.gameController.activatePickup (PickupType.CarDestroyer, 0); //have to let the GameController know about it too

		this.pickupLevels [(int)PickupType.DecreasePlayerSpeed] = 0;
		this.gameController.activatePickup (PickupType.DecreasePlayerSpeed, 0);

		this.pickupLevels [(int)PickupType.DecreaseTrafficSpeed] = 0;
		this.gameController.activatePickup (PickupType.DecreaseTrafficSpeed, 0);

		this.pickupLevels [(int)PickupType.IncreasePlayerSpeed] = 0;
		this.gameController.activatePickup (PickupType.IncreasePlayerSpeed, 0);

		this.pickupLevels [(int)PickupType.IncreaseTrafficSpeed] = 0;
		this.gameController.activatePickup (PickupType.IncreaseTrafficSpeed, 0);

		this.pickupLevels [(int)PickupType.ScoreMultiplier] = 0;
		this.gameController.activatePickup (PickupType.ScoreMultiplier, 0);
	}


	//called by children processes ONLY unless otherwise needed
	public void activatePickup (PickupType pickup)
	{
		//Increment pickup level by 1
		//Opposing pickup types (like Traffic Up/Down) need to reset the opposite count to 0 when picked up
		//goto default; line is required for pickup level to increment
		/// endPickup by using a level of 0 when using GameController.activatePickup
		switch (pickup) {
		case PickupType.IncreasePlayerSpeed:
			this.resetPickupLevel (PickupType.DecreasePlayerSpeed); //Resets DecreasePlayerSpeed to 0
			this.gameController.activatePickup (PickupType.DecreasePlayerSpeed, 0);
			goto default;
		case PickupType.DecreasePlayerSpeed:
			this.resetPickupLevel (PickupType.IncreasePlayerSpeed);
			this.gameController.activatePickup (PickupType.IncreasePlayerSpeed, 0);
			goto default;
		case PickupType.IncreaseTrafficSpeed:
			this.resetPickupLevel (PickupType.DecreaseTrafficSpeed);
			this.gameController.activatePickup (PickupType.DecreaseTrafficSpeed, 0);
			goto default;
		case PickupType.DecreaseTrafficSpeed:
			this.resetPickupLevel (PickupType.IncreaseTrafficSpeed);
			this.gameController.activatePickup (PickupType.IncreaseTrafficSpeed, 0);
			goto default;
		default:
			this.increasePickupLevel (pickup);
			break;
		}

		//Applies Pickup via GameController
		gameController.activatePickup (pickup, pickupLevels [(int)pickup]);
	}


	public void enablePickupSpawning (bool isSpawning)
	{
		this.gameActive = isSpawning;
	}

	private PickupController.PickupType getRandomPickupType ()
	{
		//Needs to generate based on probabilities from PickupController.PickupProbabilities
		//To generate probability:
		//Add all probability numbers together
		//Divide that by 6
		//Multiply each number individually by the /6 number to get the number each ratio point is worth 
		uint[] probabilities = new uint[6];
		uint[] probMin = new uint[6];
		uint[] probMax = new uint[6];

		//Fill probabilities array
		for (int i = 0; i < 6; i++) {
			probabilities [i] = this.pickupProbabilities.getPickupProbability ((PickupController.PickupType)i);
		}
		//max starts out as 0
		//min = max + 1
		//max = (min - 1) + prob
		uint max = 0;
		for (int i = 0; i < 6; i++) {
			probMin [i] = max + 1;
			probMax [i] = (probMin [i] - 1) + probabilities [i];
			max = probMax [i];

		}

		//generate a random number, and from that equal the #i to the int of the PickupType enum
		//For each range in i.
		int randomNumber = Random.Range (1, 100);
		for (int i = 0; i < 6; i++) {
			if (randomNumber >= probMin [i] && randomNumber <= probMax [i]) {
				return (PickupController.PickupType)i;
			}
		}
		//show error
		Debug.LogError ("PICKUP ERROR");
		return (PickupController.PickupType)(Random.Range (0, 5));
	}
}
