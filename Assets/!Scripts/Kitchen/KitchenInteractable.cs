using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class KitchenInteractable : MonoBehaviour
{   
    [SerializeField] protected bool isInteractionDirectionless = false;
    [SerializeField] protected Vector3 referenceInteractionDirection = Vector3.forward;
    [SerializeField] BoxCollider interactionTrigger;

    [SerializeField, Range(0f, 1f)]
    protected float minDirectionAlignment = 0.8f;

    [Header("Stats")]
    [SerializeField] ContainerConfig m_data;
    SlotContainer container;
    ContainerReflectionSystem reflection;

    void Awake()
    {
        interactionTrigger = this.GetComponent<BoxCollider>();
        container ??= new(m_data);   
    }

    void Start()
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
