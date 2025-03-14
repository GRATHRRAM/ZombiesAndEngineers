using Godot;
using System;

namespace Guns
{
	public static class FireMode
	{
		public const bool Semi = false;
		public const bool Auto = true;
	}

	public partial class Gun : Node
	{
		/*
		IN (Youre Gun Name).cs!!!
		 
		public int Damage;
		public int MaxAmmo;
		public bool FireModes;
		
		*/

		private int AmmoInMag = 0;

		public Gun(Node3D Self, Vector3 LocalBulletStartingPosition)
		{

		}
	}
}
