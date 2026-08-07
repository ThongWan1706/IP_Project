using System.Collections;
using UnityEngine;

public class TrafficLightController1 : MonoBehaviour
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
            // REVERSE START
            // Light 1 = RED
            // Light 2 & 3 = GREEN
            // ==================================

            SetLight(trafficLight1, true, false, false);

            SetLight(trafficLight2, false, false, true);
            SetLight(trafficLight3, false, false, true);

            yield return new WaitForSeconds(greenTime);


            // ==================================
            // Light 1 = RED
            // Light 2 & 3 = YELLOW
            // ==================================

            SetLight(trafficLight1, true, false, false);

            SetLight(trafficLight2, false, true, false);
            SetLight(trafficLight3, false, true, false);

            yield return new WaitForSeconds(yellowTime);


            // ==================================
            // Light 1 = GREEN
            // Light 2 & 3 = RED
            // ==================================

            SetLight(trafficLight1, false, false, true);

            SetLight(trafficLight2, true, false, false);
            SetLight(trafficLight3, true, false, false);

            yield return new WaitForSeconds(greenTime);


            // ==================================
            // Light 1 = YELLOW
            // Light 2 & 3 = RED
            // ==================================

            SetLight(trafficLight1, false, true, false);

            SetLight(trafficLight2, true, false, false);
            SetLight(trafficLight3, true, false, false);

            yield return new WaitForSeconds(yellowTime);
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