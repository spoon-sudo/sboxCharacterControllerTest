using System;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.Utility;
using Steamworks.Data;
using Sandbox.UI;
public sealed class CameraRotate : Component
{

	[Property] public float Distance { get; set; } = 125f;
	[Property] public float transitionDuration { get; set; } = 1f;
	[Property] public PlayerMovement Player { get; set; }
	[Property] public GameObject head { get; set; }

	
	[Property] public GameObject body { get; set; }
	
	[Property] public GameObject screenUI { get; set; }
	public bool isFirstPerson => Distance == 0f;
	private CameraComponent camera;
	private Vector3 CurrentOffset = Vector3.Zero;
	private ModelRenderer BodyRenderer;

	private ScreenPanel screen;

	
	protected override void OnAwake()
	{
		camera = Components.Get<CameraComponent>();
		BodyRenderer = body.Components.Get<ModelRenderer>();
		screen = screenUI.Components.Get<ScreenPanel>();
		screen.Opacity = 0f;
	}


	protected override void OnUpdate()

	{
		
		var eyeAngles = head.Transform.Rotation.Angles();
		eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		eyeAngles.roll = 0f;
		eyeAngles.pitch = eyeAngles.pitch.Clamp( -89.9f, 89.9f );
		head.Transform.Rotation = eyeAngles.ToRotation();



		if ( Input.Pressed( "Voice" ) )
		{
			float targetDistance = Distance == 0f ? 125f : 0f;
			_ = SmoothTransition( targetDistance );

			if (screen is not null)
			{
				screen.Opacity = isFirstPerson ? 0f : 1f;
			}
		}

		var targetOffset = Vector3.Zero;
		if ( Player.isCrouching ) targetOffset += Vector3.Down * 32f;
		CurrentOffset = Vector3.Lerp( CurrentOffset, targetOffset, Time.Delta * 10f );

		if ( camera is not null )
		{
			var camPos = head.Transform.Position + CurrentOffset;

			if ( !isFirstPerson )
			{
				var camForward = eyeAngles.ToRotation().Forward;
				var camTrace = Scene.Trace.Ray( camPos, camPos - (camForward * Distance) ).WithoutTags( "player", "trigger" ).Run();

				if ( camTrace.Hit )
				{
					camPos = camTrace.HitPosition + camTrace.Normal;
				}
				else
				{
					camPos = camTrace.EndPosition;
				}
				BodyRenderer.RenderType = ModelRenderer.ShadowRenderType.On;

			}
			else
			{
				BodyRenderer.RenderType = ModelRenderer.ShadowRenderType.ShadowsOnly;
			}

			camera.Transform.Position = camPos;
			camera.Transform.Rotation = eyeAngles.ToRotation();
		}

	}

	private async Task SmoothTransition( float targetDistance )
	{
		float startDistance = Distance;
		float elapsedTime = 0f;

		while ( elapsedTime < transitionDuration )
		{
			float t = elapsedTime / transitionDuration;

			t = EaseInOutCubic( t );

			Distance = Lerp( startDistance, targetDistance, t );
			elapsedTime += Time.Delta;
			await Task.Delay( 1 );
		}

		Distance = targetDistance;
	}

	private float Lerp( float start, float end, float t )
	{
		return start + (end - start) * t;
	}
	private float EaseInOutCubic( float t )
	{
		return t < 0.5f ? 4f * t * t * t : 1f - MathF.Pow( -2f * t + 2f, 3f ) / 2f;
	}

}
