using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    FPS_control input_control;
    Rigidbody rb;
    public GameObject ground;
    private Animator animator;

 
    [Header("Basic Movement")]
    private float movement_force = 50000;
    private float jumpforce = 300;
    private float gravityScale = 1.0f;
    private float globalGravity = -9.81f;
    Vector2 move;
    bool isgrounded = true;


    [Header("Skills")]
    private bool isJumped = false;
    private bool enableDoubleJump = false;
    private bool isRunning = false;
    private bool isSquated = false;
    private float energyDelay = 0f;
    public float dashForce = 4500;


    [Header("Cooldown Controls")]
    public Cooldown cdDoubleJump;
    public Cooldown cdRun;
    public Cooldown cdDash;


    [Header("Key")]
    private int numberOfKeys = 0;
    public Text KeyNumberText;

    void Start()
    {
        animator = GetComponent<Animator>();

        input_control = new FPS_control();
        input_control.Player_map.Enable();

        rb = GetComponent<Rigidbody>();

        move = Vector2.zero;

        cdDoubleJump.totalCD = 5;
        cdDoubleJump.currCD = 0f;
        cdRun.totalCD = 4f;
        cdRun.currCD = 0;
        cdDash.totalCD = 7f;
        cdDash.currCD = 0;
    }
    private void Update()
    {
        PickKey();
        OpenDoor();
        float dashInputValue = input_control.Player_map.Dash.ReadValue<float>();

        if (dashInputValue > 0 && cdDash.currCD <= 0)
            Dash();


        cdDoubleJump.currCD -= Time.deltaTime;
        cdDash.currCD -= Time.deltaTime;

        if (!isRunning)
        {
            energyDelay -= Time.deltaTime;
            cdRun.currCD -= Time.deltaTime / 2;
        }

        if (cdRun.currCD >= cdRun.totalCD)    //when run energy is over, you cannot run for 3 secs
        {
            energyDelay = 3f;
        }

        float jumpInputValue = input_control.Player_map.Jump.ReadValue<float>();

        if (jumpInputValue == 0 && isJumped)
            enableDoubleJump = true;

        if (jumpInputValue > 0)
        {
            if (!isJumped && isgrounded)
            {
                Playerjump();
                isJumped = true;
            }
            else if (enableDoubleJump && isJumped && cdDoubleJump.currCD <= 0)
            {
                PlayerDoubleJump();
                isJumped = false;
            }
        }

    }
    void FixedUpdate()
    {   
        //Gravity
        Vector3 gravity = globalGravity * gravityScale * Vector3.up;
        rb.AddForce(Physics.gravity * gravityScale * Time.fixedDeltaTime, ForceMode.Acceleration);

        Groundcheck();
        Playermove();

        if (isRunning) movement_force = 80000;
        else movement_force = 50000;

    }
    
    void Groundcheck()
    {
        if (transform.position.y - ground.transform.position.y > 2)
        {
            isgrounded = false;
            rb.drag = 2f;
        }
        else
        {
            isgrounded = true;
            rb.drag = 3f;
            isJumped = false;
            enableDoubleJump = false;
        }
    }
    void Playermove()
    {
        move = input_control.Player_map.Movement.ReadValue<Vector2>();
        float squatInputValue = input_control.Player_map.Squat.ReadValue<float>();
        float runInputValue = input_control.Player_map.Run.ReadValue<float>();

        float forceZ = move.x * movement_force * Time.deltaTime;
        float forceX = move.y * movement_force * Time.deltaTime;

        if (isgrounded && !isSquated)
        {
            rb.AddForce(transform.forward * forceX, ForceMode.Force);
            rb.AddForce(transform.right * forceZ, ForceMode.Force);
        }

        if (squatInputValue > 0)
        {
            animator.SetBool("Squat", true);
            isSquated = true;
        }
        else
        {
            animator.SetBool("Squat", false);
            isSquated = false;
        }

        if (isgrounded && energyDelay <= 0 && move.magnitude > 0 && runInputValue > 0 && cdRun.currCD <= cdRun.totalCD)
        {
            cdRun.currCD += Time.deltaTime;
            animator.SetBool("IsRunning", true);
            isRunning = true;
        }
        else
        {
            animator.SetBool("IsRunning", false);
            isRunning = false;
        }

        if (move.magnitude > 0 && isgrounded)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }

        if (!isgrounded)    //fly a little
        {
            rb.AddForce(transform.forward * forceX / 4, ForceMode.Force);
            rb.AddForce(transform.right * forceZ / 4, ForceMode.Force);
        }

        if (move.magnitude > 0 && !isgrounded)
        {
            animator.SetTrigger("Fly");
        }

    }
    void Playerjump()
    {
        rb.AddForce(Vector3.up * jumpforce, ForceMode.Impulse);
        animator.SetTrigger("Jump");
    }

    void PlayerDoubleJump()
    {
        rb.AddForce(Vector3.up * jumpforce * 3 / 2, ForceMode.Impulse);
        animator.SetTrigger("DoubleJump");
        cdDoubleJump.currCD = cdDoubleJump.totalCD;
    }

    private void Dash()
    {
        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
        animator.SetTrigger("Dash");
        cdDash.currCD = cdDash.totalCD;
    }
    private void PickKey()
    {
        var layermask = 6;  //key layer
        layermask = ~layermask;
        var ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 5, layermask))
        {
            if (isSquated)
            {
                Destroy(hit.transform.gameObject);
                numberOfKeys += 1;
                KeyNumberText.text = "Keys: " + numberOfKeys;
            }      
        }
    }

    private bool isRotating = false;
    private void OpenDoor()
    {
        var layermask = 7;  //door layer
        layermask = ~layermask;
        var ray = new Ray(this.transform.position, this.transform.forward);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5, layermask))
        {
            float openDoorInputValue = input_control.Player_map.OpenDoor.ReadValue<float>();
            if (openDoorInputValue > 0 && !isRotating)
            {
                Transform childTransform = hit.transform.GetChild(0);

                Vector3 currentRotation = childTransform.localRotation.eulerAngles;

                StartCoroutine(RotateChildObject(childTransform, 1.0f));
            }
        }
    }
    IEnumerator RotateChildObject(Transform childTransform, float delay)
    {
        isRotating = true;
        Vector3 currentRotation = childTransform.localRotation.eulerAngles;

        if (currentRotation.y == 0 && numberOfKeys > 0)   //open
        {
            childTransform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y + 80f, currentRotation.z);

            numberOfKeys -= 1;
            KeyNumberText.text = "Keys: " + numberOfKeys;
            yield return new WaitForSeconds(delay);
        }
        else if (currentRotation.y == 80)   //close
        {
            childTransform.localRotation = Quaternion.Euler(currentRotation.x, 0f, 0);
            yield return new WaitForSeconds(delay);
        }
        isRotating = false;
    }
}