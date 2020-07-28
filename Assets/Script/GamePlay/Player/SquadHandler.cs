using System.Collections.Generic;
using Data;
using Data.DataObjects;
using GamePlay.Characters;
using UnityEngine;

namespace GamePlay.Player {
	public class SquadHandler : MonoBehaviour {
		public Hero[] Squad;

//		[Space(10)]
		[Tooltip("This is the position the player sprites move to")]
		public Transform PlayerAttackPosition;
		private Vector2 _playerAttackPosition;

		private int _numberOfSelectedCharacters;

		private Hero _target;		//The baddie

		private GameController _gameController;

//---------------------------------------------------------------------------------------
		private void OnEnable() {
			Hero.OnAttacking += attacking;
			Hero.OnAttackComplete += attackComplete;
		}

//---------------------------------------------------------------------------------------
		private void OnDisable() {
			Hero.OnAttacking -= attacking;
			Hero.OnAttackComplete -= attackComplete;
		}

//---------------------------------------------------------------------------------------
//Here we're going to set up our squad, i.e. pull our data in and place it gently into the Hero instances
		public void Init() {
			_gameController = GameController.Instance;

//Get the position the player squad will move to to perform an attack (We can turn off the gameObject afterwards,
//it's just to make life easier in the Editor)
			_playerAttackPosition = PlayerAttackPosition.position;
			PlayerAttackPosition.gameObject.SetActive(false);

			List<Character_SO> chars = DataManager.Data.GetPlayerSquad();
			_numberOfSelectedCharacters = chars.Count;

//Populate Hero(s) with our juicy data
			Hero heroRef;
			int cnt = -1;
			while (++cnt!=_numberOfSelectedCharacters) {
				heroRef = Squad[cnt];
				heroRef.ThisHero = Hero.HeroType.Player;
				heroRef.heroID = cnt;
				heroRef.SetInitialHealth(100);
				heroRef.AttackPosition = _playerAttackPosition;
//Get the correct animation SO for this character
				Animation_SO animData = _gameController.PlayerAnimationDataHolder.CharacterAnimFrames[chars[cnt].ID];
//Init him, init him good and proper
				heroRef.Init(chars[cnt], animData);
			}

//Turn off any we're not using
			cnt--;
			while (++cnt!=3) {
				Squad[cnt].gameObject.SetActive(false);
			}

//Get a reference to our baddie
			_target = _gameController.BaddieHandler.Baddie;
		}

//---------------------------------------------------------------------------------------
		public void PlayersTurn() {
			int cnt = -1;
			while (++cnt != _numberOfSelectedCharacters) {
				Squad[cnt].EnableAttack();
			}
		}

//---------------------------------------------------------------------------------------
		private void Update() {
			float delta = Time.deltaTime;

			int cnt = -1;
			while (++cnt != _numberOfSelectedCharacters) {
				Squad[cnt].Mainloop(delta);
			}
		}

//---------------------------------------------------------------------------------------
//This is called from BaddieHandler
		public Hero[] GetPossibleTargets() {
			List<Hero> tempArray = new List<Hero>();
			Hero heroRef;
			int cnt = -1;
			while (++cnt != _numberOfSelectedCharacters) {
				heroRef = Squad[cnt];
				if (heroRef.IsAlive) {
					tempArray.Add(heroRef);
				}
			}

			return tempArray.ToArray();
		}

//---------------------------------------------------------------------------------------
//This is called from BaddieHandler, it ensures the baddie always has a higher level than
//our best hero
		public int GetHighestHeroLevel() {
			int highestLevel = 1;
			Hero heroRef;

			int cnt = -1;
			while (++cnt != _numberOfSelectedCharacters) {
				heroRef = Squad[cnt];
				if (heroRef.CharacterData.Level > highestLevel) {
					highestLevel = heroRef.CharacterData.Level;
				}
			}

			return highestLevel+1;
		}

//---------------------------------------------------------------------------------------
		public void AwardWinBonus() {
			int cnt = -1;
			while (++cnt != _numberOfSelectedCharacters) {
				Squad[cnt].WinBonus(50);
			}
		}

//---------------------------------------------------------------------------------------
		private void attacking(Hero hero) {
			if (hero.ThisHero == Hero.HeroType.Baddie) {
				return;
			}

//We've got the hero who is attacking and the target so lets get CombatHandler involved
			var attackValues = _gameController.CombatHandler.CalcAttack(hero, _target);
//Hurt the target by the DamageScore
			_target.Hurt(attackValues.DamageScore);

//Now we've got a bunch of juicy data in our struct we can use for the PFX
			Vector2 pos = hero.transform.position;
			_gameController.PFXHandler.RequestXPText("XP +" + attackValues.XPAwarded, pos);
			pos.y += 0.5f;
			_gameController.PFXHandler.RequestAttackText("ATK " + attackValues.AttackScore, pos);

			pos = _target.transform.position;
			_gameController.PFXHandler.RequestDamageText("- " + attackValues.DamageScore, pos);

			if (attackValues.CritHit) {
				pos.y += 0.5f;
				_gameController.PFXHandler.RequestCritText(pos);
			}

//Increment this hero's XP
			hero.IncXP(attackValues.XPAwarded);
		}

//---------------------------------------------------------------------------------------
		private void attackComplete(Hero hero) {
			if (hero.ThisHero == Hero.HeroType.Baddie) {
				return;
			}

			_gameController.PlayersTurnComplete();
		}

	}
}
