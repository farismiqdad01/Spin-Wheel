using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>, ISubject
{
    private List<IObserver> observers = new List<IObserver>();
    private Wheel activeWheel;

    protected override void Awake()
    {
        base.Awake();
        // Additional initialization if needed
    }

    public void RegisterWheel(Wheel wheel)
    {
        activeWheel = wheel;
    }

    public Wheel GetActiveWheel()
    {
        return activeWheel;
    }

    public void RegisterObserver(IObserver observer)
    {
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }

    public void NotifyObservers(string message)
    {
        foreach (var observer in observers)
        {
            observer.OnNotify(message);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // Custom cleanup code here if needed
    }
}