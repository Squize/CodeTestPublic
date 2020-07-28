using UnityEngine;

namespace Data.DataObjects {
	public class Character_SO : ScriptableObject {
		public string Name;
		public int Attack;
		public int Defence;
		public int XP;
		public int Level;

		public int ID;

		public int UnlockWorld;
		public bool IsLocked;

//---------------------------------------------------------------------------------------
/// <summary>
/// Creates and Init's this ScriptableObject, and returns it
/// </summary>
/// <usage>
/// Character_SO instance = Character_SO.CreateInstance(ID);
/// </usage>
/// <returns>Character_SO</returns>
		public static Character_SO CreateInstance(int idRef) {
			var data = ScriptableObject.CreateInstance<Character_SO>();
			data.init(idRef);
			return data;
		}

//---------------------------------------------------------------------------------------
		private void init(int idRef) {
			ID = idRef;
		}

//---------------------------------------------------------------------------------------
		public void LoadData(string json) {
			JsonUtility.FromJsonOverwrite(json, this);
		}

//---------------------------------------------------------------------------------------
		public void TestForUnlocked(int world) {
			if (world >= UnlockWorld) {
				IsLocked = true;
			}
		}

//---------------------------------------------------------------------------------------
		public bool IncXP(int amount) {
			XP += amount;
			int nextLevelTargetXP = DataManager.Data.Ranks.GetNextRankXP(Level-1);
			if (XP >= nextLevelTargetXP) {
//Cool we've leveled up
				Level++;
				return true;
			}

			return false;
		}
	}
}
