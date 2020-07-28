using System.Collections.Generic;
using Data.DataObjects;
using UnityEngine;

namespace Data {
	public class DataManager : MonoBehaviour {
		public static DataManager Data { get; private set; }

		[HideInInspector]
		public Character_SO[] PlayerCharacters;

		[HideInInspector]
		public bool[] SelectedCharacters;

		[HideInInspector]
		public Character_SO BaddieCharacter;

		private int _numberOfChars = 10;
		public int NumberOfChars {
			get { return _numberOfChars; }
		}

		[HideInInspector]
		public Rank_SO Ranks;

		public int World = 1; //Actual game level so we can have some progression through this epic romp

//Stats to populate our characters with
		private string[] _names;
		private int[] _attack;
		private int[] _defence;

		[HideInInspector]
		public string[] BaddieNames;

//---------------------------------------------------------------------------------------
		void Awake() {
			if (Data == null) {
				DontDestroyOnLoad(gameObject);
				Data = this;
				init();
			}
			else {
				if (Data != this) {
					Destroy(gameObject);
				}
			}
		}

//---------------------------------------------------------------------------------------
		void OnApplicationQuit() {
			save();
		}

//---------------------------------------------------------------------------------------
		void OnApplicationPause(bool pauseStatus) {
			if (pauseStatus) {
				save();
			}
		}

//---------------------------------------------------------------------------------------
		void OnApplicationFocus(bool hasFocus) {
			if (hasFocus == false) {
				save();
			}
		}

//---------------------------------------------------------------------------------------
		public void ToggleSelectedCharacter(int charID) {
			SelectedCharacters[charID] = !SelectedCharacters[charID];
		}

//---------------------------------------------------------------------------------------
		public int GetNumberOfSelectedCharacters() {
			int selectedChars = 0;
			int cnt = -1;
			while (++cnt != _numberOfChars) {
				selectedChars += SelectedCharacters[cnt] ? 1 : 0;
			}

			return selectedChars;
		}

//---------------------------------------------------------------------------------------
		public List<Character_SO> GetPlayerSquad() {
			List<Character_SO> squad = new List<Character_SO>();
			int cnt = -1;
			while (++cnt != _numberOfChars) {
				if (SelectedCharacters[cnt]) {
					squad.Add(PlayerCharacters[cnt]);
				}
			}

			return squad;
		}

//---------------------------------------------------------------------------------------
		public void BumpLevel() {
			if (++World > 50) {
//No one is going to play this until level 50, no one. But just in case
				World = 50;
			}
//Test to see if we've unlocked any new characters. Exciting!
			int cnt = -1;
			while (++cnt != _numberOfChars) {
				PlayerCharacters[cnt].TestForUnlocked(World);
			}
		}

//---------------------------------------------------------------------------------------
		private void init() {
//Create our Rank SO
			Ranks = Rank_SO.CreateInstance();

//Set up the baddie next as we never save his data it's just created at runtime (Aside from the names)
			BaddieNames = new string[10];
			BaddieNames[0] = "Clanon Jonesness";
			BaddieNames[1] = "Merlturner Hawkinsness";
			BaddieNames[2] = "Jarramos";
			BaddieNames[3] = "Chavethumper";
			BaddieNames[4] = "Bareth";
			BaddieNames[5] = "Andersonganto";
			BaddieNames[6] = "Cal";
			BaddieNames[7] = "Khanbell";
			BaddieNames[8] = "Tinkerdley";
			BaddieNames[9] = "Wandlee Fordelline";

			BaddieCharacter = Character_SO.CreateInstance(_numberOfChars); //Unique ID

//Have we played this before ? If so we can just load that data in
			if (PlayerPrefs.HasKey("World")) {
				load();
				return;
			}

//From https://www.name-generator.org.uk/
			_names = new string[_numberOfChars];
			_names[0] = "Hubus Harkrper";
			_names[1] = "Holmesstomper";
			_names[2] = "Sanchewand Anspell";
			_names[3] = "Gogmaders";
			_names[4] = "Nuaknight";
			_names[5] = "Evansspell Aumrobertson";
			_names[6] = "Shizne";
			_names[7] = "Elmsquez Bellden";
			_names[8] = "Gigantlane";
			_names[9] = "Hagmagog";

//Going to use random numbers for now, deadlines and all that
			_attack = new int[_numberOfChars];
			_defence = new int[_numberOfChars];

			int attackValue;
			int defenceValue;

			int cnt = -1;
			while (++cnt != _numberOfChars) {
				attackValue = Random.Range(5, 20);
				defenceValue = 35 - attackValue;
				_attack[cnt] = attackValue;
				_defence[cnt] = defenceValue;
			}

//Set our world unlock values and the default Character level
			int[] unlockWorldStorage = new int[_numberOfChars];
			int[] defaultCharLevel = new int[_numberOfChars];

			int unlockWorld = 4;
			int charLevel = 3;

			cnt = -1;
			while (++cnt != _numberOfChars) {
				if (cnt < 3) {
					unlockWorldStorage[cnt] = 1;
					defaultCharLevel[cnt] = 1;
				}
				else {
					unlockWorldStorage[cnt] = unlockWorld;
					unlockWorld += 3;
					defaultCharLevel[cnt] = charLevel;
					charLevel++;
				}
			}


//Now we can finally create our characters
			Character_SO character;
			PlayerCharacters = new Character_SO[_numberOfChars];
			cnt = -1;
			while (++cnt != _numberOfChars) {
				character = Character_SO.CreateInstance(cnt);
				character.Name = _names[cnt];
				character.Attack = _attack[cnt];
				character.Defence = _defence[cnt];
				character.Level = defaultCharLevel[cnt];
				character.UnlockWorld= unlockWorldStorage[cnt];
				character.TestForUnlocked(World);
				PlayerCharacters[cnt] = character;
			}

//Lets select the default characters
			SelectedCharacters = new bool[_numberOfChars];
			SelectedCharacters[0] = true;
			SelectedCharacters[1] = true;
			SelectedCharacters[2] = true;

//We're at the stage where we can save our data
			save();
		}

//---------------------------------------------------------------------------------------
		private void load() {
			World = PlayerPrefs.GetInt("World");

//Load in our character data
			PlayerCharacters = new Character_SO[_numberOfChars];
			Character_SO character;

			int cnt = -1;
			while (++cnt != _numberOfChars) {
				character = Character_SO.CreateInstance(cnt);
				PlayerCharacters[cnt] = character;

				string loadedJson = PlayerPrefs.GetString("char_" + cnt);
				character.LoadData(loadedJson);
			}

//And load in the characters the player has selected
			SelectedCharacters = new bool[_numberOfChars];
			cnt = -1;
			while (++cnt != _numberOfChars) {
				SelectedCharacters[cnt] = PlayerPrefs.GetInt("selectedChar_" + cnt) != 0;
			}
		}

//---------------------------------------------------------------------------------------
		private void save() {
			PlayerPrefs.SetInt("World",World);

//Convert all our Character_SO instances into json
			int cnt = -1;
			while (++cnt!=_numberOfChars) {
				string json = JsonUtility.ToJson(PlayerCharacters[cnt], true);
				PlayerPrefs.SetString("char_"+cnt,json);
			}

//Save our selected characters ( We could do it the loop above but trying to make this a little easier to read )
			cnt = -1;
			while (++cnt != _numberOfChars) {
				PlayerPrefs.SetInt("selectedChar_"+cnt,(SelectedCharacters[cnt] ? 1:0 ));
			}

			PlayerPrefs.Save();
		}

	}
}
