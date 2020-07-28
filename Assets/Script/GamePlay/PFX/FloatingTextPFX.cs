using TMPro;
using UnityEngine;

namespace GamePlay.PFX {
	public class FloatingTextPFX : MonoBehaviour {
		[HideInInspector]
		public int id;

		private TextMeshProUGUI _textInstance;
		private RectTransform _rectTransform;

		private enum state {
			InitalWait,
			FadingIn,
			Floating,
			Wait,
			FadingOut
		}

		private state _currentState;

		private float _startDelay;
		private float _waitDelay;
		private int _floatingCnt;

		[HideInInspector]
		public PFXHandler Owner;


//---------------------------------------------------------------------------------------
		public void Init(string copy, Vector2 pos,Color32 colour,float delay=0) {
			if (_textInstance == null) {
				_textInstance = GetComponent<TextMeshProUGUI>();
				_rectTransform = GetComponent<RectTransform>();
			}

			_textInstance.text = copy;
			_textInstance.color = colour;
			_textInstance.alpha = 0;

			transform.localScale = Vector2.one;

//Going to offset the text slightly
			pos.y += Random.Range(0, 2);

			_rectTransform.position = pos;

			_startDelay = delay;
			if (_startDelay == 0) {
				_currentState = state.FadingIn;
			}
			else {
				_currentState = state.InitalWait;
			}

			_floatingCnt = 0;
			_waitDelay = 0;

			gameObject.SetActive(true);
		}

//---------------------------------------------------------------------------------------
		private void Update() {
//Sigh, oh to be able to use DOTween
			switch (_currentState) {
				case state.InitalWait:
					wait();
					break;
				case state.FadingIn:
					fadeIn();
					break;
				case state.Floating:
					floating();
					break;
				case state.Wait:
					waiting();
					break;
				case state.FadingOut:
					fadeOut();
					break;
			}
		}

//---------------------------------------------------------------------------------------
		private void wait() {
			_startDelay -= Time.deltaTime;
			if (_startDelay <= 0) {
				_currentState = state.FadingIn;
			}
		}

//---------------------------------------------------------------------------------------
		private void fadeIn() {
			_textInstance.alpha += 2f * Time.deltaTime;
			if (_textInstance.alpha >= 1) {
				_textInstance.alpha = 1;
				_currentState = state.Floating;
			}
		}

//---------------------------------------------------------------------------------------
		private void floating() {
			var pos = _rectTransform.position;
			pos.y += 8 * Time.deltaTime;
			_rectTransform.position = pos;

			if (++_floatingCnt == 10) {
				_currentState = state.Wait;
			}
		}

//---------------------------------------------------------------------------------------
		private void waiting() {
			float delta = Time.deltaTime;

			var pos = _rectTransform.position;
			pos.y += delta;
			_rectTransform.position = pos;

			_waitDelay += delta;
			if (_waitDelay > 0.7f) {
				_currentState = state.FadingOut;
			}
		}

//---------------------------------------------------------------------------------------
		private void fadeOut() {
			float delta = Time.deltaTime;
//Float away whilst fading
			var pos = _rectTransform.position;
			pos.y += delta;
			_rectTransform.position = pos;

			_textInstance.alpha -= 3 * delta;
			if (_textInstance.alpha <= 0) {
				_textInstance.alpha = 0;
				gameObject.SetActive(false);
				returnToPool();
			}
		}

//---------------------------------------------------------------------------------------
		private void returnToPool() {
			Owner.returnFloatingTextPFXToPool(id);
		}

	}
}
