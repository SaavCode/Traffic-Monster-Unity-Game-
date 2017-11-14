//Nestor Saavedra
//11/16/2017

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

//UIController ONLY handles view operations. StatController tells UIController when to update its information.


[System.Serializable]
public class GameUI {
	public Text playerScoreText;
	public Text gameOverText;
	public Text multiplierText; //Shows nothing if there is no multiplier
	public Text timeCounterText;
	public Text restartText;
	public Text trafficSpeedText;
	public Text trafficSpeedLevelText;
	public Text playerSpeedText;
	public Text playerSpeedLevelText;
	public Text carDestroyerText; //Car Destroyer doesn't have a level progression
	public Color textColor;
	public Color pickupPositiveColor;
	public Color pickupNegativeColor;
}

[System.Serializable]
public class UIStrings {
	public string scoreString;
	public string multiplierString;
	public string gameOverString;
	public string pickupLevelString;
	public string timeSeparator;
	public string mSecondsSeparator;
	public string playerSpeedText = "Player";
	public string trafficSpeedText = "Traffic";
	public char upCharacter = '\u25B2';
	public char downCharacter = '\u25BC';
}

public class UIController : MonoBehaviour {
	//private StatController statController
	public GameUI gameUI;
	public UIStrings uiStrings;
	private enum TextCategory {
		Score,
		GameOver,
		Multiplier,
		Restart,
		PickupTextTraffic,
		PickupTextPlayer,
		PickupTextCarDestroyer,
		All,
	};

	// Use this for initialization
	void Start () {
		this.updateScore(0);
		this.updateTime (0, 0, 0);
		this.uiStrings.upCharacter = '\u25B2';
		this.uiStrings.downCharacter = '\u25BC';
	}


	//issue with the text being changed
	void changeTextColor(TextCategory textObject, Color color) {
		switch (textObject) {
		case TextCategory.Score:
			this.gameUI.playerScoreText.color = color;
			break;
		case TextCategory.GameOver:
			this.gameUI.gameOverText.color = color;
			break;
		case TextCategory.Multiplier:
			this.gameUI.restartText.color = color;
			break;
		case TextCategory.Restart:
			this.gameUI.restartText.color = color;
			break;
		case TextCategory.All:
			this.changeTextColor (TextCategory.Score, color);
			this.changeTextColor (TextCategory.Restart, color);
			this.changeTextColor (TextCategory.Multiplier, color);
			this.changeTextColor (TextCategory.GameOver, color);
			break;
		default:
			break;
		}
	}
	
	// Updateonce per frame
	void Update () {

	
	}

	#region Interface Updating

	//All of these will be called by StatController the majority of the time. UIController does not update itself
	public void updateScore(ulong newScore) {
		this.gameUI.playerScoreText.text = this.uiStrings.scoreString + System.Convert.ToString (newScore);
	}
		
	public void updateScoreMultiplier(float newValue) {
		float multiplierValue = (newValue + 1) - (newValue / 2);
		if (multiplierValue > 1) {
			this.gameUI.multiplierText.text = this.uiStrings.multiplierString + System.Convert.ToString (multiplierValue);
		} else if (multiplierValue <= 1) {
			this.gameUI.multiplierText.text = "";
		}
			
	}

	public void updatePickupText(PickupController.PickupType pickup, uint level) {
		//Turn on/off via illuminatePickupText
		if (level == 0) {
			this.deactivatePickupText (pickup);
		} else {
			this.illuminatePickupText (pickup);
		}
		this.changePickupText (pickup);
		this.changePickupLevelText (pickup, level);

		//Change text to show if there's an UP arrow or DOWN arrow
		//Change PickupLevel text
	}
		
	public void updateTime(uint minutes, uint seconds, uint mSeconds) {
		string timeString;
		string minutesString;
		string secondsString;
		string mSecondsString;


		minutesString = this.formatForTimeDisplay (minutes);
		secondsString = this.formatForTimeDisplay (seconds);
		mSecondsString = System.Convert.ToString (mSeconds);

		timeString = minutesString + this.uiStrings.timeSeparator + secondsString + this.uiStrings.mSecondsSeparator + mSecondsString;
		this.gameUI.timeCounterText.text = timeString;
	}

	private string formatForTimeDisplay(uint time) {
		if (time < 10) {
			return "0" + System.Convert.ToString (time);
		} else
			return System.Convert.ToString(time);
	}

