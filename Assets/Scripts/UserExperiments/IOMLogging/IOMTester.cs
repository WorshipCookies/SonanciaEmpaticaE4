using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;

public class IOMTester : MonoBehaviour {

    private string serverIP = "127.0.0.1";
    private string Port = "8888";

    private IPEndPoint ip;
    private Socket s;

    // Use this for initialization
    void Start () {
        ip = new IPEndPoint(IPAddress.Parse(serverIP), int.Parse(Port));
        s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        s.Connect(ip);
    }

    public void IOMStart()
    {
        byte[] data = new byte[1024];

        data = Encoding.ASCII.GetBytes(DateTime.Now.TimeOfDay + "-START");
        s.SendTo(data, data.Length, SocketFlags.None, ip);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
