using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
[HideInInspector]
public class StringEvent : UnityEvent<string>
{
}

[System.Serializable]
[HideInInspector]
public class FloatEvent : UnityEvent<float>
{
}

[System.Serializable]
[HideInInspector]
public class IntEvent : UnityEvent<int>
{
}