using UnityEngine;
[RequireComponent(typeof(CharacterController))]

// drives player movement and body rotation
public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerStatData playerData;
    [SerializeField] Transform bodyTransform;
    CharacterController cc;
    Camera cam;
    Vector3 inputDirection;
    Vector3 intendedDirection;
    Vector3 targetVelocity;
    Vector3 currentVelocity;
    Vector3 lastVelocity;

    Vector3 startPosition;
    Quaternion startRotation;
    bool isActive = true;

    private void Awake()
    {
        // cache the controller and camera once
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void OnEnable()
    {   
        // listen for the game state changes that affect movement
        EventManager.OnGameStarted.AddListener(ResetTransform);
        EventManager.OnGameStarted.AddListener(EnableInteraction);
        EventManager.OnGamePaused.AddListener(DisableInteraction);
        EventManager.OnGameResumed.AddListener(EnableInteraction);
        EventManager.OnGameOver.AddListener(DisableInteraction);
        EventManager.OnContainerOpened.AddListener(DisableInteraction);
        EventManager.OnContainerClosed.AddListener(EnableInteraction);
    }

    void Start()
    {
        // remember the spawn point for new rounds
        startPosition = this.transform.position;
        startRotation = this.transform.rotation;
    }

    void Update()
    {   
        // read input and steer the body each frame
        PollInputs();
        GetIntendedDirection();

        // keep the body facing the travel direction

        BodyRotationPass();
    }

    void FixedUpdate()
    {
        // apply the movement using fixed timing
        CalculateVelocity();
    }

    #region Locomotion
    void PollInputs()
    {   
        // ignore movement while the player is inactive
        if(!isActive)
        {   
            inputDirection = Vector3.zero;
            return;
        }
        
        inputDirection.x =  Input.GetAxisRaw("Horizontal");
        inputDirection.z =  Input.GetAxisRaw("Vertical");
    }

    void GetIntendedDirection()
    {
        // project input onto the ground plane
        intendedDirection = cam.transform.TransformDirection(inputDirection);
        intendedDirection = Vector3.ProjectOnPlane(intendedDirection, Vector3.up);
    }

    void CalculateVelocity()
    {
        // build the current velocity toward the target speed
        targetVelocity = intendedDirection.normalized * playerData.MaxSpeed;

        if (targetVelocity.sqrMagnitude > 0)
        {
            currentVelocity = lastVelocity + (GetAccelaration() * Time.fixedDeltaTime);
        }
        else 
        {
            currentVelocity *= playerData.Deacclaration;
        }

        currentVelocity = Vector3.ClampMagnitude(currentVelocity, playerData.MaxSpeed);

        lastVelocity = currentVelocity;
    }
    #endregion

    #region BodyRotation
    void BodyRotationPass()
    {   
        // rotate the body only when there is movement input
        if(inputDirection.sqrMagnitude <= 0.01f)
            return;

        Vector3 forwardDirection = cam.transform.TransformDirection(inputDirection);
        forwardDirection = Vector3.ProjectOnPlane(forwardDirection, Vector3.up);
        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
        bodyTransform.localRotation = Quaternion.RotateTowards(bodyTransform.localRotation,targetRotation , playerData.bodyRotationSpeed);
    }
    #endregion


    void LateUpdate()
    {   
        // move the controller after the other motion passes
        cc.Move(currentVelocity * Time.deltaTime);
    }

    #region OTHERS
    Vector3 GetAccelaration()
    {
        // push the current speed toward the target speed
        Vector3 accDir = targetVelocity - currentVelocity;
        return accDir.normalized * playerData.Accelaration;
    }
    
    void ResetTransform()
    {
        // return the player to the start of the round
        cc.enabled = false;
        transform.SetPositionAndRotation(startPosition, startRotation);
        cc.enabled = true;
    }

    void EnableInteraction()
    {
        // allow movement again
        isActive = true;
    }

    void DisableInteraction()
    {
        // freeze movement and clear any carried velocity
        isActive = false;
        lastVelocity = currentVelocity = Vector3.zero;
    }

    #endregion

    void OnDisable()
    {   
        // remove the shared event hooks
        EventManager.OnGameStarted.RemoveListener(ResetTransform);
        EventManager.OnGameStarted.RemoveListener(EnableInteraction);
        EventManager.OnGamePaused.RemoveListener(DisableInteraction);
        EventManager.OnGameResumed.RemoveListener(EnableInteraction);
        EventManager.OnGameOver.RemoveListener(DisableInteraction);
        EventManager.OnContainerOpened.RemoveListener(DisableInteraction);
        EventManager.OnContainerClosed.RemoveListener(EnableInteraction);
    }

    

}
