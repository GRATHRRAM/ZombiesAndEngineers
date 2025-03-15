using Godot;
using System;

namespace PlayerTools
{
	public class Placer
	{
		private PackedScene     Node2Place = null;
		private MeshInstance3D  PlaceHolder = null;
		private RayCast3D       RayCast = null;
		private CharacterBody3D Player = null;

		private bool InitFail = false;

		public Vector3 Rotation = new Vector3();
		
		public UInt16 Snap = 1;
		public bool RotSnap = true;

		public Placer(CharacterBody3D _Player, string PlacingObjPath, string PlaceHolderPath)
		{
			Player = _Player;
			if (Player == null) { GD.Print("Placer: Player is NULL!!"); InitFail = true; return;}

			RayCast = Player.GetNode("head/Camera/RayCast3D") as RayCast3D;
			if (RayCast == null) { GD.Print("Placer: RayCast is NULL!!"); InitFail = true; return;}

			PackedScene ScenePlaceHolder = GD.Load(PlaceHolderPath) as PackedScene;
			if (ScenePlaceHolder == null) { GD.Print("Placer: Cant Load PlaceHolder!!!"); InitFail = true; return;}

			Node2Place = GD.Load(PlacingObjPath) as PackedScene;
			if (Node2Place == null) { GD.Print("Placer: Cant Load Node2Place!!!"); InitFail = true; return;}

			PlaceHolder = ScenePlaceHolder.Instantiate() as MeshInstance3D;
			PlaceHolder.Visible = false;
			Player.GetParent().CallDeferred("add_child", PlaceHolder);
		}
		
		private Vector3 _Snap(Vector3 Position)
		{
			if (Snap == 0) return Position;
			else if (Snap == 1) return (Vector3I) Position;
			else if (Snap == 2)
			{
				Position.X = (float) Math.Floor(Position.X / 0.5f) * 0.5f;
				Position.Y = (float) Math.Floor(Position.Y / 0.5f) * 0.5f;
				Position.Z = (float )Math.Floor(Position.Z / 0.5f) * 0.5f;
				return Position;
			}
			else
			{
				Position.X = (float)Math.Floor(Position.X / 0.1f) * 0.1f;
				Position.Y = (float)Math.Floor(Position.Y / 0.1f) * 0.1f;
				Position.Z = (float)Math.Floor(Position.Z / 0.1f) * 0.1f;
				return Position;
			}
		}

		public void Place()
		{
			StaticBody3D obj = Node2Place.Instantiate() as StaticBody3D;
			obj.Position = _Snap(RayCast.GetCollisionPoint() + (RayCast.GetCollisionNormal() / 2));
			obj.Rotation = Rotation;
			Player.GetParent().CallDeferred("add_child", obj);
		}

		public void Destroy()
		{
			if(RayCast.GetCollider() != null) 
				if(RayCast.GetCollider().HasMeta("PlayerPlaced"))
					RayCast.GetCollider().Free();
		}

		public void PlaceHolderUpdate()
		{
			PlaceHolder.Position = _Snap(RayCast.GetCollisionPoint() + (RayCast.GetCollisionNormal() / 2));
			PlaceHolder.Rotation = Rotation;
		}

		public void HidePlaceHolder()
		{
			PlaceHolder.Visible = false;
		}

		public void ShowPlaceHolder()
		{
			PlaceHolder.Visible = true;
		}
	};
}
