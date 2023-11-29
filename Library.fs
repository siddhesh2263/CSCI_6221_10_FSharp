//********
// Author: Siddhesh K.
// Date: 11/27/2023
// Objective: Game Development in 3D Space using Unity and F#
//********

namespace fs

// Load the dependencies from Unity COM module.
open UnityEngine
open UnityEngine.InputSystem

// The below line indicates the name of the script that will be consumed by Unity IDE.
type AnimationMovementController() = 

    // The below line inherits the standard Unity type for default scripts.
    inherit MonoBehaviour()

    //********
    // The below mutable data types are created in order to track character controls and its movement.
    //********

    let mutable playerInput: PlayerInput = null // To control character movement using keyboard inputs.
    let mutable animator: Animator = null   // To handle animations.
    let mutable characterController: CharacterController = null
    let mutable characterControls: InputActionMap = null    // To connect player input actions with the actions defined in Unity.
    let mutable mainCamera: Camera = null

    // The 'move' and 'run' variables are defined to capture the action when input is used.
    let mutable move = null
    let mutable run = null

    // The 'currentMovementInput' will be the current movement input which will be captured as a Vector2 type.
    let mutable currentMovementInput = Unchecked.defaultof<Vector2>

    // The 'currentMovement' will mark the movement using the Vector3 that is taking place in Unity's 3D space.
    let mutable currentMovement = Unchecked.defaultof<Vector3>

    // The 'movementPressed' is a Boolean variable, that tracks when the movement actually occurs.
    let mutable movementPressed = Unchecked.defaultof<bool>

    // The 'runPressed' variable is used to track when the run action, i.e., the Left Shift key is pressed.
    let mutable runPressed = Unchecked.defaultof<bool>

    let walkMultiplier = 1.5f
    let runMultiplier = 4.0f
    let gravity = -9.8f
    let groundedGravity = -0.05f
    let rotationPerFrame = 10.0f
    let IsWalkingKey = Animator.StringToHash("IsWalking")
    let IsRunningKey = Animator.StringToHash("IsRunning")

    //********
    // The processMove() function takes in the Vector2 values from the player input,
    // and assigns them to a Vecotr3 type currentMoved, on order to map controls from
    // 2D space to 3D space.
    //********
    let processMove (context: InputAction.CallbackContext) = 
        currentMovementInput <- context.ReadValue<Vector2>()
        currentMovement.x <- currentMovementInput.x
        currentMovement.z <- currentMovementInput.y
        movementPressed <- currentMovementInput.x <> 0f || currentMovementInput.y <> 0f

    let processRun (context: InputAction.CallbackContext) = 
        runPressed <- context.ReadValueAsButton()

    // The OnEnable() and OnDisable() functions are controls from Unity, which are
    // used to enable to disable the player input controls.
    member this.OnEnable() = 
        characterControls.Enable()

    member this.OnDisable() = 
        characterControls.Disable()

    // The Awake() method is used to bind all the objects defined above. It binds the
    // objects defined in Unity, with the objects declared in the F# source code.
    // Most of the below objects are defined in Unity's State Machine window.
    member this.Awake() = 
        playerInput <- this.GetComponent<PlayerInput>()
        animator <- this.GetComponent<Animator>()
        characterController <- this.GetComponent<CharacterController>()
        mainCamera <- Camera.main
        characterControls <- playerInput.actions.FindActionMap("CharacterControls", throwIfNotFound = true)
        move <- characterControls.FindAction("Move", throwIfNotFound = true)
        run <- characterControls.FindAction("Run", throwIfNotFound = true)

        move.add_started processMove
        move.add_canceled processMove
        move.add_performed processMove

        run.add_started processRun
        run.add_canceled processRun

    //********
    // The HandleGravity() function manages the gravity component. Grounded gravity is added to 
    // prevent too much push through. It makes the physics engine more stable.
    // The below function does the following - if the player is not grounded, it will
    // fall at the rate of gravtity (set in a variable as -9.8f), and will added to
    // itself to give a cumulative effect.
    //********
    member _.HandleGravity() = 
        if characterController.isGrounded then
            let currentGravity = groundedGravity * Time.deltaTime
            currentMovement.y <- currentGravity
        else
            let currentGravity = gravity * Time.deltaTime
            currentMovement.y <- currentMovement.y + currentGravity

    //********
    // The HandleRotation() function is responsible for making the character face in the
    // direction it is moving. It takes the current direction in which the character is
    // facing, storing it in a Vector3 type variable. The Quaternion.LookRotation() and
    // Quaternion.Slerp() function are used to transform the direction from current to
    // the target location, based on the player input. The Quaternion.Slerp() function
    // performs a linear interpolation between the 2 rotations.
    //********
    member this.HandleRotation(relativeMovement: Vector3) = 
        if movementPressed then
            let positionToLookAt = Vector3(relativeMovement.x, 0.0f, relativeMovement.z)
            let currentRotation = this.transform.rotation
            let targetRotation = Quaternion.LookRotation(positionToLookAt)
            this.transform.rotation <- Quaternion.Slerp(currentRotation, targetRotation, rotationPerFrame * Time.deltaTime)

    //********
    // The HandleAnimation() function does the following - it maps the Boolean variabled defined in Unity IDE, 
    // with the corresponding variables in F# code. It then manages the boolean flags when the character is idle, 
    // walking, or running. Changes made by the F# code directly affects the character movement in the Unity environment.
    //********
    member _.HandleAnimation() = 
        let isWalkingAnimation = animator.GetBool(IsWalkingKey)
        let isRunningAnimation = animator.GetBool(IsRunningKey)

        if movementPressed && not isWalkingAnimation then
            animator.SetBool(IsWalkingKey, true)
        else if (not movementPressed && isWalkingAnimation) then
            animator.SetBool(IsWalkingKey, false)

        if ((movementPressed || runPressed) && not isRunningAnimation) then
            animator.SetBool(IsRunningKey, true)
        else if ((not movementPressed || not runPressed) && isRunningAnimation) then
            animator.SetBool(IsRunningKey, false)

    // The Update() method encapsulates all the functions defined to calculate
    // animation, gravity control, and rotations.
    member this.Update() = 
        this.HandleAnimation()
        this.HandleGravity()

        let relativeMovement = 
            let movement = 
                // Based on the keyboard input pressed, 
                if runPressed then currentMovement * (runMultiplier * Time.deltaTime)
                else currentMovement * (walkMultiplier * Time.deltaTime)
            Quaternion.Euler(0f, mainCamera.transform.rotation.eulerAngles.y, 0f) * movement

        // The relative movement for the character is calculated based on the operations
        // done in the HandleRotation() function. This function helps point the
        // character in the required direction.
        this.HandleRotation(relativeMovement)

        characterController.Move(relativeMovement)