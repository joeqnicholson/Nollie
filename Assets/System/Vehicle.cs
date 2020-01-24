using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Vehicle : MonoBehaviour{
	
	// Main vehicle component
	
	public bool controllable = true;

	[Header("Components")]
	
	public Transform vehicleModel;
    public Transform turnCube;
    public Rigidbody sphere;
	
	[Header("Controls")]
	
	public KeyCode accelerate;
	public KeyCode brake;
	public KeyCode steerLeft;
	public KeyCode steerRight;
	public KeyCode jump;
	
	[Header("Parameters")]
	
	[Range(5.0f, 40.0f)] public float acceleration = 30f;
	[Range(20.0f, 160.0f)] public float steering = 80f;
    [Range(100.0f, 200.0f)] public float airSteering = 150f;
    [Range(50.0f, 80.0f)] public float jumpForce = 60f;
	[Range(0.0f, 20.0f)] public float gravity = 10f;
	[Range(0.0f, 1.0f)] public float drift = 1f;
	
	[Header("Switches")]
	
	public bool jumpAbility = false;
	public bool steerInAir = true;
	public bool motorcycleTilt = false;
	public bool alwaysSmoke = false;
    public bool canJump = false;
    public bool crouched = false;
    public bool jumping;
    float jumpTimer;
    // Vehicle components

    Transform container, wheelFrontLeft, wheelFrontRight;
	Transform body;
	TrailRenderer trailLeft, trailRight;
	
	ParticleSystem smoke;
	
	// Private
	
	float speed, speedTarget;
	float rotate, rotateTarget;
	
	bool nearGround, onGround, farGround, hitLiftOff;
	
	Vector3 containerBase;
	
	// Functions
	
	void Awake(){
		
		foreach(Transform t in GetComponentsInChildren<Transform>()){
			
			switch(t.name){
				
				// Vehicle components
				
				case "wheelFrontLeft": wheelFrontLeft = t; break;
				case "wheelFrontRight": wheelFrontRight = t; break;
				case "body": body = t; break;
				
				// Vehicle effects
				
				case "smoke": smoke = t.GetComponent<ParticleSystem>(); break;
				case "trailLeft": trailLeft = t.GetComponent<TrailRenderer>(); break;
				case "trailRight": trailRight = t.GetComponent<TrailRenderer>(); break;
				
			}
			
		}
		
		container = vehicleModel.GetChild(0);
		containerBase = container.localPosition;
		
	}
	
	void Update(){
		
		// Acceleration
		
		speedTarget = Mathf.SmoothStep(speedTarget, speed, Time.deltaTime * 12f); speed = 0f;
		
		//if(Input.GetKey(accelerate)){ ControlAccelerate(); }
        ControlAccelerate();
        //if(Input.GetKey(brake)){ ControlBrake(); }

        // Steering

        rotateTarget = Mathf.Lerp(rotateTarget, rotate, Time.deltaTime * 4f); rotate = 0f;
		
		if(Input.GetKey(steerLeft)) { ControlSteer(-1); }
		if(Input.GetKey(steerRight)){ ControlSteer( 1); }

		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, transform.eulerAngles.y + rotateTarget, 0)), Time.deltaTime * 2.0f);
        if (nearGround)
        {
            turnCube.rotation = transform.rotation;
            turnCube.position = transform.position;
        }
        else
        {
            turnCube.position = transform.position;

        }
        // Jump
        
        ControlJump();
		
		// Wheel and body tilt
		
		if(wheelFrontLeft != null){  wheelFrontLeft.localRotation  = Quaternion.Euler(0, rotateTarget / 2, 0); }
		if(wheelFrontRight != null){ wheelFrontRight.localRotation = Quaternion.Euler(0, rotateTarget / 2, 0); }
		
		//body.localRotation = Quaternion.Slerp(body.localRotation, Quaternion.Euler(new Vector3(speedTarget / 4, 0, rotateTarget / 6)), Time.deltaTime * 4.0f);
		
		// Vehicle tilt
		
		float tilt = 0.0f; if(motorcycleTilt){ tilt = -rotateTarget / 3.5f; }
		
		container.localPosition = containerBase + new Vector3(0, Mathf.Abs(tilt) / 2000, 0);
		container.localRotation = Quaternion.Slerp(container.localRotation, Quaternion.Euler(0, rotateTarget / 8, tilt), Time.deltaTime * 10.0f);
	
		// Effects
		
		if(!motorcycleTilt){ smoke.transform.localPosition = new Vector3(-rotateTarget / 100, smoke.transform.localPosition.y, smoke.transform.localPosition.z); }
		
		ParticleSystem.EmissionModule smokeEmission = smoke.emission;
		smokeEmission.enabled = onGround && sphere.velocity.magnitude > (acceleration / 4) && (Vector3.Angle(sphere.velocity, vehicleModel.forward) > 30.0f || alwaysSmoke);
		
		if(trailLeft != null){   trailLeft.emitting = smoke.emission.enabled; }
		if(trailRight != null){ trailRight.emitting = smoke.emission.enabled; }
		
		// Stops vehicle from floating around when standing still
		
		if(speed == 0 && sphere.velocity.magnitude < 4f){ sphere.velocity = Vector3.Lerp(sphere.velocity, Vector3.zero, Time.deltaTime * 2.0f); }
		
	}
	
	// Physics update
	
	void FixedUpdate(){
		
		RaycastHit hitOn;
		RaycastHit hitNear;
        RaycastHit liftOff;
        RaycastHit far;
		
		onGround   = Physics.Raycast(transform.position, Vector3.down, out hitOn, 1.8f);
		nearGround = Physics.Raycast(transform.position, Vector3.down, out hitNear, 2.0f);
        hitLiftOff = Physics.Raycast(transform.position, Vector3.down, out liftOff, 6.0f);
        farGround = Physics.Raycast(transform.position, Vector3.down, out far, 60.0f);
        print(transform.position.y);
        // Normal
        Debug.DrawRay(transform.position, Vector3.down * 12, Color.green);
        if (far.distance > 10)
        {
            vehicleModel.up = Vector3.Lerp(vehicleModel.up, far.normal, Time.deltaTime * 15f / (far.distance/2) );
        }
        else if(far.distance > 3)
        {
            vehicleModel.up = Vector3.Lerp(vehicleModel.up, liftOff.normal, Time.deltaTime * 4f);
        }
        else
        {
            vehicleModel.up = Vector3.Lerp(vehicleModel.up, liftOff.normal, Time.deltaTime * 8f);
        }
        vehicleModel.Rotate(0, transform.eulerAngles.y, 0);

		// Movement
		
		if(nearGround){
			
			sphere.AddForce(vehicleModel.forward * speedTarget, ForceMode.Acceleration);
			
		}else{
			
			sphere.AddForce(turnCube.forward * (speedTarget / 1.3f), ForceMode.Acceleration);
			
			// Simulated gravity
			
			sphere.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
			
		}
		
		transform.position = sphere.transform.position + new Vector3(0, 0.35f, 0);

        //Simulated drag on ground thanks to Adam Hunt


        Vector3 localVelocity = transform.InverseTransformVector(sphere.velocity);
        localVelocity.x *= 0.9f + (drift / 10);

        if (nearGround)
        {

            sphere.velocity = transform.TransformVector(localVelocity);

        }

    }
	
	// Controls
	
	public void ControlAccelerate(){

		speed = acceleration;

	}
	
	//public void ControlBrake(){
		
	//	if(!controllable){ return; }
		
	//	speed = -acceleration;
		
	//}
	
	public void ControlJump(){
        if (nearGround)
        {
            if (Input.GetKey("down"))
            {
                if (jumpForce < 70)
                {
                    jumpForce += Time.deltaTime * 20;
                }
                crouched = true;
            }
            if (Input.GetKeyUp("down"))
            {
                crouched = false;
                canJump = true;
                jumpTimer = 3;
            }
            if (canJump == true)
            {
                JumpTimer();

            }
            if (jumping == true)
            {
                sphere.AddForce(Vector3.up * (jumpForce * 20), ForceMode.Impulse);
            }
            if (crouched == false && canJump == false)
            {
                jumpForce = 50.0f;
            }
        }
        
    }
    void JumpTimer()
    {
        jumpTimer -= Time.deltaTime * 50;

        if (jumpTimer > 0 && Input.GetKey("up"))
        {
            jumping = true;
        }

        if (jumpTimer <= 0)
        {
            canJump = false;
            jumping = false;
        }
    }

    public void ControlSteer(int direction){
		
		if(!controllable){ return; }
		
		if(nearGround){ rotate = steering * direction; }
        if(!nearGround){ rotate = airSteering * direction; }
		
	}
	
	// Hit objects
	
	void OnTriggerEnter(Collider other){
		
		if(other.GetComponent<PhysicsObject>()){ other.GetComponent<PhysicsObject>().Hit(sphere.velocity); }
		
    }
	
	// Functions
	
	public void SetPosition(Vector3 position, Quaternion rotation){
		
		// Stop vehicle
		
		speed = rotate = 0.0f;
		
		sphere.velocity = Vector3.zero;
		sphere.position = position;
		
		// Set new position
		
		transform.position = position;
		transform.rotation = rotation;
		
	}
	
}