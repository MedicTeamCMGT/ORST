using System;
using UnityEngine;

namespace ORST.Core.Movement
{

    public class TeleportAudioHandler : TeleportSupport
    {
        [SerializeField] private AudioClipInfo m_EnterAim;
        [SerializeField] private AudioClipInfo m_CancelAim;
        [SerializeField] private AudioClipInfo m_Teleporting;

        private AudioClipInfo m_CurrentAudioClip;
        private AudioSource m_PlayerAudioSource;
        private AudioSource m_DestAudioSource;

        private LocomotionTeleport.AimData m_Obj;


        private AdvancedLocomotionTeleport m_AdvancedLocomotionTeleport;

        //private LocomotionTeleport m_currentState;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (LocomotionTeleport is AdvancedLocomotionTeleport teleport)
            {
                teleport.AudioHandler = this;
                m_AdvancedLocomotionTeleport = teleport;
                //teleport.EnteredIntersection += IntersectEnterState;
                //teleport.ExitedIntersection += IntersectExitState;
            }

            m_PlayerAudioSource = gameObject.AddComponent<AudioSource>();
            m_PlayerAudioSource.playOnAwake = false;

            //m_currentState = LocomotionTeleport.States.Ready;


        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Destroy(m_PlayerAudioSource);
        }

        protected override void AddEventHandlers()
        {
            base.AddEventHandlers();
            LocomotionTeleport.EnterStateAim += EnterAimState;
            LocomotionTeleport.ExitStateAim += ExitAimState;
            LocomotionTeleport.EnterStateTeleporting += TeleportState;
        }

        protected override void RemoveEventHandlers()
        {
            base.RemoveEventHandlers();
            LocomotionTeleport.EnterStateAim -= EnterAimState;
            LocomotionTeleport.ExitStateAim -= ExitAimState;
            LocomotionTeleport.EnterStateTeleporting -= TeleportState;
        }

        private void EnterAimState() => OnStateSet(m_EnterAim);
        private void ExitAimState() => OnStateSet(m_CancelAim);
        private void TeleportState() => OnStateSet(m_Teleporting);

        private void OnStateSet(AudioClipInfo audioClip)
        {
            if (m_PlayerAudioSource.isPlaying && audioClip.Priority < m_CurrentAudioClip.Priority)
            {
                return;
            }

            Debug.LogError(AdvancedLocomotionTeleport.Instance.CurrentIntention);
            Debug.LogError(LocomotionTeleport.TeleportIntentions.Aim);
            Debug.LogError(m_Obj.TargetValid);


            if (AdvancedLocomotionTeleport.Instance.CurrentIntention == LocomotionTeleport.TeleportIntentions.Aim || m_Obj.TargetValid) {

                m_CurrentAudioClip = m_EnterAim;
                m_PlayerAudioSource.clip = m_CurrentAudioClip.AudioClip;
                m_PlayerAudioSource.loop = m_CurrentAudioClip.Loop;
                m_PlayerAudioSource.Play();
            } else {
                m_PlayerAudioSource.Stop();
            }
        }

        public void SetDestAudioSource(AudioSource audioSource)
        {
            m_DestAudioSource = audioSource;
        }

        [Serializable]
        private struct AudioClipInfo
        {
            public AudioClip AudioClip;
            public bool Loop;
            public ushort Priority;
        }
    }
}
