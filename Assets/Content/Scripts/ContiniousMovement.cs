using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;


public class ContiniousMovement : MonoBehaviour
{
    [SerializeField] private XRNode inputSource;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float additionalHeight = 0.2f;

    private float gravity = -9.81f;
    private float fallingSpeed;
	private XROrigin rig;
    private Vector2 inputAxis;
    private CharacterController character;

    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XROrigin>();
    }

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);

    }

	private void FixedUpdate() {
        CapsuleFollowHeadset();

        Quaternion headYaw = Quaternion.Euler(0f, rig.Camera.transform.eulerAngles.y, 0f);
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0f, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);

        //gravity
        bool isGrounded = IsGrounded();

        if (isGrounded)
            fallingSpeed = 0f;
        else 
            fallingSpeed += gravity * Time.fixedDeltaTime;

        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime); 
	}

    private bool IsGrounded() {
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
	}

    private void CapsuleFollowHeadset() {
        character.height = rig.CameraInOriginSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.Camera.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height / 2f + character.skinWidth, capsuleCenter.z);
	}
}
