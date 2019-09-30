using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRender : MonoBehaviour {
    [SerializeField]
    GameObject armLeft, armRight;
    [SerializeField]
    Transform handLeft, handRight;
    LineRenderer leftLine, rightLine;

	// Use this for initialization
	void Start () {
        leftLine = armLeft.GetComponent<LineRenderer>();
        leftLine.SetVertexCount(2);
        rightLine = armRight.GetComponent<LineRenderer>();
        rightLine.SetVertexCount(2);
    }

    // Update is called once per frame
    void Update () {
        //Vector3 handLPoint = handLeft.transform.position - new Vector3(0.0f, 0.1f, 0.0f);
        //Vector3 handRPoint = handRight.transform.position - new Vector3(0.0f, 0.1f, 0.0f);

        leftLine.SetPosition(0, armLeft.transform.position);
        leftLine.SetPosition(1, handLeft.transform.position);
        //leftLine.SetPosition(1, handLPoint);
        rightLine.SetPosition(0, armRight.transform.position);
        rightLine.SetPosition(1, handRight.transform.position);
        //rightLine.SetPosition(1, handRPoint);
    }
}
