 using System;
 using System.Collections;
 using Unity.Cinemachine;
 using TMPro;
 using Unity.VisualScripting.FullSerializer;
 using UnityEngine;
 using Random = UnityEngine.Random;
 using Task = System.Threading.Tasks.Task;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */
 
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonControllerTwinStick : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        
        [Tooltip("Dash speed of the character in m")]
        public float DashSpeed = 10.67f;

        [Tooltip("Sound Effect for the Dash")] 
        public AudioClip DashSound;

        [Tooltip("Volume of the Dash Sound Effect")]
        public float DashVolume = 1f;
        
        [Tooltip("Dash length of the character in s")]
        public float DashLength = 1f;
        
        [Tooltip("Dash cooldown of the character in s")]
        public float DashCoolDown = .50f;
        
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        
        // public GameObject directionalIndicator;

        // Shooting actions
        public static Action shootInput;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDMoveX;
        private int _animIDMoveZ;

        // Speed Trail
        public TrailRenderer _trail;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private TwinStickInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool pausePressed;

        // private GameStateScreen gameStateScreen;

        private GameObject mainCamera;
        private Vector2 aimVector2;
        

        private bool IsCurrentDeviceMouse
        {
            get
            {
                #if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
                #else
				return false;
                #endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            // turn boostertrail off
            _trail.enabled = false;
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<TwinStickInputs>();
            #if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
            #else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
            #endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            
            // gameStateScreen = FindObjectOfType<GameStateScreen>();
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            
            #if ENABLE_INPUT_SYSTEM
                        // Subscribe to the control scheme change event
                        _playerInput.onControlsChanged += OnControlsChanged;
            #endif  
            
            

            // GetComponent<PlayerIK>().Setup(transform);
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
            PlayerInput();
            
        }
        
        #if ENABLE_INPUT_SYSTEM
        private void OnControlsChanged(PlayerInput playerInput)
        {
            // Check if the current control scheme is KeyboardMouse or Gamepad
            bool isUsingGamepad = playerInput.currentControlScheme == "Gamepad";
            Debug.Log("Am I using gamepad: " + isUsingGamepad);
            
            // Optionally, lock the cursor to the center of the window when using the mouse
            Cursor.lockState = isUsingGamepad ? CursorLockMode.Locked : CursorLockMode.None;
        
            // Hide or show the cursor based on the control scheme
            Cursor.visible = !isUsingGamepad;
            

            // You can add additional logic here if you need to adjust UI elements or other aspects of your game based on the input device
        }
        #endif


        private void PlayerInput()
        {
            if (_input.shoot && shootInput != null)
            {
                shootInput?.Invoke();
            }

        }
        
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDMoveX = Animator.StringToHash("MoveX");
            _animIDMoveZ = Animator.StringToHash("MoveZ");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }
        

        private void LookAt(Vector3 lookPoint)
        {
            Vector3 heightCorrectedPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
            transform.LookAt(heightCorrectedPoint);
        }

        private void Move()
        {
            // if (gameStateScreen.isPaused || gameStateScreen.gameEnded) return;
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            
            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            
            
            _targetRotation = Mathf.Atan2(_input.move.x, _input.move.y) * Mathf.Rad2Deg;
            
            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            
            aimVector2 = _input.look;

            if (Mathf.Abs(aimVector2.x) > 0.05f || MathF.Abs(aimVector2.y) > 0.05f || MathF.Abs(_input.move.x) > 0.05f || MathF.Abs(_input.move.y) > 0.05f )
            {
                if (!IsCurrentDeviceMouse)
                {
                    // directionalIndicator.SetActive(aimVector2 != Vector2.zero);
                    aimVector2 = JoystickToScreenCoords(aimVector2 == Vector2.zero ? _input.move : aimVector2);
                }

                Ray ray = Camera.main.ScreenPointToRay(aimVector2);
                Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
                float rayDistance;

                if (groundPlane.Raycast(ray, out rayDistance))
                {
                    Vector3 point = ray.GetPoint(rayDistance);
                    point.y = transform.position.y;
                    LookAt(point);
                    CinemachineCameraTarget.transform.position = Vector3.Lerp(transform.position, point, 0.05f);
                }
            }
            else
            {
                // directionalIndicator.SetActive(false);
            }
            
            Quaternion rotation = Quaternion.Euler(0, -transform.eulerAngles.y, 0);
            Vector3 animationMovement = rotation * inputDirection;

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            
            // Update animation parameters
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                _animator.SetFloat(_animIDMoveX, animationMovement.x);
                _animator.SetFloat(_animIDMoveZ, animationMovement.z);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void SetMoveSpeed(float newSpeedAdjustment)
        {
            MoveSpeed += newSpeedAdjustment;
            SprintSpeed += newSpeedAdjustment;
        }

        public void SetBoosters(bool activeState)
        {
            _trail.enabled = activeState;
        Debug.Log("Boosters set to: " + activeState);
        }
        
        Vector2 JoystickToScreenCoords(Vector2 joystickInput)
        {
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            
            float x = (joystickInput.x + 1) / 2 * screenWidth;
            float y = (joystickInput.y + 1) / 2 * screenHeight;

            return new Vector2(x, y);
        }
    }
