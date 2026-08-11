using UnityEngine;

public class CarCollisionTrigger : MonoBehaviour
{
    [SerializeField] private CarCrashManager crashManager;
    [SerializeField] private GameObject targetCar;

    private bool hasCollided = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasCollided || targetCar == null)
            return;

        if (other.gameObject == targetCar || other.transform.IsChildOf(targetCar.transform))
        {
            hasCollided = true;
            Debug.Log("Cars collided!");

            if (crashManager != null)
            {
                crashManager.TriggerCrashSequence();
            }
        }
    }
}