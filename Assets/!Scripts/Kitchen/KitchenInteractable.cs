using UnityEngine;
using System;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(BoxCollider))]
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
    Coroutine slotFunctionRoutineReference;

    protected virtual void Awake()
    {
        interactionTrigger = this.GetComponent<BoxCollider>();
        container ??= new(m_data, this);   
    }

    protected virtual void Start()
    {
        reflection = EventManager.GetKitchenContainerReflectionReference.Invoke();
    }

    Vector3 GetInteractionDirection() => this.transform.TransformDirection(this.referenceInteractionDirection);

    public void EnableInteraction()
    {
        EventManager.RefreshContainerReflections.AddListener(RefreshContainerReflection);
    }

    public void TryInitiateInteraction(Vector3 playerForward)
    {   
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
        this.container.UpdateReflection(reflection);
    }

    public void DisableInteraction()
    {
        EventManager.RefreshContainerReflections.RemoveListener(RefreshContainerReflection);
    }

    public void InitiateSlotFunctionTimer(float duration , Action<float> OnProgressUpdation = null , Action OnTimerCompletion = null)
    {
        slotFunctionRoutineReference = StartCoroutine(SlotFunctionTimerRoutine(duration, OnProgressUpdation , OnTimerCompletion));
    }

    IEnumerator SlotFunctionTimerRoutine(float duration ,Action<float> OnProgressUpdation = null, Action OnTimerCompletion = null)
    {   
        // Avoiding WaitForSeconds due to hidden allocations
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

#region UNITY_EDITOR

    void OnValidate()
    {
        interactionTrigger = this.GetComponent<BoxCollider>();
    }

    void OnDrawGizmos()
    {
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
