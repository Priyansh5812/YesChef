using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
// base interactable for kitchen containers
public class KitchenInteractable : MonoBehaviour
{   
    [SerializeField] protected bool isInteractionDirectionless = false;
    [SerializeField] protected Vector3 referenceInteractionDirection = Vector3.forward;
    [SerializeField] BoxCollider interactionTrigger;

    [SerializeField, Range(0f, 1f)]
    protected float minDirectionAlignment = 0.8f;

    [Header("Stats")]
    [SerializeField] protected ContainerConfig m_data;
    protected SlotContainer container;
    protected ContainerReflectionSystem reflection;

    protected virtual void Awake()
    {
        // cache the collider and prepare the container
        interactionTrigger = this.GetComponent<BoxCollider>();
        container ??= new(m_data, this);   
    }

    protected virtual void OnEnable()
    {
        // listen for shared game events
        InitListeners();
    }

    protected virtual void Start()
    {
        // grab the reflection view used by this interactable
        reflection = EventManager.GetKitchenContainerReflectionReference.Invoke();
    }

    void InitListeners()
    {
        // close the container when the round ends
        EventManager.OnGameOver.AddListener(ForceContainerClosure);
    }

    Vector3 GetInteractionDirection() => this.transform.TransformDirection(this.referenceInteractionDirection);


    public void EnableInteraction()
    {
        // allow the player to refresh the reflection view
        EventManager.RefreshContainerReflections.AddListener(RefreshContainerReflection);
    }

    public void TryInitiateInteraction(Vector3 playerForward)
    {   
        // only open when the player is facing the right direction
        if(isInteractionDirectionless)
        {
            InitiateInteraction();
            return;
        }

        if(Vector3.Dot(playerForward , GetInteractionDirection()) >= minDirectionAlignment)
        {
            InitiateInteraction();
        }
        else
        {
            Debug.LogError("Interaction Vector not matched");
        }
    }
    

    void InitiateInteraction()
    {   
        // toggle the container open state
        if(!container.IsOpened)
        {   
            EventManager.RefreshContainerReflections.Invoke();
            container.OpenContainer();
        }
        else
            container.CloseContainer();
    }

    void RefreshContainerReflection()
    {   
        // push the latest data into the reflection view
        this.container.UpdateReflection(reflection);
    }

    public void DisableInteraction()
    {
        // shut the container and remove the refresh hook
        if(container.IsOpened)
        {
            container.CloseContainer();
        }

        EventManager.RefreshContainerReflections.RemoveListener(RefreshContainerReflection);
    }

    public void InitiateSlotFunctionTimer(float duration , Action<float> OnProgressUpdation = null , Action OnTimerCompletion = null)
    {
        // run a timed container action
        StartCoroutine(SlotFunctionTimerRoutine(duration, OnProgressUpdation , OnTimerCompletion));
    }

    IEnumerator SlotFunctionTimerRoutine(float duration ,Action<float> OnProgressUpdation = null, Action OnTimerCompletion = null)
    {   
        // avoids waitforseconds allocations
        float t = duration;
        float norm; 
        while(t > 0)
        {
            t -= Time.deltaTime;
            norm = 1 - (t / duration);
            norm = Mathf.Clamp01(norm);
            OnProgressUpdation?.Invoke(norm);
            yield return null;
        }
        OnTimerCompletion?.Invoke();
    }

    void ForceContainerClosure()
    {
        // clear the container when the game ends
        if(this.container.IsOpened)
        {   
            this.container.ResetContainer();
        }
    }

    void DeInitListeners()
    {
        // remove shared listeners before shutdown
        EventManager.OnGameOver.RemoveListener(ForceContainerClosure);
    }

    protected virtual void OnDisable()
    {
        // stop listening when this object is disabled
        DeInitListeners();
    }

#region UNITY_EDITOR

    void OnValidate()
    {
        // keep the collider reference in sync
        interactionTrigger = this.GetComponent<BoxCollider>();
    }

    void OnDrawGizmos()
    {
        // draw the trigger and facing direction
        var color = Color.green;
        color.a = 0.5f;
        Gizmos.color =  color;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(interactionTrigger.center,interactionTrigger.size);
        Gizmos.matrix = Matrix4x4.identity;
        
        if(this.isInteractionDirectionless)
            return;
            
        Handles.color = Color.red;
        Handles.ArrowHandleCap(1,this.transform.TransformPoint(interactionTrigger.center) + Vector3.up * (1.0f + interactionTrigger.size.y / 2)  , Quaternion.LookRotation(GetInteractionDirection().normalized),2f,EventType.Repaint);
    }
#endregion
}
