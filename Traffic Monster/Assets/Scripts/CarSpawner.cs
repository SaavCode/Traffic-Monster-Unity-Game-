//Nestor Saavedra
//11/16/2017
using UnityEngine;
using System.Collections;

public class CarSpawner : MonoBehaviour {

    //Car Lanes
    public int[] Lane = {0,1,2,3,4};            //lanes from zero to 4 array
    public GameObject car;          // car object
    public float delayTimer = 0.5f; //initial delay time
    public int zpos;                //z position
    float timer;                    //timer for car spawning
    int LaneRnd;                    //random lane
    
    // Use this for initialization
    void Start () {
       
                    }

    // Update is called once per frame
     void Update () {
  
        Lanes();                        //call lanes function
        
        LaneRnd = Random.Range(0, Lane.Length);//set LaneRnd to a random lane

        timer -= Time.deltaTime;        //update timer

        timeDelay();                    //call timeDelay
    }


    public void Lanes()
    {

            //set lane sizes and position on the z - axis
            switch (LaneRnd)
            {
                case 4:
                    zpos = 15;          //lane1
                    break;
                case 3:
                    zpos = 45;          //lane 2
                    break;
                case 2:
                    zpos = 75;          //lane 3
                    break;
                case 1:
                    zpos = 105;         //lane 4
                    break;
                case 0:
                    zpos = 135;         //lane 5
                    break;
                default:
                    zpos = 15;          //lane 1 default
                    break;
            }
        
    }

    public void timeDelay()
    {   //When the timer reaches zero spawnthe vehicle
        if (timer <= 0)
        {
            Vector3 carPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, zpos);// set car position
            Instantiate(car, carPos, transform.rotation);//create a clone of the car prefab
            timer = delayTimer; //set the timer to the delay time
        }
    }

}
