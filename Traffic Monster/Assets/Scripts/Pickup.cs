//Nestor Saavedra
//11/16/2017

using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour
{
	private PickupController pickupController;


	[System.Serializable]
	public class PersistanceTimeRange
	{
		public float minTime = 5;
		public float maxTime = 10;
	}

	public PersistanceTimeRange persistTime; // range


	//Parent of object
	public PickupController.PickupType pickupType;
	private Rigidbody physics;
	private MeshRenderer meshRender;
	private Material material;

	private ulong pickupUID;

	private float existenceTime = 0;
	private float totalExistenceTime;

	// initialization
	void Start ()
	{
		this.physics = GetComponent<Rigidbody> ();
		this.pickupController = this.gameObject.GetComponentInParent<PickupController> ();
		this.material = GetComponent<Material> ();
		this.meshRender = GetComponent<MeshRenderer> ();


		this.totalExistenceTime = this.getRandomPersistTime();
		this.pickupUID = this.generateUniqueId ();

		InvokeRepeating ("countDownToDestruction", 0.1f, 0.1f);

	}

	private ulong generateUniqueId() {
		return System.Convert.ToUInt64(System.Math.Abs (System.DateTime.Now.ToBinary ()));
	}

	private float getRandomPersistTime() {
		return Random.Range(persistTime.minTime, persistTime.maxTime);
	}
	
	// Update once per frame
	void Update ()
	{

	}

    //Erase the pickup
	private void countDownToErase() {
	

		if (System.Convert.ToUInt32(this.existenceTime) == System.Convert.ToUInt32(this.totalExistenceTime)) {
			this.destroySelf ();
		}
		this.existenceTime += 0.1f;
	}
	//Alert PickupController 
	void OnTriggerEnter (Collider other)
	{
		//If collide with player, activate pickup.
		//if collide with other pickup, deactivate new pickup
		switch (other.tag) {
		case "Player":
			this.pickupController.activatePickup (this.pickupType);
			Destroy (other.gameObject);
			break;
		case "Pickup":
			Destroy (other.gameObject);
			break;
		default:
			break;
		}
	}
	private void destroySelf() {
		Destroy (this.gameObject);
	}
}
