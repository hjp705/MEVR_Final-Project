using UnityEngine;
using UnityEngine.XR;

// Keep the desktop available for the bird view while OpenXR renders the headset.
public sealed class BirdDesktopView : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.5f, -6f);
    [SerializeField] private Vector3 lookOffset = new Vector3(0f, 0.7f, 3f);

    private GameViewRenderMode previousMode;
    private bool mirrorOverridden;

    private void LateUpdate()
    {
        if (XRSettings.isDeviceActive)
        {
            if (!mirrorOverridden)
            {
                previousMode = XRSettings.gameViewRenderMode;
                mirrorOverridden = true;
            }

            if (XRSettings.gameViewRenderMode != GameViewRenderMode.None)
                XRSettings.gameViewRenderMode = GameViewRenderMode.None;
        }

        if (target == null)
            return;

        transform.position = target.position + target.rotation * offset;
        transform.LookAt(target.position + target.rotation * lookOffset, Vector3.up);
    }

    private void OnDisable()
    {
        if (mirrorOverridden)
            XRSettings.gameViewRenderMode = previousMode;
        mirrorOverridden = false;
    }
}
