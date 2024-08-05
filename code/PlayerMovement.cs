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
		
	}

	void BuildWishVelocity()
	{
		wishVelocity = 0;
		
		var rot = head.Transform.Rotation;

		if (Input.Down("Forward")) wishVelocity += rot.Forward;
		if (Input.Down("Backward")) wishVelocity += rot.Backward;
		if (Input.Down("Left")) wishVelocity += rot.Left;
		if (Input.Down("Right")) wishVelocity += rot.Right;

		
	}
}
