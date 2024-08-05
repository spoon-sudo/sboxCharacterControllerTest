using Microsoft.VisualBasic;
using Sandbox;
using Sandbox.Citizen;

public sealed class PlayerMovement : Component
{

	[Property] public float groundControl {get; set;} = 4.0f;
	[Property] public float airControl {get; set;} = 0.1f;
	[Property] public float maxForce {get; set;} = 50f;
	[Property] public float speed {get; set;} = 160f;
	[Property] public float runSpeed {get; set;} = 290f;
	[Property] public float crouchSpeed {get; set;} = 90f;
	[Property] public float jumpForce {get; set;} = 400f;
	[Property] public GameObject head {get; set;}
	[Property] public GameObject body {get; set;}

	public Vector3 wishVelocity = Vector3.Zero;

	public bool isCrouching = false;

	public bool isSprinting = false;

	private CharacterController characterController;

	private CitizenAnimationHelper animation;


	protected override void OnAwake()
	{
		characterController = Components.Get<CharacterController>();
		animation = Components.Get<CitizenAnimationHelper>();
	}

	protected override void OnUpdate()
	{
		isCrouching = Input.Down("Duck");
		isSprinting = Input.Down("Run");
	}

	protected override void OnFixedUpdate()
	{
		BuildWishVelocity();
		Move();
	}

	void BuildWishVelocity()
	{
		wishVelocity = 0;
		
		var rot = head.Transform.Rotation;
		if (Input.Down("Forward")) wishVelocity += rot.Forward;
		if (Input.Down("Backward")) wishVelocity += rot.Backward;
		if (Input.Down("Left")) wishVelocity += rot.Left;
		if (Input.Down("Right")) wishVelocity += rot.Right;

		wishVelocity = wishVelocity.WithZ(0);

		if(!wishVelocity.IsNearZeroLength) wishVelocity = wishVelocity.Normal;

		if (isCrouching) wishVelocity *= crouchSpeed;
		else if (isSprinting) wishVelocity *= runSpeed;
		else wishVelocity *= speed;
	}

	void Move()
	{
		// Get gravity from our scene
		var gravity = Scene.PhysicsWorld.Gravity;
		
		if (characterController.IsOnGround)
		{
			characterController.Velocity = characterController.Velocity.WithZ(0);
			characterController.Accelerate(wishVelocity);
			characterController.ApplyFriction(groundControl);

		}
		else
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate(wishVelocity.ClampLength(maxForce));
			characterController.ApplyFriction(airControl);
		}

		characterController.Move();

		if(!characterController.IsOnGround)
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			characterController.Velocity = characterController.Velocity.WithZ(0);
		}

	}


}
