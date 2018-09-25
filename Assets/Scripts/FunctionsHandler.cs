using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityRESTRequest;

public class FunctionsHandler : MonoBehaviour {

    public TextMesh textMesh;

    private DeviceDetails[] devices;

    public GetDeviceTelemetry functionGetDeviceTelemetry;

    /// <summary>
    /// Azure Function handler for devices request success
    /// </summary>
    /// <param name="response"></param>
    public void OnGetDevicesSuccess(Response response)
    {
        var res = response as ResponseData<DeviceDetails[]>;
        if (res == null)
        {
            Debug.LogError("Failed to cast Response data event args. Check type is correct!");
            return;
        }
        devices = res.Data;

        var sb = new StringBuilder("Got devices " + devices.Length + "\n");
        foreach(var device in devices)
        {
            sb.AppendLine("DeviceId: " + device.DeviceId);
        }
        UpdateText(sb.ToString());
    }

    /// <summary>
    /// Azure Function handler for device request success
    /// </summary>
    /// <param name="response"></param>
    public void OnGetDeviceTelemetrySuccess(Response response)
    {
        var res = response as ResponseData<DeviceDetails>;
        if (res == null)
        {
            Debug.LogError("Failed to cast Response data event args. Check type is correct!");
            return;
        }
        var device = res.Data;
        UpdateText("Show DeviceId: " + device.DeviceId);
    }

    /// <summary>
    /// Azure Functions handler for request errors
    /// </summary>
    /// <param name="response"></param>
    public void OnRequestError(Response response)
    {
        var message = "";
        var res = response as ResponseText;
        if (res != null)
        {
            message = res.Text;
        }
        Debug.LogErrorFormat("Request error status: {0} message: {1} \n{2}", response.StatusCode, message,  response.Url);
        UpdateText("Request error status: " + response.StatusCode);
    }

    public void UpdateText(string message)
    {
        Debug.Log("Text\n" + message);
        if (textMesh == null) return;
        textMesh.text = message;
    }

    #region VuForia VuMark type handlers

    /// <summary>
    /// VuMark string Id handler
    /// The encoded string is the device Id.
    /// </summary>
    /// <param name="deviceId"></param>
    public void FindDevice(string deviceId)
    {
        // NB: The VuMark Id needs to cleaned of null characters if you don't use the full string length available.
        var cleanedDeviceId = deviceId.Replace("\0", string.Empty); 
        UpdateText("Found " + cleanedDeviceId);
        SendDeviceTelemetryRequest(cleanedDeviceId);
    }

    /// <summary>
    /// VuMark numeric Id handler
    /// The number code is used as an index of the device list. 
    /// It is better to use VuMark string type to provide the actual device Id 
    /// but this handler is included for convenience when using the VuForia VuMark samples.
    /// </summary>
    /// <param name="code"></param>
    public void FindDevice(ulong code)
    {
        uint index = 0;
        try
        {
            index = Convert.ToUInt32(code);
        }
        catch (OverflowException)
        {
            Debug.LogErrorFormat("The {0} value {1} is outside the range of the Int32 type.", code.GetType().Name, code);
        }
        if (index == 0)
        {
            Debug.LogWarning("Unhandled code '0'. The VuMark numeric type samples start at 1.");
            return;
        }
        DeviceDetails device = GetDevice(index - 1);
        if (device != null)
        {
            UpdateText("Found " + device.DeviceId);
            SendDeviceTelemetryRequest(device.DeviceId);
        }
        else
        {
            UpdateText("Unknown device");
        }
    }

    public void SendDeviceTelemetryRequest(string deviceId)
    {
        if (functionGetDeviceTelemetry == null) return;
        UpdateText(string.Format("Loading Device Id: '{0}'", deviceId));

        functionGetDeviceTelemetry.AddQueryParam("deviceId", deviceId);
        functionGetDeviceTelemetry.Send();
    }

    #endregion

    #region Device look-up methods

    private DeviceDetails GetDevice(string deviceId)
    {
        if (devices == null || devices.Length < 1)
        {
            return null;
        }
        foreach (var device in devices)
        {
            if (string.Equals(deviceId, device.DeviceId))
            {
                Debug.Log("Found Device Id: " + device.DeviceId + "\n" + device.Status);
                return device;
            }
        }
        return null;
    }

    private DeviceDetails GetDevice(uint index)
    {
        if (devices == null || devices.Length < index)
        {
            return null;
        }
        DeviceDetails device = devices[index];
        Debug.Log("Device Id: " + device.DeviceId + "\n" + device.Status);
        return device;
    }

    #endregion
}
