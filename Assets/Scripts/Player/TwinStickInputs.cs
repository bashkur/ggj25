using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif


	public class TwinStickInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool dash;
		public bool sprint;
		public bool reload;
		public bool shoot;
		public bool pause;
		public bool pickup;


		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorInputForLook = true;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnDash(InputValue value)
		{
			DashInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}
		
		public void OnReload(InputValue value)
		{
			ReloadInput(value.isPressed);
		}
		
		public void OnPause(InputValue value)
		{
			PauseInput(value.isPressed);
		}
		
		public void OnPickup(InputValue value)
		{
			PickupInput(value.isPressed);
		}
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}
		
		public void DashInput(bool newDashState)
		{
			dash = newDashState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}
		public void ReloadInput(bool newReloadState)
		{
			reload = newReloadState;
		}
		
		public void PauseInput(bool newPauseState)
		{
			pause = newPauseState;
		}
		
		public void PickupInput(bool newPickupState)
		{
			pickup = newPickupState;
		}
	}