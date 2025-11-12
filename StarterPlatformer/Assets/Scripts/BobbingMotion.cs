using UnityEngine;

public class BobbingMotion : MonoBehaviour
{
    Vector3 startPos;
    public float bobSpeed = 1f;
    public float bobMagnitude = 0.3f;

    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + Vector3.up *
            bobMagnitude * Mathf.Sin(Time.time * bobSpeed);
    }
}
