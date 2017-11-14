//Nestor Saavedra
//11/14/2017

//Mirrors structure of StatController in XML form, to save to disk
using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;
using System.Xml.Linq;

public class XMLSaveParser : MonoBehaviour
{
	public string nameOfSaveFile = "GameStatistics.xml";

	#region Data Holders for Start() Load Operation

	private StatController.LongTermStatistics longTermStats;
	private StatController.LongTermStatistics.TrafficStats trafficStats;
	private StatController.LongTermStatistics.PickupCounts pickupCounts;
	private StatController.LongTermStatistics.PickupCounts.CarDestroyer carDestroyerCounts;
	private StatController.LongTermStatistics.PickupCounts.DecreasePlayerSpeed decreasePlayerSpeedCounts;
	private StatController.LongTermStatistics.PickupCounts.DecreaseTrafficSpeed decreaseTrafficSpeedCounts;
	private StatController.LongTermStatistics.PickupCounts.IncreasePlayerSpeed increasePlayerSpeedCounts;
	private StatController.LongTermStatistics.PickupCounts.IncreaseTrafficSpeed increaseTrafficSpeedCounts;
	private StatController.LongTermStatistics.PickupCounts.ScoreMultiplier scoreMultiplierCounts;

	#endregion


	//load statistics file when game starts
	//stats stored in StatController object, saved when game ends
	//The save file is loaded when the game starts
	//Then Data Holder objects are dereferenced and saved into memory.
	void Start ()
	{
		this.loadData ();
	}


	public void saveUserStats (uint userId, string userName, 
	                           StatController.LongTermStatistics longTermStats,
	                           StatController.LongTermStatistics.TrafficStats trafficStats,
	                           StatController.LongTermStatistics.PickupCounts pickupCounts,
	                           StatController.LongTermStatistics.PickupCounts.CarDestroyer carDestroyerCounts,
	                           StatController.LongTermStatistics.PickupCounts.DecreasePlayerSpeed decreasePlayerSpeedCounts,
	                           StatController.LongTermStatistics.PickupCounts.DecreaseTrafficSpeed decreaseTrafficSpeedCounts,
	                           StatController.LongTermStatistics.PickupCounts.IncreasePlayerSpeed increasePlayerSpeedCounts,
	                           StatController.LongTermStatistics.PickupCounts.IncreaseTrafficSpeed increaseTrafficSpeedCounts,
	                           StatController.LongTermStatistics.PickupCounts.ScoreMultiplier scoreMultiplierCounts)
	{
		XDocument doc = new XDocument (
			                new XDeclaration ("1.0", "UTF-8", string.Empty), 
			                new XElement ("PlayingInTraffic",
				                new XElement ("Users",
					                new XElement ("User",
						                new XAttribute ("id", System.Convert.ToString (userId)),
						                new XAttribute ("name", userName),
						                new XElement ("GameStats",
							                new XElement ("GeneralStats",
								                new XElement ("TotalGamePoints", System.Convert.ToString (longTermStats.totalGamePoints)),
								                new XElement ("TotalGameTimeInMinutes", System.Convert.ToString (longTermStats.totalGameTimeInMinutes)),
								                new XElement ("TotalGamesPlayed", System.Convert.ToString (longTermStats.totalGamesPlayed)),
								                new XElement ("TotalRandomEvents", System.Convert.ToString (longTermStats.totalRandomEvents)),
								                new XElement ("TotalDeaths", System.Convert.ToString (longTermStats.totalDeaths))),
							                new XElement ("PickupStats",
								                new XElement ("TotalPickupsCollected", System.Convert.ToString (pickupCounts.totalPickupsCollected)),
								                new XElement ("TotalLevelUps", System.Convert.ToString (pickupCounts.totalLevelUps)),
								                new XElement ("PickupType",
									                new XElement ("DecreaseTrafficSpeed", 
										                new XElement ("TotalPickups", System.Convert.ToString (decreaseTrafficSpeedCounts.totalPickups)),
										                new XElement ("MaxLevel", System.Convert.ToString (decreaseTrafficSpeedCounts.maxLevel))
									                ),
									                new XElement ("IncreaseTrafficSpeed",
										                new XElement ("TotalPickups", System.Convert.ToString (increaseTrafficSpeedCounts.totalPickups)),
										                new XElement ("MaxLevel", System.Convert.ToString (increaseTrafficSpeedCounts.maxLevel))
									                ),
									                new XElement ("IncreasePlayerSpeed", 
										                new XElement ("TotalPickups", System.Convert.ToString (increasePlayerSpeedCounts.totalPickups)),
										                new XElement ("MaxLevel", System.Convert.ToString (increasePlayerSpeedCounts.maxLevel))
									                ),
									                new XElement ("DecreasePlayerSpeed", 
										                new XElement ("TotalPickups", System.Convert.ToString (decreasePlayerSpeedCounts.totalPickups)),
										                new XElement ("MaxLevel", System.Convert.ToString (decreasePlayerSpeedCounts.maxLevel))
									                ),
									                new XElement ("ScoreMultiplier", 
										                new XElement ("TotalPickups", System.Convert.ToString (scoreMultiplierCounts.totalPickups)),
										                new XElement ("MaxLevel", System.Convert.ToString (scoreMultiplierCounts.maxLevel))
									                ),
									                new XElement ("CarDestroyer", 
										                new XElement ("TotalPickups", System.Convert.ToString (scoreMultiplierCounts.totalPickups)),
										                new XElement ("MaxLevel", System.Convert.ToString (scoreMultiplierCounts.maxLevel))
									                )
								                )
							                ),
							                new XElement ("TrafficStats", 
								                new XElement ("CarsDestroyed", System.Convert.ToString (trafficStats.carsDestroyed)), 
								                new XElement ("AggressiveDrivers", System.Convert.ToString (trafficStats.aggressiveDrivers)),
								                new XElement ("PassiveDrivers", System.Convert.ToString (trafficStats.passiveDrivers)),
								                new XElement ("AccidentsCaused", System.Convert.ToString (trafficStats.accidentsCaused)),
								                new XElement ("TotalCarsSpawned", System.Convert.ToString (trafficStats.totalCarsSpawned))
							                )
						                )
					                )
				                )
			                )
		                );

		//Create backup of previous file
		string backupName = this.nameOfSaveFile + ".bkp";
		if (System.IO.File.Exists (this.nameOfSaveFile)) {
			if (System.IO.File.Exists(backupName)) {
				System.IO.File.Delete (backupName);
			} 
			System.IO.File.Copy (this.nameOfSaveFile, backupName);
		}
		//Save file
		doc.Save (this.nameOfSaveFile); 
	}

