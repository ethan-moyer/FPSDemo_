using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class StringEvent : UnityEvent<string>
{
}

[System.Serializable]
public class FloatEvent : UnityEvent<float>
{
}

[System.Serializable]
public class IntEvent : UnityEvent<int>
{
}

[System.Serializable]
public class IntIntEvent : UnityEvent<int, int>
{
}

[System.Serializable]
public class AudioEvent : UnityEvent<AudioClip>
{
}