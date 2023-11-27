namespace fs

open UnityEngine
open UnityEngine.InputSystem

type AnimationMovementController() = 
    inherit MonoBehaviour()

    let mutable playerInput: PlayerInput = null
    let mutable animator: Animator = null
    let mutable characterController: CharacterController = null
    let mutable characterControls: InputActionMap = null
    let mutable mainCamera: Camera = null

    let mutable move = null
    let mutable run = null
    let mutable currentMovementInput = Unchecked.defaultof<Vector2>
    let mutable currentMovement = Unchecked.defaultof<Vector3>
    let mutable movementPressed = Unchecked.defaultof<bool>
    let mutable runPressed = Unchecked.defaultof<bool>

    let walkMultiplier = 1.5f
    let runMultiplier = 4.0f
    let gravity = -9.8f
    let groundedGravity = -0.05f
    let rotationPerFrame = 10.0f
    let IsWalkingKey = Animator.StringToHash("IsWalking")
    let IsRunningKey = Animator.StringToHash("IsRunning")

    let processMove (context: InputAction.CallbackContext) = 
        currentMovementInput <- context.ReadValue<Vector2>()
        currentMovement.x <- currentMovementInput.x
        currentMovement.z <- currentMovementInput.y
        movementPressed <- currentMovementInput.x <> 0f || currentMovementInput.y <> 0f

    let processRun (context: InputAction.CallbackContext) = 
        runPressed <- context.ReadValueAsButton()

    member this.OnEnable() = 
        characterControls.Enable()

    member this.OnDisable() = 
        characterControls.Disable()

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

    member _.HandleGravity() = 
        if characterController.isGrounded then
            let currentGravity = groundedGravity * Time.deltaTime
            currentMovement.y <- currentGravity
        else
            let currentGravity = gravity * Time.deltaTime
            currentMovement.y <- currentMovement.y + currentGravity

    member this.HandleRotation(relativeMovement: Vector3) = 
        if movementPressed then
            let positionToLookAt = Vector3(relativeMovement.x, 0.0f, relativeMovement.z)
            let currentRotation = this.transform.rotation
            let targetRotation = Quaternion.LookRotation(positionToLookAt)
            this.transform.rotation <- Quaternion.Slerp(currentRotation, targetRotation, rotationPerFrame * Time.deltaTime)

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

    member this.Update() = 
        this.HandleAnimation()
        this.HandleGravity()
        let relativeMovement = 
            let movement = 
                if runPressed then currentMovement * (runMultiplier * Time.deltaTime)
                else currentMovement * (walkMultiplier * Time.deltaTime)
            Quaternion.Euler(0f, mainCamera.transform.rotation.eulerAngles.y, 0f) * movement

        this.HandleRotation(relativeMovement)

        characterController.Move(relativeMovement)