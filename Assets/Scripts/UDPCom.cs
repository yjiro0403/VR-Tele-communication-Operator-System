using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

public class UDPCom : MonoBehaviour
{
    //UDP Setting
    public int listenPort = 8080;
    private static bool received = false;
    public float[] bodies;

    private struct UdpState
    {
        public IPEndPoint endPoint;
        public UdpClient udpClient;
    }

    const int jointNum = 12;
    const char splitSymbol = ',';

    public float[] GetBody()
    {
        Debug.Log("UDPCOM.cs : received floats " + bodies.Length);
        return bodies;
    }

    void Start()
    {
        //bodies = new float[75]; //最初の ,の前の空白を除外

        StartCoroutine(ReceiveBone());
        Debug.Log("UDP Start");
    }

    public void SendHmdPosition(float[] position, string ipAddress)
    {
        string body = "";

        var remote = new IPEndPoint(
          IPAddress.Parse(ipAddress),
          listenPort);

        //var message = Encoding.UTF8.GetBytes("Hello world !");

        var client = new UdpClient(listenPort);
        client.Connect(remote);

        //Bodyをstringにして変換
        for (int i = 0; i < position.Length; i++) {
            body += splitSymbol + position[i].ToString();
        }
        var message = Encoding.UTF8.GetBytes(body);
        client.Send(message, message.Length);

        client.Close();
    }

    public void ReceiveCallback(System.IAsyncResult ar) {
        UdpClient udpClient = ((UdpState)(ar.AsyncState)).udpClient;
        IPEndPoint endPoint = ((UdpState)(ar.AsyncState)).endPoint;
        var receiveBytes = udpClient.EndReceive(ar, ref endPoint);
        var receiveString = Encoding.UTF8.GetString(receiveBytes);

        //Debug.Log(string.Format("Received: {0}", receiveString));
                   
        string[] arrayData = receiveString.Split(splitSymbol);

        for (int i = 1; i < arrayData.Length; i++) {
            bodies[i-1] = float.Parse(arrayData[i]);
        }
        Debug.Log("Bodies i = " + bodies[0]);
        Debug.Log("Bodies Length = " + bodies.Length);

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
            Debug.Log("Bodies2 Length = " + GetBody().Length);

            udpClient.BeginReceive(new System.AsyncCallback(ReceiveCallback),udpState);

            while (!received)
            {
                yield return null;
            }

        }

        //yield return null;
    }


}
