using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float moveDistance = 20f;
    public float moveSpeed = 5f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;

        Debug.Log("Car movement script started!");
    }

    void Update()
    {
        float distanceTravelled =
            Vector3.Distance(startPosition, transform.position);

        if (distanceTravelled < moveDistance)
        {
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
    }
}
