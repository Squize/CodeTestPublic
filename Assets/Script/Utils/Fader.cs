using System;
using UnityEngine;

namespace Utils {
	public class Fader : MonoBehaviour {
		public static Fader Instance { get; private set; }

		private CanvasGroup _target;
		private float _lerpTime = 0.2f;		 //Time in seconds
		private float _currentLerpTime;

		private float _fadeDirection = 0;	//0 fade up, 1 fade down

		private Action _completeCallBack;

//---------------------------------------------------------------------------------------
		void Awake() {
			if (Instance == null) {
				DontDestroyOnLoad(gameObject);
				Instance = this;
				enabled = false;
			}
			else {
				if (Instance != this) {
					Destroy(gameObject);
				}
			}
		}

//---------------------------------------------------------------------------------------
		public void FadeUp(CanvasGroup targetRef, float duration,Action callBack) {
			_lerpTime = duration;
			_currentLerpTime = 0;
			_target = targetRef;
			_fadeDirection = 0;
			_completeCallBack += callBack;
			enabled = true;
		}

//---------------------------------------------------------------------------------------
		public void FadeDown(CanvasGroup targetRef, float duration, Action callBack) {
			_lerpTime = duration;
			_currentLerpTime = 0;
			_target = targetRef;
			_fadeDirection = 1;
			_completeCallBack += callBack;
			enabled = true;
		}

//---------------------------------------------------------------------------------------
		private void Update() {
			_currentLerpTime += Time.deltaTime;
			if (_currentLerpTime > _lerpTime) {
				_currentLerpTime = _lerpTime;
			}

			float perc = 1f - Mathf.Cos((_currentLerpTime / _lerpTime) * Mathf.PI * 0.5f); //Ease In
			_target.alpha = Mathf.Abs(perc- _fadeDirection);

			if (perc >= 1) {
				enabled = false;
				if (_completeCallBack != null) {
					_completeCallBack();
					_completeCallBack = null;
				}
			}
		}

//---------------------------------------------------------------------------------------

	}
}
