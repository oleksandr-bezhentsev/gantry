using Configs;
using Core;
using Media;
using Network;
using Screens;
using UnityEngine;

namespace Common
{
	public class BootstrapperAndroid : MonoBehaviour
	{
		[SerializeField] private MainConfig _mainConfig;
		[SerializeField] private Transform _canvasTransform;

		private ICommonFactory _factory;
		private ScreensManager _screensManager;
		private NetworkController _networkController;
		private MediaController _mediaController;

		private void Awake()
		{
			CameraHelper.Init();

			_factory = new CommonFactory();
#if UNITY_ANDROID
			_mediaController = new MediaController(_mainConfig.ThumbnailsConfig);

			_screensManager = new ScreensManager(_factory, _mainConfig, _canvasTransform);

			InitNetwork();
		}

		private void InitNetwork()
		{
			_networkController = new NetworkController();
#endif
		}
	}
}
