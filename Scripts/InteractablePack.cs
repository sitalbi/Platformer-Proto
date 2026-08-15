using Godot;
using System;

public partial class InteractablePack : Node3D
{
    [Export]
    public PackedScene Interactable;

    [Export]
    public int Count = 100;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        for (int i = 0; i < Count; ++i)
        {
            RigidBody3D instantiatedInteractable = Interactable.Instantiate<RigidBody3D>();
            AddChild(instantiatedInteractable);
            instantiatedInteractable.GlobalPosition = GlobalPosition;
        }
    }
}
