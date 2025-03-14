using Godot;
using System;
using PlayerTools;

public partial class Player : CharacterBody3D
{
	[Export]
	public float WalkSpeed = 5f;

	[Export]
	public float RunSpeed = 12f;

	[Export]
	public float CrouchSpeed = 2f;

	private float Speed = 0f;

	[Export]
	public float Sensitivity = 0.002f;

	[Export]
	public float Gravity = 9.81f;

	[Export]
	public float JumpForce = 5f;

	private bool Focused = true;

	private bool Crouch = false;

	private bool Run = false;

	private bool ShowGui = true;

	/* Head Bob */
	[Export]
	public float BobFreq = 2.0f;

	[Export]
	public float BobAmp  = 0.08f;

	private float Bob_t = 0.0f;

	/* Nodes */
	
	private Node3D head = null;
	private Camera3D Camera = null;
	private Control Gui = null;

	/* Placer */

	private PlayerTools.Placer Placer = null;
	private bool BuildMode = false;

	private float Bool2Float(bool _Bool)
	{
		if (_Bool) return 1.0f;
		else return 0.0f;
	}

	private Vector3 HeadBob(float time)
	{
		return new Vector3((float) Math.Cos(time * BobFreq / 2) * BobAmp,(float) Math.Sin(time * BobFreq) * BobAmp,0 );
	}

	private void UpdateGui()
	{
		if (ShowGui)
		{
			Gui.Visible = true;
			Gui.GetNode<Label>("fps").Text = $"FPS {Engine.GetFramesPerSecond()}";
			if (BuildMode)
			{
				Control BuildMenu = Gui.GetNode("BuildMenu") as Control;
				BuildMenu.Visible = true;

				if (Placer.Snap == 0) BuildMenu.GetNode<Label>("Snap").Text = "Snap: None";
				if (Placer.Snap == 1) BuildMenu.GetNode<Label>("Snap").Text = "Snap: 1";
				if (Placer.Snap == 2) BuildMenu.GetNode<Label>("Snap").Text = "Snap: 0.5";
				if (Placer.Snap == 3) BuildMenu.GetNode<Label>("Snap").Text = "Snap: 0.1";

				if ( Placer.RotSnap) BuildMenu.GetNode<Label>("Snap").Text += "\nRotSnap: True";
				if (!Placer.RotSnap) BuildMenu.GetNode<Label>("Snap").Text += "\nRotSnap: False";

				BuildMenu.GetNode<Label>("Rotation").Text = "Rotation: {" +
					Placer.Rotation.X + "," +
					Placer.Rotation.Y + "," +
					Placer.Rotation.Z + "}";
			}
			else Gui.GetNode<Control>("BuildMenu").Visible = false;
		}
		else Gui.Visible = false;
	}

	public override void _Ready()
	{
		base._Ready();
		Input.MouseMode = Input.MouseModeEnum.Captured;

		head = GetNode("head") as Node3D;
		Camera = GetNode("head/Camera") as Camera3D;
		Gui = GetNode("head/Camera/Gui") as Control;

		Speed = WalkSpeed;

		Placer = new Placer(this, "res://items/Box.tscn", "res://items/PlaceHolder.tscn");

		Gui.GetNode<TextureRect>("Cursor").Position = new Vector2(GetWindow().Size.X / 2, GetWindow().Size.Y / 2);
	}

