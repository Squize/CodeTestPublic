using Data;
using Data.DataObjects;
using GamePlay.Characters;
using UnityEngine;

namespace GamePlay.Baddies {
	public class BaddieHandler : MonoBehaviour {
//All bad guys are heroes in their own minds
//(That's an excuse for this maybe being slightly weird)
		public Hero Baddie;

		public string BaddieName {
			get {
				return Baddie.CharacterData.Name;
			}
		}

		[Tooltip("These are the positions in front of the player heroes")]
		public Transform[] BaddieAttackPositions;
		private Vector2[] _baddieAttackPositions;

		private Hero _target;

		private Character_SO BaddieCharData;

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
		public void Init() {
			_gameController = GameController.Instance;

//Get the positions in front of the player heroes, that's where the baddie will move to for attacking
			int len = BaddieAttackPositions.Length;
			_baddieAttackPositions=new Vector2[len];

			int cnt = -1;
			while (++cnt!=len) {
				_baddieAttackPositions[cnt] = BaddieAttackPositions[cnt].position;
				BaddieAttackPositions[cnt].gameObject.SetActive(false);		//No longer needed
			}

			int world = DataManager.Data.World;

//Get our Character_SO
			BaddieCharData = DataManager.Data.BaddieCharacter;
//Set the name
			var baddieNames = DataManager.Data.BaddieNames;
			BaddieCharData.Name = baddieNames[wrap(world -1,0, baddieNames.Length)];

//We don't care about the baddies XP, just it's level, as that affects it's stats
			BaddieCharData.Level = _gameController.SquadHandler.GetHighestHeroLevel();

//Attack and defence values
			int attackValue = Random.Range(10, 20);
			int defenceValue = 35 - attackValue;

//We're going to cheat here a little and weaken the baddie on world 1 as we want the player to win the first
//time they play
			if (DataManager.Data.World == 1) {
				defenceValue -= 10;
			}

			BaddieCharData.Attack = attackValue;
			BaddieCharData.Defence = defenceValue;


//Hardcoded 5 baddie types, it *is* a tight deadline
			int baddieAnims = wrap(world, 0, 5);
			Animation_SO animData = _gameController.BaddieAnimationDataHolder.CharacterAnimFrames[baddieAnims];

//Finally we can populate our (anti)Hero's data
			Baddie.SetInitialHealth(200);		//They've got to have more health than the player
			Baddie.ThisHero = Hero.HeroType.Baddie;
			Baddie.Init(BaddieCharData, animData);
		}

//---------------------------------------------------------------------------------------
		private void Update() {
			Baddie.Mainloop(Time.deltaTime);
		}

//---------------------------------------------------------------------------------------
		public bool IsBaddieAlive() {
			return Baddie.IsAlive;
		}

//---------------------------------------------------------------------------------------
		public void BaddiesTurn() {
//Ok, so we're going to want to pick a target
			Hero[] targets = _gameController.SquadHandler.GetPossibleTargets();

//Lets early out if we can
			if (targets.Length == 1) {
				_target = targets[0];
			}
			else {
//No such luck. For now we're going to pick a target at random, we can do something smarter later time permitting
				_target = targets[Random.Range(0, targets.Length)];
			}

//Cool, so we know who we're going to attack, we should really attack them
//Get the attack position
			Baddie.AttackPosition = _baddieAttackPositions[_target.heroID];
			Baddie.BaddieAttacking();
		}

//---------------------------------------------------------------------------------------
		private void attacking(Hero hero) {
			if (hero.ThisHero == Hero.HeroType.Player) {
				return;
			}

//We've got the hero who is attacking and the target so lets get CombatHandler involved
			var attackValues = _gameController.CombatHandler.CalcAttack(hero, _target);
//Hurt the target by the DamageScore
			_target.Hurt(attackValues.DamageScore);

//Do our PFX text thing ( No XP or crit for the baddie, screw that guy )
			Vector2 pos = hero.transform.position;
			pos.y += 0.5f;
			_gameController.PFXHandler.RequestAttackText("ATK " + attackValues.AttackScore, pos);

			pos = _target.transform.position;
			_gameController.PFXHandler.RequestDamageText("- " + attackValues.DamageScore, pos);
		}

//---------------------------------------------------------------------------------------
		private void attackComplete(Hero hero) {
			if (hero.ThisHero == Hero.HeroType.Player) {
				return;
			}

			_gameController.BaddiesTurnComplete();
		}

//---------------------------------------------------------------------------------------
		private int wrap(int value, int min, int max) {
			var range = max - min;
			return (min + ((((value - min) % range) + range) % range));
		}
	}
}
