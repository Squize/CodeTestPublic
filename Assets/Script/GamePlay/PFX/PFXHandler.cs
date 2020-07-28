using UnityEngine;

namespace GamePlay.PFX {
	public class PFXHandler : MonoBehaviour {
		public FloatingTextPFX FloatingTextPFX;

		public Color32 AttackColour;
		public Color32 DamageColour;
		public Color32 CritColour;

		private FloatingTextPFX[] _text_Pool;
		private int[] _text_Offset;
		private int _numberOfFloatingText = 16;

//---------------------------------------------------------------------------------------
		private void Awake() {
			_text_Pool = new FloatingTextPFX[_numberOfFloatingText];
			_text_Offset = new int[_numberOfFloatingText];

			GameObject source = FloatingTextPFX.gameObject;
			GameObject clone;
			FloatingTextPFX scriptRef;
			Transform parent = FloatingTextPFX.transform.parent;

			int cnt = -1;
			while (++cnt != _numberOfFloatingText) {
				clone = Instantiate(source);
				clone.SetActive(false);
				clone.transform.SetParent(parent);
				clone.hideFlags = HideFlags.HideInHierarchy;

				scriptRef = clone.GetComponent<FloatingTextPFX>();
				scriptRef.id = cnt;
				scriptRef.Owner = this;
				_text_Pool[cnt] = scriptRef;
				_text_Offset[cnt] = cnt;
			}

			Destroy(source.gameObject);
		}

//---------------------------------------------------------------------------------------
		public void RequestXPText(string copy, Vector2 pos) {
			var pfx = getFloatingTextPFX();
			if (pfx != null) {
				pfx.Init(copy, pos,Color.white,0.6f);
			}
		}

//---------------------------------------------------------------------------------------
		public void RequestAttackText(string copy, Vector2 pos) {
			var pfx = getFloatingTextPFX();
			if (pfx != null) {
				pfx.Init(copy, pos, AttackColour);
			}
		}

//---------------------------------------------------------------------------------------
		public void RequestDamageText(string copy, Vector2 pos) {
			var pfx = getFloatingTextPFX();
			if (pfx != null) {
				pfx.Init(copy, pos, DamageColour,0.3f);
			}
		}

//---------------------------------------------------------------------------------------
		public void RequestCritText(Vector2 pos) {
			var pfx = getFloatingTextPFX();
			if (pfx != null) {
				pfx.Init("CRIT!", pos, CritColour, 0.5f);
			}
		}

//---------------------------------------------------------------------------------------
		public void RequestLevelUpText(Vector2 pos) {
			var pfx = getFloatingTextPFX();
			if (pfx != null) {
				pfx.Init("LEVELED UP!", pos, Color.white);
			}
		}

//---------------------------------------------------------------------------------------
		public void returnFloatingTextPFXToPool(int id) {
			_text_Offset[id] = id;
		}

//---------------------------------------------------------------------------------------
		private FloatingTextPFX getFloatingTextPFX() {
			int cnt = -1;
			while (++cnt != _numberOfFloatingText) {
				if (_text_Offset[cnt] != -1) {
					_text_Offset[cnt] = -1;
					return _text_Pool[cnt];
				}
			}

			return null;
		}

	}
}
