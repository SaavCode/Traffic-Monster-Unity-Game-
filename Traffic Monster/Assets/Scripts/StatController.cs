//Nestor Saavedra
//11/16/2017

using UnityEngine;
using System.Collections;

//ALL PICKUP COUNTING FUNCTIONS COMPLETE
//StatController calls upon UIController to update.


#region Information about StatController
//The Score multiplier is set based on 'levels', with 0 being a 1x multiplier (no multiplier), and each number after that being
//an increase of .5

//
#endregion


	

public class StatController : MonoBehaviour
{
	public GameController gameController; 
	public UIController uiController;
	public XMLSaveParser saveParser;

	public float saveIntervalInSeconds = 10.0f; //A value of Zero disables autosave
	private uint defaultUserId = 0;
	public string defaultUserName = "default";

	//private bool isGameActive = false;

	//Top Level Structure of CurrentGameStatistics
	private CurrentGameStatistics currentStats = new CurrentGameStatistics ();
	//Pickup Levels for Current Game
	private CurrentGameStatistics.PickupLevels currentPickupLevels = new CurrentGameStatistics.PickupLevels ();

	//TLS for Long Term Statistics, includes general stats
	private LongTermStatistics generalLongTermStats;
	//Traffic Statistics
	private LongTermStatistics.TrafficStats trafficStats;
	//Statistics that include ALL Pickup totals as one
	private LongTermStatistics.PickupCounts allPickupStats;
	//Stats for CarDestroyer pickup
	private LongTermStatistics.PickupCounts.CarDestroyer pickupStatsCarDestroyer;
	//Stats for DecreasePlayerSpeed pickup
	private LongTermStatistics.PickupCounts.DecreasePlayerSpeed pickupStatsDecreasePlayerSpeed;
	//Stats for DecreaseTrafficSpeed pickup
	private LongTermStatistics.PickupCounts.DecreaseTrafficSpeed pickupStatsDecreaseTrafficSpeed;
	//Stats for IncreasePlayerSpeed pickup
	private LongTermStatistics.PickupCounts.IncreasePlayerSpeed pickupStatsIncreasePlayerSpeed;
	//Stats for IncreaseTrafficSpeed pickup
	private LongTermStatistics.PickupCounts.IncreaseTrafficSpeed pickupStatsIncreaseTrafficSpeed;
	//Stats for ScoreMultiplier pickup
	private LongTermStatistics.PickupCounts.ScoreMultiplier pickupStatsScoreMultiplier;

	private struct CurrentGameStatistics
	{
		public float scoreMultiplier;
		public uint score;

		public struct PickupLevels
		{
			public uint decreaseTrafficSpeed;
			public uint increaseTrafficSpeed;
			public uint increasePlayerSpeed;
			public uint decreasePlayerSpeed;
			public float scoreMultiplier;
			public uint carDestroyer;

		}
	}

	public struct LongTermStatistics
	{
 		//Used by XMLSaveParser to control access to data portions
		//Check XMLSaveParser file for more information.
		public enum Stats
		{
			TotalGamePoints,
			TotalGameTimeInMinutes,
			TotalPickupsCollected,
			TotalLevelUps,
			TotalRandomEvents,
			TotalDeaths,
			PickupCountDecreaseTrafficSpeed,
			PickupCountIncreaseTrafficSpeed,
			PickupCountIncreasePlayerSpeed,
			PickupCountDecreasePlayerSpeed,
			PickupCountScoreMultiplier,
			PickupCountCarDestroyer,
			MaxLevelDecreaseTrafficSpeed,
			MaxLevelIncreaseTrafficSpeed,
			MaxLevelIncreasePlayerSpeed,
			MaxLevelDecreasePlayerSpeed,
			MaxLevelScoreMultiplier,
			MaxLevelCarDestroyer,
		};

		public ulong totalGamePoints; 
		public uint totalGameTimeInMinutes; 
		public uint totalGamesPlayed; 
		public uint totalRandomEvents;
		public uint totalDeaths;

		public struct PickupCounts
		{
			public uint totalPickupsCollected;
			//Derived from addition of totalPickups from all pickup types
			public uint totalLevelUps;
			//Derived from addition of maxLevel from all pickup types
			public struct DecreaseTrafficSpeed
			{
				public uint totalPickups;
				public uint maxLevel;
			}

			public struct IncreaseTrafficSpeed
			{
				public uint totalPickups;
				public uint maxLevel;
			}

			public struct IncreasePlayerSpeed
			{
				public uint totalPickups;
				public uint maxLevel;
			}

