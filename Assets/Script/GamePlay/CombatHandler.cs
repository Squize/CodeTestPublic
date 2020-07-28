using GamePlay.Characters;
using UnityEngine;

namespace GamePlay {
	public class CombatHandler : MonoBehaviour {
//This is the home of the magic numbers

//---------------------------------------------------------------------------------------
		public CombatValuesStruct CalcAttack(Hero attacker, Hero defender) {
			CombatValuesStruct returnStruct =new CombatValuesStruct();

//Start with getting the attackers attack value
			int attack = attacker.CharacterData.Attack;

//The level is a multiplier, the higher the level the greater the values
			int attackersLevel = attacker.CharacterData.Level;

//This is just a magic number, it feels "nice"
			returnStruct.AttackScore = Mathf.RoundToInt(attack * (attackersLevel+1 * 0.8f));

			int defendersLevel = defender.CharacterData.Level;
			int def = defender.CharacterData.Defence;
//Same here with this number
			int finalDef= Mathf.RoundToInt(def * (defendersLevel+2 * 1.7f));

			int atk = returnStruct.AttackScore;
			returnStruct.DamageScore = atk * atk / (atk + finalDef);

//If it's the player attacking give them a chance for a Crit hit ( Plus double the DamageScore )
			if (attacker.ThisHero == Hero.HeroType.Player) {
				returnStruct.DamageScore += returnStruct.DamageScore;

				float critChance = Random.value;
				float critTarget = 0.2f;
//If the hero's health is low increase the chance of a Crit
				if (attacker.HealthPercentage < 0.5f) {
					critTarget = 0.3f;
				}

				if (critChance < critTarget) {
					returnStruct.CritHit = true;
					returnStruct.DamageScore *= 2;
				}
			}

//It can happen!
			if (returnStruct.DamageScore == 0) {
				returnStruct.DamageScore = 2;
			}

			returnStruct.XPAwarded = Mathf.RoundToInt(returnStruct.DamageScore*0.5f);
			if (returnStruct.XPAwarded < 5) {
//This ensures that there's always some XP, got to have some for just trying
				returnStruct.XPAwarded = 5;
			}

			return returnStruct;
		}

//---------------------------------------------------------------------------------------
		public struct CombatValuesStruct {
			public int XPAwarded;

			public int AttackScore;

			public int DamageScore;

			public bool CritHit;

		}
	}
}
