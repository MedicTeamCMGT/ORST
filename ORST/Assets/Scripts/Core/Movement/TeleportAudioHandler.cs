using UnityEngine;

namespace ORST.Core {
    [System.Serializable]
    private struct AudioClipInfo {
        public AudioClip AudioClip;
        public bool Loop;
        public ushort Priority;
    }

    public class TeleportAudioHandler : TeleportSupport {
        [SerializeField] private AudioClipInfo m_EnterAim;
        [SerializeField] private AudioClipInfo m_CancelAim;
        [SerializeField] private AudioClipInfo m_Teleporting;
        [SerializeField] private AudioClipInfo m_IntersectEnter;
        [SerializeField] private AudioClipInfo m_IntersectExit;
        private AudioClipInfo m_CurrentAudioClip;
        private AudioSource m_PlayerAudioSource;
        private AudioSource m_DestAudioSource;

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

        private void EnterAimState() => OnStateSet(m_EnterAim);
        private void ExitAimState() => OnStateSet(m_CancelAim);
        private void TeleportState() => OnStateSet(m_Teleporting);
        private void IntersectEnterState() => OnStateSet(m_IntersectEnter);
        private void IntersectExitState() => OnStateSet(m_IntersectExit);

        private void OnStateSet(AudioClipInfo audioClip) {
            if (m_PlayerAudioSource.isPlaying && audioClip.Priority < m_CurrentAudioClip.Priority) {
                return;
            }

            m_CurrentAudioClip = audioClip;
            m_PlayerAudioSource.clip = m_CurrentAudioClip.AudioClip;
            m_PlayerAudioSource.loop = m_CurrentAudioClip.Loop;
            m_PlayerAudioSource.Play();
        }

        public void SetDestAudioSource(AudioSource audioSource) {
            m_DestAudioSource = audioSource;
        }
    }
}
