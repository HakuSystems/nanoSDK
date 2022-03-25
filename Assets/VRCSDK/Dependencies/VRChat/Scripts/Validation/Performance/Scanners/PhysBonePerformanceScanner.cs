using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using VRC.SDKBase.Validation.Performance.Stats;
using VRC.SDK3.Dynamics.PhysBone.Components;
using VRC.Dynamics;

namespace VRC.SDKBase.Validation.Performance.Scanners
{
    #if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "PhysBonePerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/PhysBonePerformanceScanner"
    )]
    #endif
    public sealed class PhysBonePerformanceScanner : AbstractPerformanceScanner
    {
        public override IEnumerator RunPerformanceScanEnumerator(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            //Get components
            var physBoneBuffer = new List<VRCPhysBone>();
            var colliderBuffer = new List<VRCPhysBoneColliderBase>();
            yield return ScanAvatarForComponentsOfType(avatarObject, physBoneBuffer);
            if(shouldIgnoreComponent != null)
            {
                physBoneBuffer.RemoveAll(c => shouldIgnoreComponent(c));
            }

            int totalBones = 0;
            int totalCollisionChecks = 0;

            Profiler.BeginSample("Analyze Dynamic Bones");
            foreach(var physBone in physBoneBuffer)
            {
                Profiler.BeginSample("Single VRCPhysBone Component");

                //Count transforms
                bool hasEndBones = physBone.endpointPosition != Vector3.zero; // Add extra bones to the end of each chain if end bones are being used
                physBone.InitTransforms();
                totalBones += physBone.bones.Count;

                //Count colliders
                int colliders = 0;
                if(physBone.colliders != null)
                {
                    foreach(var collider in physBone.colliders)
                    {
                        if(collider != null && !colliderBuffer.Contains(collider))
                        {
                            colliders += 1;
                            colliderBuffer.Add(collider);
                        }
                    }
                }

                //Check bones
                if(colliders > 0)
                {
                    foreach (var bone in physBone.bones)
                    {
                        if (bone.childIndex > 0 || (bone.isEndBone && hasEndBones))
                        {
                            totalCollisionChecks += colliders;
                        }
                    }
                }

                Profiler.EndSample();
            }
            Profiler.EndSample();

            yield return null;

            //Record
            if (physBoneBuffer.Count > 0)
            {
                var stats = new AvatarPerformanceStats.PhysBoneStats();
                stats.componentCount = physBoneBuffer.Count;
                stats.transformCount = totalBones;
                stats.colliderCount = colliderBuffer.Count;
                stats.collisionCheckCount = totalCollisionChecks;
                perfStats.physBone = stats;
            }
            else
                perfStats.physBone = null;
        }
    }
}
