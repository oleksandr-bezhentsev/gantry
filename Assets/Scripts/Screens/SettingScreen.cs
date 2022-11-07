using System;
using Core;
using Library;
using UnityEngine;
using UnityEngine.UI;

namespace Screens
{
	public class SettingScreen : MonoBehaviour
	{
		[SerializeField] private OptionsMenu _optionsMenu;
		[SerializeField] private LibraryScreen _libraryScreen;
		[SerializeField] private Button _exitButton;
		
		public void Init(ICommonFactory factory, Action quitAction, OptionsSettings settings, Transform debugPanel)
		{
			_optionsMenu.Init(settings, debugPanel);
			_libraryScreen.Init(factory);
			
			_exitButton.onClick.AddListener(() => Quit(quitAction));
		}

		private void Quit(Action quitAction)
		{
			_optionsMenu.SaveAndExit();
			_libraryScreen.SaveAndExit();
			
			_exitButton.onClick.RemoveAllListeners();
			
			quitAction?.Invoke();
			
			Destroy(gameObject);
		}
	}
}