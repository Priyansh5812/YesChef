using System;
using UnityEngine;

// central event hub for gameplay and ui flow
public static class EventManager
{
    // shared references for the container views
    public static FuncEvent<IContainerReflectionSystem> GetInventoryReflectionReference
    {
        get; private set;
    } = new();
    public static FuncEvent<IContainerReflectionSystem> GetKitchenContainerReflectionReference
    {
        get; private set;
    } = new();
    
    // container life cycle events
    public static ActionEvent OnContainerOpened
    {
        get; private set;
    } = new();

    public static ActionEvent OnContainerClosed
    {
        get; private set;
    } = new();
    
    // ui refresh for container reflections
    public static ActionEvent RefreshContainerReflections
    {
        get; private set;
    } = new();
    
    // order and game flow events
    public static ActionEvent<OrderServeData> PreOrderServed
    {
        get; private set;
    } = new();

    public static ActionEvent<int> OnOrderServed
    {
        get; private set;
    } = new();

    public static ActionEvent OnGameStarted
    {
        get; private set;
    } = new();

    public static ActionEvent OnGamePaused
    {
        get; private set;
    } = new();
    
    public static ActionEvent OnGameResumed
    {
        get; private set;
    } = new();
    
    public static ActionEvent OnGameOver
    {
        get; private set;
    } = new();

}

// action events with no payload
public class ActionEvent
{
    private event Action baseAction;

    public void Invoke() => baseAction?.Invoke();

    public void AddListener(Action action) => baseAction += action;

    public void RemoveListener(Action action) => baseAction -= action;
}

// action events with one value
public class ActionEvent<T1>
{
    private event Action<T1> baseAction;

    public void Invoke(T1 value) => baseAction?.Invoke(value);

    public void AddListener(Action<T1> action) => baseAction += action;

    public void RemoveListener(Action<T1> action) => baseAction -= action;


}

// action events with two values
public class ActionEvent<T1, T2>
{
    private event Action<T1, T2> baseAction;

    public void Invoke(T1 value_1 , T2 value_2) => baseAction?.Invoke(value_1 , value_2);
    public void AddListener(Action<T1, T2> action) => baseAction += action;

    public void RemoveListener(Action<T1, T2> action) => baseAction -= action;
}

// function events that return one value
public class FuncEvent<T1>
{
    private event Func<T1> baseFunc;

    public T1 Invoke() => baseFunc.Invoke();

    public void AddListener(Func<T1> action) => baseFunc += action;

    public void RemoveListener(Func<T1> action) => baseFunc -= action;

}

// function events that return a mapped value
public class FuncEvent<T1 , T2>
{
    private event Func<T1 , T2> baseFunc;

    public T2 Invoke(T1 value) => baseFunc.Invoke(value);

    public void AddListener(Func<T1 , T2> action) => baseFunc += action;

    public void RemoveListener(Func<T1 , T2> action) => baseFunc -= action;

}

// function events that return a third value
public class FuncEvent<T1, T2, T3>
{
    private event Func<T1, T2, T3> baseFunc;

    public T3 Invoke(T1 value_1, T2 value_2) => baseFunc.Invoke(value_1, value_2);

    public void AddListener(Func<T1, T2, T3> action) => baseFunc += action;

    public void RemoveListener(Func<T1, T2, T3> action) => baseFunc -= action;

}



