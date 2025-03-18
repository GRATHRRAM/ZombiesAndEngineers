using Godot;
using System;

public partial class MainScene : Node3D
{
	public override void _EnterTree()
	{
		base._EnterTree();

		Global gl = GetNode<Global>("/root/_Global");
		gl.MainScene = this;
	}
}
