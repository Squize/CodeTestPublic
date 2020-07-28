using Data.DataObjects;
using UI;
using UnityEngine;

namespace GamePlay.Characters {
	public class Hero : MonoBehaviour {
		public delegate void AttackStarted();
		public static event AttackStarted OnAttackStarted;

		public delegate void Attacking(Hero thisInstance);
		public static event Attacking OnAttacking;

		public delegate void AttackComplete(Hero thisInstance);
		public static event AttackComplete OnAttackComplete;

		public delegate void CharacterPopUpRequested(Hero heroRef);
		public static event CharacterPopUpRequested OnCharacterPopUpRequested;

		public enum HeroType {
			Player,
			Baddie
		}

		public HeroType ThisHero;

		public enum State {
			Waiting,
			MovingToAttack,
			Attacking,
			WaitingForAttackToComplete,
			ReturningFromAttack,
			Dying,
			Dead,
			None
		}

		public State CurrentState;

		[Header("Visual Properties")]
		public SpriteRenderer HeroSprite;
		public SpriteRenderer HealthBarSurround;
		public SpriteRenderer HealthBar;

		[HideInInspector]
		public Vector2 AttackPosition;		//Set via SquadHandler / BaddieHandler

//Animation properties
		public enum AnimationTypes {
			Idle,
			Walking,
			None
		}

		public AnimationTypes AnimationState;

		private Animation_SO _animFrames;

//Used by the info pop-up
		public Sprite Avatar {
			get { return _animFrames.IdleFrames[0]; }
		}

		private float _animFlipFlop;
		private int _animOffset;
		private int _animIdleLength;
		private int _animWalkingLength;

		[Header("Data")]
//Health properties
		public int Health;
		private int _maxHealth;

		private Vector2 _healthBarScale;
		private float _healthBarXScaleTarget;

		public bool IsAlive {
			get { return Health > 0; }
		}

		public float HealthPercentage {
			get { return (float) Health / _maxHealth; }
		}

//Attack properties
		private bool _alreadyAttacked;

		public bool CanAttack {
			get { return IsAlive && _alreadyAttacked==false; }
		}

		private float _attackDelay;			//Simple delay when we perform an attack

//Movement properties
		private float _walkSpeed = 12;

		private Vector2 _startPos;

//This is the id in the game world, i.e 0..2
		private int _heroID;
		public int heroID {
			get { return _heroID; }
			set { _heroID = value; }
		}

//Misc working vars
		private float _delta;

		private bool _hasLeveledUp;

//Properties for the info pop-up
		private bool _isDown;			//The mouse, not just in general
		private float _heldCnt;
		private bool _isHolding;
		private float _fillSpeed = 1.5f;
		private float _fillAmount;

//External classes / components
		private BoxCollider2D _hitArea;

		private Character_SO _characterData;
		public Character_SO CharacterData {
			get { return _characterData; }
		}

