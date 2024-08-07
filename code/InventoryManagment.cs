using Sandbox;

public sealed class InventoryManagment : Component
{

	
	

	[Property] public GameObject head { get; set; }

	protected override void OnUpdate()
	{
		itemPickup();
	}

	void itemPickup()
	{
		var inv = InventorySystem.Instance;
		var startPos = head.Transform.Position;
		var dir = head.Transform.Rotation.Forward;
		var tr = Scene.Trace.Ray( startPos, startPos + (dir * 150f) ).Run();
		
		if ( tr.Hit && Input.Pressed("use"))
		{
			var item = tr.GameObject;
			
			inv.AddItem(new Item(item.Name, 1, item));
			item.Destroy();

		}
	}
}