	#region Loading User Data

	private string getXMLString ()
	{
		string outString;
		using (StreamReader readXML = new StreamReader (this.nameOfSaveFile, System.Text.Encoding.UTF8)) {
			outString = readXML.ReadToEnd ();
		}
		return outString;
	}

	public void loadData ()
	{
		XDocument doc = new XDocument ();
		doc = XDocument.Parse (this.getXMLString (), LoadOptions.None);

		var results = doc.Element ("PlayingInTraffic").Nodes ();
		foreach (XElement node in results) {
			foreach (XElement userNode in node.Nodes()) {
				if (userNode.Name == "User") {


					foreach (XElement gameStats in userNode.Nodes()) {
						//GameStats
						if (gameStats.Name == "GameStats") {
							foreach (XElement gameStatsChildren in gameStats.Nodes()) {
								if (gameStatsChildren.Name == "GeneralStats") {
									foreach (XElement generalStat in gameStatsChildren.Nodes()) {
										switch (System.Convert.ToString (generalStat.Name)) {
										case "TotalGamePoints":
											this.longTermStats.totalGamePoints = System.Convert.ToUInt64 (generalStat.Value);
											break;
										case "TotalGameTimeInMinutes":
											this.longTermStats.totalGameTimeInMinutes = System.Convert.ToUInt32 (generalStat.Value);
											break;
										case "TotalGamesPlayed":
											this.longTermStats.totalGamesPlayed = System.Convert.ToUInt32 (generalStat.Value);
											break;
										case "TotalRandomEvents":
											this.longTermStats.totalRandomEvents = System.Convert.ToUInt32 (generalStat.Value);
											break;
										case "TotalDeaths":
											this.longTermStats.totalDeaths = System.Convert.ToUInt32 (generalStat.Value);
											break;
										}
										;
									}
								} else if (gameStatsChildren.Name == "PickupStats") {
									foreach (XElement generalPickupStat in gameStatsChildren.Nodes()) {
										switch (System.Convert.ToString (generalPickupStat.Name)) {
										case "TotalPickupsCollected":
											this.pickupCounts.totalPickupsCollected = System.Convert.ToUInt32 (generalPickupStat.Value);
											break;
										case "TotalLevelUps":
											this.pickupCounts.totalLevelUps = System.Convert.ToUInt32 (generalPickupStat.Value);
											break;
										case "PickupType":
											//PickupType
											foreach (XElement pickupTypeRoot in generalPickupStat.Nodes()) {
												foreach (XElement pickupTypeStat in pickupTypeRoot.Nodes()) {
													switch (System.Convert.ToString (pickupTypeStat.Name)) {
													case "TotalPickups":
														switch (System.Convert.ToString (pickupTypeRoot.Name)) {
														case "DecreaseTrafficSpeed":
															this.decreaseTrafficSpeedCounts.totalPickups = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "IncreaseTrafficSpeed":
															this.increaseTrafficSpeedCounts.totalPickups = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "IncreasePlayerSpeed":
															this.increasePlayerSpeedCounts.totalPickups = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "DecreasePlayerSpeed":
															this.decreasePlayerSpeedCounts.totalPickups = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "ScoreMultiplier":
															this.scoreMultiplierCounts.totalPickups = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "CarDestroyer":
															this.carDestroyerCounts.totalPickups = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														}
														;
														break;
													case "MaxLevel":
														switch (System.Convert.ToString (pickupTypeRoot.Name)) {
														case "DecreaseTrafficSpeed":
															this.decreaseTrafficSpeedCounts.maxLevel = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "IncreaseTrafficSpeed":
															this.increaseTrafficSpeedCounts.maxLevel = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "IncreasePlayerSpeed":
															this.increasePlayerSpeedCounts.maxLevel = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "DecreasePlayerSpeed":
															this.decreasePlayerSpeedCounts.maxLevel = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "ScoreMultiplier":
															this.scoreMultiplierCounts.maxLevel = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														case "CarDestroyer":
															this.carDestroyerCounts.maxLevel = System.Convert.ToUInt32 (pickupTypeStat.Value);
															break;
														}
														break;
													}

												}
											}
											break;
										}
									}
								} else if (gameStatsChildren.Name == "TrafficStats") {
									foreach (XElement trafficStat in gameStatsChildren.Nodes()) {
										switch (System.Convert.ToString (trafficStat.Name)) {
										case "CarsDestroyed":
											this.trafficStats.carsDestroyed = System.Convert.ToUInt32 (trafficStat.Value);
											break;
										case "AggressiveDrivers":
											this.trafficStats.aggressiveDrivers = System.Convert.ToUInt32 (trafficStat.Value);
											break;
										case "PassiveDrivers":
											this.trafficStats.passiveDrivers = System.Convert.ToUInt32 (trafficStat.Value);
											break;
										case "AccidentsCaused":
											this.trafficStats.accidentsCaused = System.Convert.ToUInt32 (trafficStat.Value);
											break;
										case "TotalCarsSpawned":
											this.trafficStats.totalCarsSpawned = System.Convert.ToUInt64 (trafficStat.Value);
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}


	public StatController.LongTermStatistics getGeneralStats (uint userId)
	{
		return this.longTermStats;
	}

	public StatController.LongTermStatistics.TrafficStats getTrafficStats (uint userId)
	{
		return this.trafficStats;
	}

	public StatController.LongTermStatistics.PickupCounts getGeneralPickupStats (uint userId)
	{
		return this.pickupCounts;
	}

	public StatController.LongTermStatistics.PickupCounts.CarDestroyer getPickupStatsForCarDestroyer (uint userId)
	{
		return this.carDestroyerCounts;
	}

	public StatController.LongTermStatistics.PickupCounts.DecreasePlayerSpeed getPickupStatsForDecreasePlayerSpeed (uint userId)
	{
		return this.decreasePlayerSpeedCounts;
	}

	public StatController.LongTermStatistics.PickupCounts.DecreaseTrafficSpeed getPickupStatsForDecreaseTrafficSpeed (uint userId)
	{
		return this.decreaseTrafficSpeedCounts;
	}

	public StatController.LongTermStatistics.PickupCounts.IncreasePlayerSpeed getPickupStatsForIncreasePlayerSpeed (uint userId)
	{
		return this.increasePlayerSpeedCounts;
	}

	public StatController.LongTermStatistics.PickupCounts.IncreaseTrafficSpeed getPickupStatsForIncreaseTrafficSpeed (uint userId)
	{
		return this.increaseTrafficSpeedCounts;
	}

	public StatController.LongTermStatistics.PickupCounts.ScoreMultiplier getPickupStatsForScoreMultiplier (uint userId)
	{
		return this.scoreMultiplierCounts;
	}



	#endregion
}
