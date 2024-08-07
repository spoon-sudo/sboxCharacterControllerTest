using Sandbox;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

public sealed class InventorySystem : Component
{
	private static InventorySystem instance;
	private List<Item> items;
	
	public static InventorySystem Instance
	{
		get
		{
			if ( instance == null )
			{
				instance = new InventorySystem();
				
			}
			return instance;
			
		}
	}
	public int GetTotalItemCount()
    {
        return items.Sum(item => item.Quantity);
    }


	public InventorySystem()
	{
		items = new List<Item>();
	}

	public void ClearInventory()
	{
		items.Clear();
		Log.Info( "Inventory cleared." );
	}

	public void AddItem( Item item )
	{
		try
		{
			items.Add( item );
			
			Log.Info( $"{item.Name} added to inventory." );
			
			
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to add item: {ex.Message}" );
		}
	}

	public void RemoveItem( Item item )
	{
		try
		{
			if ( items.Contains( item ) )
			{
				items.Remove( item );
				Log.Info( $"{item.Name} removed from inventory." );
			}
			else
			{
				Log.Warning( $"{item.Name} not found." );
			}
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to remove: {ex.Message}" );
		}
	}

	public Item GetItem( string itemName )
	{
		try
		{
			return items.Find( item => item.Name == itemName );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to get: {ex.Message}" );
			return null;
		}
	}

	public List<Item> GetAllItems()
	{
		try
		{
			return new List<Item>( items );
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to get all items: {ex.Message}" );
			return new List<Item>();
		}
	}

	public void DisplayInventory()
	{
		try
		{
			Log.Info( "Inventory:" );
			foreach ( var item in items )
			{
				Log.Info( $"- {item.Name} (Quantity: {item.Quantity})" );

			}
		}
		catch ( Exception ex )
		{
			Log.Error( $"Failed to display inventory: {ex.Message}" );
		}
	}

	protected override void OnUpdate()
	{

	}
}

public class Item
{
	public string Name { get; set; }
	public int Quantity { get; set; }
	public GameObject Prefab { get; set; }

	public Item( string name, int quantity, GameObject prefab )
	{
		Name = name;
		Quantity = quantity;
		Prefab = prefab;
	}
}
