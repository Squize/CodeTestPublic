using UnityEngine;

namespace Data.DataObjects {
	public class Rank_SO : ScriptableObject {
		private int[] _XPLevels;

//---------------------------------------------------------------------------------------
		/// <summary>
		/// Creates and Init's this ScriptableObject, and returns it
		/// </summary>
		/// <usage>
		/// Rank_SO instance = Rank_SO.CreateInstance();
		/// </usage>
		/// <returns>Rank_SO</returns>
		public static Rank_SO CreateInstance() {
			var data = ScriptableObject.CreateInstance<Rank_SO>();
			data.Init();
			return data;
		}

//---------------------------------------------------------------------------------------
		private void Init() {
//@see http://titanfall.gamepedia.com/Experience
			_XPLevels = new int[52];
			_XPLevels[0] = 0;
			_XPLevels[1] = 1500;
			_XPLevels[2] = 3300;
			_XPLevels[3] = 5800;
			_XPLevels[4] = 8800;
			_XPLevels[5] = 12500;
			_XPLevels[6] = 16700;
			_XPLevels[7] = 21500;
			_XPLevels[8] = 26900;
			_XPLevels[9] = 32900;
			_XPLevels[10] = 39400;
			_XPLevels[11] = 46500;
			_XPLevels[12] = 54100;
			_XPLevels[13] = 62300;
			_XPLevels[14] = 71100;
			_XPLevels[15] = 80300;
			_XPLevels[16] = 90100;
			_XPLevels[17] = 100500;
			_XPLevels[18] = 111300;
			_XPLevels[19] = 122700;
			_XPLevels[20] = 134700;
			_XPLevels[21] = 147100;
			_XPLevels[22] = 160100;
			_XPLevels[23] = 173600;
			_XPLevels[24] = 187600;
			_XPLevels[25] = 202100;
			_XPLevels[26] = 217100;
			_XPLevels[27] = 232600;
			_XPLevels[28] = 248700;
			_XPLevels[29] = 265200;
			_XPLevels[30] = 282300;
			_XPLevels[31] = 265200;
			_XPLevels[32] = 282300;
			_XPLevels[33] = 299800;
			_XPLevels[34] = 317900;
			_XPLevels[35] = 336500;
			_XPLevels[36] = 356000;
			_XPLevels[37] = 377000;
			_XPLevels[38] = 399000;
			_XPLevels[39] = 422000;
			_XPLevels[40] = 446000;
			_XPLevels[41] = 471000;
			_XPLevels[42] = 497000;
			_XPLevels[43] = 524000;
			_XPLevels[44] = 552000;
			_XPLevels[45] = 581000;
			_XPLevels[46] = 612000;
			_XPLevels[47] = 650000;
			_XPLevels[48] = 695000;
			_XPLevels[49] = 750000;
			_XPLevels[50] = 818000;
			_XPLevels[51] = 900000;

//Note, I've divided by 20 because we're not super generous with our XP in this game
			int cnt = -1;
			int len = _XPLevels.Length;
			while (++cnt!=len) {
				_XPLevels[cnt] /= 20;
			}
		}

//---------------------------------------------------------------------------------------
		public int GetNextRankXP(int currentLevel) {
			int nextRank = currentLevel + 1;
			if (nextRank >= _XPLevels.Length - 1) {
//This may well break, but if anyone gets to rank 50 in this then they have other things to worry about
				return -1;
			}

			return _XPLevels[nextRank];
		}

//---------------------------------------------------------------------------------------
		public int GetCurrentRankXP(int currentLevel) {
			return _XPLevels[currentLevel];
		}
	}
}
