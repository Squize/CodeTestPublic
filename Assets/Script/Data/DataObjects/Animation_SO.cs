using UnityEngine;

namespace Data.DataObjects {
	[CreateAssetMenu(fileName = "CharXAnimationFrames", menuName = "GYW/Character Animation Frames Asset")]
	public class Animation_SO : ScriptableObject {
		[SerializeField]
		public Sprite[] IdleFrames;

		[SerializeField]
		public Sprite[] WalkingFrames;
	}
}
