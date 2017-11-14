//Nestor Saavedra
//11/16/2017
using UnityEngine;
using System.Collections;
public class CarController : MonoBehaviour
{
    public float Speed;         //Car speed
    public float minspeed = 50f;//The vehicles minimum speed
    //public bool LaneSwitch;
    public float brakeTimer;
    
    int DecisionTime;
    

    // Use this for initialization
    void Start()
    {
        Speed = Random.Range(50, 100);                      //generate a random car speed within this range
        brakeTimer = Random.Range(5f, 10f);//timer for the when the car stops
        DecisionTime = Random.Range(0, 1);
    }

    // Update is called once per frame
    void Update()
    {
       
        brakeTime();//the time it takes for the car to resume driving after detecting player
        carManager();//clean up cars spawned too close to each other
        detectPlayer();//use raycast to detect the player
        Position();//set the vehicle postion
    }

    public void brakeTime()
    {
        if (brakeTimer > 0 && Speed <= 0)
        {
            brakeTimer -= Time.deltaTime;
        }
        if (brakeTimer <= 0)
        {
            Speed = minspeed;
        }
        
    }

    public void carManager()
    {

        RaycastHit hit;
        float Distance;
        Vector3 fwd = transform.TransformDirection(2,0,0) * 15;
        Debug.DrawRay(transform.position, fwd, Color.blue);

        if (Physics.Raycast(transform.position, fwd, out hit))
        {
            Distance = hit.distance;

            if (Distance <= 50 && gameObject.transform.position.x <= 25)
            {
                Speed = minspeed;
                Destroy(gameObject);
            }
          else   if (Distance <= 25 )
            {
                Speed = minspeed;
            }
             if (hit.collider.gameObject.tag == "Vehicle" && Distance <= 20)
            {
                Speed = 0;
            }
        }
        //stop cars from spawning if they are too close to the spawn point
        
        
    }
  
    public void Position() {

        Vector3 position = gameObject.transform.position;   //transform cars position
        transform.Translate(Vector3.right * Speed * Time.deltaTime);
        position.x += Speed * Time.deltaTime;
        float currentPosition = position.x;                 //car object's current position

        //Destroy at position x = 450
        if (currentPosition >= 450)
        {
            Destroy(gameObject);
        }
    }
 
    //If object has tag Vehicle destroy

    public void detectPlayer()
    {
        RaycastHit hit;
        float playerDistance;
        Vector3 fwd = transform.TransformDirection(1, 0, 0) * 15;
        Debug.DrawRay(transform.position, fwd, Color.red);

        if (Physics.Raycast(transform.position, fwd, out hit))
        {
            playerDistance = hit.distance;
            if (hit.collider.gameObject.tag == "Player" && playerDistance <= 25)
            {
                Speed = 0;
            }

        }

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Vehicle")   //look for vehicle tag
        {
            //Speed = 5;
            Destroy(other.gameObject);//Destroy the game object
        }
    }
    public void LSwitch()
    {

        /*  if (zpos <= 105 && zpos >= 45)
       {
           if (LaneSwitch == true)
           {
               zpos = (zpos + 30);
           }
           else
           {
               zpos = (zpos + 0);
           }
       }
       */
    }

}
