//Nestor Saavedra
//11/16/2017
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameController gameController;
    public UIController uiController; //Needs UIController for controlling UIs with the input keys


    private float baseSpeed = 1.0f;
    private float speedMultiplier = 1.0f;

    private bool isMenuActive; //Changed by the menu button
    public float mSpeed;
    void Start()
    {
        mSpeed = 100f;

    }
    void Update()
    {
        transform.Translate(mSpeed * Input.GetAxis("Horizontal") * Time.deltaTime, 0f, mSpeed * Input.GetAxis("Vertical") * Time.deltaTime);

    }

    #region From GameController
    //Either of these with a level of 0 will undo their effects
    public void increasePlayerSpeed(uint pickupLevel)
    {

    }

    public void decreasePlayerSpeed(uint pickupLevel)
    {

    }
    #endregion

    public void resetPlayerSpeed()
    {
        this.speedMultiplier = 1.0f;
    }
}