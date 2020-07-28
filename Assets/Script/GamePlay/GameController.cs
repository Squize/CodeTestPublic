using Data;
using GamePlay.Baddies;
using GamePlay.Characters;
using GamePlay.PFX;
using GamePlay.Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace GamePlay {
	public class GameController : MonoBehaviour {
		public enum GameState {
			Waiting,
			PlayersTurn,
			BaddiesTurn,
			Won,
			Lost
		}

		public GameState CurrentGameState;

		[Header("External Classes")]
		public SquadHandler SquadHandler;
		public AnimationDataHolder PlayerAnimationDataHolder;

		public BaddieHandler BaddieHandler;
		public AnimationDataHolder BaddieAnimationDataHolder;

		public CombatHandler CombatHandler;

		public PFXHandler PFXHandler;

		public HUD.HUD HUD;

		public PopUpHandler PopUpHandler;

		[Header("Cover All Properties")]
		public Canvas CoverAllCanvas;
		public CanvasGroup CoverAllCanvasGroup;

		public static GameController Instance { get; private set; }

//---------------------------------------------------------------------------------------
		void Awake() {
			if (Instance == null) {
				Instance = this;

//Hide everything away whilst we set the display up
				CoverAllCanvasGroup.alpha = 1;
				CoverAllCanvas.enabled = true;
			}
			else {
				if (Instance != this) {
					Destroy(gameObject);
				}
			}
		}

		//var damage = (attacker.power - defender.ac) * random.range(0.9f,1.1f);

//---------------------------------------------------------------------------------------
		private void Start() {
//Lets set up all our goodness
			CurrentGameState = GameState.Waiting;
			SquadHandler.Init();
			BaddieHandler.Init();
			HUD.Init();

//All done, so lets fade down the cover and kick things off. I'm excited are you ?
			Fader.Instance.FadeDown(CoverAllCanvasGroup, 0.7f, startGame);
		}

//---------------------------------------------------------------------------------------
		public void PlayersTurnComplete() {
//So the player has finished their attack, have they killed the baddie (i.e. won ?)
			if (BaddieHandler.IsBaddieAlive() == false) {
//Cool!
				CurrentGameState = GameState.Won;
				SquadHandler.AwardWinBonus();
				HUD.PlayerWon(BaddieHandler.BaddieName);

//Bump the level here, we're only going to do it when the player wins so they can grind if they face a hard baddie
				DataManager.Data.BumpLevel();

				return;
			}

//Nope we've not won yet, so it's the baddies turn to attack
			CurrentGameState = GameState.BaddiesTurn;
			BaddieHandler.BaddiesTurn();
		}

//---------------------------------------------------------------------------------------
		public void BaddiesTurnComplete() {
//Have all the players squad being slaughtered in a shocking blood bath ?
			Hero[] squadMembers = SquadHandler.GetPossibleTargets();
			if (squadMembers.Length == 0) {
//Oh, they have. That's not good. Game over, it's game over man
				CurrentGameState = GameState.Lost;
				HUD.BaddieWon();
				return;
			}

//Hurray! We're still in the game, so lets loop back to the players turn
			CurrentGameState = GameState.PlayersTurn;
			SquadHandler.PlayersTurn();
		}

//---------------------------------------------------------------------------------------
//This is enabled via the HUD at level complete
		public void OnContinueButtonPressed() {
			Fader.Instance.FadeUp(CoverAllCanvasGroup, 0.7f, levelComplete);
		}

//---------------------------------------------------------------------------------------
		private void OnDestroy() {
			Instance = null;
		}

//---------------------------------------------------------------------------------------
		private void startGame() {
			CoverAllCanvas.enabled = false;
//There really should be some sort on intro here, we'll see

			CurrentGameState = GameState.PlayersTurn;
			SquadHandler.PlayersTurn();
		}

//---------------------------------------------------------------------------------------
		private void levelComplete() {
			SceneManager.LoadScene("CharacterSelect", LoadSceneMode.Single);
		}

	}
}
