using System.Collections;
using UnityEngine;

public class HmdControllerSend : MonoBehaviour {
    float[] positionFloats;

    UDPCom _udpCom;
    public int sendInterval = 10;
    int ct = 0;
    public string ipAddress = "192.168.10.4";

    public Transform hmd, controllerLeft, controllerRight, handLTip, handRTip;
    
     [SerializeField]
     Controller leftController, rightController;
           

	// Use this for initialization
	void Start () {
        //positionFloats = new float[3 * 6]; //hmd, controllerLR と x,y,z, 角度
        positionFloats = new float[(3 * 6)+2]; //hmd, controllerLR と x,y,z, 角度

        _udpCom = new UDPCom();
	}
	
	// Update is called once per frame
	void Update () {
        positionFloats[0] = hmd.transform.position.x;
        positionFloats[1] = hmd.transform.position.y;
        positionFloats[2] = hmd.transform.position.z;
        positionFloats[3] = hmd.transform.eulerAngles.x;
        positionFloats[4] = hmd.transform.eulerAngles.y;
        positionFloats[5] = hmd.transform.eulerAngles.z;

        positionFloats[6] = handLTip.transform.position.x;
        positionFloats[7] = handLTip.transform.position.y;
        positionFloats[8] = handLTip.transform.position.z;
        positionFloats[9] = controllerLeft.transform.localEulerAngles.x;
        positionFloats[10] = controllerLeft.transform.localEulerAngles.y;
        positionFloats[11] = controllerLeft.transform.localEulerAngles.z;

        positionFloats[12] = handRTip.transform.position.x;
        positionFloats[13] = handRTip.transform.position.y;
        positionFloats[14] = handRTip.transform.position.z;
        positionFloats[15] = controllerRight.transform.localEulerAngles.x;
        positionFloats[16] = controllerRight.transform.localEulerAngles.y;
        positionFloats[17] = controllerRight.transform.localEulerAngles.z;
        
        if (leftController.handStateCheck())
        {
            positionFloats[18] = 1.0f;
        }else { positionFloats[18] = 0.0f; }

        if (rightController.handStateCheck())
        {
            positionFloats[19] = 1.0f;
        }
        else { positionFloats[19] = 0.0f; }
        

        if (ct % sendInterval == 0){
            //Debug.Log("hmd x = " + positionFloats[3] + " y = " + positionFloats[4] + " z = " + positionFloats[5]);
            _udpCom.SendHmdPosition(positionFloats, ipAddress);
        }
        ct++;


    }
}
