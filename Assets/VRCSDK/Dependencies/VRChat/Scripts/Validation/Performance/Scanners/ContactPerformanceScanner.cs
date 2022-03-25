using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase.Validation.Performance.Stats;
using VRC.SDK3.Dynamics.Contact.Components;

namespace VRC.SDKBase.Validation.Performance.Scanners
{
#if VRC_CLIENT
    [CreateAssetMenu(
        fileName = "ContactPerformanceScanner",
        menuName = "VRC Scriptable Objects/Performance/Avatar/Scanners/ContactPerformanceScanner"
    )]
#endif
    public sealed class ContactPerformanceScanner : AbstractPerformanceScanner
    {
        public override IEnumerator RunPerformanceScanEnumerator(GameObject avatarObject, AvatarPerformanceStats perfStats, AvatarPerformance.IgnoreDelegate shouldIgnoreComponent)
        {
            //VRCContactSender
            int componentCount = 0;
            var compBuffer = new List<Component>();
            yield return ScanAvatarForComponentsOfType(typeof(VRCContactSender), avatarObject, compBuffer);
            componentCount += compBuffer.Count;

            //VRCContactReceiver
            yield return ScanAvatarForComponentsOfType(typeof(VRCContactReceiver), avatarObject, compBuffer);
            componentCount += compBuffer.Count;

            //Ignore
            if (shouldIgnoreComponent != null)
            {
                componentCount = 0;
            }

            //Record stats
            perfStats.contactCount = componentCount;
        }
    }
}
