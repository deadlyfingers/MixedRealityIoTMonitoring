using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VuMarkHandler : MonoBehaviour {

    VuMarkManager vuMarkManager;

    public TextMesh textMesh;

    public VuMarkNumericHandler foundNumericHandler;
    public VuMarkStringHandler foundStringHandler;
    public VuMarkBytesHandler foundBytesHandler;

    // Use this for initialization
    void Start () {
        vuMarkManager = TrackerManager.Instance.GetStateManager().GetVuMarkManager();
        vuMarkManager.RegisterVuMarkDetectedCallback(VuMarkDetected);
        vuMarkManager.RegisterVuMarkLostCallback(VuMarkLost);
    }

    void OnDestroy()
    {
        vuMarkManager.UnregisterVuMarkDetectedCallback(VuMarkDetected);
        vuMarkManager.UnregisterVuMarkLostCallback(VuMarkLost);
    }

    private void VuMarkDetected(VuMarkTarget target)
    {
        string code = GetVuMarkCode(target);
        UpdateText(code, false);
        Debug.Log("VuMark code: " + code);
    }

    private void VuMarkLost(VuMarkTarget target)
    {
        UpdateText("");
    }

    private string GetVuMarkCode(VuMarkTarget target)
    {
        switch(target.InstanceId.DataType)
        {
            case InstanceIdType.NUMERIC:
                // trigger handler event when VuMark numeric code is found
                if (foundNumericHandler != null)
                {
                    foundNumericHandler.Invoke(target.InstanceId.NumericValue);
                }
                return target.InstanceId.NumericValue.ToString();
            case InstanceIdType.STRING:
                // trigger handler event when VuMark string id is found
                if (foundStringHandler != null)
                {
                    foundStringHandler.Invoke(target.InstanceId.StringValue);
                }
                return target.InstanceId.StringValue;
            case InstanceIdType.BYTES:
                // trigger handler event when VuMark numeric code is found
                if (foundBytesHandler != null)
                {
                    foundBytesHandler.Invoke(target.InstanceId.Buffer);
                }
                return target.InstanceId.HexStringValue;
        }
        return "";
    }

    private void UpdateText(string text, bool shouldUpdate = true)
    {
        if (textMesh == null)  return;

        if (!string.IsNullOrEmpty(textMesh.text) && !shouldUpdate)
        {
            return;
        }
        textMesh.text = text;
    }
}
