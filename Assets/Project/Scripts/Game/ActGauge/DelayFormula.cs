using UnityEngine;

public class DelayFormula
{
    const float MaxDelayTime = 30f;
    const float Base = 3f;
    const float Damping = 0.01f;

    public float GetTime(float speed)
    {
        return MaxDelayTime / Mathf.Pow(Base, Damping * speed);
    }
}