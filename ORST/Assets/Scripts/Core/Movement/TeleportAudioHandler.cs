using UnityEngine;

namespace ORST.Core {
    [System.Serializable]
    struct AudioClipInfo {
        public AudioClip AudioClip;
        public bool Loop;
    }

    public class TeleportAudioHandler : TeleportSupport {
        [SerializeField] private AudioClipInfo EnterAim;
        [SerializeField] private AudioClipInfo CancelAim;
        [SerializeField] private AudioClipInfo Teleporting;
        [SerializeField] private AudioClipInfo IntersectEnter;
        [SerializeField] private AudioClipInfo IntersectExit;
        private AudioSource m_PlayerAudioSource;

        protected override void OnEnable() {
            base.OnEnable();
            if (LocomotionTeleport is AdvancedLocomotionTeleport teleport) {
                teleport.AudioHandler = this;
                teleport.OnIntersectEnter += IntersectEnterState;
                teleport.OnIntersectExit += IntersectExitState;
            }

            m_PlayerAudioSource = gameObject.AddComponent<AudioSource>();
        }

        protected override void OnDisable() {
            base.OnDisable();
            Destroy(m_PlayerAudioSource);
        }

        protected override void AddEventHandlers() {
            base.AddEventHandlers();
            LocomotionTeleport.EnterStateAim += EnterAimState;
            LocomotionTeleport.ExitStateAim += ExitAimState;
            LocomotionTeleport.EnterStateTeleporting += TeleportState;
        }

        protected override void RemoveEventHandlers() {
            base.RemoveEventHandlers();
            LocomotionTeleport.EnterStateAim -= EnterAimState;
            LocomotionTeleport.ExitStateAim -= ExitAimState;
            LocomotionTeleport.EnterStateTeleporting -= TeleportState;
        }

        private void EnterAimState() => OnStateSet(EnterAim);
        private void ExitAimState() => OnStateSet(CancelAim);
        private void TeleportState() => OnStateSet(Teleporting);
        private void IntersectEnterState() => OnStateSet(IntersectEnter);
        private void IntersectExitState() => OnStateSet(IntersectExit);

        private void OnStateSet(AudioClipInfo audioClip) {
            m_PlayerAudioSource.clip = audioClip.AudioClip;
            m_PlayerAudioSource.loop = audioClip.Loop;
            m_PlayerAudioSource.Play();
        }
    }
}
