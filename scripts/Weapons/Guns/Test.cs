using Godot;
using System;

public partial class Test : Node3D
{
	[Export]
	public int MaxMagAmmo = 7;

	[Export]
	public int Damage = 25;

	[Export]
	public bool OnlySemi = true;

	[Export]
	public Node3D BulletSpawnPoint = null;

	[Export]
	public PackedScene BulletScene = null;
}
