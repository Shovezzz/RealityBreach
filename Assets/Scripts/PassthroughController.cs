using UnityEngine;

public class PassthroughController : MonoBehaviour
{
    void Start()
    {
        OVRPassthroughLayer layer = GetComponent<OVRPassthroughLayer>();
        if (layer != null)
        {
            layer.enabled = true;
            layer.textureOpacity = 1;
        }

        if (OVRManager.instance != null)
        {
            OVRManager.instance.isInsightPassthroughEnabled = true;
        }
    }
}