using Godot;
using System;

public partial class MeleeTest : RigidBody3D
{
	[Export]
	public int Mode = 0; /* 0 = hidden ; 1 = InHand ; 2 = Item */

	[Export]
	public int Damage = 25;

	private bool AttackFinished = true;

	public override void _EnterTree()
	{
		base._EnterTree();

		if (Mode == 0) { Freeze = true;  Visible = false; }
		if (Mode == 1) { Freeze = true;  Visible = true;  }
		if (Mode == 2) { Freeze = false; Visible = true;  }
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (Mode == 0) { Freeze = true; Visible = false; return; }

		if (Mode == 1) 
		{ 
			Freeze = true;
			Visible = true; 

			if (Input.IsActionJustPressed("Shoot") && AttackFinished)
			{
				GetNode<AnimationPlayer>("Ap").Play("Attack");
				AttackFinished = false;
			}

			if (Input.IsActionJustPressed("Aim"))
			{

			}


			if (Input.IsActionJustPressed("Drop"))
			{
				//Reparent()
			}
		}

		if (Mode == 2) { Freeze = false; Visible = true; }
	}

	public void _AnimationFinished(StringName AName)
	{
		AttackFinished = true;
	}
}
