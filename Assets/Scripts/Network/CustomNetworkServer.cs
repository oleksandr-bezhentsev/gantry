using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Network;
using UnityEngine;

public class CustomNetworkServer : MonoBehaviour {

    private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static readonly List<Socket> clientSockets = new List<Socket>();
    private static readonly List<SocketStruct> clientSocketStructList = new List<SocketStruct>(); 
    private const int BUFFER_SIZE = 2048;
    private const int PORT = 8888;
    private static readonly byte[] buffer = new byte[BUFFER_SIZE];

    [Serializable]
    public struct SocketStruct
    {
        public string id;
        public Socket socket;
    }

    void OnDestroy()
    {
        CloseAllSockets();
    }

    // Use this for initialization
    void Start ()
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        SetupServer();
#endif
    }

    // Update is called once per frame
    void Update () {
		if(Input.GetKeyDown(KeyCode.Space))
            SendCommandToID(0);
	}

    private static void SetupServer()
    {
        Debug.Log("Setting up server...");
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = ipHostInfo.AddressList[1];
        var endPoint = new IPEndPoint(IPAddress.Parse(NetworkController.IP), NetworkController.PORT);
        serverSocket.Bind(endPoint);
        serverSocket.Listen(0);
        serverSocket.BeginAccept(AcceptCallback, null);
        Debug.Log("Server setup complete on address: " + serverSocket.LocalEndPoint);
    }

    private static void CloseAllSockets()
    {
        foreach (Socket socket in clientSockets)
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        serverSocket.Close();
    }

    private static void AcceptCallback(IAsyncResult AR)
    {
        Socket socket;

        try {
            socket = serverSocket.EndAccept(AR);
        }
        catch (ObjectDisposedException)
        {
            return;
        }

        clientSockets.Add(socket);
        socket.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, socket);
        Debug.Log("Client connected, waiting for request...");
        serverSocket.BeginAccept(AcceptCallback, null);
    }

    private static void ReceiveCallback(IAsyncResult AR)
    {
        Socket current = (Socket)AR.AsyncState;
        int received;

        try {
            received = current.EndReceive(AR);
        }
        catch (SocketException)
        {
            Debug.Log("Client forcefully disconnected: ");
            SetUnRegister(current);
            current.Close();
            clientSockets.Remove(current);
            return;
        }

        if (received <= 0)
        {
            Debug.Log("Client forcefully disconnected: ");
            SetUnRegister(current);
            current.Close();
            clientSockets.Remove(current);
            return;
        }

        byte[] recBuf = new byte[received];
        Array.Copy(buffer, recBuf, received);
        string text = Encoding.ASCII.GetString(recBuf);
        Debug.Log("Server Received Text: " + text);

        SetRegister(current, text);
        current.BeginReceive(buffer, 0, BUFFER_SIZE, SocketFlags.None, ReceiveCallback, current);
    }

    private static void SetRegister(Socket socket, string text)
    {
        string[] subStrings = text.Split(',');
        if (subStrings[0].Contains("register"))
        {
            SocketStruct cs = new SocketStruct();
            cs.socket = socket;
            cs.id = subStrings[1];
            clientSocketStructList.Add(cs);
        }

        if (text.Contains("exit"))
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            clientSockets.Remove(socket);
            Debug.Log("Client forcefully disconnected: ");
        }
    }

    private static void SetUnRegister(Socket socket)
    {
        SocketStruct cl = clientSocketStructList.Find(c => c.socket == socket);
        clientSocketStructList.Remove(cl);
    }

    public static void SendCommandToID(int id)
    {
        SocketStruct cl = clientSocketStructList.Find(c => Convert.ToInt32(c.id) == id);
        string command = "OpenVRX";
        byte[] data = Encoding.ASCII.GetBytes(command);
        cl.socket.Send(data);
        Debug.Log("Command sent to Client");
        SetUnRegister(cl.socket);
    }

}
