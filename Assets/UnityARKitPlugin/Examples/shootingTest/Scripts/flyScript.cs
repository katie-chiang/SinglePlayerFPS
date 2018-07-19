using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flyScript : MonoBehaviour {
    private float speed = 0.7f;
 
    Vector3 newPosition;
    
    //set the initial position of the ghost to be in front of the camera
    void Start ()
    {
        newPosition = Camera.main.transform.position + Vector3.forward * 6f;
    }
 
    void PositionChange()
    {   
        //fix this so that it doesn't cross the camera and stays a distance away
        //make it only stay in front of camera for now
        //(left/right, up/down, forward/backward)
        newPosition = new Vector3(Camera.main.transform.position.x + Random.Range(-3.5f, 3.5f), 
                                  Camera.main.transform.position.y + Random.Range(-1f, 3f), 
                                  Camera.main.transform.position.z + Random.Range(5f, 7f));
		//Debug.Log("position: " + newPosition.x + "," + newPosition.y + "," + newPosition.z);   
	}

    //all position is relative to the camera
    void Update ()
    {
        if(Vector3.Distance(transform.position, newPosition) < 0.2f){
            PositionChange();
		}
 
        transform.position = Vector3.Lerp(transform.position ,newPosition,Time.deltaTime*speed);

        //make the object always look at the camera
        transform.LookAt(Camera.main.transform);
    }
 
    // void LookAtCamera(Vector3 lookAtPosition)
    // {
    //     // float distanceX = lookAtPosition.x - transform.position.x;
    //     // float distanceY = lookAtPosition.y - transform.position.y;
    //     // float angle = Mathf.Atan2(distanceX, distanceY) * Mathf.Rad2Deg;
       
    //     // Quaternion endRotation = Quaternion.AngleAxis(angle, Vector3.up);
    //     // transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * rotateSpeed);

    //     Vector3 dir = lookAtPosition - transform.position;
    //     dir.y = 0; // keep the direction strictly horizontal
    //     Quaternion rot = Quaternion.LookRotation(dir);
    //     // slerp to the desired rotation over time
    //     transform.rotation = Quaternion.Slerp(transform.rotation, rot, speed * Time.deltaTime);
    // }

}