	private void changePickupText(PickupController.PickupType pickup) {
		switch (pickup) {
		case PickupController.PickupType.DecreasePlayerSpeed:
			this.gameUI.playerSpeedText.text = this.uiStrings.playerSpeedText + " " + this.uiStrings.downCharacter;
			break;
		case PickupController.PickupType.DecreaseTrafficSpeed:
			this.gameUI.trafficSpeedText.text = this.uiStrings.trafficSpeedText + " " +  this.uiStrings.downCharacter;
			break;
		case PickupController.PickupType.IncreasePlayerSpeed:
			this.gameUI.playerSpeedText.text = this.uiStrings.playerSpeedText + " " +  this.uiStrings.upCharacter;
			break;
		case PickupController.PickupType.IncreaseTrafficSpeed:
			this.gameUI.trafficSpeedText.text = this.uiStrings.trafficSpeedText + " " +  this.uiStrings.upCharacter;
			break;
		default:
			break;
		}
	}
	//ONLY affects the color of PickupTexts
	private void illuminatePickupText(PickupController.PickupType pickup) {
		switch (pickup) {
		case PickupController.PickupType.CarDestroyer:
			//Change CarDestroyer Text to POSITIVE color
			if (!this.gameUI.carDestroyerText.IsActive())
				this.gameUI.carDestroyerText.gameObject.SetActive(true);
			this.gameUI.carDestroyerText.color = this.gameUI.pickupPositiveColor;
			//Debug.Log ("Car Destroyer");
			break;
		case PickupController.PickupType.DecreasePlayerSpeed:
			//Activate text object if it is not active
			if (!this.gameUI.playerSpeedText.IsActive ())
				this.gameUI.playerSpeedText.gameObject.SetActive (true);
			//Change Player Speed Pickup text to the NEGATIVE color
			this.gameUI.playerSpeedText.color = this.gameUI.pickupNegativeColor;
			//Change Player Speed Pickup text to show a DOWN arrow glyph
			break;
		case PickupController.PickupType.DecreaseTrafficSpeed:
			if (!this.gameUI.trafficSpeedText.IsActive ())
				this.gameUI.trafficSpeedText.gameObject.SetActive (true);
			//POSITIVE color
			this.gameUI.trafficSpeedText.color = this.gameUI.pickupPositiveColor;
			//DOWN arrow
			break;
		case PickupController.PickupType.IncreasePlayerSpeed:
			if (!this.gameUI.playerSpeedText.IsActive ())
				this.gameUI.playerSpeedText.gameObject.SetActive (true);
			//Change Player Speed Pickup text to the POSITIVE color
			this.gameUI.playerSpeedText.color = this.gameUI.pickupPositiveColor;
			//Change Player Speed text to show a UP arrow glyph
			break;
		case PickupController.PickupType.IncreaseTrafficSpeed:
			if (!this.gameUI.trafficSpeedText.IsActive ())
				this.gameUI.trafficSpeedText.gameObject.SetActive (true);
			//NEGATIVE color
			this.gameUI.trafficSpeedText.color = this.gameUI.pickupNegativeColor;
			//UP arrow
			break;
		default:
			break;
		};
	}
	private void deactivatePickupText(PickupController.PickupType pickup) {
		switch (pickup) {
		case PickupController.PickupType.CarDestroyer:
			this.gameUI.carDestroyerText.gameObject.SetActive (false);
			break;
		case PickupController.PickupType.DecreasePlayerSpeed:
			goto case PickupController.PickupType.IncreasePlayerSpeed;
		case PickupController.PickupType.DecreaseTrafficSpeed:
			goto case PickupController.PickupType.IncreaseTrafficSpeed;
		case PickupController.PickupType.IncreasePlayerSpeed:
			this.gameUI.playerSpeedText.gameObject.SetActive (false);
			break;
		case PickupController.PickupType.IncreaseTrafficSpeed:
			this.gameUI.trafficSpeedText.gameObject.SetActive (false);
			break;
		}
	}
		
	private void changePickupLevelText(PickupController.PickupType pickup, uint level) {
		string levelString = (level == 0) ? "": this.uiStrings.pickupLevelString + level;
		switch (pickup) {
		case PickupController.PickupType.CarDestroyer:
			//Levels don't currently matter to this one
			break;
		case PickupController.PickupType.DecreasePlayerSpeed:
			this.gameUI.playerSpeedLevelText.text = levelString;
			break;
		case PickupController.PickupType.DecreaseTrafficSpeed:
			this.gameUI.trafficSpeedLevelText.text = levelString;
			break;
		case PickupController.PickupType.IncreasePlayerSpeed:
			goto case PickupController.PickupType.DecreasePlayerSpeed;
		case PickupController.PickupType.IncreaseTrafficSpeed:
			goto case PickupController.PickupType.DecreaseTrafficSpeed;
		default:
			break;
		};

	}





	#endregion


	#region DEBUG - FOR TESTING

	#endregion
}
