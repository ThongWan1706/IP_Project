using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class NoHelmetViolationTrigger : MonoBehaviour
{
    private readonly HashSet<NoHelmetVehicleViolation> triggeredVehicles =
        new HashSet<NoHelmetVehicleViolation>();

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void Awake()
    {
        Collider triggerCollider = GetComponent<Collider>();

        if (triggerCollider != null)
            triggerCollider.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        NoHelmetVehicleViolation vehicleViolation =
            other.GetComponentInParent<NoHelmetVehicleViolation>();

        if (vehicleViolation == null)
            return;

        // Only activate this trigger once for each violating vehicle.
        if (triggeredVehicles.Contains(vehicleViolation))
            return;

        triggeredVehicles.Add(vehicleViolation);
        vehicleViolation.BeginViolationWarning();
    }
}