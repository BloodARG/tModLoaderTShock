using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.GameContent.Achievements;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Terraria
{
	public class Mount
	{
		private class DrillBeam
		{
			public Point16 curTileTarget;

			public int cooldown;

			public DrillBeam()
			{
				this.curTileTarget = Point16.NegativeOne;
				this.cooldown = 0;
			}
		}

		private class DrillMountData
		{
			public float diodeRotationTarget;

			public float diodeRotation;

			public float outerRingRotation;

			public Mount.DrillBeam[] beams;

			public int beamCooldown;

			public Vector2 crosshairPosition;

			public DrillMountData()
			{
				this.beams = new Mount.DrillBeam[4];
				for (int i = 0; i < this.beams.Length; i++)
				{
					this.beams[i] = new Mount.DrillBeam();
				}
			}
		}

		private class MountData
		{
			public Texture2D backTexture;

			public Texture2D backTextureGlow;

			public Texture2D backTextureExtra;

			public Texture2D backTextureExtraGlow;

			public Texture2D frontTexture;

			public Texture2D frontTextureGlow;

			public Texture2D frontTextureExtra;

			public Texture2D frontTextureExtraGlow;

			public int textureWidth;

			public int textureHeight;

			public int xOffset;

			public int yOffset;

			public int[] playerYOffsets;

			public int bodyFrame;

			public int playerHeadOffset;

			public int heightBoost;

			public int buff;

			public int extraBuff;

			public int flightTimeMax;

			public bool usesHover;

			public float runSpeed;

			public float dashSpeed;

			public float swimSpeed;

			public float acceleration;

			public float jumpSpeed;

			public int jumpHeight;

			public float fallDamage;

			public int fatigueMax;

			public bool constantJump;

			public bool blockExtraJumps;

			public int abilityChargeMax;

			public int abilityDuration;

			public int abilityCooldown;

			public int spawnDust;

			public bool spawnDustNoGravity;

			public int totalFrames;

			public int standingFrameStart;

			public int standingFrameCount;

			public int standingFrameDelay;

			public int runningFrameStart;

			public int runningFrameCount;

			public int runningFrameDelay;

			public int flyingFrameStart;

			public int flyingFrameCount;

			public int flyingFrameDelay;

			public int inAirFrameStart;

			public int inAirFrameCount;

			public int inAirFrameDelay;

			public int idleFrameStart;

			public int idleFrameCount;

			public int idleFrameDelay;

			public bool idleFrameLoop;

			public int swimFrameStart;

			public int swimFrameCount;

			public int swimFrameDelay;

			public int dashingFrameStart;

			public int dashingFrameCount;

			public int dashingFrameDelay;

			public bool Minecart;

			public bool MinecartDirectional;

			public Action<Vector2> MinecartDust;

			public Vector3 lightColor = Vector3.One;

			public bool emitsLight;
		}

		public const int None = -1;

		public const int Rudolph = 0;

		public const int Bunny = 1;

		public const int Pigron = 2;

		public const int Slime = 3;

		public const int Turtle = 4;

		public const int Bee = 5;

		public const int Minecart = 6;

		public const int UFO = 7;

		public const int Drill = 8;

		public const int Scutlix = 9;

		public const int Unicorn = 10;

		public const int MinecartMech = 11;

		public const int CuteFishron = 12;

		public const int MinecartWood = 13;

		public const int Basilisk = 14;

		public const int maxMounts = 15;

		public const int FrameStanding = 0;

		public const int FrameRunning = 1;

		public const int FrameInAir = 2;

		public const int FrameFlying = 3;

		public const int FrameSwimming = 4;

		public const int FrameDashing = 5;

		public const int DrawBack = 0;

		public const int DrawBackExtra = 1;

		public const int DrawFront = 2;

		public const int DrawFrontExtra = 3;

		public const int scutlixBaseDamage = 50;

		public const int drillTextureWidth = 80;

		public const float drillRotationChange = 0.05235988f;

		public const float maxDrillLength = 48f;

		public static int currentShader = 0;

		private static Mount.MountData[] mounts;

		private static Vector2[] scutlixEyePositions;

		private static Vector2 scutlixTextureSize;

		public static Vector2 drillDiodePoint1 = new Vector2(36f, -6f);

		public static Vector2 drillDiodePoint2 = new Vector2(36f, 8f);

		public static Vector2 drillTextureSize;

		public static int drillPickPower = 210;

		public static int drillPickTime = 6;

		public static int drillBeamCooldownMax = 1;

		private Mount.MountData _data;

		private int _type;

		private bool _flipDraw;

		private int _frame;

		private float _frameCounter;

		private int _frameExtra;

		private float _frameExtraCounter;

		private int _frameState;

		private int _flyTime;

		private int _idleTime;

		private int _idleTimeNext;

		private float _fatigue;

		private float _fatigueMax;

		private bool _abilityCharging;

		private int _abilityCharge;

		private int _abilityCooldown;

		private int _abilityDuration;

		private bool _abilityActive;

		private bool _aiming;

		public List<DrillDebugDraw> _debugDraw;

		private object _mountSpecificData;

		private bool _active;

		public bool Active
		{
			get
			{
				return this._active;
			}
		}

		public int Type
		{
			get
			{
				return this._type;
			}
		}

		public int FlyTime
		{
			get
			{
				return this._flyTime;
			}
		}

		public int BuffType
		{
			get
			{
				return this._data.buff;
			}
		}

		public int BodyFrame
		{
			get
			{
				return this._data.bodyFrame;
			}
		}

		public int XOffset
		{
			get
			{
				return this._data.xOffset;
			}
		}

		public int YOffset
		{
			get
			{
				return this._data.yOffset;
			}
		}

		public int PlayerOffset
		{
			get
			{
				if (!this._active)
				{
					return 0;
				}
				return this._data.playerYOffsets[this._frame];
			}
		}

		public int PlayerOffsetHitbox
		{
			get
			{
				if (!this._active)
				{
					return 0;
				}
				return this._data.playerYOffsets[0] - this._data.playerYOffsets[this._frame] + this._data.playerYOffsets[0] / 4;
			}
		}

		public int PlayerHeadOffset
		{
			get
			{
				if (!this._active)
				{
					return 0;
				}
				return this._data.playerHeadOffset;
			}
		}

		public int HeightBoost
		{
			get
			{
				return this._data.heightBoost;
			}
		}

		public float RunSpeed
		{
			get
			{
				if (this._type == 4 && this._frameState == 4)
				{
					return this._data.swimSpeed;
				}
				if (this._type == 12 && this._frameState == 4)
				{
					return this._data.swimSpeed;
				}
				if (this._type == 12 && this._frameState == 2)
				{
					return this._data.runSpeed + 11f;
				}
				if (this._type == 5 && this._frameState == 2)
				{
					float num = this._fatigue / this._fatigueMax;
					return this._data.runSpeed + 4f * (1f - num);
				}
				return this._data.runSpeed;
			}
		}

		public float DashSpeed
		{
			get
			{
				return this._data.dashSpeed;
			}
		}

		public float Acceleration
		{
			get
			{
				return this._data.acceleration;
			}
		}

		public float FallDamage
		{
			get
			{
				return this._data.fallDamage;
			}
		}

		public bool AutoJump
		{
			get
			{
				return this._data.constantJump;
			}
		}

		public bool BlockExtraJumps
		{
			get
			{
				return this._data.blockExtraJumps;
			}
		}

		public bool Cart
		{
			get
			{
				return this._data != null && this._active && this._data.Minecart;
			}
		}

		public bool Directional
		{
			get
			{
				return this._data == null || this._data.MinecartDirectional;
			}
		}

		public Action<Vector2> MinecartDust
		{
			get
			{
				if (this._data == null)
				{
					return new Action<Vector2>(DelegateMethods.Minecart.Sparks);
				}
				return this._data.MinecartDust;
			}
		}

		public Vector2 Origin
		{
			get
			{
				return new Vector2((float)this._data.textureWidth / 2f, (float)this._data.textureHeight / (2f * (float)this._data.totalFrames));
			}
		}

		public bool CanFly
		{
			get
			{
				return this._active && this._data.flightTimeMax != 0;
			}
		}

		public bool CanHover
		{
			get
			{
				return this._active && this._data.usesHover;
			}
		}

		public bool AbilityReady
		{
			get
			{
				return this._abilityCooldown == 0;
			}
		}

		public bool AbilityCharging
		{
			get
			{
				return this._abilityCharging;
			}
		}

		public bool AbilityActive
		{
			get
			{
				return this._abilityActive;
			}
		}

		public float AbilityCharge
		{
			get
			{
				return (float)this._abilityCharge / (float)this._data.abilityChargeMax;
			}
		}

		public bool AllowDirectionChange
		{
			get
			{
				int type = this._type;
				return type != 9 || this._abilityCooldown < this._data.abilityCooldown / 2;
			}
		}

		public Mount()
		{
			this._debugDraw = new List<DrillDebugDraw>();
			this.Reset();
		}

		public void Reset()
		{
			this._active = false;
			this._type = -1;
			this._flipDraw = false;
			this._frame = 0;
			this._frameCounter = 0f;
			this._frameExtra = 0;
			this._frameExtraCounter = 0f;
			this._frameState = 0;
			this._flyTime = 0;
			this._idleTime = 0;
			this._idleTimeNext = -1;
			this._fatigueMax = 0f;
			this._abilityCharging = false;
			this._abilityCharge = 0;
			this._aiming = false;
		}

		public static void Initialize()
		{
			Mount.mounts = new Mount.MountData[15];
			Mount.MountData mountData = new Mount.MountData();
			Mount.mounts[0] = mountData;
			mountData.spawnDust = 57;
			mountData.spawnDustNoGravity = false;
			mountData.buff = 90;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 160;
			mountData.runSpeed = 5.5f;
			mountData.dashSpeed = 12f;
			mountData.acceleration = 0.09f;
			mountData.jumpHeight = 17;
			mountData.jumpSpeed = 5.31f;
			mountData.totalFrames = 12;
			int[] array = new int[mountData.totalFrames];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = 30;
			}
			array[1] += 2;
			array[11] += 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 13;
			mountData.bodyFrame = 3;
			mountData.yOffset = -7;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 6;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 6;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 4;
			mountData.idleFrameDelay = 30;
			mountData.idleFrameStart = 2;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.rudolphMountTexture[0];
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.rudolphMountTexture[1];
				mountData.frontTextureExtra = Main.rudolphMountTexture[2];
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[2] = mountData;
			mountData.spawnDust = 58;
			mountData.buff = 129;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 160;
			mountData.runSpeed = 5f;
			mountData.dashSpeed = 9f;
			mountData.acceleration = 0.08f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 6.01f;
			mountData.totalFrames = 16;
			array = new int[mountData.totalFrames];
			for (int j = 0; j < array.Length; j++)
			{
				array[j] = 22;
			}
			array[12] += 2;
			array[13] += 4;
			array[14] += 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 8;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 7;
			mountData.runningFrameCount = 5;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 11;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 1;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 3;
			mountData.idleFrameDelay = 30;
			mountData.idleFrameStart = 8;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.pigronMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[1] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 128;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.8f;
			mountData.runSpeed = 4f;
			mountData.dashSpeed = 7.5f;
			mountData.acceleration = 0.13f;
			mountData.jumpHeight = 15;
			mountData.jumpSpeed = 5.01f;
			mountData.totalFrames = 7;
			array = new int[mountData.totalFrames];
			for (int k = 0; k < array.Length; k++)
			{
				array[k] = 14;
			}
			array[2] += 2;
			array[3] += 4;
			array[4] += 8;
			array[5] += 8;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 7;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 1;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 5;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.bunnyMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[3] = mountData;
			mountData.spawnDust = 56;
			mountData.buff = 130;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.5f;
			mountData.runSpeed = 4f;
			mountData.dashSpeed = 4f;
			mountData.acceleration = 0.18f;
			mountData.jumpHeight = 12;
			mountData.jumpSpeed = 8.25f;
			mountData.constantJump = true;
			mountData.totalFrames = 4;
			array = new int[mountData.totalFrames];
			for (int l = 0; l < array.Length; l++)
			{
				array[l] = 20;
			}
			array[1] += 2;
			array[3] -= 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 10;
			mountData.playerHeadOffset = 22;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 4;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.slimeMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[6] = mountData;
			mountData.Minecart = true;
			mountData.MinecartDirectional = true;
			mountData.MinecartDust = new Action<Vector2>(DelegateMethods.Minecart.Sparks);
			mountData.spawnDust = 213;
			mountData.buff = 118;
			mountData.extraBuff = 138;
			mountData.heightBoost = 10;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 13f;
			mountData.dashSpeed = 13f;
			mountData.acceleration = 0.04f;
			mountData.jumpHeight = 15;
			mountData.jumpSpeed = 5.15f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 3;
			array = new int[mountData.totalFrames];
			for (int m = 0; m < array.Length; m++)
			{
				array[m] = 8;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 13;
			mountData.playerHeadOffset = 14;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 3;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 0;
			mountData.inAirFrameDelay = 0;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.minecartMountTexture;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[4] = mountData;
			mountData.spawnDust = 56;
			mountData.buff = 131;
			mountData.heightBoost = 26;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 2f;
			mountData.dashSpeed = 2f;
			mountData.swimSpeed = 6f;
			mountData.acceleration = 0.08f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 3.15f;
			mountData.totalFrames = 12;
			array = new int[mountData.totalFrames];
			for (int n = 0; n < array.Length; n++)
			{
				array[n] = 26;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 13;
			mountData.playerHeadOffset = 30;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 3;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = 6;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 6;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.turtleMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[5] = mountData;
			mountData.spawnDust = 152;
			mountData.buff = 132;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 0f;
			mountData.usesHover = true;
			mountData.runSpeed = 2f;
			mountData.dashSpeed = 2f;
			mountData.acceleration = 0.16f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 4f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 12;
			array = new int[mountData.totalFrames];
			for (int num = 0; num < array.Length; num++)
			{
				array[num] = 16;
			}
			array[8] = 18;
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 5;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 3;
			mountData.flyingFrameDelay = 12;
			mountData.flyingFrameStart = 5;
			mountData.inAirFrameCount = 3;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 5;
			mountData.idleFrameCount = 4;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 8;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.beeMountTexture[0];
				mountData.backTextureExtra = Main.beeMountTexture[1];
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[7] = mountData;
			mountData.spawnDust = 226;
			mountData.spawnDustNoGravity = true;
			mountData.buff = 141;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 0f;
			mountData.usesHover = true;
			mountData.runSpeed = 8f;
			mountData.dashSpeed = 8f;
			mountData.acceleration = 0.16f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 4f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 8;
			array = new int[mountData.totalFrames];
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				array[num2] = 16;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 8;
			mountData.standingFrameDelay = 4;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 8;
			mountData.runningFrameDelay = 4;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 8;
			mountData.flyingFrameDelay = 4;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 8;
			mountData.inAirFrameDelay = 4;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.UFOMountTexture[0];
				mountData.frontTextureExtra = Main.UFOMountTexture[1];
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[8] = mountData;
			mountData.spawnDust = 226;
			mountData.buff = 142;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 1f;
			mountData.usesHover = true;
			mountData.swimSpeed = 4f;
			mountData.runSpeed = 6f;
			mountData.dashSpeed = 4f;
			mountData.acceleration = 0.16f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 4f;
			mountData.blockExtraJumps = true;
			mountData.emitsLight = true;
			mountData.lightColor = new Vector3(0.3f, 0.3f, 0.4f);
			mountData.totalFrames = 1;
			array = new int[mountData.totalFrames];
			for (int num3 = 0; num3 < array.Length; num3++)
			{
				array[num3] = 4;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 4;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 1;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 1;
			mountData.flyingFrameDelay = 12;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 8;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.drillMountTexture[0];
				mountData.backTextureGlow = Main.drillMountTexture[3];
				mountData.backTextureExtra = null;
				mountData.backTextureExtraGlow = null;
				mountData.frontTexture = Main.drillMountTexture[1];
				mountData.frontTextureGlow = Main.drillMountTexture[4];
				mountData.frontTextureExtra = Main.drillMountTexture[2];
				mountData.frontTextureExtraGlow = Main.drillMountTexture[5];
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			Mount.drillTextureSize = new Vector2(80f, 80f);
#if CLIENT
			Vector2 value = new Vector2((float)mountData.textureWidth, (float)(mountData.textureHeight / mountData.totalFrames));
			if (Mount.drillTextureSize != value)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Be sure to update the Drill texture origin to match the actual texture size of ",
					mountData.textureWidth,
					", ",
					mountData.textureHeight,
					"."
				}));
			}
#endif
			mountData = new Mount.MountData();
			Mount.mounts[9] = mountData;
			mountData.spawnDust = 152;
			mountData.buff = 143;
			mountData.heightBoost = 16;
			mountData.flightTimeMax = 0;
			mountData.fatigueMax = 0;
			mountData.fallDamage = 0f;
			mountData.abilityChargeMax = 40;
			mountData.abilityCooldown = 20;
			mountData.abilityDuration = 0;
			mountData.runSpeed = 8f;
			mountData.dashSpeed = 8f;
			mountData.acceleration = 0.4f;
			mountData.jumpHeight = 22;
			mountData.jumpSpeed = 10.01f;
			mountData.blockExtraJumps = false;
			mountData.totalFrames = 12;
			array = new int[mountData.totalFrames];
			for (int num4 = 0; num4 < array.Length; num4++)
			{
				array[num4] = 16;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 6;
			mountData.playerHeadOffset = 18;
			mountData.standingFrameCount = 6;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 6;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 12;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 12;
			mountData.idleFrameStart = 6;
			mountData.idleFrameLoop = true;
			mountData.swimFrameCount = 0;
			mountData.swimFrameDelay = 12;
			mountData.swimFrameStart = 0;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.scutlixMountTexture[0];
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.scutlixMountTexture[1];
				mountData.frontTextureExtra = Main.scutlixMountTexture[2];
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			Mount.scutlixEyePositions = new Vector2[10];
			Mount.scutlixEyePositions[0] = new Vector2(60f, 2f);
			Mount.scutlixEyePositions[1] = new Vector2(70f, 6f);
			Mount.scutlixEyePositions[2] = new Vector2(68f, 6f);
			Mount.scutlixEyePositions[3] = new Vector2(76f, 12f);
			Mount.scutlixEyePositions[4] = new Vector2(80f, 10f);
			Mount.scutlixEyePositions[5] = new Vector2(84f, 18f);
			Mount.scutlixEyePositions[6] = new Vector2(74f, 20f);
			Mount.scutlixEyePositions[7] = new Vector2(76f, 24f);
			Mount.scutlixEyePositions[8] = new Vector2(70f, 34f);
			Mount.scutlixEyePositions[9] = new Vector2(76f, 34f);
			Mount.scutlixTextureSize = new Vector2(45f, 54f);
#if CLIENT
			Vector2 value2 = new Vector2((float)(mountData.textureWidth / 2), (float)(mountData.textureHeight / mountData.totalFrames));
			if (Mount.scutlixTextureSize != value2)
			{
				throw new Exception(string.Concat(new object[]
				{
					"Be sure to update the Scutlix texture origin to match the actual texture size of ",
					mountData.textureWidth,
					", ",
					mountData.textureHeight,
					"."
				}));
			}
#endif
			for (int num5 = 0; num5 < Mount.scutlixEyePositions.Length; num5++)
			{
				Mount.scutlixEyePositions[num5] -= Mount.scutlixTextureSize;
			}
			mountData = new Mount.MountData();
			Mount.mounts[10] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 162;
			mountData.heightBoost = 34;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.2f;
			mountData.runSpeed = 4f;
			mountData.dashSpeed = 12f;
			mountData.acceleration = 0.3f;
			mountData.jumpHeight = 10;
			mountData.jumpSpeed = 8.01f;
			mountData.totalFrames = 16;
			array = new int[mountData.totalFrames];
			for (int num6 = 0; num6 < array.Length; num6++)
			{
				array[num6] = 28;
			}
			array[3] += 2;
			array[4] += 2;
			array[7] += 2;
			array[8] += 2;
			array[12] += 2;
			array[13] += 2;
			array[15] += 4;
			mountData.playerYOffsets = array;
			mountData.xOffset = 5;
			mountData.bodyFrame = 3;
			mountData.yOffset = 1;
			mountData.playerHeadOffset = 31;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 7;
			mountData.runningFrameDelay = 15;
			mountData.runningFrameStart = 1;
			mountData.dashingFrameCount = 6;
			mountData.dashingFrameDelay = 40;
			mountData.dashingFrameStart = 9;
			mountData.flyingFrameCount = 6;
			mountData.flyingFrameDelay = 6;
			mountData.flyingFrameStart = 1;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 15;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.unicornMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[11] = mountData;
			mountData.Minecart = true;
			mountData.MinecartDust = new Action<Vector2>(DelegateMethods.Minecart.SparksMech);
			mountData.spawnDust = 213;
			mountData.buff = 167;
			mountData.extraBuff = 166;
			mountData.heightBoost = 12;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 20f;
			mountData.dashSpeed = 20f;
			mountData.acceleration = 0.1f;
			mountData.jumpHeight = 15;
			mountData.jumpSpeed = 5.15f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 3;
			array = new int[mountData.totalFrames];
			for (int num7 = 0; num7 < array.Length; num7++)
			{
				array[num7] = 9;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = -1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 11;
			mountData.playerHeadOffset = 14;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 3;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 0;
			mountData.inAirFrameDelay = 0;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.minecartMechMountTexture[0];
				mountData.frontTextureGlow = Main.minecartMechMountTexture[1];
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[12] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 168;
			mountData.heightBoost = 20;
			mountData.flightTimeMax = 320;
			mountData.fatigueMax = 320;
			mountData.fallDamage = 0f;
			mountData.usesHover = true;
			mountData.runSpeed = 2f;
			mountData.dashSpeed = 1f;
			mountData.acceleration = 0.2f;
			mountData.jumpHeight = 4;
			mountData.jumpSpeed = 3f;
			mountData.swimSpeed = 16f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 23;
			array = new int[mountData.totalFrames];
			for (int num8 = 0; num8 < array.Length; num8++)
			{
				array[num8] = 12;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 2;
			mountData.bodyFrame = 3;
			mountData.yOffset = 16;
			mountData.playerHeadOffset = 31;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 8;
			mountData.runningFrameCount = 7;
			mountData.runningFrameDelay = 14;
			mountData.runningFrameStart = 8;
			mountData.flyingFrameCount = 8;
			mountData.flyingFrameDelay = 16;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 8;
			mountData.inAirFrameDelay = 6;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = 8;
			mountData.swimFrameDelay = 4;
			mountData.swimFrameStart = 15;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.cuteFishronMountTexture[0];
				mountData.backTextureGlow = Main.cuteFishronMountTexture[1];
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[13] = mountData;
			mountData.Minecart = true;
			mountData.MinecartDirectional = true;
			mountData.MinecartDust = new Action<Vector2>(DelegateMethods.Minecart.Sparks);
			mountData.spawnDust = 213;
			mountData.buff = 184;
			mountData.extraBuff = 185;
			mountData.heightBoost = 10;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 1f;
			mountData.runSpeed = 10f;
			mountData.dashSpeed = 10f;
			mountData.acceleration = 0.03f;
			mountData.jumpHeight = 12;
			mountData.jumpSpeed = 5.15f;
			mountData.blockExtraJumps = true;
			mountData.totalFrames = 3;
			array = new int[mountData.totalFrames];
			for (int num9 = 0; num9 < array.Length; num9++)
			{
				array[num9] = 8;
			}
			mountData.playerYOffsets = array;
			mountData.xOffset = 1;
			mountData.bodyFrame = 3;
			mountData.yOffset = 13;
			mountData.playerHeadOffset = 14;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 3;
			mountData.runningFrameDelay = 12;
			mountData.runningFrameStart = 0;
			mountData.flyingFrameCount = 0;
			mountData.flyingFrameDelay = 0;
			mountData.flyingFrameStart = 0;
			mountData.inAirFrameCount = 0;
			mountData.inAirFrameDelay = 0;
			mountData.inAirFrameStart = 0;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			if (Main.netMode != 2)
			{
				mountData.backTexture = null;
				mountData.backTextureExtra = null;
				mountData.frontTexture = Main.minecartWoodMountTexture;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.frontTexture.Width;
				mountData.textureHeight = mountData.frontTexture.Height;
			}
			mountData = new Mount.MountData();
			Mount.mounts[14] = mountData;
			mountData.spawnDust = 15;
			mountData.buff = 193;
			mountData.heightBoost = 8;
			mountData.flightTimeMax = 0;
			mountData.fallDamage = 0.2f;
			mountData.runSpeed = 8f;
			mountData.acceleration = 0.25f;
			mountData.jumpHeight = 20;
			mountData.jumpSpeed = 8.01f;
			mountData.totalFrames = 8;
			array = new int[mountData.totalFrames];
			for (int num10 = 0; num10 < array.Length; num10++)
			{
				array[num10] = 8;
			}
			array[1] += 2;
			array[3] += 2;
			array[6] += 2;
			mountData.playerYOffsets = array;
			mountData.xOffset = 4;
			mountData.bodyFrame = 3;
			mountData.yOffset = 9;
			mountData.playerHeadOffset = 10;
			mountData.standingFrameCount = 1;
			mountData.standingFrameDelay = 12;
			mountData.standingFrameStart = 0;
			mountData.runningFrameCount = 6;
			mountData.runningFrameDelay = 30;
			mountData.runningFrameStart = 2;
			mountData.inAirFrameCount = 1;
			mountData.inAirFrameDelay = 12;
			mountData.inAirFrameStart = 1;
			mountData.idleFrameCount = 0;
			mountData.idleFrameDelay = 0;
			mountData.idleFrameStart = 0;
			mountData.idleFrameLoop = false;
			mountData.swimFrameCount = mountData.inAirFrameCount;
			mountData.swimFrameDelay = mountData.inAirFrameDelay;
			mountData.swimFrameStart = mountData.inAirFrameStart;
			if (Main.netMode != 2)
			{
				mountData.backTexture = Main.basiliskMountTexture;
				mountData.backTextureExtra = null;
				mountData.frontTexture = null;
				mountData.frontTextureExtra = null;
				mountData.textureWidth = mountData.backTexture.Width;
				mountData.textureHeight = mountData.backTexture.Height;
			}
		}

		public static int GetHeightBoost(int MountType)
		{
			if (MountType <= -1 || MountType >= 15)
			{
				return 0;
			}
			return Mount.mounts[MountType].heightBoost;
		}

		public int JumpHeight(float xVelocity)
		{
			int num = this._data.jumpHeight;
			switch (this._type)
			{
			case 0:
				num += (int)(Math.Abs(xVelocity) / 4f);
				break;
			case 1:
				num += (int)(Math.Abs(xVelocity) / 2.5f);
				break;
			case 4:
				if (this._frameState == 4)
				{
					num += 5;
				}
				break;
			}
			return num;
		}

		public float JumpSpeed(float xVelocity)
		{
			float num = this._data.jumpSpeed;
			switch (this._type)
			{
			case 0:
			case 1:
				num += Math.Abs(xVelocity) / 7f;
				break;
			case 4:
				if (this._frameState == 4)
				{
					num += 2.5f;
				}
				break;
			}
			return num;
		}

		public void StartAbilityCharge(Player mountedPlayer)
		{
			if (Main.myPlayer == mountedPlayer.whoAmI)
			{
				int type = this._type;
				if (type != 9)
				{
					return;
				}
				float num = Main.screenPosition.X + (float)Main.mouseX;
				float num2 = Main.screenPosition.Y + (float)Main.mouseY;
				float ai = num - mountedPlayer.position.X;
				float ai2 = num2 - mountedPlayer.position.Y;
				Projectile.NewProjectile(num, num2, 0f, 0f, 441, 0, 0f, mountedPlayer.whoAmI, ai, ai2);
				this._abilityCharging = true;
				return;
			}
			else
			{
				int type2 = this._type;
				if (type2 != 9)
				{
					return;
				}
				this._abilityCharging = true;
				return;
			}
		}

		public void StopAbilityCharge()
		{
			int type = this._type;
			if (type != 9)
			{
				return;
			}
			this._abilityCharging = false;
			this._abilityCooldown = this._data.abilityCooldown;
			this._abilityDuration = this._data.abilityDuration;
		}

		public bool CheckBuff(int buffID)
		{
			return this._data.buff == buffID || this._data.extraBuff == buffID;
		}

		public void AbilityRecovery()
		{
			if (this._abilityCharging)
			{
				if (this._abilityCharge < this._data.abilityChargeMax)
				{
					this._abilityCharge++;
				}
			}
			else if (this._abilityCharge > 0)
			{
				this._abilityCharge--;
			}
			if (this._abilityCooldown > 0)
			{
				this._abilityCooldown--;
			}
			if (this._abilityDuration > 0)
			{
				this._abilityDuration--;
			}
		}

		public void FatigueRecovery()
		{
			if (this._fatigue > 2f)
			{
				this._fatigue -= 2f;
				return;
			}
			this._fatigue = 0f;
		}

		public bool Flight()
		{
			if (this._flyTime <= 0)
			{
				return false;
			}
			this._flyTime--;
			return true;
		}

		public void UpdateDrill(Player mountedPlayer, bool controlUp, bool controlDown)
		{
			Mount.DrillMountData drillMountData = (Mount.DrillMountData)this._mountSpecificData;
			for (int i = 0; i < drillMountData.beams.Length; i++)
			{
				Mount.DrillBeam drillBeam = drillMountData.beams[i];
				if (drillBeam.cooldown > 1)
				{
					drillBeam.cooldown--;
				}
				else if (drillBeam.cooldown == 1)
				{
					drillBeam.cooldown = 0;
					drillBeam.curTileTarget = Point16.NegativeOne;
				}
			}
			drillMountData.diodeRotation = drillMountData.diodeRotation * 0.85f + 0.15f * drillMountData.diodeRotationTarget;
			if (drillMountData.beamCooldown > 0)
			{
				drillMountData.beamCooldown--;
			}
		}

		public void UseDrill(Player mountedPlayer)
		{
			if (this._type != 8 || !this._abilityActive)
			{
				return;
			}
			Mount.DrillMountData drillMountData = (Mount.DrillMountData)this._mountSpecificData;
			if (drillMountData.beamCooldown == 0)
			{
				int i = 0;
				while (i < drillMountData.beams.Length)
				{
					Mount.DrillBeam drillBeam = drillMountData.beams[i];
					if (drillBeam.cooldown == 0)
					{
						Point16 point = this.DrillSmartCursor(mountedPlayer, drillMountData);
						if (point != Point16.NegativeOne)
						{
							drillBeam.curTileTarget = point;
							int pickPower = Mount.drillPickPower;
							bool flag = mountedPlayer.whoAmI == Main.myPlayer;
							if (flag)
							{
								bool flag2 = true;
								if (WorldGen.InWorld((int)point.X, (int)point.Y, 0) && Main.tile[(int)point.X, (int)point.Y] != null && Main.tile[(int)point.X, (int)point.Y].type == 26 && !Main.hardMode)
								{
									flag2 = false;
									mountedPlayer.Hurt(PlayerDeathReason.ByOther(4), mountedPlayer.statLife / 2, -mountedPlayer.direction, false, false, false, -1);
								}
								if (mountedPlayer.noBuilding)
								{
									flag2 = false;
								}
								if (flag2)
								{
									mountedPlayer.PickTile((int)point.X, (int)point.Y, pickPower);
								}
							}
							Vector2 vector = new Vector2((float)(point.X << 4) + 8f, (float)(point.Y << 4) + 8f);
							Vector2 v = vector - mountedPlayer.Center;
							float num = v.ToRotation();
							for (int j = 0; j < 2; j++)
							{
								float num2 = num + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
								float num3 = (float)Main.rand.NextDouble() * 2f + 2f;
								Vector2 vector2 = new Vector2((float)Math.Cos((double)num2) * num3, (float)Math.Sin((double)num2) * num3);
								int num4 = Dust.NewDust(vector, 0, 0, 230, vector2.X, vector2.Y, 0, default(Color), 1f);
								Main.dust[num4].noGravity = true;
								Main.dust[num4].customData = mountedPlayer;
							}
							if (flag)
							{
								Tile.SmoothSlope((int)point.X, (int)point.Y, true);
							}
							drillBeam.cooldown = Mount.drillPickTime;
							break;
						}
						break;
					}
					else
					{
						i++;
					}
				}
				drillMountData.beamCooldown = Mount.drillBeamCooldownMax;
			}
		}

		private Point16 DrillSmartCursor(Player mountedPlayer, Mount.DrillMountData data)
		{
			Vector2 value;
			if (mountedPlayer.whoAmI == Main.myPlayer)
			{
				value = Main.screenPosition + new Vector2((float)Main.mouseX, (float)Main.mouseY);
			}
			else
			{
				value = data.crosshairPosition;
			}
			Vector2 center = mountedPlayer.Center;
			Vector2 value2 = value - center;
			float num = value2.Length();
			if (num > 224f)
			{
				num = 224f;
			}
			num += 32f;
			value2.Normalize();
			Vector2 start = center;
			Vector2 end = center + value2 * num;
			Point16 tilePoint = new Point16(-1, -1);
			bool flag = !Utils.PlotTileLine(start, end, 65.6f, delegate(int x, int y)
			{
				tilePoint = new Point16(x, y);
				for (int i = 0; i < data.beams.Length; i++)
				{
					if (data.beams[i].curTileTarget == tilePoint)
					{
						return true;
					}
				}
				return !WorldGen.CanKillTile(x, y) || Main.tile[x, y] == null || Main.tile[x, y].inActive() || !Main.tile[x, y].active();
			});
			if (flag)
			{
				return tilePoint;
			}
			return new Point16(-1, -1);
		}

		public void UseAbility(Player mountedPlayer, Vector2 mousePosition, bool toggleOn)
		{
			switch (this._type)
			{
			case 8:
				if (Main.myPlayer == mountedPlayer.whoAmI)
				{
					if (!toggleOn)
					{
						this._abilityActive = false;
						return;
					}
					if (!this._abilityActive)
					{
						if (mountedPlayer.whoAmI == Main.myPlayer)
						{
							float num = Main.screenPosition.X + (float)Main.mouseX;
							float num2 = Main.screenPosition.Y + (float)Main.mouseY;
							float ai = num - mountedPlayer.position.X;
							float ai2 = num2 - mountedPlayer.position.Y;
							Projectile.NewProjectile(num, num2, 0f, 0f, 453, 0, 0f, mountedPlayer.whoAmI, ai, ai2);
						}
						this._abilityActive = true;
						return;
					}
				}
				else
				{
					this._abilityActive = toggleOn;
				}
				break;
			case 9:
				if (Main.myPlayer == mountedPlayer.whoAmI)
				{
					mousePosition = this.ClampToDeadZone(mountedPlayer, mousePosition);
					Vector2 vector;
					vector.X = mountedPlayer.position.X + (float)(mountedPlayer.width / 2);
					vector.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
					int num3 = (this._frameExtra - 6) * 2;
					for (int i = 0; i < 2; i++)
					{
						Vector2 vector2;
						vector2.Y = vector.Y + Mount.scutlixEyePositions[num3 + i].Y + (float)this._data.yOffset;
						if (mountedPlayer.direction == -1)
						{
							vector2.X = vector.X - Mount.scutlixEyePositions[num3 + i].X - (float)this._data.xOffset;
						}
						else
						{
							vector2.X = vector.X + Mount.scutlixEyePositions[num3 + i].X + (float)this._data.xOffset;
						}
						Vector2 vector3 = mousePosition - vector2;
						vector3.Normalize();
						vector3 *= 14f;
						int damage = 100;
						vector2 += vector3;
						Projectile.NewProjectile(vector2.X, vector2.Y, vector3.X, vector3.Y, 606, damage, 0f, Main.myPlayer, 0f, 0f);
					}
					return;
				}
				break;
			default:
				return;
			}
		}

		public bool Hover(Player mountedPlayer)
		{
			if (this._frameState == 2 || this._frameState == 4)
			{
				bool flag = true;
				float num = 1f;
				float num2 = mountedPlayer.gravity / Player.defaultGravity;
				if (mountedPlayer.slowFall)
				{
					num2 /= 3f;
				}
				if (num2 < 0.25f)
				{
					num2 = 0.25f;
				}
				if (this._type != 7 && this._type != 8 && this._type != 12)
				{
					if (this._flyTime > 0)
					{
						this._flyTime--;
					}
					else if (this._fatigue < this._fatigueMax)
					{
						this._fatigue += num2;
					}
					else
					{
						flag = false;
					}
				}
				if (this._type == 12 && !mountedPlayer.MountFishronSpecial)
				{
					num = 0.5f;
				}
				float num3 = this._fatigue / this._fatigueMax;
				if (this._type == 7 || this._type == 8 || this._type == 12)
				{
					num3 = 0f;
				}
				float num4 = 4f * num3;
				float num5 = 4f * num3;
				if (num4 == 0f)
				{
					num4 = -0.001f;
				}
				if (num5 == 0f)
				{
					num5 = -0.001f;
				}
				float num6 = mountedPlayer.velocity.Y;
				if ((mountedPlayer.controlUp || mountedPlayer.controlJump) && flag)
				{
					num4 = -2f - 6f * (1f - num3);
					num6 -= this._data.acceleration * num;
				}
				else if (mountedPlayer.controlDown)
				{
					num6 += this._data.acceleration * num;
					num5 = 8f;
				}
				else
				{
					int arg_189_0 = mountedPlayer.jump;
				}
				if (num6 < num4)
				{
					if (num4 - num6 < this._data.acceleration)
					{
						num6 = num4;
					}
					else
					{
						num6 += this._data.acceleration * num;
					}
				}
				else if (num6 > num5)
				{
					if (num6 - num5 < this._data.acceleration)
					{
						num6 = num5;
					}
					else
					{
						num6 -= this._data.acceleration * num;
					}
				}
				mountedPlayer.velocity.Y = num6;
				mountedPlayer.fallStart = (int)(mountedPlayer.position.Y / 16f);
			}
			else if (this._type != 7 && this._type != 8 && this._type != 12)
			{
				mountedPlayer.velocity.Y = mountedPlayer.velocity.Y + mountedPlayer.gravity * mountedPlayer.gravDir;
			}
			else if (mountedPlayer.velocity.Y == 0f)
			{
				mountedPlayer.velocity.Y = 0.001f;
			}
			if (this._type == 7)
			{
				float num7 = mountedPlayer.velocity.X / this._data.dashSpeed;
				if ((double)num7 > 0.95)
				{
					num7 = 0.95f;
				}
				if ((double)num7 < -0.95)
				{
					num7 = -0.95f;
				}
				float fullRotation = 0.7853982f * num7 / 2f;
				float num8 = Math.Abs(2f - (float)this._frame / 2f) / 2f;
				Lighting.AddLight((int)(mountedPlayer.position.X + (float)(mountedPlayer.width / 2)) / 16, (int)(mountedPlayer.position.Y + (float)(mountedPlayer.height / 2)) / 16, 0.4f, 0.2f * num8, 0f);
				mountedPlayer.fullRotation = fullRotation;
			}
			else if (this._type == 8)
			{
				float num9 = mountedPlayer.velocity.X / this._data.dashSpeed;
				if ((double)num9 > 0.95)
				{
					num9 = 0.95f;
				}
				if ((double)num9 < -0.95)
				{
					num9 = -0.95f;
				}
				float fullRotation2 = 0.7853982f * num9 / 2f;
				mountedPlayer.fullRotation = fullRotation2;
				Mount.DrillMountData drillMountData = (Mount.DrillMountData)this._mountSpecificData;
				float num10 = drillMountData.outerRingRotation;
				num10 += mountedPlayer.velocity.X / 80f;
				if (num10 > 3.14159274f)
				{
					num10 -= 6.28318548f;
				}
				else if (num10 < -3.14159274f)
				{
					num10 += 6.28318548f;
				}
				drillMountData.outerRingRotation = num10;
			}
			return true;
		}

		public void UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
		{
			if (this._frameState != state)
			{
				this._frameState = state;
				this._frameCounter = 0f;
			}
			if (state != 0)
			{
				this._idleTime = 0;
			}
			if (this._data.emitsLight)
			{
				Point point = mountedPlayer.Center.ToTileCoordinates();
				Lighting.AddLight(point.X, point.Y, this._data.lightColor.X, this._data.lightColor.Y, this._data.lightColor.Z);
			}
			switch (this._type)
			{
			case 5:
				if (state != 2)
				{
					this._frameExtra = 0;
					this._frameExtraCounter = 0f;
				}
				break;
			case 7:
				state = 2;
				break;
			case 8:
				if (state == 0 || state == 1)
				{
					Vector2 position;
					position.X = mountedPlayer.position.X;
					position.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
					int num = (int)(position.X / 16f);
					float arg_195_0 = position.Y / 16f;
					float num2 = 0f;
					float num3 = (float)mountedPlayer.width;
					while (num3 > 0f)
					{
						float num4 = (float)((num + 1) * 16);
						float num5 = num4 - position.X;
						if (num5 > num3)
						{
							num5 = num3;
						}
						num2 += Collision.GetTileRotation(position) * num5;
						num3 -= num5;
						position.X += num5;
						num++;
					}
					float num6 = num2 / (float)mountedPlayer.width - mountedPlayer.fullRotation;
					float num7 = 0f;
					float num8 = 0.157079637f;
					if (num6 < 0f)
					{
						if (num6 > -num8)
						{
							num7 = num6;
						}
						else
						{
							num7 = -num8;
						}
					}
					else if (num6 > 0f)
					{
						if (num6 < num8)
						{
							num7 = num6;
						}
						else
						{
							num7 = num8;
						}
					}
					if (num7 != 0f)
					{
						mountedPlayer.fullRotation += num7;
						if (mountedPlayer.fullRotation > 0.7853982f)
						{
							mountedPlayer.fullRotation = 0.7853982f;
						}
						if (mountedPlayer.fullRotation < -0.7853982f)
						{
							mountedPlayer.fullRotation = -0.7853982f;
						}
					}
				}
				break;
			case 9:
				if (!this._aiming)
				{
					this._frameExtraCounter += 1f;
					if (this._frameExtraCounter >= 12f)
					{
						this._frameExtraCounter = 0f;
						this._frameExtra++;
						if (this._frameExtra >= 6)
						{
							this._frameExtra = 0;
						}
					}
				}
				break;
			case 10:
			{
				bool flag = Math.Abs(velocity.X) > this.DashSpeed - this.RunSpeed / 2f;
				if (state == 1)
				{
					bool flag2 = false;
					if (flag)
					{
						state = 5;
						if (this._frameExtra < 6)
						{
							flag2 = true;
						}
						this._frameExtra++;
					}
					else
					{
						this._frameExtra = 0;
					}
					if (flag2)
					{
						Vector2 vector = mountedPlayer.Center + new Vector2((float)(mountedPlayer.width * mountedPlayer.direction), 0f);
						Vector2 value = new Vector2(40f, 30f);
						float num9 = 6.28318548f * Main.rand.NextFloat();
						for (float num10 = 0f; num10 < 14f; num10 += 1f)
						{
							Dust dust = Main.dust[Dust.NewDust(vector, 0, 0, Utils.SelectRandom<int>(Main.rand, new int[]
							{
								176,
								177,
								179
							}), 0f, 0f, 0, default(Color), 1f)];
							Vector2 vector2 = Vector2.UnitY.RotatedBy((double)(num10 * 6.28318548f / 14f + num9), default(Vector2));
							vector2 *= 0.2f * (float)this._frameExtra;
							dust.position = vector + vector2 * value;
							dust.velocity = vector2 + new Vector2(this.RunSpeed - (float)(Math.Sign(velocity.X) * this._frameExtra * 2), 0f);
							dust.noGravity = true;
							dust.scale = 1f + Main.rand.NextFloat() * 0.8f;
							dust.fadeIn = Main.rand.NextFloat() * 2f;
							dust.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
						}
					}
				}
				if (flag)
				{
					Dust dust2 = Main.dust[Dust.NewDust(mountedPlayer.position, mountedPlayer.width, mountedPlayer.height, Utils.SelectRandom<int>(Main.rand, new int[]
					{
						176,
						177,
						179
					}), 0f, 0f, 0, default(Color), 1f)];
					dust2.velocity = Vector2.Zero;
					dust2.noGravity = true;
					dust2.scale = 0.5f + Main.rand.NextFloat() * 0.8f;
					dust2.fadeIn = 1f + Main.rand.NextFloat() * 2f;
					dust2.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
				}
				break;
			}
			case 14:
			{
				bool flag3 = Math.Abs(velocity.X) > this.RunSpeed / 2f;
				float num11 = (float)Math.Sign(mountedPlayer.velocity.X);
				float num12 = 12f;
				float num13 = 40f;
				if (!flag3)
				{
					mountedPlayer.basiliskCharge = 0f;
				}
				else
				{
					mountedPlayer.basiliskCharge = Utils.Clamp<float>(mountedPlayer.basiliskCharge + 0.00555555569f, 0f, 1f);
				}
				if ((double)mountedPlayer.position.Y > Main.worldSurface * 16.0 + 160.0)
				{
					Lighting.AddLight(mountedPlayer.Center, 0.5f, 0.1f, 0.1f);
				}
				if (flag3 && velocity.Y == 0f)
				{
					for (int i = 0; i < 2; i++)
					{
						Dust dust3 = Main.dust[Dust.NewDust(mountedPlayer.BottomLeft, mountedPlayer.width, 6, 31, 0f, 0f, 0, default(Color), 1f)];
						dust3.velocity = new Vector2(velocity.X * 0.15f, Main.rand.NextFloat() * -2f);
						dust3.noLight = true;
						dust3.scale = 0.5f + Main.rand.NextFloat() * 0.8f;
						dust3.fadeIn = 0.5f + Main.rand.NextFloat() * 1f;
						dust3.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
					}
					if (mountedPlayer.cMount == 0)
					{
						mountedPlayer.position += new Vector2(num11 * 24f, 0f);
						mountedPlayer.FloorVisuals(true);
						mountedPlayer.position -= new Vector2(num11 * 24f, 0f);
					}
				}
				if (num11 == (float)mountedPlayer.direction)
				{
					for (int j = 0; j < (int)(3f * mountedPlayer.basiliskCharge); j++)
					{
						Dust dust4 = Main.dust[Dust.NewDust(mountedPlayer.BottomLeft, mountedPlayer.width, 6, 6, 0f, 0f, 0, default(Color), 1f)];
						Vector2 value2 = mountedPlayer.Center + new Vector2(num11 * num13, num12);
						dust4.position = mountedPlayer.Center + new Vector2(num11 * (num13 - 2f), num12 - 6f + Main.rand.NextFloat() * 12f);
						dust4.velocity = (dust4.position - value2).SafeNormalize(Vector2.Zero) * (3.5f + Main.rand.NextFloat() * 0.5f);
						if (dust4.velocity.Y < 0f)
						{
							Dust expr_858_cp_0 = dust4;
							expr_858_cp_0.velocity.Y = expr_858_cp_0.velocity.Y * (1f + 2f * Main.rand.NextFloat());
						}
						dust4.velocity += mountedPlayer.velocity * 0.55f;
						dust4.velocity *= mountedPlayer.velocity.Length() / this.RunSpeed;
						dust4.velocity *= mountedPlayer.basiliskCharge;
						dust4.noGravity = true;
						dust4.noLight = true;
						dust4.scale = 0.5f + Main.rand.NextFloat() * 0.8f;
						dust4.fadeIn = 0.5f + Main.rand.NextFloat() * 1f;
						dust4.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMount, mountedPlayer);
					}
				}
				break;
			}
			}
			switch (state)
			{
			case 0:
				if (this._data.idleFrameCount != 0)
				{
					if (this._type == 5)
					{
						if (this._fatigue != 0f)
						{
							if (this._idleTime == 0)
							{
								this._idleTimeNext = this._idleTime + 1;
							}
						}
						else
						{
							this._idleTime = 0;
							this._idleTimeNext = 2;
						}
					}
					else if (this._idleTime == 0)
					{
						this._idleTimeNext = Main.rand.Next(900, 1500);
					}
					this._idleTime++;
				}
				this._frameCounter += 1f;
				if (this._data.idleFrameCount != 0 && this._idleTime >= this._idleTimeNext)
				{
					float num14 = (float)this._data.idleFrameDelay;
					if (this._type == 5)
					{
						num14 *= 2f - 1f * this._fatigue / this._fatigueMax;
					}
					int num15 = (int)((float)(this._idleTime - this._idleTimeNext) / num14);
					if (num15 >= this._data.idleFrameCount)
					{
						if (this._data.idleFrameLoop)
						{
							this._idleTime = this._idleTimeNext;
							this._frame = this._data.idleFrameStart;
						}
						else
						{
							this._frameCounter = 0f;
							this._frame = this._data.standingFrameStart;
							this._idleTime = 0;
						}
					}
					else
					{
						this._frame = this._data.idleFrameStart + num15;
					}
					if (this._type == 5)
					{
						this._frameExtra = this._frame;
						return;
					}
				}
				else
				{
					if (this._frameCounter > (float)this._data.standingFrameDelay)
					{
						this._frameCounter -= (float)this._data.standingFrameDelay;
						this._frame++;
					}
					if (this._frame < this._data.standingFrameStart || this._frame >= this._data.standingFrameStart + this._data.standingFrameCount)
					{
						this._frame = this._data.standingFrameStart;
						return;
					}
				}
				break;
			case 1:
			{
				int type = this._type;
				float num16;
				if (type != 6)
				{
					if (type != 9)
					{
						if (type != 13)
						{
							num16 = Math.Abs(velocity.X);
						}
						else
						{
							num16 = (this._flipDraw ? velocity.X : (-velocity.X));
						}
					}
					else if (this._flipDraw)
					{
						num16 = -Math.Abs(velocity.X);
					}
					else
					{
						num16 = Math.Abs(velocity.X);
					}
				}
				else
				{
					num16 = (this._flipDraw ? velocity.X : (-velocity.X));
				}
				this._frameCounter += num16;
				if (num16 >= 0f)
				{
					if (this._frameCounter > (float)this._data.runningFrameDelay)
					{
						this._frameCounter -= (float)this._data.runningFrameDelay;
						this._frame++;
					}
					if (this._frame < this._data.runningFrameStart || this._frame >= this._data.runningFrameStart + this._data.runningFrameCount)
					{
						this._frame = this._data.runningFrameStart;
						return;
					}
				}
				else
				{
					if (this._frameCounter < 0f)
					{
						this._frameCounter += (float)this._data.runningFrameDelay;
						this._frame--;
					}
					if (this._frame < this._data.runningFrameStart || this._frame >= this._data.runningFrameStart + this._data.runningFrameCount)
					{
						this._frame = this._data.runningFrameStart + this._data.runningFrameCount - 1;
						return;
					}
				}
				break;
			}
			case 2:
				this._frameCounter += 1f;
				if (this._frameCounter > (float)this._data.inAirFrameDelay)
				{
					this._frameCounter -= (float)this._data.inAirFrameDelay;
					this._frame++;
				}
				if (this._frame < this._data.inAirFrameStart || this._frame >= this._data.inAirFrameStart + this._data.inAirFrameCount)
				{
					this._frame = this._data.inAirFrameStart;
				}
				if (this._type == 4)
				{
					if (velocity.Y < 0f)
					{
						this._frame = 3;
						return;
					}
					this._frame = 6;
					return;
				}
				else if (this._type == 5)
				{
					float num17 = this._fatigue / this._fatigueMax;
					this._frameExtraCounter += 6f - 4f * num17;
					if (this._frameExtraCounter > (float)this._data.flyingFrameDelay)
					{
						this._frameExtra++;
						this._frameExtraCounter -= (float)this._data.flyingFrameDelay;
					}
					if (this._frameExtra < this._data.flyingFrameStart || this._frameExtra >= this._data.flyingFrameStart + this._data.flyingFrameCount)
					{
						this._frameExtra = this._data.flyingFrameStart;
						return;
					}
				}
				break;
			case 3:
				this._frameCounter += 1f;
				if (this._frameCounter > (float)this._data.flyingFrameDelay)
				{
					this._frameCounter -= (float)this._data.flyingFrameDelay;
					this._frame++;
				}
				if (this._frame < this._data.flyingFrameStart || this._frame >= this._data.flyingFrameStart + this._data.flyingFrameCount)
				{
					this._frame = this._data.flyingFrameStart;
					return;
				}
				break;
			case 4:
				this._frameCounter += (float)((int)((Math.Abs(velocity.X) + Math.Abs(velocity.Y)) / 2f));
				if (this._frameCounter > (float)this._data.swimFrameDelay)
				{
					this._frameCounter -= (float)this._data.swimFrameDelay;
					this._frame++;
				}
				if (this._frame < this._data.swimFrameStart || this._frame >= this._data.swimFrameStart + this._data.swimFrameCount)
				{
					this._frame = this._data.swimFrameStart;
					return;
				}
				break;
			case 5:
			{
				int type2 = this._type;
				float num16;
				if (type2 != 6)
				{
					if (type2 != 9)
					{
						if (type2 != 13)
						{
							num16 = Math.Abs(velocity.X);
						}
						else
						{
							num16 = (this._flipDraw ? velocity.X : (-velocity.X));
						}
					}
					else if (this._flipDraw)
					{
						num16 = -Math.Abs(velocity.X);
					}
					else
					{
						num16 = Math.Abs(velocity.X);
					}
				}
				else
				{
					num16 = (this._flipDraw ? velocity.X : (-velocity.X));
				}
				this._frameCounter += num16;
				if (num16 >= 0f)
				{
					if (this._frameCounter > (float)this._data.dashingFrameDelay)
					{
						this._frameCounter -= (float)this._data.dashingFrameDelay;
						this._frame++;
					}
					if (this._frame < this._data.dashingFrameStart || this._frame >= this._data.dashingFrameStart + this._data.dashingFrameCount)
					{
						this._frame = this._data.dashingFrameStart;
						return;
					}
				}
				else
				{
					if (this._frameCounter < 0f)
					{
						this._frameCounter += (float)this._data.dashingFrameDelay;
						this._frame--;
					}
					if (this._frame < this._data.dashingFrameStart || this._frame >= this._data.dashingFrameStart + this._data.dashingFrameCount)
					{
						this._frame = this._data.dashingFrameStart + this._data.dashingFrameCount - 1;
					}
				}
				break;
			}
			default:
				return;
			}
		}

		public void UpdateEffects(Player mountedPlayer)
		{
			mountedPlayer.autoJump = this.AutoJump;
			switch (this._type)
			{
			case 8:
				if (mountedPlayer.ownedProjectileCounts[453] < 1)
				{
					this._abilityActive = false;
					return;
				}
				break;
			case 9:
			{
				Vector2 center = mountedPlayer.Center;
				Vector2 vector = center;
				bool flag = false;
				float num = 1500f;
				for (int i = 0; i < 200; i++)
				{
					NPC nPC = Main.npc[i];
					if (nPC.CanBeChasedBy(this, false))
					{
						Vector2 v = nPC.Center - center;
						float num2 = v.Length();
						if ((Vector2.Distance(vector, center) > num2 && num2 < num) || !flag)
						{
							bool flag2 = true;
							float num3 = Math.Abs(v.ToRotation());
							if (mountedPlayer.direction == 1 && (double)num3 > 1.0471975949079879)
							{
								flag2 = false;
							}
							else if (mountedPlayer.direction == -1 && (double)num3 < 2.0943951461045853)
							{
								flag2 = false;
							}
							if (Collision.CanHitLine(center, 0, 0, nPC.position, nPC.width, nPC.height) && flag2)
							{
								num = num2;
								vector = nPC.Center;
								flag = true;
							}
						}
					}
				}
				bool flag3 = flag;
				if (!flag3)
				{
					this._abilityCharging = false;
					this.ResetHeadPosition();
					return;
				}
				if (this._abilityCooldown == 0 && mountedPlayer.whoAmI == Main.myPlayer)
				{
					this.AimAbility(mountedPlayer, vector);
					this.StopAbilityCharge();
					this.UseAbility(mountedPlayer, vector, false);
					return;
				}
				this.AimAbility(mountedPlayer, vector);
				this._abilityCharging = true;
				return;
			}
			case 10:
				mountedPlayer.doubleJumpUnicorn = true;
				if (Math.Abs(mountedPlayer.velocity.X) > mountedPlayer.mount.DashSpeed - mountedPlayer.mount.RunSpeed / 2f)
				{
					mountedPlayer.noKnockback = true;
				}
				if (mountedPlayer.itemAnimation > 0 && mountedPlayer.inventory[mountedPlayer.selectedItem].type == 1260)
				{
					AchievementsHelper.HandleSpecialEvent(mountedPlayer, 5);
					return;
				}
				break;
			case 11:
			{
				Vector3 value = new Vector3(0.4f, 0.12f, 0.15f);
				float num4 = 1f + Math.Abs(mountedPlayer.velocity.X) / this.RunSpeed * 2.5f;
				mountedPlayer.statDefense += (int)(2f * num4);
				int num5 = Math.Sign(mountedPlayer.velocity.X);
				if (num5 == 0)
				{
					num5 = mountedPlayer.direction;
				}
				if (Main.netMode != 2)
				{
					value *= num4;
					Lighting.AddLight(mountedPlayer.Center, value.X, value.Y, value.Z);
					Lighting.AddLight(mountedPlayer.Top, value.X, value.Y, value.Z);
					Lighting.AddLight(mountedPlayer.Bottom, value.X, value.Y, value.Z);
					Lighting.AddLight(mountedPlayer.Left, value.X, value.Y, value.Z);
					Lighting.AddLight(mountedPlayer.Right, value.X, value.Y, value.Z);
					float num6 = -24f;
					if (mountedPlayer.direction != num5)
					{
						num6 = -22f;
					}
					if (num5 == -1)
					{
						num6 += 1f;
					}
					Vector2 value2 = new Vector2(num6 * (float)num5, -19f).RotatedBy((double)mountedPlayer.fullRotation, default(Vector2));
					Vector2 vector2 = new Vector2(MathHelper.Lerp(0f, -8f, mountedPlayer.fullRotation / 0.7853982f), MathHelper.Lerp(0f, 2f, Math.Abs(mountedPlayer.fullRotation / 0.7853982f))).RotatedBy((double)mountedPlayer.fullRotation, default(Vector2));
					if (num5 == Math.Sign(mountedPlayer.fullRotation))
					{
						vector2 *= MathHelper.Lerp(1f, 0.6f, Math.Abs(mountedPlayer.fullRotation / 0.7853982f));
					}
					Vector2 vector3 = mountedPlayer.Bottom + value2 + vector2;
					Vector2 vector4 = mountedPlayer.oldPosition + mountedPlayer.Size * new Vector2(0.5f, 1f) + value2 + vector2;
					if (Vector2.Distance(vector3, vector4) > 3f)
					{
						int num7 = (int)Vector2.Distance(vector3, vector4) / 3;
						if (Vector2.Distance(vector3, vector4) % 3f != 0f)
						{
							num7++;
						}
						for (float num8 = 1f; num8 <= (float)num7; num8 += 1f)
						{
							Dust dust = Main.dust[Dust.NewDust(mountedPlayer.Center, 0, 0, 182, 0f, 0f, 0, default(Color), 1f)];
							dust.position = Vector2.Lerp(vector4, vector3, num8 / (float)num7);
							dust.noGravity = true;
							dust.velocity = Vector2.Zero;
							dust.customData = mountedPlayer;
							dust.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMinecart, mountedPlayer);
						}
					}
					else
					{
						Dust dust2 = Main.dust[Dust.NewDust(mountedPlayer.Center, 0, 0, 182, 0f, 0f, 0, default(Color), 1f)];
						dust2.position = vector3;
						dust2.noGravity = true;
						dust2.velocity = Vector2.Zero;
						dust2.customData = mountedPlayer;
						dust2.shader = GameShaders.Armor.GetSecondaryShader(mountedPlayer.cMinecart, mountedPlayer);
					}
				}
				if (mountedPlayer.whoAmI == Main.myPlayer && mountedPlayer.velocity.X != 0f)
				{
					Vector2 minecartMechPoint = Mount.GetMinecartMechPoint(mountedPlayer, 20, -19);
					int damage = 60;
					int num9 = 0;
					float num10 = 0f;
					for (int j = 0; j < 200; j++)
					{
						NPC nPC2 = Main.npc[j];
						if (nPC2.active && nPC2.immune[mountedPlayer.whoAmI] <= 0 && !nPC2.dontTakeDamage && nPC2.Distance(minecartMechPoint) < 300f && nPC2.CanBeChasedBy(mountedPlayer, false) && Collision.CanHitLine(nPC2.position, nPC2.width, nPC2.height, minecartMechPoint, 0, 0) && Math.Abs(MathHelper.WrapAngle(MathHelper.WrapAngle(nPC2.AngleFrom(minecartMechPoint)) - MathHelper.WrapAngle((mountedPlayer.fullRotation + (float)num5 == -1f) ? 3.14159274f : 0f))) < 0.7853982f)
						{
							Vector2 v2 = nPC2.position + nPC2.Size * Utils.RandomVector2(Main.rand, 0f, 1f) - minecartMechPoint;
							num10 += v2.ToRotation();
							num9++;
							int num11 = Projectile.NewProjectile(minecartMechPoint.X, minecartMechPoint.Y, v2.X, v2.Y, 591, 0, 0f, mountedPlayer.whoAmI, (float)mountedPlayer.whoAmI, 0f);
							Main.projectile[num11].Center = nPC2.Center;
							Main.projectile[num11].damage = damage;
							Main.projectile[num11].Damage();
							Main.projectile[num11].damage = 0;
							Main.projectile[num11].Center = minecartMechPoint;
						}
					}
				}
				break;
			}
			case 12:
				if (mountedPlayer.MountFishronSpecial)
				{
					Vector3 value3 = Colors.CurrentLiquidColor.ToVector3();
					value3 *= 0.4f;
					Point point = (mountedPlayer.Center + Vector2.UnitX * (float)mountedPlayer.direction * 20f + mountedPlayer.velocity * 10f).ToTileCoordinates();
					if (!WorldGen.SolidTile(point.X, point.Y))
					{
						Lighting.AddLight(point.X, point.Y, value3.X, value3.Y, value3.Z);
					}
					else
					{
						Lighting.AddLight(mountedPlayer.Center + Vector2.UnitX * (float)mountedPlayer.direction * 20f, value3.X, value3.Y, value3.Z);
					}
					mountedPlayer.meleeDamage += 0.15f;
					mountedPlayer.rangedDamage += 0.15f;
					mountedPlayer.magicDamage += 0.15f;
					mountedPlayer.minionDamage += 0.15f;
					mountedPlayer.thrownDamage += 0.15f;
				}
				if (mountedPlayer.statLife <= mountedPlayer.statLifeMax2 / 2)
				{
					mountedPlayer.MountFishronSpecialCounter = 60f;
				}
				if (mountedPlayer.wet)
				{
					mountedPlayer.MountFishronSpecialCounter = 300f;
					return;
				}
				break;
			default:
				return;
			}
		}

		public static Vector2 GetMinecartMechPoint(Player mountedPlayer, int offX, int offY)
		{
			int num = Math.Sign(mountedPlayer.velocity.X);
			if (num == 0)
			{
				num = mountedPlayer.direction;
			}
			float num2 = (float)offX;
			int num3 = Math.Sign(offX);
			if (mountedPlayer.direction != num)
			{
				num2 -= (float)num3;
			}
			if (num == -1)
			{
				num2 -= (float)num3;
			}
			Vector2 value = new Vector2(num2 * (float)num, (float)offY).RotatedBy((double)mountedPlayer.fullRotation, default(Vector2));
			Vector2 vector = new Vector2(MathHelper.Lerp(0f, -8f, mountedPlayer.fullRotation / 0.7853982f), MathHelper.Lerp(0f, 2f, Math.Abs(mountedPlayer.fullRotation / 0.7853982f))).RotatedBy((double)mountedPlayer.fullRotation, default(Vector2));
			if (num == Math.Sign(mountedPlayer.fullRotation))
			{
				vector *= MathHelper.Lerp(1f, 0.6f, Math.Abs(mountedPlayer.fullRotation / 0.7853982f));
			}
			return mountedPlayer.Bottom + value + vector;
		}

		public void ResetFlightTime(float xVelocity)
		{
			this._flyTime = (this._active ? this._data.flightTimeMax : 0);
			if (this._type == 0)
			{
				this._flyTime += (int)(Math.Abs(xVelocity) * 20f);
			}
		}

		public void CheckMountBuff(Player mountedPlayer)
		{
			if (this._type == -1)
			{
				return;
			}
			for (int i = 0; i < 22; i++)
			{
				if (mountedPlayer.buffType[i] == this._data.buff)
				{
					return;
				}
				if (this.Cart && mountedPlayer.buffType[i] == this._data.extraBuff)
				{
					return;
				}
			}
			this.Dismount(mountedPlayer);
		}

		public void ResetHeadPosition()
		{
			if (this._aiming)
			{
				this._aiming = false;
				this._frameExtra = 0;
				this._flipDraw = false;
			}
		}

		private Vector2 ClampToDeadZone(Player mountedPlayer, Vector2 position)
		{
			int num;
			int num2;
			switch (this._type)
			{
			case 8:
				num = (int)Mount.drillTextureSize.Y;
				num2 = (int)Mount.drillTextureSize.X;
				break;
			case 9:
				num = (int)Mount.scutlixTextureSize.Y;
				num2 = (int)Mount.scutlixTextureSize.X;
				break;
			default:
				return position;
			}
			Vector2 center = mountedPlayer.Center;
			position -= center;
			if (position.X > (float)(-(float)num2) && position.X < (float)num2 && position.Y > (float)(-(float)num) && position.Y < (float)num)
			{
				float num3 = (float)num2 / Math.Abs(position.X);
				float num4 = (float)num / Math.Abs(position.Y);
				if (num3 > num4)
				{
					position *= num4;
				}
				else
				{
					position *= num3;
				}
			}
			return position + center;
		}

		public bool AimAbility(Player mountedPlayer, Vector2 mousePosition)
		{
			this._aiming = true;
			switch (this._type)
			{
			case 8:
			{
				Vector2 vector = this.ClampToDeadZone(mountedPlayer, mousePosition);
				vector -= mountedPlayer.Center;
				Mount.DrillMountData drillMountData = (Mount.DrillMountData)this._mountSpecificData;
				float num = vector.ToRotation();
				if (num < 0f)
				{
					num += 6.28318548f;
				}
				drillMountData.diodeRotationTarget = num;
				float num2 = drillMountData.diodeRotation % 6.28318548f;
				if (num2 < 0f)
				{
					num2 += 6.28318548f;
				}
				if (num2 < num)
				{
					if (num - num2 > 3.14159274f)
					{
						num2 += 6.28318548f;
					}
				}
				else if (num2 - num > 3.14159274f)
				{
					num2 -= 6.28318548f;
				}
				drillMountData.diodeRotation = num2;
				drillMountData.crosshairPosition = mousePosition;
				return true;
			}
			case 9:
			{
				int frameExtra = this._frameExtra;
				int direction = mountedPlayer.direction;
				Vector2 vector = this.ClampToDeadZone(mountedPlayer, mousePosition);
				vector -= mountedPlayer.Center;
				float num3 = MathHelper.ToDegrees(vector.ToRotation());
				if (num3 > 90f)
				{
					mountedPlayer.direction = -1;
					num3 = 180f - num3;
				}
				else if (num3 < -90f)
				{
					mountedPlayer.direction = -1;
					num3 = -180f - num3;
				}
				else
				{
					mountedPlayer.direction = 1;
				}
				if ((mountedPlayer.direction > 0 && mountedPlayer.velocity.X < 0f) || (mountedPlayer.direction < 0 && mountedPlayer.velocity.X > 0f))
				{
					this._flipDraw = true;
				}
				else
				{
					this._flipDraw = false;
				}
				if (num3 >= 0f)
				{
					if ((double)num3 < 22.5)
					{
						this._frameExtra = 8;
					}
					else if ((double)num3 < 67.5)
					{
						this._frameExtra = 9;
					}
					else if ((double)num3 < 112.5)
					{
						this._frameExtra = 10;
					}
				}
				else if ((double)num3 > -22.5)
				{
					this._frameExtra = 8;
				}
				else if ((double)num3 > -67.5)
				{
					this._frameExtra = 7;
				}
				else if ((double)num3 > -112.5)
				{
					this._frameExtra = 6;
				}
				float abilityCharge = this.AbilityCharge;
				if (abilityCharge > 0f)
				{
					Vector2 vector2;
					vector2.X = mountedPlayer.position.X + (float)(mountedPlayer.width / 2);
					vector2.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
					int num4 = (this._frameExtra - 6) * 2;
					for (int i = 0; i < 2; i++)
					{
						Vector2 vector3;
						vector3.Y = vector2.Y + Mount.scutlixEyePositions[num4 + i].Y;
						if (mountedPlayer.direction == -1)
						{
							vector3.X = vector2.X - Mount.scutlixEyePositions[num4 + i].X - (float)this._data.xOffset;
						}
						else
						{
							vector3.X = vector2.X + Mount.scutlixEyePositions[num4 + i].X + (float)this._data.xOffset;
						}
						Lighting.AddLight((int)(vector3.X / 16f), (int)(vector3.Y / 16f), 1f * abilityCharge, 0f, 0f);
					}
				}
				return this._frameExtra != frameExtra || mountedPlayer.direction != direction;
			}
			default:
				return false;
			}
		}

		public void Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, Vector2 Position, Color drawColor, SpriteEffects playerEffect, float shadow)
		{
			if (playerDrawData == null)
			{
				return;
			}
			Texture2D texture2D;
			Texture2D texture2D2;
			switch (drawType)
			{
			case 0:
				texture2D = this._data.backTexture;
				texture2D2 = this._data.backTextureGlow;
				break;
			case 1:
				texture2D = this._data.backTextureExtra;
				texture2D2 = this._data.backTextureExtraGlow;
				break;
			case 2:
				if (this._type == 0 && this._idleTime >= this._idleTimeNext)
				{
					return;
				}
				texture2D = this._data.frontTexture;
				texture2D2 = this._data.frontTextureGlow;
				break;
			case 3:
				texture2D = this._data.frontTextureExtra;
				texture2D2 = this._data.frontTextureExtraGlow;
				break;
			default:
				texture2D = null;
				texture2D2 = null;
				break;
			}
			if (texture2D == null)
			{
				return;
			}
			int type = this._type;
			if ((type == 0 || type == 9) && drawType == 3 && shadow != 0f)
			{
				return;
			}
			int num = this.XOffset;
			int num2 = this.YOffset + this.PlayerOffset;
			if (drawPlayer.direction <= 0 && (!this.Cart || !this.Directional))
			{
				num *= -1;
			}
			Position.X = (float)((int)(Position.X - Main.screenPosition.X + (float)(drawPlayer.width / 2) + (float)num));
			Position.Y = (float)((int)(Position.Y - Main.screenPosition.Y + (float)(drawPlayer.height / 2) + (float)num2));
			int type2 = this._type;
			int num3;
			if (type2 != 5)
			{
				if (type2 == 9)
				{
					switch (drawType)
					{
					case 0:
						num3 = this._frame;
						goto IL_1D6;
					case 2:
						num3 = this._frameExtra;
						goto IL_1D6;
					case 3:
						num3 = this._frameExtra;
						goto IL_1D6;
					}
					num3 = 0;
				}
				else
				{
					num3 = this._frame;
				}
			}
			else
			{
				switch (drawType)
				{
				case 0:
					num3 = this._frame;
					break;
				case 1:
					num3 = this._frameExtra;
					break;
				default:
					num3 = 0;
					break;
				}
			}
			IL_1D6:
			int num4 = this._data.textureHeight / this._data.totalFrames;
			Rectangle value = new Rectangle(0, num4 * num3, this._data.textureWidth, num4);
			int type3 = this._type;
			if (type3 != 0)
			{
				switch (type3)
				{
				case 7:
					if (drawType == 3)
					{
						drawColor = new Color(250, 250, 250, 255) * drawPlayer.stealth * (1f - shadow);
					}
					break;
				case 9:
					if (drawType == 3)
					{
						if (this._abilityCharge == 0)
						{
							return;
						}
						drawColor = Color.Multiply(Color.White, (float)this._abilityCharge / (float)this._data.abilityChargeMax);
						drawColor.A = 0;
					}
					break;
				}
			}
			else if (drawType == 3)
			{
				drawColor = Color.White;
			}
			Color color = new Color(drawColor.ToVector4() * 0.25f + new Vector4(0.75f));
			switch (this._type)
			{
			case 11:
				if (drawType == 2)
				{
					color = Color.White;
					color.A = 127;
				}
				break;
			case 12:
				if (drawType == 0)
				{
					float scale = MathHelper.Clamp(drawPlayer.MountFishronSpecialCounter / 60f, 0f, 1f);
					color = Colors.CurrentLiquidColor;
					if (color == Color.Transparent)
					{
						color = Color.White;
					}
					color.A = 127;
					color *= scale;
				}
				break;
			}
			float num5 = 0f;
			switch (this._type)
			{
			case 7:
				num5 = drawPlayer.fullRotation;
				break;
			case 8:
			{
				Mount.DrillMountData drillMountData = (Mount.DrillMountData)this._mountSpecificData;
				if (drawType == 0)
				{
					num5 = drillMountData.outerRingRotation - num5;
				}
				else if (drawType == 3)
				{
					num5 = drillMountData.diodeRotation - num5 - drawPlayer.fullRotation;
				}
				break;
			}
			}
			Vector2 origin = this.Origin;
			int type4 = this._type;
			float scale2 = 1f;
			SpriteEffects effect;
			switch (this._type)
			{
			case 6:
			case 13:
				effect = (this._flipDraw ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
				goto IL_454;
			case 7:
				effect = SpriteEffects.None;
				goto IL_454;
			case 8:
				effect = ((drawPlayer.direction == 1 && drawType == 2) ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
				goto IL_454;
			case 11:
				effect = ((Math.Sign(drawPlayer.velocity.X) == -drawPlayer.direction) ? (playerEffect ^ SpriteEffects.FlipHorizontally) : playerEffect);
				goto IL_454;
			}
			effect = playerEffect;
			IL_454:
			bool flag = false;
			int type5 = this._type;
			if (!flag)
			{
				DrawData item = new DrawData(texture2D, Position, new Rectangle?(value), drawColor, num5, origin, scale2, effect, 0);
				item.shader = Mount.currentShader;
				playerDrawData.Add(item);
				if (texture2D2 != null)
				{
					item = new DrawData(texture2D2, Position, new Rectangle?(value), color * ((float)drawColor.A / 255f), num5, origin, scale2, effect, 0);
					item.shader = Mount.currentShader;
				}
				playerDrawData.Add(item);
			}
			int type6 = this._type;
			if (type6 != 8)
			{
				return;
			}
			if (drawType == 3)
			{
				Mount.DrillMountData drillMountData2 = (Mount.DrillMountData)this._mountSpecificData;
				Rectangle value2 = new Rectangle(0, 0, 1, 1);
				Vector2 vector = Mount.drillDiodePoint1.RotatedBy((double)drillMountData2.diodeRotation, default(Vector2));
				Vector2 vector2 = Mount.drillDiodePoint2.RotatedBy((double)drillMountData2.diodeRotation, default(Vector2));
				for (int i = 0; i < drillMountData2.beams.Length; i++)
				{
					Mount.DrillBeam drillBeam = drillMountData2.beams[i];
					if (!(drillBeam.curTileTarget == Point16.NegativeOne))
					{
						for (int j = 0; j < 2; j++)
						{
							Vector2 value3 = new Vector2((float)(drillBeam.curTileTarget.X * 16 + 8), (float)(drillBeam.curTileTarget.Y * 16 + 8)) - Main.screenPosition - Position;
							Vector2 vector3;
							Color color2;
							if (j == 0)
							{
								vector3 = vector;
								color2 = Color.CornflowerBlue;
							}
							else
							{
								vector3 = vector2;
								color2 = Color.LightGreen;
							}
							color2.A = 128;
							color2 *= 0.5f;
							Vector2 v = value3 - vector3;
							float num6 = v.ToRotation();
							float y = v.Length();
							Vector2 scale3 = new Vector2(2f, y);
							DrawData item = new DrawData(Main.magicPixel, vector3 + Position, new Rectangle?(value2), color2, num6 - 1.57079637f, Vector2.Zero, scale3, SpriteEffects.None, 0);
							item.ignorePlayerRotation = true;
							item.shader = Mount.currentShader;
							playerDrawData.Add(item);
						}
					}
				}
			}
		}

		public void Dismount(Player mountedPlayer)
		{
			if (!this._active)
			{
				return;
			}
			bool cart = this.Cart;
			this._active = false;
			mountedPlayer.ClearBuff(this._data.buff);
			this._mountSpecificData = null;
			if (cart)
			{
				mountedPlayer.ClearBuff(this._data.extraBuff);
				mountedPlayer.cartFlip = false;
				mountedPlayer.lastBoost = Vector2.Zero;
			}
			mountedPlayer.fullRotation = 0f;
			mountedPlayer.fullRotationOrigin = Vector2.Zero;
			if (Main.netMode != 2)
			{
				for (int i = 0; i < 100; i++)
				{
					if (this._type == 6 || this._type == 11 || this._type == 13)
					{
						if (i % 10 == 0)
						{
							int type = Main.rand.Next(61, 64);
							int num = Gore.NewGore(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), Vector2.Zero, type, 1f);
							Main.gore[num].alpha = 100;
							Main.gore[num].velocity = Vector2.Transform(new Vector2(1f, 0f), Matrix.CreateRotationZ((float)(Main.rand.NextDouble() * 6.2831854820251465)));
						}
					}
					else
					{
						int num2 = Dust.NewDust(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), mountedPlayer.width + 40, mountedPlayer.height, this._data.spawnDust, 0f, 0f, 0, default(Color), 1f);
						Main.dust[num2].scale += (float)Main.rand.Next(-10, 21) * 0.01f;
						if (this._data.spawnDustNoGravity)
						{
							Main.dust[num2].noGravity = true;
						}
						else if (Main.rand.Next(2) == 0)
						{
							Main.dust[num2].scale *= 1.3f;
							Main.dust[num2].noGravity = true;
						}
						else
						{
							Main.dust[num2].velocity *= 0.5f;
						}
						Main.dust[num2].velocity += mountedPlayer.velocity * 0.8f;
					}
				}
			}
			this.Reset();
			mountedPlayer.position.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
			mountedPlayer.height = 42;
			mountedPlayer.position.Y = mountedPlayer.position.Y - (float)mountedPlayer.height;
			if (mountedPlayer.whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(13, -1, -1, "", mountedPlayer.whoAmI, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		public void SetMount(int m, Player mountedPlayer, bool faceLeft = false)
		{
			if (this._type == m || m <= -1 || m >= 15)
			{
				return;
			}
			if (m == 5 && mountedPlayer.wet)
			{
				return;
			}
			if (this._active)
			{
				mountedPlayer.ClearBuff(this._data.buff);
				if (this.Cart)
				{
					mountedPlayer.ClearBuff(this._data.extraBuff);
					mountedPlayer.cartFlip = false;
					mountedPlayer.lastBoost = Vector2.Zero;
				}
				mountedPlayer.fullRotation = 0f;
				mountedPlayer.fullRotationOrigin = Vector2.Zero;
				this._mountSpecificData = null;
			}
			else
			{
				this._active = true;
			}
			this._flyTime = 0;
			this._type = m;
			this._data = Mount.mounts[m];
			this._fatigueMax = (float)this._data.fatigueMax;
			if (this.Cart && !faceLeft && !this.Directional)
			{
				mountedPlayer.AddBuff(this._data.extraBuff, 3600, true);
				this._flipDraw = true;
			}
			else
			{
				mountedPlayer.AddBuff(this._data.buff, 3600, true);
				this._flipDraw = false;
			}
			if (this._type == 9 && this._abilityCooldown < 20)
			{
				this._abilityCooldown = 20;
			}
			mountedPlayer.position.Y = mountedPlayer.position.Y + (float)mountedPlayer.height;
			for (int i = 0; i < mountedPlayer.shadowPos.Length; i++)
			{
				Vector2[] expr_14D_cp_0 = mountedPlayer.shadowPos;
				int expr_14D_cp_1 = i;
				expr_14D_cp_0[expr_14D_cp_1].Y = expr_14D_cp_0[expr_14D_cp_1].Y + (float)mountedPlayer.height;
			}
			mountedPlayer.height = 42 + this._data.heightBoost;
			mountedPlayer.position.Y = mountedPlayer.position.Y - (float)mountedPlayer.height;
			for (int j = 0; j < mountedPlayer.shadowPos.Length; j++)
			{
				Vector2[] expr_1AC_cp_0 = mountedPlayer.shadowPos;
				int expr_1AC_cp_1 = j;
				expr_1AC_cp_0[expr_1AC_cp_1].Y = expr_1AC_cp_0[expr_1AC_cp_1].Y - (float)mountedPlayer.height;
			}
			if (this._type == 7 || this._type == 8)
			{
				mountedPlayer.fullRotationOrigin = new Vector2((float)(mountedPlayer.width / 2), (float)(mountedPlayer.height / 2));
			}
			if (this._type == 8)
			{
				this._mountSpecificData = new Mount.DrillMountData();
			}
			if (Main.netMode != 2)
			{
				for (int k = 0; k < 100; k++)
				{
					if (this._type == 6 || this._type == 11 || this._type == 13)
					{
						if (k % 10 == 0)
						{
							int type = Main.rand.Next(61, 64);
							int num = Gore.NewGore(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), Vector2.Zero, type, 1f);
							Main.gore[num].alpha = 100;
							Main.gore[num].velocity = Vector2.Transform(new Vector2(1f, 0f), Matrix.CreateRotationZ((float)(Main.rand.NextDouble() * 6.2831854820251465)));
						}
					}
					else
					{
						int num2 = Dust.NewDust(new Vector2(mountedPlayer.position.X - 20f, mountedPlayer.position.Y), mountedPlayer.width + 40, mountedPlayer.height, this._data.spawnDust, 0f, 0f, 0, default(Color), 1f);
						Main.dust[num2].scale += (float)Main.rand.Next(-10, 21) * 0.01f;
						if (this._data.spawnDustNoGravity)
						{
							Main.dust[num2].noGravity = true;
						}
						else if (Main.rand.Next(2) == 0)
						{
							Main.dust[num2].scale *= 1.3f;
							Main.dust[num2].noGravity = true;
						}
						else
						{
							Main.dust[num2].velocity *= 0.5f;
						}
						Main.dust[num2].velocity += mountedPlayer.velocity * 0.8f;
					}
				}
			}
			if (mountedPlayer.whoAmI == Main.myPlayer)
			{
				NetMessage.SendData(13, -1, -1, "", mountedPlayer.whoAmI, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		public bool CanMount(int m, Player mountingPlayer)
		{
			int num = 42 + Mount.mounts[m].heightBoost;
			Vector2 position = mountingPlayer.position + new Vector2(0f, (float)(mountingPlayer.height - num)) + mountingPlayer.velocity;
			return Collision.IsClearSpotTest(position, 16f, mountingPlayer.width, num, true, true, 1, true, false);
		}

		public bool FindTileHeight(Vector2 position, int maxTilesDown, out float tileHeight)
		{
			int num = (int)(position.X / 16f);
			int num2 = (int)(position.Y / 16f);
			for (int i = 0; i <= maxTilesDown; i++)
			{
				Tile tile = Main.tile[num, num2];
				bool flag = Main.tileSolid[(int)tile.type];
				bool flag2 = Main.tileSolidTop[(int)tile.type];
				if (tile.active())
				{
					if (flag)
					{
						if (flag2)
						{
						}
					}
				}
				num2++;
			}
			tileHeight = 0f;
			return true;
		}
	}
}
