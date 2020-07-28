using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlay.HUD {
	public class HUD : MonoBehaviour {
		public TextMeshProUGUI WorldText;
		public TextMeshProUGUI WinnerText;
		public TextMeshProUGUI LostText;

		public TextMeshProUGUI DefeatedText;
		public TextMeshProUGUI BaddieName;

		public Image[] HoldProgressDials;
		public Transform[] Heroes;

		public Image Ghost;
		private Color _ghostColour;

		public Button ContinueButton;

		private TextMeshProUGUI _levelCompleteText;

		private bool _playerWon;

		private Canvas _canvas;

//---------------------------------------------------------------------------------------
		private void Awake() {
//This is only because we hide the HUD in the scene by disabling the Canvas as it's just clutter
			_canvas = GetComponent<Canvas>();
			_canvas.enabled = true;

//Position the Progress dials correctly (Handles aspect ratio awkwardness)
//Also hide / reset them
			Vector2 halfScreenSize = _canvas.GetComponent<RectTransform>().sizeDelta / 2f;
			CanvasScaler canvasScaler = _canvas.GetComponent<CanvasScaler>();
			Vector2 scaleReference = new Vector2(canvasScaler.referenceResolution.x / Screen.width,
				canvasScaler.referenceResolution.y / Screen.height);

			Image dial;

			int len = HoldProgressDials.Length;
			int cnt = -1;
			while (++cnt!=len) {
				dial = HoldProgressDials[cnt];
				dialPositioner(Heroes[cnt].position, dial.GetComponent<RectTransform>(), halfScreenSize, scaleReference);
				dial.enabled = false;
				dial.fillAmount = 0;
			}
		}

//---------------------------------------------------------------------------------------
		public void Init() {
			WorldText.text="WORLD: "+ DataManager.Data.World;

			WinnerText.gameObject.SetActive(false);
			LostText.gameObject.SetActive(false);
			Ghost.gameObject.SetActive(false);

			DefeatedText.gameObject.SetActive(false);
			BaddieName.gameObject.SetActive(false);

			ContinueButton.gameObject.SetActive(false);

			_ghostColour = Ghost.color;

			enabled = false;
		}

//---------------------------------------------------------------------------------------
//Hurray!
		public void PlayerWon(string baddieName) {
			_levelCompleteText = WinnerText;

//Set up the additional text here
			BaddieName.text = baddieName;
			DefeatedText.alpha = 0;
			BaddieName.alpha = 0;
			DefeatedText.gameObject.SetActive(true);
			BaddieName.gameObject.SetActive(true);
			textSetUp();
		}

//---------------------------------------------------------------------------------------
//Booooo!
		public void BaddieWon() {
			_levelCompleteText = LostText;
//Just in case
			DefeatedText.gameObject.SetActive(false);
			BaddieName.gameObject.SetActive(false);
			textSetUp();
		}

//---------------------------------------------------------------------------------------
		public void DisplayProgressDial(int id) {
			HoldProgressDials[id].fillAmount = 0;
			HoldProgressDials[id].enabled = true;
		}

//---------------------------------------------------------------------------------------
		public void UpdateProgressDial(int id, float amount) {
			HoldProgressDials[id].fillAmount = amount;
		}

//---------------------------------------------------------------------------------------
		public void HideProgressDial(int id) {
			HoldProgressDials[id].enabled = false;
		}

//---------------------------------------------------------------------------------------
		public void OnContinueButtonPressed() {
			ContinueButton.interactable = false;
			GameController.Instance.OnContinueButtonPressed();
		}

//---------------------------------------------------------------------------------------
		private void Update() {
			fadeIn();
		}

//---------------------------------------------------------------------------------------
		private void textSetUp() {
			_levelCompleteText.alpha = 0;
			_levelCompleteText.gameObject.SetActive(true);

			_ghostColour.a = 0;
			Ghost.color = _ghostColour;
			Ghost.gameObject.SetActive(true);

			enabled = true;
		}

//---------------------------------------------------------------------------------------
		private void fadeIn() {
//Fade up the ghost
			_ghostColour.a += 1f * Time.deltaTime;
			if (_ghostColour.a <= 0.5f) {
				Ghost.color = _ghostColour;
			}

//Now the text
			_levelCompleteText.alpha += 2f * Time.deltaTime;

//It doesn't do any harm to fade these up if they're displayed or not
			float alpha = _levelCompleteText.alpha;
			DefeatedText.alpha = alpha;
			BaddieName.alpha = alpha;

			if (_levelCompleteText.alpha >= 1) {
				_levelCompleteText.alpha = 1;
				enabled = false;

//Turn on the continue button (We're not fading that in  :) )
				ContinueButton.gameObject.SetActive(true);
			}
		}

//---------------------------------------------------------------------------------------
		private void dialPositioner(Vector2 heroPosition, RectTransform rectTransform, Vector2 halfScreenSize,Vector2
			scaleReference) {
			Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, heroPosition);
			screenPoint.Scale(scaleReference);
			rectTransform.anchoredPosition = screenPoint - halfScreenSize;
		}

	}
}
