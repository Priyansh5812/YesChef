using UnityEngine;
[RequireComponent(typeof(CharacterController))]

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
    private void Awake()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main;
    }

    private void OnEnable()
    {

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {   
        PollInputs();
        GetIntendedDirection();

        //--------------

        BodyRotationPass();
    }

    void FixedUpdate()
    {
        CalculateVelocity();
    }

    #region Locomotion
    void PollInputs()
    {  
        inputDirection.x =  Input.GetAxisRaw("Horizontal");
        inputDirection.z =  Input.GetAxisRaw("Vertical");
    }

    void GetIntendedDirection()
    {
        intendedDirection = cam.transform.TransformDirection(inputDirection);
        intendedDirection = Vector3.ProjectOnPlane(intendedDirection, Vector3.up);
    }

    void CalculateVelocity()
    {
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
        if(inputDirection.sqrMagnitude <= 0.01f)
            return;

        Vector3 forwardDirection = inputDirection;
        forwardDirection = Vector3.ProjectOnPlane(forwardDirection, Vector3.up);
        Quaternion targetRotation = Quaternion.LookRotation(forwardDirection);
        bodyTransform.localRotation = Quaternion.RotateTowards(bodyTransform.localRotation,targetRotation , playerData.bodyRotationSpeed);
    }
    #endregion


    void LateUpdate()
    {
        cc.Move(currentVelocity * Time.deltaTime);
    }


    #region HELPERS
    Vector3 GetAccelaration()
    {
        Vector3 accDir = targetVelocity - currentVelocity;
        return accDir.normalized * playerData.Accelaration;
    }
    #endregion


    

}