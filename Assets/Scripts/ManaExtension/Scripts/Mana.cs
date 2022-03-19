using System.Collections;
using MoreMountains.CorgiEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

namespace ManaExtension
{
    public struct ManaUpdateEvent
    {
        public GameObject Target;
        public float Mana;
        public float MaxMana;

        private static ManaUpdateEvent e;
        public static void Trigger(GameObject target, float mana, float maxMana)
        {
            e.Target = target;
            e.Mana = mana;
            e.MaxMana = maxMana;
            MMEventManager.TriggerEvent(e);
        }
    }

    [AddComponentMenu("Corgi Engine Extensions/Mana")]
    public class Mana : MonoBehaviour, MMEventListener<MMStateChangeEvent<Weapon.WeaponStates>>
    {
        [Header("Status")]

        [SerializeField]
        [MMReadOnly]
        [Tooltip("the current mana of the character")]
        private float _currentMana;

        public float CurrentMana
        {
            get => _currentMana;
            set
            {
                var oldValue = _currentMana;
                if (value > MaximumMana)
                    _currentMana = MaximumMana;
                else if (value < 0)
                    _currentMana = 0;
                else
                    _currentMana = value;
                ManaUpdateEvent.Trigger(gameObject, _currentMana, MaximumMana);
                if (value >= oldValue) return;
                if (_currentMana < 0.001f) OutOfManaFeedbacks?.PlayFeedbacks();
                if (_recovering)
                    StopCoroutine(_recovery);
                _recovery = StartCoroutine(RecoverMana());

                IEnumerator RecoverMana()
                {
                    _recovering = true;
                    yield return MMCoroutine.WaitFor(ManaRecoveryDelay);
                    while (CurrentMana < MaximumMana)
                    {
                        CurrentMana += ManaRecoveryRate * Time.deltaTime;
                        yield return MMCoroutine.WaitForFrames(1);
                    }
                    _recovering = false;
                }
            }
        }

        [Header("Mana")]

        [MMInformation("Add this component to an object and it'll use mana when using weapon and won't be able to use the weapon if it doesn't have enough of it.", MMInformationAttribute.InformationType.Info, false)]
        [Tooltip("the initial amount of mana of the object")]
        public float InitialMana = 100f;
        [Tooltip("the maximum amount of mana of the object")]
        public float MaximumMana = 100f;

        [Header("Usage")]

        [Tooltip("how much mana using weapon will consume (per second)")]
        public float WeaponManaConsumption = 80f;
        
     //   [Tooltip("how much mana dashing will consume (per dash)")]

     //   public float DashingManaConsumption = 20;

        [Header("Recovery")]

        [Tooltip("the number of seconds it takes to start recovering mana after using it")]
        public float ManaRecoveryDelay = 5;
        [Tooltip("how much mana will be recovered each second")]
        public float ManaRecoveryRate = 20;

        [Header("Feedbacks")]
        public MMFeedbacks OutOfManaFeedbacks;

        private const string _shootStopMethodName = nameof(Weapon.TurnWeaponOff);

      //  private const string _dashStopMethodName = nameof(CharacterDash.StopDash);
        private bool _shooting;
        private bool _recovering;
        private Coroutine _recovery;

        private void Start()
        {
            CurrentMana = InitialMana;
            OutOfManaFeedbacks?.Initialization();
        }

        public void OnMMEvent(MMStateChangeEvent<Weapon.WeaponStates> weaponEvent)
        {
            if (weaponEvent.Target != gameObject) 
            {
                return;
            }
            
            if (weaponEvent.NewState != Weapon.WeaponStates.WeaponUse) _shooting = false;
            switch (weaponEvent.NewState)
            {
                case Weapon.WeaponStates.WeaponUse:
                    StartCoroutine(ConsumeShootingMana());
                    break;
            }

            IEnumerator ConsumeShootingMana()
            {
                _shooting = true;
                while (_shooting)
                {
                    if (CurrentMana > 0)
                    {
                        CurrentMana -= WeaponManaConsumption;
                        yield return MMCoroutine.WaitForFrames(1);
                    }
                    else
                    {
                        _shooting = false;
                        BroadcastMessage(_shootStopMethodName);
                    }
                }
            }
        }

        private void OnEnable()
        {
            this.MMEventStartListening();
        }

        private void OnDisable()
        {
            this.MMEventStopListening();
        }
    }
}
