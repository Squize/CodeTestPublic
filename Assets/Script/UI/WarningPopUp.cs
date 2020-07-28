using UnityEngine;
using Utils;

namespace UI {
	public class WarningPopUp : MonoBehaviour {
		private Canvas _canvas;
		private CanvasGroup _canvasGroup;

//---------------------------------------------------------------------------------------
		private void Awake() {
			_canvas = GetComponent<Canvas>();
			_canvasGroup = GetComponent<CanvasGroup>();
			_canvasGroup.alpha = 0;
			_canvas.enabled = false;
		}

//---------------------------------------------------------------------------------------
		private void OnEnable() {
			CharacterSelect.OnDisplayingWarningOverlay += displayWarning;
		}

//---------------------------------------------------------------------------------------
		private void OnDisable() {
			CharacterSelect.OnDisplayingWarningOverlay -= displayWarning;
		}

//---------------------------------------------------------------------------------------
		public void OnCloseButtonPressed() {
			Fader.Instance.FadeDown(_canvasGroup, 0.1f, hide);
		}

//---------------------------------------------------------------------------------------
		private void hide() {
			_canvas.enabled = false;
		}

//---------------------------------------------------------------------------------------
		private void displayWarning() {
			_canvasGroup.alpha = 0;
			_canvas.enabled = true;
			Fader.Instance.FadeUp(_canvasGroup, 0.2f, null);
		}

	}
}
