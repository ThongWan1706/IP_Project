using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
    [System.Serializable]
    public class TrafficLightSet
    {
        public Light red;
        public Light yellow;
        public Light green;
    }

    [Header("Traffic Lights")]
    public TrafficLightSet trafficLight1;
    public TrafficLightSet trafficLight2;
    public TrafficLightSet trafficLight3;

    [Header("Timing")]
    public float greenTime = 5f;
    public float yellowTime = 2f;

    private void Start()
    {
        StartCoroutine(TrafficSequence());
    }

    private IEnumerator TrafficSequence()
    {
        while (true)
        {
            // ==================================
            // TRAFFIC LIGHT 1 = GREEN
            // TRAFFIC LIGHT 2 & 3 = RED
            // ==================================

            SetLight(trafficLight1, false, false, true);

            SetLight(trafficLight2, true, false, false);
            SetLight(trafficLight3, true, false, false);

            yield return new WaitForSeconds(greenTime);


            // ==================================
            // TRAFFIC LIGHT 1 = YELLOW
            // TRAFFIC LIGHT 2 & 3 = RED
            // ==================================

            SetLight(trafficLight1, false, true, false);

            SetLight(trafficLight2, true, false, false);
            SetLight(trafficLight3, true, false, false);

            yield return new WaitForSeconds(yellowTime);


            // ==================================
            // TRAFFIC LIGHT 1 = RED
            // TRAFFIC LIGHT 2 & 3 = GREEN
            // Happens at the same time
            // ==================================

            SetLight(trafficLight1, true, false, false);

            SetLight(trafficLight2, false, false, true);
            SetLight(trafficLight3, false, false, true);

            yield return new WaitForSeconds(greenTime);


            // ==================================
            // TRAFFIC LIGHT 1 = RED
            // TRAFFIC LIGHT 2 & 3 = YELLOW
            // ==================================

            SetLight(trafficLight1, true, false, false);

            SetLight(trafficLight2, false, true, false);
            SetLight(trafficLight3, false, true, false);

            yield return new WaitForSeconds(yellowTime);


            // ==================================
            // TRAFFIC LIGHT 1 = GREEN
            // TRAFFIC LIGHT 2 & 3 = RED
            // Happens at the same time
            // ==================================

            SetLight(trafficLight1, false, false, true);

            SetLight(trafficLight2, true, false, false);
            SetLight(trafficLight3, true, false, false);
        }
    }

    private void SetLight(
        TrafficLightSet trafficLight,
        bool red,
        bool yellow,
        bool green)
    {
        if (trafficLight.red != null)
            trafficLight.red.enabled = red;

        if (trafficLight.yellow != null)
            trafficLight.yellow.enabled = yellow;

        if (trafficLight.green != null)
            trafficLight.green.enabled = green;
    }
}