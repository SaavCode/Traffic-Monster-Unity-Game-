//Nestor Saavedra
//11/16/2017

using UnityEngine;
using System.Collections;

//ScoreController handles the generation of the score value to add during each score interval.

public class ScoreController : MonoBehaviour {
	public GameController gameController; //GameController tells ScoreController when to start and stop adding to the score
											


	public bool isScoreCounting = false;

	//This is used for tracking minutes passed
	private uint secondsCounter = 0;
	private uint minutesCounter = 0;
	private uint mSecondsCounter = 0;

	private uint baseScoreValue;
	private uint scoreMultiplierLevel;
	public float scoreMultiplier;

	// Use this for initialization
	void Start () {
		this.baseScoreValue = this.gameController.settings.baseScoreValue;
		this.scoreMultiplierLevel = 0;
		this.setMultiplier (this.scoreMultiplierLevel);

		//Score Counter
		InvokeRepeating ("incrementScore", 0.2f, 0.2f);

		//Time Counter
		InvokeRepeating ("countTime", 0.1f, 0.1f);
	
	}
	
	// Update once per frame
	void Update () {

	
	}

	public void resetCounts() {
		this.scoreMultiplierLevel = 0;
		this.setMultiplier (this.scoreMultiplierLevel);

	}

	private void countTime() {
		if (this.isScoreCounting) {
			if (this.mSecondsCounter < 9) {
				this.mSecondsCounter += 1;
			} else if (this.mSecondsCounter == 9) {
				this.mSecondsCounter = 0;
				if (this.secondsCounter != 59) {
					this.secondsCounter += 1;
				} else if (this.secondsCounter == 59) {
					this.secondsCounter = 0;
					this.minutesCounter += 1;
					this.gameController.increaseTotalGameTimeByOneMinute ();
				}
			}
			this.gameController.reportGameTime (minutesCounter, secondsCounter, mSecondsCounter);
		}
	}

	//Increments score by calculated amount
	private void incrementScore() {
		if (this.isScoreCounting) {
			this.gameController.addScorePoints(this.calculateScore());
		}
	}


		
	public void enableCountScore(bool shouldCountScore) {
		this.isScoreCounting = shouldCountScore;
	}

	uint calculateScore() { //returns the baseScoreValue multiplied by the scoreMultiplier
		float newScore = this.baseScoreValue  * this.scoreMultiplier;
		return (uint)newScore ; 
	}
		

	public void setMultiplier (uint multiplierLevel)
	{ 
		this.scoreMultiplierLevel = multiplierLevel;
		this.scoreMultiplier = ((multiplierLevel + 1) - (multiplierLevel / 2));
	}

	public uint getMultiplierLevel () {
		return this.scoreMultiplierLevel;
	}
}
