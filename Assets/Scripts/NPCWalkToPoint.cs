using UnityEngine;

public class ProceduralNPCWalk : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 1.5f;

    [Header("Actual Skeleton Bones")]
    [SerializeField] private Transform leftUpperArm;
    [SerializeField] private Transform rightUpperArm;
    [SerializeField] private Transform leftUpperLeg;
    [SerializeField] private Transform rightUpperLeg;

    [Header("Arm Rest Position")]
    [Tooltip("Adjust this until the left arm points downward.")]
    [SerializeField] private Vector3 leftArmRestOffset =
        new Vector3(0f, 0f, 70f);

    [Tooltip("Adjust this until the right arm points downward.")]
    [SerializeField] private Vector3 rightArmRestOffset =
        new Vector3(0f, 0f, -70f);

    [Header("Walking Motion")]
    [SerializeField] private float walkingSpeed = 6f;
    [SerializeField] private float armSwingAmount = 20f;
    [SerializeField] private float legSwingAmount = 25f;

    [Tooltip("The axis used for forward and backward limb movement.")]
    [SerializeField] private Vector3 swingAxis = Vector3.right;

    private Quaternion leftArmStart;
    private Quaternion rightArmStart;
    private Quaternion leftLegStart;
    private Quaternion rightLegStart;

    private float walkTime;

    private void Start()
    {
        if (leftUpperArm == null ||
            rightUpperArm == null ||
            leftUpperLeg == null ||
            rightUpperLeg == null)
        {
            Debug.LogError(
                "Assign the real upper-arm and upper-leg bones.",
                this
            );

            enabled = false;
            return;
        }

        leftArmStart = leftUpperArm.localRotation;
        rightArmStart = rightUpperArm.localRotation;
        leftLegStart = leftUpperLeg.localRotation;
        rightLegStart = rightUpperLeg.localRotation;
    }

    private void Update()
    {
        transform.position +=
            transform.forward * moveSpeed * Time.deltaTime;

        walkTime += walkingSpeed * Time.deltaTime;
    }

    private void LateUpdate()
    {
        float swing = Mathf.Sin(walkTime);

        Quaternion leftArmDown =
            Quaternion.Euler(leftArmRestOffset);

        Quaternion rightArmDown =
            Quaternion.Euler(rightArmRestOffset);

        Quaternion leftArmSwing =
            Quaternion.AngleAxis(
                -swing * armSwingAmount,
                swingAxis.normalized
            );

        Quaternion rightArmSwing =
            Quaternion.AngleAxis(
                swing * armSwingAmount,
                swingAxis.normalized
            );

        Quaternion leftLegSwing =
            Quaternion.AngleAxis(
                swing * legSwingAmount,
                swingAxis.normalized
            );

        Quaternion rightLegSwing =
            Quaternion.AngleAxis(
                -swing * legSwingAmount,
                swingAxis.normalized
            );

        leftUpperArm.localRotation =
            leftArmStart * leftArmDown * leftArmSwing;

        rightUpperArm.localRotation =
            rightArmStart * rightArmDown * rightArmSwing;

        leftUpperLeg.localRotation =
            leftLegStart * leftLegSwing;

        rightUpperLeg.localRotation =
            rightLegStart * rightLegSwing;
    }
}