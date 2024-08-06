using System;
using Microsoft.VisualBasic;
using Sandbox;
using Sandbox.Citizen;

public sealed class PlayerMovement : Component
{

	[Property] public float groundControl { get; set; } = 4.0f;
	[Property] public float airControl { get; set; } = 0.1f;
	[Property] public float maxForce { get; set; } = 50f;
	[Property] public float speed { get; set; } = 160f;
	[Property] public float runSpeed { get; set; } = 290f;
	[Property] public float crouchSpeed { get; set; } = 90f;
	[Property] public float jumpForce { get; set; } = 400f;
	[Property] public GameObject head { get; set; }
	[Property] public GameObject body { get; set; }

	[Property] public GameObject prefabClone { get; set; }



	public Vector3 wishVelocity = Vector3.Zero;

	public bool isCrouching = false;

	public bool isSprinting = false;

	public bool isSitting = false;

	private CharacterController characterController;

	private CitizenAnimationHelper animation;


	protected override void OnAwake()
	{
		characterController = Components.Get<CharacterController>();
		animation = Components.Get<CitizenAnimationHelper>();

	}

	protected override void OnUpdate()
	{
		drawObjects();
		UpdateCrouch();
		isSprinting = Input.Down( "Run" );
		if ( Input.Pressed( "Jump" ) && !isSitting ) jump();
		sit();
		rotateBody();
		updateAnimations();


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
		if ( Input.Down( "Forward" ) ) wishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) wishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) wishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) wishVelocity += rot.Right;

		wishVelocity = wishVelocity.WithZ( 0 );

		if ( !wishVelocity.IsNearZeroLength ) wishVelocity = wishVelocity.Normal;

		if ( isCrouching ) wishVelocity *= crouchSpeed;
		else if ( isSprinting ) wishVelocity *= runSpeed;
		else wishVelocity *= speed;
	}

	void Move()
	{

		var gravity = Scene.PhysicsWorld.Gravity;

		if ( characterController.IsOnGround )
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );
			characterController.Accelerate( wishVelocity );
			characterController.ApplyFriction( groundControl );

		}
		else
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
			characterController.Accelerate( wishVelocity.ClampLength( maxForce ) );
			characterController.ApplyFriction( airControl );
		}

		if ( !isSitting )
		{
			characterController.Move();
		}

		if ( !characterController.IsOnGround )
		{
			characterController.Velocity += gravity * Time.Delta * 0.5f;
		}
		else
		{
			characterController.Velocity = characterController.Velocity.WithZ( 0 );

		}

	}

	void rotateBody()
	{
		if ( body is null ) return;

		var targetangles = new Angles( 0, head.Transform.Rotation.Yaw(), 0 ).ToRotation();
		float rotateDiffrence = body.Transform.Rotation.Distance( targetangles );

		if ( !isSitting )
		{
			if ( rotateDiffrence > 50 || characterController.Velocity.Length > 10f )
			{
				body.Transform.Rotation = Rotation.Lerp( body.Transform.Rotation, targetangles, Time.Delta * 10f );
			}
		}



	}

	void jump()
	{
		if ( !characterController.IsOnGround ) return;

		characterController.Punch( Vector3.Up * jumpForce );
		animation?.TriggerJump();

	}


	void updateAnimations()
	{
		if ( animation is null ) return;

		animation.WithWishVelocity( wishVelocity );
		animation.WithVelocity( characterController.Velocity );
		animation.AimAngle = head.Transform.Rotation;
		animation.IsGrounded = characterController.IsOnGround;
		animation.WithLook( head.Transform.Rotation.Forward, 1f, 7.5f, 0.5f );
		animation.MoveStyle = isSprinting ? CitizenAnimationHelper.MoveStyles.Run : CitizenAnimationHelper.MoveStyles.Walk;
		animation.DuckLevel = isCrouching ? 1f : 0f;

		animation.Sitting = isSitting ? CitizenAnimationHelper.SittingStyle.Floor : CitizenAnimationHelper.SittingStyle.None;

	}

	void UpdateCrouch()
	{
		if ( characterController is null ) return;

		if ( Input.Pressed( "Duck" ) && !isCrouching )
		{
			isCrouching = true;
			characterController.Height /= 2f;
		}
		if ( Input.Released( "Duck" ) && isCrouching )
		{
			isCrouching = false;
			characterController.Height *= 2f;

		}

	}

	void sit()
	{
		if ( characterController is null ) return;

		if ( Input.Pressed( "Sit" ) && characterController.IsOnGround )
		{
			isSitting = isSitting ? false : true;


		}
	}

	void drawObjects()
	{
		var startPos = head.Transform.Position;
		var dir = head.Transform.Rotation.Forward;
		var tr = Scene.Trace.Ray( startPos, startPos + (dir * 1000f) ).Run();
		var box = new BBox( new Vector3( -30, -10, -30 ), new Vector3( 30, 0, 30 ) );

		if ( tr.Hit )
		{
			Gizmo.Draw.Arrow( startPos, tr.HitPosition );
			
			var hitNormal = tr.Normal;
			var rotation = Rotation.LookAt(hitNormal, Vector3.Up) * Rotation.FromAxis(Vector3.Down, 90);
			var finalBox = box.Rotate(rotation).Translate(tr.HitPosition - box.Center);
			
			
			if (Input.Pressed("Attack1")) 
			{
				prefabClone.Clone(tr.EndPosition + tr.Normal);
			}
			
			Gizmo.Draw.LineBBox( finalBox );



		}

	}



}


