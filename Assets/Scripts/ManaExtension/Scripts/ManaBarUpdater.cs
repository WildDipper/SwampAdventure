using System;
using MoreMountains.CorgiEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace ManaExtension
{
    public class ManaBarUpdater : MonoBehaviour, MMEventListener<ManaUpdateEvent>, MMEventListener<CorgiEngineEvent>
    {
        [SerializeField]
        [Tooltip("if this is false, the player character will be set as target automatically")]
        private bool UseCustomTarget;
        [MMCondition(nameof(UseCustomTarget), true)]
        [SerializeField]
        private GameObject Target;

        private MMProgressBar _bar;

        private void Awake()
        {
            _bar = GetComponent<MMProgressBar>();
        }

        public void OnMMEvent(ManaUpdateEvent manaUpdateEvent)
        {
//            if (manaUpdateEvent.Target != Target) return;
            _bar.UpdateBar(manaUpdateEvent.Mana, 0, manaUpdateEvent.MaxMana);
        }

        public void OnMMEvent(CorgiEngineEvent corgiEngineEvent)
        {
            if (corgiEngineEvent.EventType == CorgiEngineEventTypes.SpawnCharacterStarts && !UseCustomTarget) Target = LevelManager.Instance.Players[0].gameObject;
        }
        
        private void OnEnable()
        {
            this.MMEventStartListening<ManaUpdateEvent>();
            this.MMEventStartListening<CorgiEngineEvent>();
        }
    
        private void OnDisable()
        {
            this.MMEventStopListening<ManaUpdateEvent>();
            this.MMEventStopListening<CorgiEngineEvent>();
        }
    }
}