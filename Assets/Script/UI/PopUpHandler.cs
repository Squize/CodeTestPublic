using Data;
using Data.DataObjects;
using GamePlay.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI {
	public class PopUpHandler : MonoBehaviour {
		public delegate void PopUpClosed();
		public static event PopUpClosed OnPopUpClosed;

		public Image Avatar;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI AttackStat;
		public TextMeshProUGUI DefenceStat;
		public Image XPBar;
		public TextMeshProUGUI LevelStat;


		private Canvas _canvas;
		private CanvasGroup _canvasGroup;

		private DataManager _dataManager;

//---------------------------------------------------------------------------------------
		private void Awake() {
			_canvas = GetComponent<Canvas>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.alpha = 0;
			_canvas.enabled = false;
		}

//---------------------------------------------------------------------------------------
		private void OnEnable() {
			CharacterPanel.OnCharacterPopUpRequested += OnCharacterPopUpRequested;
			Hero.OnCharacterPopUpRequested += OnCharacterPopUpRequested;
		}

//---------------------------------------------------------------------------------------
		private void OnDisable() {
			CharacterPanel.OnCharacterPopUpRequested -= OnCharacterPopUpRequested;
			Hero.OnCharacterPopUpRequested -= OnCharacterPopUpRequested;
		}

//---------------------------------------------------------------------------------------
		public void OnCloseButtonPressed() {
			Fader.Instance.FadeDown(_canvasGroup, 0.1f, hide);
		}

//---------------------------------------------------------------------------------------
		private void hide() {
			_canvas.enabled = false;

//This is only used in-game, it re-enables the Heroes to listen for clicks
			if (OnPopUpClosed != null) {
				OnPopUpClosed();
			}
		}

//---------------------------------------------------------------------------------------
		private void OnCharacterPopUpRequested(int charID, Sprite AvatarSprite) {
			if (_dataManager == null) {
				_dataManager = DataManager.Data;
			}

//Populate the pop-up with our character data
			Avatar.sprite = AvatarSprite;
			populateDataAndDisplay(_dataManager.PlayerCharacters[charID]);
		}

//---------------------------------------------------------------------------------------
		private void OnCharacterPopUpRequested(Hero heroRef) {
			if (_dataManager == null) {
				_dataManager = DataManager.Data;
			}

//Populate the pop-up with our character data from the Hero class
			Avatar.sprite = heroRef.Avatar;
			populateDataAndDisplay(heroRef.CharacterData);
		}

//---------------------------------------------------------------------------------------
		private void populateDataAndDisplay(Character_SO charData) {
			Name.text = charData.Name;
			AttackStat.text = charData.Attack.ToString();
			DefenceStat.text = charData.Defence.ToString();
			LevelStat.text = charData.Level.ToString();

//The XP bar is a little more convoluted to work out, because it seems I'm treating having a week to do this
//like a year
			int nextXPLevel = _dataManager.Ranks.GetNextRankXP(charData.Level - 1);
//So that's the next XP we need to bump a level, work out our percentage of being the way there
			int currentXP = charData.XP;
			float perc = ((float) currentXP / nextXPLevel);
			XPBar.fillAmount = perc;

//All done so we can display it now
			_canvasGroup.alpha = 0;
			_canvas.enabled = true;
			Fader.Instance.FadeUp(_canvasGroup, 0.2f, null);
		}

	}
}
