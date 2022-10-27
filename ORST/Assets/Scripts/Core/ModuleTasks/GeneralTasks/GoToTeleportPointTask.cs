using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ORST.Core.ModuleTasks;
using Oculus.Interaction;

namespace ORST.Core {
    public class GoToTeleportPointTask : ModuleTask {
        [SerializeField] private LocomotionTeleport m_LocomotionTeleport;
        
        private void Start() {
            m_LocomotionTeleport.EnterStateTeleporting += ProcessTeleportingEvent;
        }

        private void ProcessTeleportingEvent() {
            //m_LocomotionTeleport.
        }
        
        public override ModuleTaskState ExecuteModuleTask() {
            //LocomotionTeleport.
            return ModuleTaskState.Running;
        }
    }
}
