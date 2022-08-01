using UnityEngine;

namespace Network
{
	public class NetworkController
	{
		private TCPTestClient _client;
		private TCPTestServer _server;

		public NetworkController()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			_server = new TCPTestServer();
#elif UNITY_ANDROID && !UNITY_EDITOR
			Debug.Log("Android");
			_client = new TCPTestClient();
#endif
		}

		public void SendMessage()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			_server.SendMessage();
#elif UNITY_ANDROID && !UNITY_EDITOR
			_client.SendMessage();
			Debug.Log("Android");
#endif
		}
	}
}
