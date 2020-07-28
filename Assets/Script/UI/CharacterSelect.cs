using Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;

namespace UI {
	public class CharacterSelect : MonoBehaviour {
		public delegate void DisplayingWarningOverlay();

		public static event DisplayingWarningOverlay OnDisplayingWarningOverlay;

		public CharacterPanel CharPanelRef;

		public Transform Holder;

		public Sprite[] Avatars;

		public Button PlayButton;

		public Canvas CoverAllCanvas;
		public CanvasGroup CoverAllCanvasGroup;

		private CharacterPanel[] _panelStorage;
		private DataManager _dataManager;

//---------------------------------------------------------------------------------------
		private void Start() {
//Lets get all our lovely data so we can plot the panels
			_dataManager = DataManager.Data;

			int numberOfChars = _dataManager.NumberOfChars;
			_panelStorage = new CharacterPanel[numberOfChars];

			CharacterPanel scriptRef;
			GameObject gO;

//Create all our character panels and shove the data into them
			int cnt = -1;
			while (++cnt != numberOfChars) {
				gO = Instantiate(CharPanelRef.gameObject);
				gO.transform.SetParent(Holder);
				gO.transform.localScale = Vector2.one;

				scriptRef = gO.GetComponent<CharacterPanel>();
				scriptRef.Init(_dataManager.PlayerCharacters[cnt], Avatars[cnt], _dataManager.SelectedCharacters[cnt]);
				_panelStorage[cnt] = scriptRef;
			}
		}

//---------------------------------------------------------------------------------------
		private void OnEnable() {
			CharacterPanel.OnCharacterSelectionChanged += OnCharacterSelectionChanged;
		}

//---------------------------------------------------------------------------------------
		private void OnDisable() {
			CharacterPanel.OnCharacterSelectionChanged -= OnCharacterSelectionChanged;
		}

//---------------------------------------------------------------------------------------
		public void OnPlayButtonPressed() {
			CoverAllCanvasGroup.alpha = 0;
			CoverAllCanvas.enabled = true;
			Fader.Instance.FadeUp(CoverAllCanvasGroup, 0.3f, loadGame);
		}

//---------------------------------------------------------------------------------------
		private void loadGame() {
			SceneManager.LoadScene("GamePlay", LoadSceneMode.Single);
		}

//---------------------------------------------------------------------------------------
		private void OnCharacterSelectionChanged(CharacterPanel characterPanel) {
//Ok, is this CharacterPanel already selected ? If so we have it easy
			if (characterPanel.IsSelected) {
				_dataManager.ToggleSelectedCharacter(characterPanel.ID);
				characterPanel.IsSelected = false;
//Has the player deselected all their characters ?
				PlayButton.interactable = _dataManager.GetNumberOfSelectedCharacters() > 0;
			}
			else {
				PlayButton.interactable = true;		//Just in case

//So the player is trying to select this character, can they ?
				int numberAlreadySelected = _dataManager.GetNumberOfSelectedCharacters();
				if (numberAlreadySelected == 3) {
//Our squad is maxed out, so lets fire off the warning event
					if (OnDisplayingWarningOverlay != null) {
						OnDisplayingWarningOverlay();
					}

					return;
				}
				else {
//Yes they can!
					_dataManager.ToggleSelectedCharacter(characterPanel.ID);
					characterPanel.IsSelected = true;
				}
			}
		}

//---------------------------------------------------------------------------------------
	}
}