	public override void _Input(InputEvent @event)
	{
		base._Input(@event);

		if (Input.IsActionJustPressed("Menu"))
		{
			Focused = !Focused;
			if (Focused)  Input.MouseMode = Input.MouseModeEnum.Captured;
			if (!Focused) Input.MouseMode = Input.MouseModeEnum.Visible;
		}

		if (Input.IsActionPressed("Run") && !Crouch) Run = true;
		else Run = false;

		if (Input.IsActionJustPressed("Crouch")) Crouch = !Crouch;

		if (Input.IsActionJustPressed("ToggleGui")) ShowGui = !ShowGui;

		if (Input.IsActionJustPressed("BuildMode"))
		{
			BuildMode = !BuildMode;

			if (BuildMode) Placer.ShowPlaceHolder();
			else Placer.HidePlaceHolder();
		}

		if (BuildMode)
		{
			if (Input.IsActionJustPressed("BuildModeRotSnap")) Placer.RotSnap = !Placer.RotSnap;

			if (Input.IsActionJustPressed("BuildMenuResetRot")) Placer.Rotation = Vector3.Zero;

			if (Input.IsActionJustPressed("BuildModeSnap"))
			{
				Placer.Snap++;
				if (Placer.Snap > 3) Placer.Snap = 0;
			}

			if (Placer.RotSnap)
			{
				if (Input.IsActionJustPressed("BuildModeRotateLeft")) Placer.Rotation.Y -= 25;
				if (Input.IsActionJustPressed("BuildModeRotateRight"))Placer.Rotation.Y += 25;
				if (Input.IsActionJustPressed("BuildModeRotateUp"))   Placer.Rotation.X -= 25;
				if (Input.IsActionJustPressed("BuildModeRotateDown")) Placer.Rotation.X += 25;

				if (Placer.Rotation.Y > 360) Placer.Rotation.Y -= 360;
				if (Placer.Rotation.Y < 0)   Placer.Rotation.Y += 360;
				if (Placer.Rotation.X > 360) Placer.Rotation.Y -= 360;
				if (Placer.Rotation.X < 0)   Placer.Rotation.Y += 360;

			}
			else
			{
				if (Input.IsActionPressed("BuildModeRotateLeft"))  Placer.Rotation.Y -= 0.01f;
				if (Input.IsActionPressed("BuildModeRotateRight")) Placer.Rotation.Y += 0.01f;
				if (Input.IsActionPressed("BuildModeRotateUp"))    Placer.Rotation.X -= 0.01f;
				if (Input.IsActionPressed("BuildModeRotateDown"))  Placer.Rotation.X += 0.01f;
			}
		}

		if (@event is InputEventMouseButton _Event)
		{
			if (_Event.IsActionPressed("BuildModePlace")   && BuildMode) Placer.Place();
			if (_Event.IsActionPressed("BuildModeDestroy") && BuildMode) Placer.Destroy();

		}

		if (@event is InputEventMouseMotion Event)
		{
			head.RotateY(-Event.Relative.X * Sensitivity);
			Camera.RotateX(-Event.Relative.Y * Sensitivity);
			float CamRot = Math.Clamp(Camera.Rotation.X, Mathf.DegToRad(-60), Mathf.DegToRad(60));

			Vector3 Rot = Camera.Rotation;
			Rot.X = CamRot;
			Camera.Rotation = Rot;
		}

	}

	/*
	public override void _UnhandledInput(InputEvent @event)
	{
		base._UnhandledInput(@event);

		if(@event is InputEventMouseMotion Event)
		{
			head.RotateY(-Event.Relative.X * Sensitivity);
			Camera.RotateX(-Event.Relative.Y * Sensitivity);
			float CamRot = Math.Clamp(Camera.Rotation.X, Mathf.DegToRad(-60), Mathf.DegToRad(60));

			Vector3 Rot = Camera.Rotation;
			Rot.X = CamRot;
			Camera.Rotation = Rot;
		}
	}*/

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);

		Vector3 Vel = Velocity;

		if (!IsOnFloor()) Vel.Y -= Gravity * (float) delta;

		if (Input.IsActionJustPressed("Jump") && IsOnFloor()) Vel.Y += JumpForce;

		Vector2 InputDir = Input.GetVector("GoLeft", "GoRight", "GoForward", "GoBackward");
		Vector3 Direction = head.Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y);
		Direction = Direction.Normalized();

		if(IsOnFloor()) 
		{ 
			if(Direction != Vector3.Zero)
			{
				Vel.X = Direction.X * Speed;
				Vel.Z = Direction.Z * Speed;
			}
			else
			{
				Vel.X = Mathf.Lerp(Vel.X, Direction.X * Speed, (float) delta * 5f);
				Vel.Z = Mathf.Lerp(Vel.Z, Direction.Z * Speed, (float) delta * 5f);
			}
		}
		else
		{
			Vel.X = Mathf.Lerp(Vel.X, Direction.X * Speed, (float)delta * 2f);
			Vel.Z = Mathf.Lerp(Vel.Z, Direction.Z * Speed, (float)delta * 2f);
		}

		Velocity = Vel;

		if (Crouch)
		{
			Vector3 _Scale = Scale;
			_Scale.Y = Mathf.Lerp(_Scale.Y, 0.5f, 0.1f);
			Scale = _Scale;

			Speed = Mathf.Lerp(Speed, CrouchSpeed, 0.1f);
		}
		else
		{
			Vector3 _Scale = Scale;
			_Scale.Y = Mathf.Lerp(_Scale.Y, 1f, 0.1f);
			Scale = _Scale;
		}

		if (Run)
		{
			Speed = Mathf.Lerp(Speed, RunSpeed, 0.1f);
			Camera.Fov = Mathf.Lerp(Camera.Fov, 100f, 0.1f);
		}
		else
		{
			if(!Crouch) Speed = Mathf.Lerp(Speed, WalkSpeed, 0.1f);
			Camera.Fov = Mathf.Lerp(Camera.Fov, 75f, 0.1f);
		}

		Bob_t += (float)delta * Velocity.Length() * Bool2Float(IsOnFloor());
		Transform3D Transform = Camera.Transform;
		Transform.Origin = HeadBob(Bob_t);
		Camera.Transform = Transform;

		if (BuildMode) Placer.PlaceHolderUpdate();
		
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		UpdateGui();
	}
}
