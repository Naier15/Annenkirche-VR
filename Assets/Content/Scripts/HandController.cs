using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class HandController : MonoBehaviour
{
    [SerializeField] private UnityEngine.InputSystem.InputActionReference inputActionReference;
    [SerializeField] private GameObject XRDeviceSimulator;
    [SerializeField] private GameObject prefab;
    [SerializeField] private InputDeviceCharacteristics controllerCharacteristics;

    private XRDeviceSimulator XRDeviceSimulatorComponent;
    private InputDevice targetDevice;
    private GameObject spawnedController;
    private Animator handAnimator;
    

	void Start()
    {
        XRDeviceSimulatorComponent = GameObject.FindGameObjectWithTag("XRInput").GetComponent<XRDeviceSimulator>();
        if (prefab) {
            spawnedController = Instantiate(prefab, transform);
            handAnimator = spawnedController.GetComponent<Animator>();
        } else {
            Debug.LogError("HandController script has no prefab of hand");
        }

        TryInitialize();
    }

    void Update()
    {
        if (!targetDevice.isValid)
            TryInitialize();
        else
            UpdateHandAnimationVR();

        UpdateHandAnimationPC();
	}

    void TryInitialize() {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (InputDevice device in devices) {
            print(device.name + device.characteristics);
		}

        if (devices.Count > 0) {
			targetDevice = devices[0];
		}

    }

    void UpdateHandAnimationVR() {

        //Получение параметров от контроллеров
        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
            handAnimator.SetFloat("Grip", gripValue);
        else
            handAnimator.SetFloat("Grip", 0f);

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggetValue))
            handAnimator.SetFloat("Trigger", triggetValue);
        else
            handAnimator.SetFloat("Trigger", 0f);
    }

    void UpdateHandAnimationPC() {
        //Получение параметров от клавиатуры
        if (XRDeviceSimulatorComponent.gripAction.action.IsPressed() && inputActionReference.action.IsPressed())
            handAnimator.SetFloat("Grip", 1);
        else
            handAnimator.SetFloat("Grip", 0);

        if (XRDeviceSimulatorComponent.triggerAction.action.IsPressed() && inputActionReference.action.IsPressed())
            handAnimator.SetFloat("Trigger", 1f);
        else
            handAnimator.SetFloat("Trigger", 0f);
    }

}
