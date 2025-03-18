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

	private Global gl = null;

	private int Ammo = 0;

	private bool ShootFinished = true;
	private bool ReloadFinished = true;
	private bool Aim = false;

	private AnimationPlayer Ap = null;

	public override void _EnterTree()
	{
		base._EnterTree();

		if (BulletSpawnPoint == null) GD.Print($"{Name} -> Class Test (Gun) -> BulletSpawnPoint == null!!!");
		if (BulletScene      == null) GD.Print($"{Name} -> Class Test (Gun) -> BulletScene == null!!!");

		Ap = GetNode<AnimationPlayer>("AnimationPlayer");
		Ammo = MaxMagAmmo;

		gl = GetNode<Global>("/root/_Global");
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (!Visible) return;
		gl.Player.GetNode<Control>("head/Camera/Gui/GunInfo").Visible = true;
		gl.Player.GetNode<Label>("head/Camera/Gui/GunInfo/Ammo").Text = Ammo.ToString() + "/" + gl.Player.Get("Ammo").AsString();

		if (Input.IsActionJustPressed("Shoot") && Ammo > 0 && ShootFinished && ReloadFinished)
		{
			Node3D Bullet = BulletScene.Instantiate() as Node3D;
			Bullet.Position = BulletSpawnPoint.GlobalPosition;
			Bullet.Rotation = BulletSpawnPoint.GlobalRotation;
			Bullet.Set("Damage", Damage);
			gl.MainScene.AddChild(Bullet);
			Ammo--;
			Ap.Play("Shoot");
			ShootFinished = false;
		}

		if (Input.IsActionJustPressed("Aim")) 
		{
			if (!Aim) gl.Player.GetNode<AnimationPlayer>("Ap").Play("Aim");
			else	  gl.Player.GetNode<AnimationPlayer>("Ap").Play("DeAim");
			Aim = !Aim; 
		}

		if (Input.IsActionJustPressed("Reload") && ReloadFinished)
		{
			Ammo += gl.Player.Get("Ammo").AsInt32();
			if (Ammo == 0) return;

			if (Ammo >= MaxMagAmmo)
			{
				gl.Player.Set("Ammo", Ammo - MaxMagAmmo);
				Ammo = MaxMagAmmo;
			}
			else gl.Player.Set("Ammo", 0);
		
			Ap.Play("Reload");
			ReloadFinished = false;
		}
	}

	public void _AnimationFinished(StringName Animation)
	{
		if (Animation == "Shoot")  ShootFinished = true;
		if (Animation == "Reload") ReloadFinished = true;
	}

	public void _SetAmmo(int _Ammo)
	{
		Ammo = _Ammo;
	}
}