			public struct DecreasePlayerSpeed
			{
				public uint totalPickups;
				public uint maxLevel;
			}

			public struct ScoreMultiplier
			{
				public uint totalPickups;
				public uint maxLevel;
			}

			public struct CarDestroyer
			{
				public uint totalPickups;
				public uint maxLevel; //Is counted, but not needed
			}
		}
		public struct TrafficStats {
			public uint carsDestroyed;
			public uint aggressiveDrivers;
			public uint passiveDrivers;
			public uint accidentsCaused;
			public ulong totalCarsSpawned;
		}
			
	}

	// Use this for initialization
	void Start ()
	{
		this.resetScore ();
		if (this.saveIntervalInSeconds > 0) {
			InvokeRepeating ("saveToDisk", this.saveIntervalInSeconds, this.saveIntervalInSeconds);
		}
	}


		
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	void OnApplicationQuit() {
		this.saveToDisk ();
	}


	#region From GameController

	private void resetScore ()
	{
		this.currentStats.score = 0;
		this.uiController.updateScore (this.currentStats.score);
	}

	private void resetTime() {
		this.updateGameTime (0, 0, 0);
	}

	private void resetPickupCounts() {
		for (int i = 0; i < 6; i++) {
			this.changeCurrentPickupLevel ((PickupController.PickupType)i, 0);
		}
	}

	private void resetCurrentStatistics () {
		this.resetScore ();
		this.resetTime ();
		this.resetPickupCounts ();
	}

	public void setGameActive (bool active) {
		//Save statistics every time a new game is started
		if (!active) {
			this.saveToDisk ();
			this.resetCurrentStatistics ();
		}
	}
		
	#endregion

	#region Statistics Recording

	//Increase Current Score by scoreToAdd
	//Also increase Long Term Statistics - Total Game Points by scoreToAdd
	public void increaseScoreCount(uint scoreToAdd) 
	{
		//Increase CurrentScore by scoreToAdd
		this.currentStats.score += scoreToAdd;
		//Increase Total Score in LongTermStatistics
		this.generalLongTermStats.totalGamePoints += scoreToAdd;
		//Remember to update the UI
		this.uiController.updateScore(this.currentStats.score);
	}

	//Increase Total Games Played by one
	public void increaseTotalGameCount() {
		this.generalLongTermStats.totalGamesPlayed += 1;
	}

	//Increase Total Random Events by one
	public void increaseRandomEventCount() {
		this.generalLongTermStats.totalRandomEvents += 1;
	}

	//Increate Total Deaths by one
	public void increaseDeathCount() {
		this.generalLongTermStats.totalDeaths += 1;
	}

	public void increaseGameTime() {
		this.generalLongTermStats.totalGameTimeInMinutes += 1;
	}

	//Just for updating the time from ScoreCounter, actual updating of Time stats reporting is done using increaseGameTime()
	public void updateGameTime(uint minutes, uint seconds, uint mSeconds) {
		this.uiController.updateTime (minutes, seconds, mSeconds);
	}

	//Increases CarsDestroyed by one
	public void increaseCarsDestroyedCount() {
		this.trafficStats.carsDestroyed += 1;
	}

	//Increase AggressiveDrivers by one
	public void increaseAggressiveDriversCount() {
		this.trafficStats.aggressiveDrivers += 1;
	}

	//Increase PassiveDrivers by one
	public void increasePassiveDriversCount() {
		this.trafficStats.passiveDrivers += 1;
	}

	//Increase AccidentsCaused by one
	public void increaseAccidentsCausedCount() {
		this.trafficStats.accidentsCaused += 1;
	}

	//IncreaseTotalCarsSpawned by one
	public void increaseTotalCarsSpawnedCount() {
		this.trafficStats.totalCarsSpawned += 1;
	}

