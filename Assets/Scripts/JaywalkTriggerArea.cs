using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JaywalkTriggerArea : MonoBehaviour
{
    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        JaywalkingNPCController jaywalker =
            other.GetComponentInParent<JaywalkingNPCController>();

        if (jaywalker == null)
            return;

        jaywalker.BeginJaywalkWarning();
    }
}
