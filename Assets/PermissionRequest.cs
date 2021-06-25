using System.Collections;
using System.Collections.Generic;
#if UNITY_IOS

using Unity.Advertisement.IosSupport;
#endif

using UnityEngine;

public class PermissionRequest : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
#if UNITY_IOS
        if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus() == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
        {
            ATTrackingStatusBinding.RequestAuthorizationTracking();
        }
#endif
    }
}