	//Increase Current Pickup Level
	//Will also calculate to check if the current level is the highest level
	public void changeCurrentPickupLevel (PickupController.PickupType pickup, uint level)
	{
		switch (pickup) {
		//Car Destroyer
		case PickupController.PickupType.CarDestroyer:
			this.currentPickupLevels.carDestroyer = level;
			if (level > 0) //if level is greater than 0 (which would mean deactivate), increase totalPickups by 1
				this.pickupStatsCarDestroyer.totalPickups += 1;
			if (level > this.pickupStatsCarDestroyer.maxLevel) //If current level is higher than current max, set max level to current level
				this.pickupStatsCarDestroyer.maxLevel = level;
			goto default;
		//Decrease Player Speed
		case PickupController.PickupType.DecreasePlayerSpeed:
			this.currentPickupLevels.decreasePlayerSpeed = level;
			if (level > 0)
				this.pickupStatsDecreasePlayerSpeed.totalPickups += 1;
			if (level > this.pickupStatsDecreasePlayerSpeed.maxLevel)
				this.pickupStatsDecreasePlayerSpeed.maxLevel = level;
			goto default;
		//Decrease Traffic Speed
		case PickupController.PickupType.DecreaseTrafficSpeed:
			this.currentPickupLevels.decreaseTrafficSpeed = level;
			if (level > 0)
				this.pickupStatsDecreaseTrafficSpeed.totalPickups += 1;
			if (level > this.pickupStatsDecreaseTrafficSpeed.maxLevel)
				this.pickupStatsDecreaseTrafficSpeed.maxLevel = level;
			goto default;
		//Increase Player Speed
		case PickupController.PickupType.IncreasePlayerSpeed:
			this.currentPickupLevels.increasePlayerSpeed = level;
			if (level > 0)
				this.pickupStatsIncreasePlayerSpeed.totalPickups += 1;
			if (level > this.pickupStatsIncreasePlayerSpeed.maxLevel)
				this.pickupStatsIncreasePlayerSpeed.maxLevel = level;
			goto default;
		//Increase Traffic Speed
		case PickupController.PickupType.IncreaseTrafficSpeed:
			this.currentPickupLevels.increaseTrafficSpeed = level;
			if (level > 0)
				this.pickupStatsIncreaseTrafficSpeed.totalPickups += 1;
			if (level > this.pickupStatsIncreaseTrafficSpeed.maxLevel)
				this.pickupStatsIncreaseTrafficSpeed.maxLevel = level;
			goto default;
		//Score Multiplier
		case PickupController.PickupType.ScoreMultiplier:
			this.currentPickupLevels.scoreMultiplier = level;
			if (level > 0)
				this.pickupStatsScoreMultiplier.totalPickups += 1;
			if (level > this.pickupStatsScoreMultiplier.maxLevel)
				this.pickupStatsScoreMultiplier.maxLevel = level;
			this.uiController.updateScoreMultiplier (level);
			break;
		default:
			this.uiController.updatePickupText (pickup, level);
			break;
		}
		if (level > 0)
			this.allPickupStats.totalLevelUps += 1;
		this.allPickupStats.totalPickupsCollected += 1;
	}



	#endregion


	#region Statistics Reporting
	//NOT SURE IF THIS WILL BE NEEDED

	public float getScoreMultiplier ()
	{
		return this.currentStats.scoreMultiplier;
	}

	public ulong getCurrentScore ()
	{
		return this.currentStats.score;
	}
		

	#endregion

	#region Save to Disk / Load
	private void saveToDisk(/*uint userId, string userName*/) {
		uint userId = this.defaultUserId;
		string userName = this.defaultUserName;
		this.saveParser.saveUserStats (userId,userName,
			this.generalLongTermStats,
			this.trafficStats,
			this.allPickupStats,
			this.pickupStatsCarDestroyer,
			this.pickupStatsDecreasePlayerSpeed,
			this.pickupStatsDecreaseTrafficSpeed,
			this.pickupStatsIncreasePlayerSpeed,
			this.pickupStatsIncreaseTrafficSpeed,
			this.pickupStatsScoreMultiplier);
		Debug.LogWarning ("Save");
	}

	public void loadFromDisk(/*uint userId*/) {
		uint userId = this.defaultUserId;

		//Assign these to the results from the different things from XMLSaveParser's Loading User Data region
		this.generalLongTermStats = this.saveParser.getGeneralStats(userId);
		this.allPickupStats = this.saveParser.getGeneralPickupStats (userId);
		this.trafficStats = this.saveParser.getTrafficStats (userId);
		this.pickupStatsCarDestroyer = this.saveParser.getPickupStatsForCarDestroyer (userId);
		this.pickupStatsDecreasePlayerSpeed = this.saveParser.getPickupStatsForDecreasePlayerSpeed (userId);
		this.pickupStatsDecreaseTrafficSpeed = this.saveParser.getPickupStatsForDecreaseTrafficSpeed (userId);
		this.pickupStatsIncreasePlayerSpeed = this.saveParser.getPickupStatsForIncreasePlayerSpeed (userId);
		this.pickupStatsIncreaseTrafficSpeed = this.saveParser.getPickupStatsForIncreaseTrafficSpeed (userId);
		this.pickupStatsScoreMultiplier = this.saveParser.getPickupStatsForScoreMultiplier (userId);


	}



	#endregion


}