		private GameController _gameController;
		private HUD.HUD _hud;

//---------------------------------------------------------------------------------------
		private void Awake() {
			_hitArea = GetComponent<BoxCollider2D>();
			_hitArea.enabled = false;

			_startPos = transform.position;
		}

//---------------------------------------------------------------------------------------
		private void OnEnable() {
			OnAttackStarted += disableInput;
			PopUpHandler.OnPopUpClosed += enableInput;
		}

//---------------------------------------------------------------------------------------
		private void OnDisable() {
			OnAttackStarted -= disableInput;
			PopUpHandler.OnPopUpClosed -= enableInput;
		}

//---------------------------------------------------------------------------------------
		public void Init(Character_SO CharacterData, Animation_SO animData) {
			_gameController = GameController.Instance;
			_hud = _gameController.HUD;

			_characterData = CharacterData;
			_animFrames = animData;

//Flip the sprite if it's a baddie
			HeroSprite.flipX = ThisHero == HeroType.Baddie;

//Set up the default frame
			_animOffset = Random.Range(0, 3);			//So the idle animations aren't in perfect sync
			HeroSprite.sprite = _animFrames.IdleFrames[_animOffset];
			_animIdleLength = _animFrames.IdleFrames.Length;
			_animWalkingLength = _animFrames.WalkingFrames.Length;

			AnimationState = AnimationTypes.Idle;

			_healthBarScale=Vector2.one;
			_healthBarXScaleTarget = 1;
		}

//---------------------------------------------------------------------------------------
		public void SetInitialHealth(int amount) {
			Health = _maxHealth = amount;
		}

//---------------------------------------------------------------------------------------
		private void OnMouseDown() {
			_isDown = true;
			_heldCnt = 0;
			_isHolding = false;
		}

//---------------------------------------------------------------------------------------
		public void OnMouseUp() {
			_heldCnt = 0;
			_isDown = false;

			if (_isHolding == false) {
//We want to attack rather than view the info
				if (CanAttack) {
//Cool, we can attack
					_alreadyAttacked = true;
//Ensures input is turned off, only for the Player heroes but does no harm for the Baddie to listen in on to
					if (OnAttackStarted != null) {
						OnAttackStarted();
					}
					triggerAttack();
				}
				return;
			}

			_isHolding = false;
			_fillAmount = 0;
			_hud.HideProgressDial(heroID);
		}

//---------------------------------------------------------------------------------------
//This is set at the start of a round (Only by the Player, the Baddie cares not for this)
		public void EnableAttack() {
			if (IsAlive) {
				_alreadyAttacked = false;
				_hitArea.enabled = true;
				CurrentState = State.Waiting;
			}
		}

//---------------------------------------------------------------------------------------
		public void BaddieAttacking() {
			_alreadyAttacked = false;
			triggerAttack();
		}

//---------------------------------------------------------------------------------------
		public void Hurt(int amount) {
//Reduce the health
			Health -= amount;
			if (Health <= 0) {
				Health = 0;
				CurrentState = State.Dying;
			}
//Calc the health as a percentage
			_healthBarXScaleTarget = (float) Health / _maxHealth;
		}

//---------------------------------------------------------------------------------------
//Called via SquadHandler. We just want to set a flag if we've leveled up and then trigger
//that once the Hero has returned from the attack
		public void IncXP(int amount) {
			_hasLeveledUp = _characterData.IncXP(amount);
		}

//---------------------------------------------------------------------------------------
		public void WinBonus(int amount) {
			if (IsAlive == false) {
//No bonus if you're dead, those are the rules
				return;
			}

			IncXP(amount);
			if (_hasLeveledUp) {
				_hasLeveledUp = false;
				_gameController.PFXHandler.RequestLevelUpText(transform.position);
			}
		}

//---------------------------------------------------------------------------------------
//Called via OnAttackStarted, stops any subsequent player attacks
		private void disableInput() {
			_hitArea.enabled = false;
		}

//---------------------------------------------------------------------------------------
//This is called via PopUpHandler.OnPopUpClosed. We have to see if we should actually turn
//input back on
		private void enableInput() {
			if (_gameController.CurrentGameState != GameController.GameState.PlayersTurn) {
				return;
			}
			if (CanAttack) {
				_hitArea.enabled = true;
				CurrentState = State.Waiting;
			}
		}

//---------------------------------------------------------------------------------------
		private void Update() {
//Do we need to tween the health bar ?
			var healthBarScale = HealthBar.transform.localScale;
			if (healthBarScale.x > _healthBarXScaleTarget) {
//Yep
				healthBarScale.x -= 0.01f;
				if (healthBarScale.x < _healthBarXScaleTarget) {
					healthBarScale.x = _healthBarXScaleTarget;
				}

				HealthBar.transform.localScale = healthBarScale;
			}

			if (_isDown == false) {
				return;
			}

			_heldCnt += Time.deltaTime;

			if (_isHolding) {
				_fillAmount += _fillSpeed * _delta;
				if (_fillAmount >= 1f) {
//Reset all our dial properties ready for next time
					_isDown = false;
					_fillAmount = 0;
					_hud.HideProgressDial(heroID);

//We also have to disable the hit areas on the Heroes to avoid accidental clicks
					if (OnAttackStarted != null) {
						OnAttackStarted();
					}

//Display our info pop-up
					if (OnCharacterPopUpRequested != null) {
						OnCharacterPopUpRequested(this);
					}
				}
				else {
					_hud.UpdateProgressDial(heroID,_fillAmount);
				}
			}
			else {
				if (_heldCnt > 0.3f) {
					_isHolding = true;
					_hud.DisplayProgressDial(heroID);
				}
			}
		}

//---------------------------------------------------------------------------------------
//Called via SquadHandler / BaddieHandler. Gives us more freedom than an Update
		public void Mainloop(float delta) {
			_delta = delta;

//Handle our state machine (That sounds posh, it's just a Switch statement)
			switch (CurrentState) {
				case State.MovingToAttack:
					movingToTarget();
					break;
				case State.Attacking:
					handleAttack();
					break;
				case State.WaitingForAttackToComplete:
					waitingForAttackToEnd();
					break;
				case State.ReturningFromAttack:
					returningBackToStart();
					break;
				case State.Dying:
					dying();
					break;
			}

//Handle our animation
			switch (AnimationState) {
				case AnimationTypes.Idle:
					runIdleAnimation();
					break;
				case AnimationTypes.Walking:
					runWalkAnimation();
					break;
			}
		}

//---------------------------------------------------------------------------------------
//This is what kicks the attack off
		private void triggerAttack() {
			AnimationState = AnimationTypes.Walking;
			CurrentState = State.MovingToAttack;
//Turn off the health bars when moving, we don't need to display it and it's visually nicer
			HealthBarSurround.enabled = false;
			HealthBar.enabled = false;
		}

//---------------------------------------------------------------------------------------
		private void movingToTarget() {
			float step = _walkSpeed * _delta;
			transform.position = Vector2.MoveTowards(transform.position, AttackPosition, step);
//Are we close enough yet ?
			float distance = Vector2.Distance(transform.position, AttackPosition);

//It does resolve as 0, but just in case of any Unity weirdness
			if (distance <= 0.1f) {
				arrivedAtAttackingPosition();
			}
		}

//---------------------------------------------------------------------------------------
		private void arrivedAtAttackingPosition() {
			AnimationState = AnimationTypes.Idle;
			CurrentState = State.Attacking;
			_attackDelay = 0;
		}

//---------------------------------------------------------------------------------------
		private void handleAttack() {
			_attackDelay += _delta;
			if (_attackDelay > 0.3f) {
//Fire off our even which will actually deal with the attack values
				if (OnAttacking != null) {
					OnAttacking(this);
				}
				attackComplete();
			}
		}

//---------------------------------------------------------------------------------------
		private void attackComplete() {
			CurrentState = State.WaitingForAttackToComplete;
			_attackDelay = 0;
		}

//---------------------------------------------------------------------------------------
		private void waitingForAttackToEnd() {
			_attackDelay += _delta;
			if (_attackDelay > 1.1f) {
//So we've made it look like we're really attacking the opponent, time to start moving back
				startToReturnBack();
			}
		}

//---------------------------------------------------------------------------------------
		private void startToReturnBack() {
			AnimationState = AnimationTypes.Walking;
			CurrentState = State.ReturningFromAttack;
		}

//---------------------------------------------------------------------------------------
		private void returningBackToStart() {
			float step = _walkSpeed * _delta;
			transform.position = Vector2.MoveTowards(transform.position, _startPos, step);

			float distance = Vector2.Distance(transform.position, _startPos);
			if (distance <= 0.1f) {
				backHomeSafeAndSound();
			}
		}

//---------------------------------------------------------------------------------------
		private void backHomeSafeAndSound() {
			CurrentState = State.Waiting;
			AnimationState = AnimationTypes.Idle;
			HealthBarSurround.enabled = true;
			HealthBar.enabled = true;

//Have we leveled up after that attack ?
			if (_hasLeveledUp) {
				_hasLeveledUp = false;			//Clear the flag
				_gameController.PFXHandler.RequestLevelUpText(transform.position);
			}

//Flag up that this attack is complete
			if (OnAttackComplete != null) {
				OnAttackComplete(this);
			}
		}

//---------------------------------------------------------------------------------------
//We don't want the Hero to die straight away, so we wait until their health bar is fully empty
		private void dying() {
			if (HealthBar.transform.localScale.x <=0) {
//Yeah, he's totally dead now
				gameObject.SetActive(false);
			}
		}

//---------------------------------------------------------------------------------------
// Animation methods
//---------------------------------------------------------------------------------------
		private void runIdleAnimation() {
			_animFlipFlop += _delta;
//0.08f is just a magic number, it looks nice
			if (_animFlipFlop < 0.08f) {
				return;
			}

			_animFlipFlop = 0;
			HeroSprite.sprite = _animFrames.IdleFrames[_animOffset];
			if (++_animOffset >= _animIdleLength) {
				_animOffset = 0;
			}
		}

//---------------------------------------------------------------------------------------
		private void runWalkAnimation() {
			_animFlipFlop += _delta;
			if (_animFlipFlop < 0.08f) {
				return;
			}

			_animFlipFlop = 0;
			HeroSprite.sprite = _animFrames.WalkingFrames[_animOffset];
			if (++_animOffset >= _animWalkingLength) {
				_animOffset = 0;
			}
		}

	}
}
