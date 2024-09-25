using System.Reflection;
using UnityEngine;

public class InputManager : MonoSingleton<InputManager>
{
    public event System.Action TouchStartEvent;
    public event System.Action TouchEndEvent;

    public void OnPointerDown()
    {
        TouchStartEvent?.Invoke();
    }

    public void OnPointerUp()
    {
        TouchEndEvent?.Invoke();
    }
}
