using UnityEngine;

public class CarCollisionTrigger : MonoBehaviour
{
    [SerializeField] private CarCrashManager crashManager;
    [SerializeField] private GameObject targetCar;

    private bool hasCollided = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollided || targetCar == null) return;

        // Check if the object entering our trigger is the target car
        if (other.gameObject == targetCar || other.transform.IsChildOf(targetCar.transform))
        {
            hasCollided = true;
            Debug.Log($"Impact registered with {other.gameObject.name}!");

            if (crashManager != null)
            {
                crashManager.TriggerCrashSequence();
            }
        }
    }
}