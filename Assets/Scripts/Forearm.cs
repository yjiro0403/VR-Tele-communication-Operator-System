using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forearm : MonoBehaviour {
    [SerializeField]
    Transform Hand;
    [SerializeField]
    float distVal = 1.0f;
    //float R, R2; //R:半径 R2:直径
    [SerializeField]
    Vector3 test;

	// Use this for initialization
	void Start () {
        //R2 = transform.localScale.y;
        //R = R2 / 2;
    }
	
	// Update is called once per frame
	void Update () {
        //Vector3 midiumPos  = (Hand.position - UpperArm.position) / 2.0f;
        float dist = Mathf.Sqrt((Hand.position - transform.position).sqrMagnitude);
        //float dist = Vector3.Distance(UpperArm.position, Hand.position);
        //Debug.Log(dist);
        //transform.position = midiumPos;
        transform.localScale = new Vector3(1.0f, dist*distVal, 1.0f);
        //transform.LookAt(Hand,test);
        //transform.LookAt(Hand);
        transform.up = Hand.position - transform.position;

        //transform.rotation = Quaternion.LookRotation(Hand.position - transform.position);
    }
}
