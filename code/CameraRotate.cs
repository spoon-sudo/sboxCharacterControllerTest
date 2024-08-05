using System;
using System.IO;
using System.Numerics;
using System.Runtime;
using System.Threading.Tasks;
using Sandbox;

public sealed class CameraRotate : Component
{

	[Property] public float Distance {get; set;} = 0f;
	[Property] public GameObject head {get; set;}

	public bool isFirstPerson => Distance == 0f;
	private CameraComponent camera;

	protected override void OnAwake()
	{
		camera = Components.Get<CameraComponent>();
	}
	

	protected override void OnUpdate()

	{
		
		var eyeAngles = head.Transform.Rotation.Angles();
		eyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		eyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		eyeAngles.roll = 0f;
		eyeAngles.pitch = eyeAngles.pitch.Clamp(-89.9f,89.9f);
		head.Transform.Rotation = eyeAngles.ToRotation();



		if (camera is not null)
		{
			var camPos = head.Transform.Position;
			
			if (!isFirstPerson)
			{
				var camForward = eyeAngles.ToRotation().Forward;
				var camTrace = Scene.Trace.Ray(camPos,camPos - (camForward * Distance)).WithoutTags("player","trigger").Run();

				if (camTrace.Hit)
				{
					camPos = camTrace.HitPosition + camTrace.Normal;
				}
				else
				{
					camPos = camTrace.EndPosition;
				}
			}

			camera.Transform.Position = camPos;
			camera.Transform.Rotation = eyeAngles.ToRotation();
		}
		
	}

}
