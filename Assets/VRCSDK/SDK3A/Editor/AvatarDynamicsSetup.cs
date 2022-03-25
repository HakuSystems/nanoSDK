using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Dynamics.PhysBone;
using VRC.Dynamics;

namespace VRC.SDK3.Avatars
{
    public static class AvatarDynamicsSetup
    {
        [RuntimeInitializeOnLoadMethod]
        private static void RuntimeInit()
        {
            //Triggers Manager
            if (ContactManager.Inst == null)
            {
                var obj = new GameObject("TriggerManager");
                UnityEngine.Object.DontDestroyOnLoad(obj);
                ContactManager.Inst = obj.AddComponent<ContactManager>();
            }

            //Triggers
            ContactBase.OnInitialize = Trigger_OnInitialize;

            //PhysBone Manager
            if (PhysBoneManager.Inst == null)
            {
                var obj = new GameObject("PhysBoneManager");
                UnityEngine.Object.DontDestroyOnLoad(obj);

                PhysBoneManager.Inst = obj.AddComponent<PhysBoneManager>();
                PhysBoneManager.Inst.IsSDK = true;
                PhysBoneManager.Inst.Init();
                obj.AddComponent<EditorGrabHelper>();
            }
            VRCPhysBoneBase.OnInitialize = PhysBone_OnInitialize;
        }
        private static bool Trigger_OnInitialize(ContactBase trigger)
        {
            var receiver = trigger as ContactReceiver;
            if (receiver != null && !string.IsNullOrWhiteSpace(receiver.parameter))
            {
                var avatarDesc = receiver.GetComponentInParent<VRCAvatarDescriptor>();
                if (avatarDesc != null)
                {
                    var animator = avatarDesc.GetComponent<Animator>();
                    if (animator != null)
                    {
                        // called from SDK, so create SDK Param access
                        receiver.paramAccess = new AnimParameterAccessAvatarSDK(animator, receiver.parameter);
                    }
                }
            }

            return true;
        }
        private static void PhysBone_OnInitialize(VRCPhysBoneBase physBone)
        {
            if(!string.IsNullOrEmpty(physBone.parameter))
            {
                var avatarDesc = physBone.GetComponentInParent<VRCAvatarDescriptor>();
                if (avatarDesc != null)
                {
                    var animator = avatarDesc.GetComponent<Animator>();
                    if (animator != null)
                    {
                        physBone.param_IsGrabbed = new AnimParameterAccessAvatarSDK(animator, physBone.parameter + VRCPhysBoneBase.PARAM_ISGRABBED);
                        physBone.param_Angle = new AnimParameterAccessAvatarSDK(animator, physBone.parameter + VRCPhysBoneBase.PARAM_ANGLE);
                        physBone.param_Stretch = new AnimParameterAccessAvatarSDK(animator, physBone.parameter + VRCPhysBoneBase.PARAM_STRETCH);
                    }
                }
            }
        }
    }
}


