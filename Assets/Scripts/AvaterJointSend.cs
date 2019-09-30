using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class AvaterJointSend : MonoBehaviour{
    public int jointNum = 20; //Kinect.JointType.ThumbRight
    public float[] skipPosition; //送信しないボーン番号を入力
    public Vector3 transferAmount = new Vector3(1.5f, 1.5f, 3.0f);

    public Transform hmdPosition, handLeft, handRight;

    Transform[] bones;
    Transform bodyRoot;
    /*
    float Hips_y = -0.007280334f;
    float Spine_y = 0.2097466f;
    float Chest_y = 0.0944221f;
    float Neck_y = 0.2862355f;
    float Head_y = 0.06456243f;
    float Head_end_y = 0.1728274f;
    */
    //UDPCom _udpCom;

    // Public variables that will get matched to bones. If empty, the Kinect will simply not track it.
    public Transform HipCenter;
    public Transform Spine;
    public Transform ShoulderCenter;
    public Transform Neck;
    //	public Transform Head;

    public Transform ClavicleLeft;
    public Transform ShoulderLeft;
    public Transform ElbowLeft;
    public Transform HandLeft;
    public Transform FingersLeft;
    //	private Transform FingerTipsLeft = null;
    //	private Transform ThumbLeft = null;

    public Transform ClavicleRight;
    public Transform ShoulderRight;
    public Transform ElbowRight;
    public Transform HandRight;
    public Transform FingersRight;
    //	private Transform FingerTipsRight = null;
    //	private Transform ThumbRight = null;

    public Transform HipLeft;
    public Transform KneeLeft;
    public Transform FootLeft;
    //	private Transform ToesLeft = null;

    public Transform HipRight;
    public Transform KneeRight;
    public Transform FootRight;
    //	private Transform ToesRight = null;

    [Tooltip("The body root node (optional).")]
    public Transform BodyRoot;

    // Offset node this transform is relative to, if any (optional)
    //public GameObject OffsetNode;


    //UDP Setting
    public int listenPort = 8000;
    private static bool received = false;
    const char splitSymbol = ',';

    float[] receivedFloats;

    private struct UdpState {
        public IPEndPoint endPoint;
        public UdpClient udpClient;
    }

    public Transform _workerLooking;
    [SerializeField]
    Vector3 workerPositionOffset, workerRotationOffset;

    // Use this for initialization
    void Start () {
        //positionFloats = new float[jointNum * 3];
        //_udpCom = new UDPCom();

        //receivedFloats = new float[jointNum * 3];
        //作業者スポットライト
        receivedFloats = new float[(jointNum+1+2) * 3]; //jointNum + body + HMDpos,rotation

        StartCoroutine(ReceiveBone());
        Debug.Log("UDP Start");

        bones = new Transform[15];
        bones[0] = HipCenter;
        bones[1] = Spine;
        bones[2] = ShoulderCenter;
        bones[3] = Neck;
        //		bones[4] = Head;

        

        bones[5] = ShoulderLeft;
        //bones[6] = ElbowLeft; // 別プログラムでいじる(Hinge Joint)
        bones[7] = HandLeft;
        bones[8] = FingersLeft;
        //		bones[9] = FingerTipsLeft;
        //		bones[10] = ThumbLeft;

        bones[11] = ShoulderRight;
        //bones[12] = ElbowRight; // 別プログラムでいじる
        bones[13] = HandRight;
        bones[14] = FingersRight;
        //		bones[15] = FingerTipsRight;
        //		bones[16] = ThumbRight;

        //bones[17] = HipLeft;
        //bones[18] = KneeLeft;
        //bones[19] = FootLeft;
        //		bones[20] = ToesLeft;

        //bones[21] = HipRight;
        //bones[22] = KneeRight;
        //bones[23] = FootRight;
        //		bones[24] = ToesRight;

        // special bones
        //bones[25] = ClavicleLeft;
        //bones[26] = ClavicleRight;

        // body root and offset
        bodyRoot = BodyRoot;

        
    }

    // Update is called once per frame
    void Update () {
        //Debug.Log("HipLeft = " + bones[17].position.x);
        //bodyRoot.transform.localPosition = new Vector3(receivedFloats[0] - transferAmount.x, receivedFloats[1] - transferAmount.y, receivedFloats[2] - transferAmount.z);
        //bodyRoot.transform.localPosition = new Vector3(hmdPosition.localPosition.x, receivedFloats[1] - transferAmount.y, hmdPosition.localPosition.z);

        int j = 1;
        for(int i = 0; i < 27; i++) {
            while (SkipBoneCheck(i)) {
                i++;
            }
            if (i >= 27) {
                //作業者スポットライト
                _workerLooking.position = new Vector3(receivedFloats[j*3] + workerPositionOffset.x, receivedFloats[(j * 3) + 1] + workerPositionOffset.y, receivedFloats[(j * 3) + 2] + workerPositionOffset.z);
                _workerLooking.eulerAngles = new Vector3(receivedFloats[(j * 3)+3]+ workerRotationOffset.x, receivedFloats[(j * 3) + 4] + workerRotationOffset.y, receivedFloats[(j * 3) + 5] + workerRotationOffset.z);
                break; }
            bones[i].localEulerAngles = new Vector3(receivedFloats[j * 3], receivedFloats[(j * 3) + 1], receivedFloats[(j * 3) + 2]);
            j++;
        }

        bodyRoot.transform.localPosition = new Vector3(hmdPosition.localPosition.x - (bones[3].transform.position.x - bones[0].transform.position.x), hmdPosition.localPosition.y - (bones[3].transform.position.y - bones[0].transform.position.y) - 1.05f, hmdPosition.localPosition.z - (bones[3].transform.position.z - bones[0].transform.position.z));
        //Debug.Log("position head = " + bones[3].transform.position.y + " position hip = " + bones[0].transform.position.y);
        //bones[6].transform.LookAt(handLeft.transform);
        //bones[12].transform.LookAt(handRight.transform);
    }

    bool SkipBoneCheck(int checknum)
    {
        for (int i = 0; i < skipPosition.Length;i++) {
            if (checknum == skipPosition[i]) {
                //Debug.Log("Skipped : " + skipPosition[i]);
                return true;
            }
        }
        return false;
    }

    public void ReceiveCallback(System.IAsyncResult ar)
    {
        UdpClient udpClient = ((UdpState)(ar.AsyncState)).udpClient;
        IPEndPoint endPoint = ((UdpState)(ar.AsyncState)).endPoint;
        var receiveBytes = udpClient.EndReceive(ar, ref endPoint);
        var receiveString = Encoding.UTF8.GetString(receiveBytes);

        //Debug.Log(string.Format("Received: {0}", receiveString));

        string[] arrayData = receiveString.Split(splitSymbol);

        for (int i = 1; i < arrayData.Length; i++)
        {
            receivedFloats[i - 1] = float.Parse(arrayData[i]);
        }
        //Debug.Log("Bodies i = " + receivedFloats[0]);
        //Debug.Log("Bodies Length = " + receivedFloats.Length);

        received = true;
    }

    public IEnumerator ReceiveBone()
    {
        var endPoint = new IPEndPoint(IPAddress.Any, listenPort);
        var udpClient = new UdpClient(endPoint);
        var udpState = new UdpState();
        udpState.endPoint = endPoint;
        udpState.udpClient = udpClient;

        while (true){
            received = false;

            udpClient.BeginReceive(new System.AsyncCallback(ReceiveCallback), udpState);

            while (!received){
                yield return null;
            }
        }

        //yield return null;
    }
}
