//Created by Nestor Saavedra
//Last edited: 11/14/2017

using UnityEngine;
using System.Collections;

//GameController handles
//***The Game Settings
//***Handles communication between the active game components i.e. (EventController, PickupController, TrafficController...)
//***Links movement controls of PlayerController menu items in UIController when needed.

[System.Serializable]
public class GameSettings
{
	public uint baseScoreValue;
}

public class GameController : MonoBehaviour
{
	public GameSettings settings;
	
	public PickupController pickupController;
	public EventController eventController;
	public TrafficController trafficController;
	public StatController statController;
	public UIController uiController;
	public PlayerController playerController;
	public ScoreController scoreController;



	private bool isPlayerAlive;
	//Check if player is alive
	private bool isGameActive = false;

	// initialization
	void Start ()
	{
		//Load StatController data
		this.statController.loadFromDisk(); //do this to prevent statistics from being changed

		this.startGame();
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void startGame() {
		if (!this.isGameActive) {
			this.resetAllComponents ();
			//ScoreController: count score
			this.scoreController.enableCountScore (true);
			//TrafficController: spawn traffic
			this.trafficController.enableTrafficSpawning (true);
			//PickupController: spawn pickups
			this.pickupController.enablePickupSpawning (true);
			//StatController:  save data
			this.statController.setGameActive(true);
			//StatController: count games played
			this.statController.increaseTotalGameCount ();
			this.isGameActive = true;
		}
	}

	// display a game over message
	public void gameOver() {

	}

	public void endGame() {
		if (this.isGameActive) {
			//ScoreController: stop counting score
			this.scoreController.enableCountScore (false);
			//TrafficController: stop spawning traffic
			this.trafficController.enableTrafficSpawning (false);
			//PickupController: stop spawning pickups
			this.pickupController.enablePickupSpawning (false);
			//StatController: game ends
			this.statController.setGameActive(false);
			this.isGameActive = false;
		}

	}

	//used before restartGame() and startGame() 
	private void resetAllComponents() {
		//Reset the score to 0 
		//this.statController.resetScore(); DONE EVERY TIME A GAME IS ENDED from the StatController.setGameActive(false) method. 
		//Reset score multiplier/PickupLevels to 0, Reset pickups
		this.endAllPickups();
	}


	public void pauseGame(bool pausedAlready) {

	}


	#region Settings

	public uint getBaseScoreValue ()
	{
		return this.settings.baseScoreValue;
	}

	#endregion

	#region From PickupController

	//A pickupLevel of ZERO deactivates a pickup, and is the only way PickupController can tell GameController to deactivate one
	//Any changes to pickups will be sent to StatController from this command.
	public void activatePickup (PickupController.PickupType pickup, uint pickupLevel)
	{ 
		
		//check if level is 0, if it is, deactivate pickup
		if (pickupLevel == 0)
			this.endPickup (pickup);

		//It even lets statController know the pickup has ended too, so the UI will be updated automatically from here
		//This will still be used even when the pickupLevel is 0, since a pickupLevel of 0 sent to StatCollector is 
		//ignored in stat totals but will cause the UI to update

		switch (pickup) {
		case PickupController.PickupType.CarDestroyer: 
			//Needs Implementation, but only if pickupLevel > 0
			this.tellStatControllerOfPickupChanges (PickupController.PickupType.CarDestroyer, pickupLevel);
			break;
		case PickupController.PickupType.DecreasePlayerSpeed: //To PlayerController
			if (pickupLevel > 0) 
				this.playerController.decreasePlayerSpeed (pickupLevel);
			this.tellStatControllerOfPickupChanges (PickupController.PickupType.DecreasePlayerSpeed, pickupLevel);
			break;
		case PickupController.PickupType.DecreaseTrafficSpeed: //To TrafficController
			if (pickupLevel > 0)
				this.trafficController.decreaseTrafficSpeed (pickupLevel);
			this.tellStatControllerOfPickupChanges (PickupController.PickupType.DecreaseTrafficSpeed, pickupLevel);
			break;
		case PickupController.PickupType.IncreasePlayerSpeed: //To PlayerController
			if (pickupLevel > 0)
				this.playerController.increasePlayerSpeed (pickupLevel);
			this.tellStatControllerOfPickupChanges (PickupController.PickupType.IncreasePlayerSpeed, pickupLevel);
			break;
		case PickupController.PickupType.IncreaseTrafficSpeed: //To TrafficController
			if (pickupLevel > 0)
				this.trafficController.increaseTrafficSpeed (pickupLevel);
			this.tellStatControllerOfPickupChanges (PickupController.PickupType.IncreaseTrafficSpeed, pickupLevel);
			break;
		case PickupController.PickupType.ScoreMultiplier: //To ScoreController
			this.scoreController.setMultiplier (pickupLevel); 
			this.tellStatControllerOfPickupChanges (PickupController.PickupType.ScoreMultiplier, pickupLevel);
			break;
		default:
			break;
		}
	}
	//Main usage would probably be to restart the game
	private void endAllPickups ()
	{
		//I feel like this should do its own thing since this is just PickupController telling GameController to tell PickupController
		//to call GameController 6 times.
		//HOWEVER, this function is only here for when games are ending so perhaps I should call this one "EndGame" or something.
		//We can figure this out later.
		//If this is endGame then it needs to tell StatController to save its stats and reset scores and stuff.
		//But there's already a function that does this.
		//This would be more like QUIT game.

		this.pickupController.resetAllPickupLevels ();
	}

	//Let StatController know about it
	//Private, called when activatePickup receives a level of 0
	//ONLY undoes effects
	private void endPickup (PickupController.PickupType pickup)
	{
		switch (pickup) {
		case PickupController.PickupType.CarDestroyer:
			break;
		case PickupController.PickupType.DecreasePlayerSpeed:
			this.playerController.resetPlayerSpeed ();
			break;
		case PickupController.PickupType.DecreaseTrafficSpeed:
			this.trafficController.resetTrafficSpeed ();
			break;
		case PickupController.PickupType.IncreasePlayerSpeed:
			this.playerController.resetPlayerSpeed ();
			break;
		case PickupController.PickupType.IncreaseTrafficSpeed:
			this.trafficController.resetTrafficSpeed ();
			break;
		//case PickupController.PickupType.ScoreMultiplier:
		//	this.scoreController.setMultiplier (0);
		//	break;
		//^^^This is omitted because activatePickup(0) works on its own on ScoreMultiplier
		default:
			break;
		}
	}

	#endregion

	//communicate with stat controller
	public void addScorePoints(uint points) {
		this.statController.increaseScoreCount (points);
	}


	public void increaseTotalGameTimeByOneMinute() {
		this.statController.increaseGameTime ();
	}

	//For Updating UI keeping game time
	public void reportGameTime(uint minutes, uint seconds, uint mSeconds) {
		this.statController.updateGameTime (minutes, seconds, mSeconds);


	}
	#region To StatController

	private void tellStatControllerOfPickupChanges(PickupController.PickupType pickup, uint level) {
		this.statController.changeCurrentPickupLevel (pickup, level);
	}
	//EventController
	#endregion


	#region For StatController
	#endregion

}
