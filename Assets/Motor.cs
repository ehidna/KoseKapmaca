using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Motor : NetworkBehaviour {
    
    Transform mytransform;
    Vector3 velocity;
    Vector3 bestGuessPosition;
    float ourLatency = 0.05f;//calculate later some client side to server latency
    float latencySmoothingFactor = 10;
	
	void Start () {
        mytransform = transform;		
	}

    // Update is called once per frame
    //void FixedUpdate () {
    //       if(Input.anyKey){
    //           MotorAngle(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    //       }
    //}

    //void MotorAngle(float x, float y){
    //    Vector3 tempVect = new Vector3(x, y, 0) * Time.deltaTime * velocity;


    //    mytransform.position += tempVect;

    //}
    void Update(){
        if(hasAuthority == false){
            velocity = Vector3.zero;
            bestGuessPosition = bestGuessPosition + (velocity * Time.deltaTime);
            mytransform.position = Vector3.Lerp(mytransform.position, bestGuessPosition, Time.deltaTime * latencySmoothingFactor);
            return;
        }

        mytransform.Translate(velocity * Time.deltaTime);

        if (Input.anyKey)
        {
            velocity = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
            CmdUpdateVelocity(velocity, mytransform.position);
        }else{
            velocity = Vector3.zero;
        }
    }

    [Command]
    void CmdUpdateVelocity(Vector3 v, Vector3 p){
        mytransform.position = p;
        velocity = v;
        RpcUpdateVelocity(velocity, mytransform.position);
    }

    [ClientRpc]
    void RpcUpdateVelocity(Vector3 v, Vector3 p){
        if(hasAuthority){
            return;
        }
        velocity = v;
        bestGuessPosition = p + (velocity * ourLatency);

    }

}
