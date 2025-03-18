using Godot;
using System;

public partial class Bullet : Node3D
{
	[Export]
	public float Speed = 100f;

	[Export]
	public float LifeTime = 10f; //Seconds

	public int Damage = 0;

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		if (LifeTime < 0) QueueFree();

		Position += Transform.Basis * new Vector3((Speed * (float)delta), 0,0);

		LifeTime -= (float) delta;
	}

	public void _OnBodyEntered(Node3D Body)
	{
		QueueFree();
	}
}
