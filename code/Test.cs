using Sandbox;

public sealed class Test : Component
{
	protected override void OnUpdate()
	{
		if (Input.Down("forward"))
		{
			Transform.Position += Vector3.Forward * 25f * Time.Delta;
		}
		if (Input.Down("backward"))
		{
			Transform.Position += Vector3.Backward * 25f * Time.Delta;
		}
		if (Input.Down("left"))
		{
			Transform.Position += Vector3.Left * 25f * Time.Delta;
		}
		if (Input.Down("right"))
		{
			Transform.Position += Vector3.Right * 25f * Time.Delta; 
		}

		
	}
}
