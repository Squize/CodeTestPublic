using Data;
using Data.DataObjects;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI {
	public class CharacterPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
		public delegate void CharacterSelectionChanged(CharacterPanel thisInstance);
		public static event CharacterSelectionChanged OnCharacterSelectionChanged;

		public delegate void CharacterPopUpRequested(int id,Sprite Avatar);
		public static event CharacterPopUpRequested OnCharacterPopUpRequested;

		public Image SelectedBorder;
		public Image Avatar;
		public TextMeshProUGUI Name;

		public Image PadLock;

		public Image HoldProgressDial;

		public bool IsSelected {
			get { return SelectedBorder.enabled; }
			set { SelectedBorder.enabled = value; }
		}

		private int id;
		public int ID {
			get { return id; }
		}

		private bool _isDown;
		private float _heldCnt;
		private float _fillSpeed=1.5f;

//---------------------------------------------------------------------------------------
		public void Init(Character_SO charData,Sprite avatarSprite, bool selected) {
			SelectedBorder.enabled = selected;
			Name.text = charData.Name;
			Avatar.sprite = avatarSprite;

			id = charData.ID;

			HoldProgressDial.fillAmount = 0;
			HoldProgressDial.enabled = false;

			PadLock.gameObject.SetActive(!charData.IsLocked);
		}

//---------------------------------------------------------------------------------------
		public void OnPointerDown(PointerEventData eventData) {
			_isDown = true;
			_heldCnt = 0;
		}

//---------------------------------------------------------------------------------------
		public void OnPointerUp(PointerEventData eventData) {
			_isDown = false;

			if (HoldProgressDial.enabled) {
//The player was holding the panel down, so don't change the selection
				HoldProgressDial.enabled = false;
				HoldProgressDial.fillAmount = 0;
			}
			else {
//Is this character locked ? If so we don't do anything now
				if (PadLock.gameObject.activeSelf) {
					return;
				}

//Fire off our event to show the player has toggled this character
				if (OnCharacterSelectionChanged != null) {
					OnCharacterSelectionChanged(this);
				}
			}
		}

//---------------------------------------------------------------------------------------
		private void Update() {
			if (_isDown==false) {
				return;
			}

			float delta = Time.deltaTime;

			if (HoldProgressDial.enabled) {
//We're already displaying the dial so increase it
				HoldProgressDial.fillAmount += _fillSpeed * delta;

				if (HoldProgressDial.fillAmount >= 1) {
					_isDown = false;
					if (OnCharacterPopUpRequested != null) {
						OnCharacterPopUpRequested(id, Avatar.sprite);
					}
				}
			}
			else {
				_heldCnt += delta;
				if (_heldCnt > 0.3f) {
					HoldProgressDial.enabled = true;
				}
			}
		}
	}
}
