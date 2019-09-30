using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Bonesystem : MonoBehaviour
{
    GameObject bodyObj;
    int jointNum = 25; //Kinect.JointType.ThumbRight
    public Vector3 transferAmount = new Vector3(1.5f, 1.5f, 3.0f);
    GameObject[] avatarJoints;
    float[] receivedFloats;

    //UDP Setting
    public int listenPort = 8000;
    private static bool received = false;
    const char splitSymbol = ',';

    LineRenderer legLine;
    LineRenderer spineLine;
    LineRenderer armLine;


    private struct UdpState
    {
        public IPEndPoint endPoint;
        public UdpClient udpClient;
    }

    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
    };

    private void SetJoints()
    {
        //左右逆になってるからコードで整合性をとる
        avatarJoints[0] = GameObject.Find("Body/SpineBase");
        avatarJoints[1] = GameObject.Find("Body/SpineMid");
        avatarJoints[2] = GameObject.Find("Body/Neck");
        avatarJoints[3] = GameObject.Find("Body/Head");
        avatarJoints[4] = GameObject.Find("Body/ShoulderRight");
        avatarJoints[5] = GameObject.Find("Body/ElbowRight");
        avatarJoints[6] = GameObject.Find("Body/WristRight");
        avatarJoints[7] = GameObject.Find("Body/HandRight");
        avatarJoints[8] = GameObject.Find("Body/ShoulderLeft");
        avatarJoints[9] = GameObject.Find("Body/ElbowLeft");
        avatarJoints[10] = GameObject.Find("Body/WristLeft");
        avatarJoints[11] = GameObject.Find("Body/HandLeft");
        avatarJoints[12] = GameObject.Find("Body/HipRight");
        avatarJoints[13] = GameObject.Find("Body/KneeRight");
        avatarJoints[14] = GameObject.Find("Body/AnkleRight");
        avatarJoints[15] = GameObject.Find("Body/FootRight");
        avatarJoints[16] = GameObject.Find("Body/HipLeft");
        avatarJoints[17] = GameObject.Find("Body/KneeLeft");
        avatarJoints[18] = GameObject.Find("Body/AnkleLeft");
        avatarJoints[19] = GameObject.Find("Body/FootLeft");
        avatarJoints[20] = GameObject.Find("Body/SpineShoulder");
        avatarJoints[21] = GameObject.Find("Body/HandTipRight");
        avatarJoints[22] = GameObject.Find("Body/ThumbRight");
        avatarJoints[23] = GameObject.Find("Body/HandTipLeft");
        avatarJoints[24] = GameObject.Find("Body/ThumbLeft");
    }

    void Start()
    {
        receivedFloats = new float[75]; //最初の ,の前の空白を除
        StartCoroutine(ReceiveBone());
        Debug.Log("UDP Start");
        //_UdpCom = new UDPCom();
        //receivedBody = CreateBodyObject();
        //GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //body = new Kinect.Body;
        bodyObj = new GameObject();
        bodyObj = GameObject.Find("Body");
        avatarJoints = new GameObject[jointNum];
        SetJoints();

        legLine = avatarJoints[19].GetComponent<LineRenderer>();
        spineLine = avatarJoints[0].GetComponent<LineRenderer>();
        armLine = avatarJoints[11].GetComponent<LineRenderer>();
        legLine.SetVertexCount(9);
        legLine.SetWidth(1.0f,0.0f);
        spineLine.SetVertexCount(5);
        spineLine.SetWidth(1.0f, 0.0f);
        armLine.SetVertexCount(9);
        armLine.SetWidth(1.0f, 0.0f);

    }

    void Update()
    {

        RefreshBodyObject(bodyObj);

    }

    private void RefreshBodyObject(GameObject bodyObject)
    {

        //Debug.Log("receivedBodies:" + receivedFloats.Length);

        for (int i = 0; i< jointNum; i++) {
            avatarJoints[i].transform.localPosition = new Vector3(receivedFloats[i*3] * -1 * transferAmount.x, receivedFloats[(i * 3)+1] * transferAmount.y, receivedFloats[(i * 3)+2] * -1 * transferAmount.z);
        }

        legLine.SetPosition(0, avatarJoints[19].transform.position);
        legLine.SetPosition(1, avatarJoints[18].transform.position);
        legLine.SetPosition(2, avatarJoints[17].transform.position);
        legLine.SetPosition(3, avatarJoints[16].transform.position);
        legLine.SetPosition(4, avatarJoints[0].transform.position);
        legLine.SetPosition(5, avatarJoints[12].transform.position);
        legLine.SetPosition(6, avatarJoints[13].transform.position);
        legLine.SetPosition(7, avatarJoints[14].transform.position);
        legLine.SetPosition(8, avatarJoints[15].transform.position);

        spineLine.SetPosition(0, avatarJoints[0].transform.position);
        spineLine.SetPosition(1, avatarJoints[1].transform.position);
        spineLine.SetPosition(2, avatarJoints[20].transform.position);
        spineLine.SetPosition(3, avatarJoints[2].transform.position);
        spineLine.SetPosition(4, avatarJoints[3].transform.position);

        armLine.SetPosition(0, avatarJoints[11].transform.position);
        armLine.SetPosition(1, avatarJoints[10].transform.position);
        armLine.SetPosition(2, avatarJoints[9].transform.position);
        armLine.SetPosition(3, avatarJoints[8].transform.position);
        armLine.SetPosition(4, avatarJoints[20].transform.position);
        armLine.SetPosition(5, avatarJoints[4].transform.position);
        armLine.SetPosition(6, avatarJoints[5].transform.position);
        armLine.SetPosition(7, avatarJoints[6].transform.position);
        armLine.SetPosition(8, avatarJoints[7].transform.position);
    }

    private static Vector3 GetVector3FromJoint(Vector3 joint)
    {
        return joint;
    }

    public void ReceiveCallback(System.IAsyncResult ar)
    {
        UdpClient udpClient = ((UdpState)(ar.AsyncState)).udpClient;
        IPEndPoint endPoint = ((UdpState)(ar.AsyncState)).endPoint;
        var receiveBytes = udpClient.EndReceive(ar, ref endPoint);
        var receiveString = Encoding.UTF8.GetString(receiveBytes);

        Debug.Log(string.Format("Received: {0}", receiveString));

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

        while (true)
        {
            received = false;

            udpClient.BeginReceive(new System.AsyncCallback(ReceiveCallback), udpState);

            while (!received)
            {
                yield return null;
            }

        }

        //yield return null;
    }
}
