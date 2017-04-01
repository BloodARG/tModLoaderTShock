using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Tile_Entities;
using Terraria.Modules;

namespace Terraria.ObjectData
{
	public class TileObjectData
	{
		internal TileObjectData _parent;
		private bool _linkedAlternates;
		private bool _usesCustomCanPlace;
		private TileObjectAlternatesModule _alternates;
		private AnchorDataModule _anchor;
		private AnchorTypesModule _anchorTiles;
		private LiquidDeathModule _liquidDeath;
		private LiquidPlacementModule _liquidPlacement;
		private TilePlacementHooksModule _placementHooks;
		private TileObjectSubTilesModule _subTiles;
		private TileObjectDrawModule _tileObjectDraw;
		private TileObjectStyleModule _tileObjectStyle;
		private TileObjectBaseModule _tileObjectBase;
		private TileObjectCoordinatesModule _tileObjectCoords;
		private bool _hasOwnAlternates;
		private bool _hasOwnAnchor;
		private bool _hasOwnAnchorTiles;
		private bool _hasOwnLiquidDeath;
		private bool _hasOwnLiquidPlacement;
		private bool _hasOwnPlacementHooks;
		private bool _hasOwnSubTiles;
		private bool _hasOwnTileObjectBase;
		private bool _hasOwnTileObjectDraw;
		private bool _hasOwnTileObjectStyle;
		private bool _hasOwnTileObjectCoords;
		internal static List<TileObjectData> _data;
		private static TileObjectData _baseObject;
		private static bool readOnlyData;
		public static TileObjectData newTile;
		public static TileObjectData newSubTile;
		public static TileObjectData newAlternate;
		public static TileObjectData StyleSwitch;
		public static TileObjectData StyleTorch;
		public static TileObjectData Style4x2;
		public static TileObjectData Style2x2;
		public static TileObjectData Style1x2;
		public static TileObjectData Style1x1;
		public static TileObjectData StyleAlch;
		public static TileObjectData StyleDye;
		public static TileObjectData Style2x1;
		public static TileObjectData Style6x3;
		public static TileObjectData StyleSmallCage;
		public static TileObjectData StyleOnTable1x1;
		public static TileObjectData Style1x2Top;
		public static TileObjectData Style1xX;
		public static TileObjectData Style2xX;
		public static TileObjectData Style3x2;
		public static TileObjectData Style3x3;
		public static TileObjectData Style3x4;
		public static TileObjectData Style5x4;
		public static TileObjectData Style3x3Wall;

		public bool LinkedAlternates
		{
			get
			{
				return this._linkedAlternates;
			}
			set
			{
				this.WriteCheck();
				if (value && !this._hasOwnAlternates)
				{
					this._hasOwnAlternates = true;
					this._alternates = new TileObjectAlternatesModule(this._alternates);
				}
				this._linkedAlternates = value;
			}
		}

		public bool UsesCustomCanPlace
		{
			get
			{
				return this._usesCustomCanPlace;
			}
			set
			{
				this.WriteCheck();
				this._usesCustomCanPlace = value;
			}
		}

		internal List<TileObjectData> Alternates
		{
			get
			{
				if (this._alternates == null)
				{
					return TileObjectData._baseObject.Alternates;
				}
				return this._alternates.data;
			}
			set
			{
				if (!this._hasOwnAlternates)
				{
					this._hasOwnAlternates = true;
					this._alternates = new TileObjectAlternatesModule(this._alternates);
				}
				this._alternates.data = value;
			}
		}

		public AnchorData AnchorTop
		{
			get
			{
				if (this._anchor == null)
				{
					return TileObjectData._baseObject.AnchorTop;
				}
				return this._anchor.top;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchor)
				{
					if (this._anchor.top == value)
					{
						return;
					}
					this._hasOwnAnchor = true;
					this._anchor = new AnchorDataModule(this._anchor);
				}
				this._anchor.top = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].AnchorTop = value;
					}
				}
			}
		}

		public AnchorData AnchorBottom
		{
			get
			{
				if (this._anchor == null)
				{
					return TileObjectData._baseObject.AnchorBottom;
				}
				return this._anchor.bottom;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchor)
				{
					if (this._anchor.bottom == value)
					{
						return;
					}
					this._hasOwnAnchor = true;
					this._anchor = new AnchorDataModule(this._anchor);
				}
				this._anchor.bottom = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].AnchorBottom = value;
					}
				}
			}
		}

		public AnchorData AnchorLeft
		{
			get
			{
				if (this._anchor == null)
				{
					return TileObjectData._baseObject.AnchorLeft;
				}
				return this._anchor.left;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchor)
				{
					if (this._anchor.left == value)
					{
						return;
					}
					this._hasOwnAnchor = true;
					this._anchor = new AnchorDataModule(this._anchor);
				}
				this._anchor.left = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].AnchorLeft = value;
					}
				}
			}
		}

		public AnchorData AnchorRight
		{
			get
			{
				if (this._anchor == null)
				{
					return TileObjectData._baseObject.AnchorRight;
				}
				return this._anchor.right;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchor)
				{
					if (this._anchor.right == value)
					{
						return;
					}
					this._hasOwnAnchor = true;
					this._anchor = new AnchorDataModule(this._anchor);
				}
				this._anchor.right = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].AnchorRight = value;
					}
				}
			}
		}

		public bool AnchorWall
		{
			get
			{
				if (this._anchor == null)
				{
					return TileObjectData._baseObject.AnchorWall;
				}
				return this._anchor.wall;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchor)
				{
					if (this._anchor.wall == value)
					{
						return;
					}
					this._hasOwnAnchor = true;
					this._anchor = new AnchorDataModule(this._anchor);
				}
				this._anchor.wall = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].AnchorWall = value;
					}
				}
			}
		}

		public int[] AnchorValidTiles
		{
			get
			{
				if (this._anchorTiles == null)
				{
					return TileObjectData._baseObject.AnchorValidTiles;
				}
				return this._anchorTiles.tileValid;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchorTiles)
				{
					if (value.deepCompare(this._anchorTiles.tileValid))
					{
						return;
					}
					this._hasOwnAnchorTiles = true;
					this._anchorTiles = new AnchorTypesModule(this._anchorTiles);
				}
				this._anchorTiles.tileValid = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						int[] anchorValidTiles = value;
						if (value != null)
						{
							anchorValidTiles = (int[])value.Clone();
						}
						this._alternates.data[i].AnchorValidTiles = anchorValidTiles;
					}
				}
			}
		}

		public int[] AnchorInvalidTiles
		{
			get
			{
				if (this._anchorTiles == null)
				{
					return TileObjectData._baseObject.AnchorInvalidTiles;
				}
				return this._anchorTiles.tileInvalid;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchorTiles)
				{
					if (value.deepCompare(this._anchorTiles.tileInvalid))
					{
						return;
					}
					this._hasOwnAnchorTiles = true;
					this._anchorTiles = new AnchorTypesModule(this._anchorTiles);
				}
				this._anchorTiles.tileInvalid = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						int[] anchorInvalidTiles = value;
						if (value != null)
						{
							anchorInvalidTiles = (int[])value.Clone();
						}
						this._alternates.data[i].AnchorInvalidTiles = anchorInvalidTiles;
					}
				}
			}
		}

		public int[] AnchorAlternateTiles
		{
			get
			{
				if (this._anchorTiles == null)
				{
					return TileObjectData._baseObject.AnchorAlternateTiles;
				}
				return this._anchorTiles.tileAlternates;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchorTiles)
				{
					if (value.deepCompare(this._anchorTiles.tileInvalid))
					{
						return;
					}
					this._hasOwnAnchorTiles = true;
					this._anchorTiles = new AnchorTypesModule(this._anchorTiles);
				}
				this._anchorTiles.tileAlternates = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						int[] anchorAlternateTiles = value;
						if (value != null)
						{
							anchorAlternateTiles = (int[])value.Clone();
						}
						this._alternates.data[i].AnchorAlternateTiles = anchorAlternateTiles;
					}
				}
			}
		}

		public int[] AnchorValidWalls
		{
			get
			{
				if (this._anchorTiles == null)
				{
					return TileObjectData._baseObject.AnchorValidWalls;
				}
				return this._anchorTiles.wallValid;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnAnchorTiles)
				{
					this._hasOwnAnchorTiles = true;
					this._anchorTiles = new AnchorTypesModule(this._anchorTiles);
				}
				this._anchorTiles.wallValid = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						int[] anchorValidWalls = value;
						if (value != null)
						{
							anchorValidWalls = (int[])value.Clone();
						}
						this._alternates.data[i].AnchorValidWalls = anchorValidWalls;
					}
				}
			}
		}

		public bool WaterDeath
		{
			get
			{
				if (this._liquidDeath == null)
				{
					return TileObjectData._baseObject.WaterDeath;
				}
				return this._liquidDeath.water;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnLiquidDeath)
				{
					if (this._liquidDeath.water == value)
					{
						return;
					}
					this._hasOwnLiquidDeath = true;
					this._liquidDeath = new LiquidDeathModule(this._liquidDeath);
				}
				this._liquidDeath.water = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].WaterDeath = value;
					}
				}
			}
		}

		public bool LavaDeath
		{
			get
			{
				if (this._liquidDeath == null)
				{
					return TileObjectData._baseObject.LavaDeath;
				}
				return this._liquidDeath.lava;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnLiquidDeath)
				{
					if (this._liquidDeath.lava == value)
					{
						return;
					}
					this._hasOwnLiquidDeath = true;
					this._liquidDeath = new LiquidDeathModule(this._liquidDeath);
				}
				this._liquidDeath.lava = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].LavaDeath = value;
					}
				}
			}
		}

		public LiquidPlacement WaterPlacement
		{
			get
			{
				if (this._liquidPlacement == null)
				{
					return TileObjectData._baseObject.WaterPlacement;
				}
				return this._liquidPlacement.water;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnLiquidPlacement)
				{
					if (this._liquidPlacement.water == value)
					{
						return;
					}
					this._hasOwnLiquidPlacement = true;
					this._liquidPlacement = new LiquidPlacementModule(this._liquidPlacement);
				}
				this._liquidPlacement.water = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].WaterPlacement = value;
					}
				}
			}
		}

		public LiquidPlacement LavaPlacement
		{
			get
			{
				if (this._liquidPlacement == null)
				{
					return TileObjectData._baseObject.LavaPlacement;
				}
				return this._liquidPlacement.lava;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnLiquidPlacement)
				{
					if (this._liquidPlacement.lava == value)
					{
						return;
					}
					this._hasOwnLiquidPlacement = true;
					this._liquidPlacement = new LiquidPlacementModule(this._liquidPlacement);
				}
				this._liquidPlacement.lava = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].LavaPlacement = value;
					}
				}
			}
		}

		public PlacementHook HookCheck
		{
			get
			{
				if (this._placementHooks == null)
				{
					return TileObjectData._baseObject.HookCheck;
				}
				return this._placementHooks.check;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnPlacementHooks)
				{
					this._hasOwnPlacementHooks = true;
					this._placementHooks = new TilePlacementHooksModule(this._placementHooks);
				}
				this._placementHooks.check = value;
			}
		}

		public PlacementHook HookPostPlaceEveryone
		{
			get
			{
				if (this._placementHooks == null)
				{
					return TileObjectData._baseObject.HookPostPlaceEveryone;
				}
				return this._placementHooks.postPlaceEveryone;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnPlacementHooks)
				{
					this._hasOwnPlacementHooks = true;
					this._placementHooks = new TilePlacementHooksModule(this._placementHooks);
				}
				this._placementHooks.postPlaceEveryone = value;
			}
		}

		public PlacementHook HookPostPlaceMyPlayer
		{
			get
			{
				if (this._placementHooks == null)
				{
					return TileObjectData._baseObject.HookPostPlaceMyPlayer;
				}
				return this._placementHooks.postPlaceMyPlayer;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnPlacementHooks)
				{
					this._hasOwnPlacementHooks = true;
					this._placementHooks = new TilePlacementHooksModule(this._placementHooks);
				}
				this._placementHooks.postPlaceMyPlayer = value;
			}
		}

		public PlacementHook HookPlaceOverride
		{
			get
			{
				if (this._placementHooks == null)
				{
					return TileObjectData._baseObject.HookPlaceOverride;
				}
				return this._placementHooks.placeOverride;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnPlacementHooks)
				{
					this._hasOwnPlacementHooks = true;
					this._placementHooks = new TilePlacementHooksModule(this._placementHooks);
				}
				this._placementHooks.placeOverride = value;
			}
		}

		internal List<TileObjectData> SubTiles
		{
			get
			{
				if (this._subTiles == null)
				{
					return TileObjectData._baseObject.SubTiles;
				}
				return this._subTiles.data;
			}
			set
			{
				if (!this._hasOwnSubTiles)
				{
					this._hasOwnSubTiles = true;
					this._subTiles = new TileObjectSubTilesModule(null, null);
				}
				if (value == null)
				{
					this._subTiles.data = null;
					return;
				}
				this._subTiles.data = value;
			}
		}

		public int DrawYOffset
		{
			get
			{
				if (this._tileObjectDraw == null)
				{
					return this.DrawYOffset;
				}
				return this._tileObjectDraw.yOffset;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectDraw)
				{
					if (this._tileObjectDraw.yOffset == value)
					{
						return;
					}
					this._hasOwnTileObjectDraw = true;
					this._tileObjectDraw = new TileObjectDrawModule(this._tileObjectDraw);
				}
				this._tileObjectDraw.yOffset = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].DrawYOffset = value;
					}
				}
			}
		}

		public bool DrawFlipHorizontal
		{
			get
			{
				if (this._tileObjectDraw == null)
				{
					return this.DrawFlipHorizontal;
				}
				return this._tileObjectDraw.flipHorizontal;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectDraw)
				{
					if (this._tileObjectDraw.flipHorizontal == value)
					{
						return;
					}
					this._hasOwnTileObjectDraw = true;
					this._tileObjectDraw = new TileObjectDrawModule(this._tileObjectDraw);
				}
				this._tileObjectDraw.flipHorizontal = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].DrawFlipHorizontal = value;
					}
				}
			}
		}

		public bool DrawFlipVertical
		{
			get
			{
				if (this._tileObjectDraw == null)
				{
					return this.DrawFlipVertical;
				}
				return this._tileObjectDraw.flipVertical;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectDraw)
				{
					if (this._tileObjectDraw.flipVertical == value)
					{
						return;
					}
					this._hasOwnTileObjectDraw = true;
					this._tileObjectDraw = new TileObjectDrawModule(this._tileObjectDraw);
				}
				this._tileObjectDraw.flipVertical = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].DrawFlipVertical = value;
					}
				}
			}
		}

		public int DrawStepDown
		{
			get
			{
				if (this._tileObjectDraw == null)
				{
					return this.DrawStepDown;
				}
				return this._tileObjectDraw.stepDown;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectDraw)
				{
					if (this._tileObjectDraw.stepDown == value)
					{
						return;
					}
					this._hasOwnTileObjectDraw = true;
					this._tileObjectDraw = new TileObjectDrawModule(this._tileObjectDraw);
				}
				this._tileObjectDraw.stepDown = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].DrawStepDown = value;
					}
				}
			}
		}

		public bool StyleHorizontal
		{
			get
			{
				if (this._tileObjectStyle == null)
				{
					return this.StyleHorizontal;
				}
				return this._tileObjectStyle.horizontal;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectStyle)
				{
					if (this._tileObjectStyle.horizontal == value)
					{
						return;
					}
					this._hasOwnTileObjectStyle = true;
					this._tileObjectStyle = new TileObjectStyleModule(this._tileObjectStyle);
				}
				this._tileObjectStyle.horizontal = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].StyleHorizontal = value;
					}
				}
			}
		}

		public int Style
		{
			get
			{
				if (this._tileObjectStyle == null)
				{
					return TileObjectData._baseObject.Style;
				}
				return this._tileObjectStyle.style;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectStyle)
				{
					if (this._tileObjectStyle.style == value)
					{
						return;
					}
					this._hasOwnTileObjectStyle = true;
					this._tileObjectStyle = new TileObjectStyleModule(this._tileObjectStyle);
				}
				this._tileObjectStyle.style = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].Style = value;
					}
				}
			}
		}

		public int StyleWrapLimit
		{
			get
			{
				if (this._tileObjectStyle == null)
				{
					return TileObjectData._baseObject.StyleWrapLimit;
				}
				return this._tileObjectStyle.styleWrapLimit;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectStyle)
				{
					if (this._tileObjectStyle.styleWrapLimit == value)
					{
						return;
					}
					this._hasOwnTileObjectStyle = true;
					this._tileObjectStyle = new TileObjectStyleModule(this._tileObjectStyle);
				}
				this._tileObjectStyle.styleWrapLimit = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].StyleWrapLimit = value;
					}
				}
			}
		}

		public int StyleMultiplier
		{
			get
			{
				if (this._tileObjectStyle == null)
				{
					return TileObjectData._baseObject.StyleMultiplier;
				}
				return this._tileObjectStyle.styleMultiplier;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectStyle)
				{
					if (this._tileObjectStyle.styleMultiplier == value)
					{
						return;
					}
					this._hasOwnTileObjectStyle = true;
					this._tileObjectStyle = new TileObjectStyleModule(this._tileObjectStyle);
				}
				this._tileObjectStyle.styleMultiplier = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].StyleMultiplier = value;
					}
				}
			}
		}

		public int Width
		{
			get
			{
				if (this._tileObjectBase == null)
				{
					return TileObjectData._baseObject.Width;
				}
				return this._tileObjectBase.width;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectBase)
				{
					if (this._tileObjectBase.width == value)
					{
						return;
					}
					this._hasOwnTileObjectBase = true;
					this._tileObjectBase = new TileObjectBaseModule(this._tileObjectBase);
					if (!this._hasOwnTileObjectCoords)
					{
						this._hasOwnTileObjectCoords = true;
						this._tileObjectCoords = new TileObjectCoordinatesModule(this._tileObjectCoords, null);
						this._tileObjectCoords.calculated = false;
					}
				}
				this._tileObjectBase.width = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].Width = value;
					}
				}
			}
		}

		public int Height
		{
			get
			{
				if (this._tileObjectBase == null)
				{
					return TileObjectData._baseObject.Height;
				}
				return this._tileObjectBase.height;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectBase)
				{
					if (this._tileObjectBase.height == value)
					{
						return;
					}
					this._hasOwnTileObjectBase = true;
					this._tileObjectBase = new TileObjectBaseModule(this._tileObjectBase);
					if (!this._hasOwnTileObjectCoords)
					{
						this._hasOwnTileObjectCoords = true;
						this._tileObjectCoords = new TileObjectCoordinatesModule(this._tileObjectCoords, null);
						this._tileObjectCoords.calculated = false;
					}
				}
				this._tileObjectBase.height = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].Height = value;
					}
				}
			}
		}

		public Point16 Origin
		{
			get
			{
				if (this._tileObjectBase == null)
				{
					return TileObjectData._baseObject.Origin;
				}
				return this._tileObjectBase.origin;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectBase)
				{
					if (this._tileObjectBase.origin == value)
					{
						return;
					}
					this._hasOwnTileObjectBase = true;
					this._tileObjectBase = new TileObjectBaseModule(this._tileObjectBase);
				}
				this._tileObjectBase.origin = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].Origin = value;
					}
				}
			}
		}

		public TileObjectDirection Direction
		{
			get
			{
				if (this._tileObjectBase == null)
				{
					return TileObjectData._baseObject.Direction;
				}
				return this._tileObjectBase.direction;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectBase)
				{
					if (this._tileObjectBase.direction == value)
					{
						return;
					}
					this._hasOwnTileObjectBase = true;
					this._tileObjectBase = new TileObjectBaseModule(this._tileObjectBase);
				}
				this._tileObjectBase.direction = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].Direction = value;
					}
				}
			}
		}

		public int RandomStyleRange
		{
			get
			{
				if (this._tileObjectBase == null)
				{
					return TileObjectData._baseObject.RandomStyleRange;
				}
				return this._tileObjectBase.randomRange;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectBase)
				{
					if (this._tileObjectBase.randomRange == value)
					{
						return;
					}
					this._hasOwnTileObjectBase = true;
					this._tileObjectBase = new TileObjectBaseModule(this._tileObjectBase);
				}
				this._tileObjectBase.randomRange = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].RandomStyleRange = value;
					}
				}
			}
		}

		public bool FlattenAnchors
		{
			get
			{
				if (this._tileObjectBase == null)
				{
					return TileObjectData._baseObject.FlattenAnchors;
				}
				return this._tileObjectBase.flattenAnchors;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectBase)
				{
					if (this._tileObjectBase.flattenAnchors == value)
					{
						return;
					}
					this._hasOwnTileObjectBase = true;
					this._tileObjectBase = new TileObjectBaseModule(this._tileObjectBase);
				}
				this._tileObjectBase.flattenAnchors = value;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].FlattenAnchors = value;
					}
				}
			}
		}

		public int[] CoordinateHeights
		{
			get
			{
				if (this._tileObjectCoords == null)
				{
					return TileObjectData._baseObject.CoordinateHeights;
				}
				return this._tileObjectCoords.heights;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectCoords)
				{
					if (value.deepCompare(this._tileObjectCoords.heights))
					{
						return;
					}
					this._hasOwnTileObjectCoords = true;
					this._tileObjectCoords = new TileObjectCoordinatesModule(this._tileObjectCoords, value);
				}
				else
				{
					this._tileObjectCoords.heights = value;
				}
				this._tileObjectCoords.calculated = false;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						int[] coordinateHeights = value;
						if (value != null)
						{
							coordinateHeights = (int[])value.Clone();
						}
						this._alternates.data[i].CoordinateHeights = coordinateHeights;
					}
				}
			}
		}

		public int CoordinateWidth
		{
			get
			{
				if (this._tileObjectCoords == null)
				{
					return TileObjectData._baseObject.CoordinateWidth;
				}
				return this._tileObjectCoords.width;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectCoords)
				{
					if (this._tileObjectCoords.width == value)
					{
						return;
					}
					this._hasOwnTileObjectCoords = true;
					this._tileObjectCoords = new TileObjectCoordinatesModule(this._tileObjectCoords, null);
				}
				this._tileObjectCoords.width = value;
				this._tileObjectCoords.calculated = false;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].CoordinateWidth = value;
					}
				}
			}
		}

		public int CoordinatePadding
		{
			get
			{
				if (this._tileObjectCoords == null)
				{
					return TileObjectData._baseObject.CoordinatePadding;
				}
				return this._tileObjectCoords.padding;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectCoords)
				{
					if (this._tileObjectCoords.padding == value)
					{
						return;
					}
					this._hasOwnTileObjectCoords = true;
					this._tileObjectCoords = new TileObjectCoordinatesModule(this._tileObjectCoords, null);
				}
				this._tileObjectCoords.padding = value;
				this._tileObjectCoords.calculated = false;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].CoordinatePadding = value;
					}
				}
			}
		}

		public Point16 CoordinatePaddingFix
		{
			get
			{
				if (this._tileObjectCoords == null)
				{
					return TileObjectData._baseObject.CoordinatePaddingFix;
				}
				return this._tileObjectCoords.paddingFix;
			}
			set
			{
				this.WriteCheck();
				if (!this._hasOwnTileObjectCoords)
				{
					if (this._tileObjectCoords.paddingFix == value)
					{
						return;
					}
					this._hasOwnTileObjectCoords = true;
					this._tileObjectCoords = new TileObjectCoordinatesModule(this._tileObjectCoords, null);
				}
				this._tileObjectCoords.paddingFix = value;
				this._tileObjectCoords.calculated = false;
				if (this._linkedAlternates)
				{
					for (int i = 0; i < this._alternates.data.Count; i++)
					{
						this._alternates.data[i].CoordinatePaddingFix = value;
					}
				}
			}
		}

		public int CoordinateFullWidth
		{
			get
			{
				if (this._tileObjectCoords == null)
				{
					return TileObjectData._baseObject.CoordinateFullWidth;
				}
				if (!this._tileObjectCoords.calculated)
				{
					this.Calculate();
				}
				return this._tileObjectCoords.styleWidth;
			}
		}

		public int CoordinateFullHeight
		{
			get
			{
				if (this._tileObjectCoords == null)
				{
					return TileObjectData._baseObject.CoordinateFullHeight;
				}
				if (!this._tileObjectCoords.calculated)
				{
					this.Calculate();
				}
				return this._tileObjectCoords.styleHeight;
			}
		}

		public int AlternatesCount
		{
			get
			{
				return this.Alternates.Count;
			}
		}

		public TileObjectData(TileObjectData copyFrom = null)
		{
			this._parent = null;
			this._linkedAlternates = false;
			if (copyFrom == null)
			{
				this._usesCustomCanPlace = false;
				this._alternates = null;
				this._anchor = null;
				this._anchorTiles = null;
				this._tileObjectBase = null;
				this._liquidDeath = null;
				this._liquidPlacement = null;
				this._placementHooks = null;
				this._tileObjectDraw = null;
				this._tileObjectStyle = null;
				this._tileObjectCoords = null;
				return;
			}
			this.CopyFrom(copyFrom);
		}

		public void CopyFrom(TileObjectData copy)
		{
			if (copy == null)
			{
				return;
			}
			this._usesCustomCanPlace = copy._usesCustomCanPlace;
			this._alternates = copy._alternates;
			this._anchor = copy._anchor;
			this._anchorTiles = copy._anchorTiles;
			this._tileObjectBase = copy._tileObjectBase;
			this._liquidDeath = copy._liquidDeath;
			this._liquidPlacement = copy._liquidPlacement;
			this._placementHooks = copy._placementHooks;
			this._tileObjectDraw = copy._tileObjectDraw;
			this._tileObjectStyle = copy._tileObjectStyle;
			this._tileObjectCoords = copy._tileObjectCoords;
		}

		public void FullCopyFrom(ushort tileType)
		{
			this.FullCopyFrom(TileObjectData.GetTileData((int)tileType, 0, 0));
		}

		public void FullCopyFrom(TileObjectData copy)
		{
			if (copy == null)
			{
				return;
			}
			this._usesCustomCanPlace = copy._usesCustomCanPlace;
			this._alternates = copy._alternates;
			this._anchor = copy._anchor;
			this._anchorTiles = copy._anchorTiles;
			this._tileObjectBase = copy._tileObjectBase;
			this._liquidDeath = copy._liquidDeath;
			this._liquidPlacement = copy._liquidPlacement;
			this._placementHooks = copy._placementHooks;
			this._tileObjectDraw = copy._tileObjectDraw;
			this._tileObjectStyle = copy._tileObjectStyle;
			this._tileObjectCoords = copy._tileObjectCoords;
			this._subTiles = new TileObjectSubTilesModule(copy._subTiles, null);
			this._hasOwnSubTiles = true;
		}

		private void SetupBaseObject()
		{
			this._alternates = new TileObjectAlternatesModule(null);
			this._hasOwnAlternates = true;
			this.Alternates = new List<TileObjectData>();
			this._anchor = new AnchorDataModule(null);
			this._hasOwnAnchor = true;
			this.AnchorTop = default(AnchorData);
			this.AnchorBottom = default(AnchorData);
			this.AnchorLeft = default(AnchorData);
			this.AnchorRight = default(AnchorData);
			this.AnchorWall = false;
			this._anchorTiles = new AnchorTypesModule(null);
			this._hasOwnAnchorTiles = true;
			this.AnchorValidTiles = null;
			this.AnchorInvalidTiles = null;
			this.AnchorAlternateTiles = null;
			this.AnchorValidWalls = null;
			this._liquidDeath = new LiquidDeathModule(null);
			this._hasOwnLiquidDeath = true;
			this.WaterDeath = false;
			this.LavaDeath = false;
			this._liquidPlacement = new LiquidPlacementModule(null);
			this._hasOwnLiquidPlacement = true;
			this.WaterPlacement = LiquidPlacement.Allowed;
			this.LavaPlacement = LiquidPlacement.NotAllowed;
			this._placementHooks = new TilePlacementHooksModule(null);
			this._hasOwnPlacementHooks = true;
			this.HookCheck = default(PlacementHook);
			this.HookPostPlaceEveryone = default(PlacementHook);
			this.HookPostPlaceMyPlayer = default(PlacementHook);
			this.HookPlaceOverride = default(PlacementHook);
			this.SubTiles = new List<TileObjectData>(467);
			this._tileObjectBase = new TileObjectBaseModule(null);
			this._hasOwnTileObjectBase = true;
			this.Width = 1;
			this.Height = 1;
			this.Origin = Point16.Zero;
			this.Direction = TileObjectDirection.None;
			this.RandomStyleRange = 0;
			this.FlattenAnchors = false;
			this._tileObjectCoords = new TileObjectCoordinatesModule(null, null);
			this._hasOwnTileObjectCoords = true;
			this.CoordinateHeights = new int[]
			{
				16
			};
			this.CoordinateWidth = 0;
			this.CoordinatePadding = 0;
			this.CoordinatePaddingFix = Point16.Zero;
			this._tileObjectDraw = new TileObjectDrawModule(null);
			this._hasOwnTileObjectDraw = true;
			this.DrawYOffset = 0;
			this.DrawFlipHorizontal = false;
			this.DrawFlipVertical = false;
			this.DrawStepDown = 0;
			this._tileObjectStyle = new TileObjectStyleModule(null);
			this._hasOwnTileObjectStyle = true;
			this.Style = 0;
			this.StyleHorizontal = false;
			this.StyleWrapLimit = 0;
			this.StyleMultiplier = 1;
		}

		private void Calculate()
		{
			if (this._tileObjectCoords.calculated)
			{
				return;
			}
			this._tileObjectCoords.calculated = true;
			this._tileObjectCoords.styleWidth = (this._tileObjectCoords.width + this._tileObjectCoords.padding) * this.Width + (int)this._tileObjectCoords.paddingFix.X;
			int num = 0;
			this._tileObjectCoords.styleHeight = 0;
			for (int i = 0; i < this._tileObjectCoords.heights.Length; i++)
			{
				num += this._tileObjectCoords.heights[i] + this._tileObjectCoords.padding;
			}
			num += (int)this._tileObjectCoords.paddingFix.Y;
			this._tileObjectCoords.styleHeight = num;
			if (this._hasOwnLiquidDeath)
			{
				if (this._liquidDeath.lava)
				{
					this.LavaPlacement = LiquidPlacement.NotAllowed;
				}
				if (this._liquidDeath.water)
				{
					this.WaterPlacement = LiquidPlacement.NotAllowed;
				}
			}
		}

		private void WriteCheck()
		{
			if (TileObjectData.readOnlyData)
			{
				throw new FieldAccessException("Tile data is locked and only accessible during startup.");
			}
		}

		public bool LiquidPlace(Tile checkTile, bool checkStay = false)
		{
			if (checkTile == null)
			{
				return false;
			}
			if (checkTile.liquid > 0)
			{
				switch (checkTile.liquidType())
				{
					case 0:
					case 2:
						if (checkStay && !this.WaterDeath)
						{
							break;
						}
						if (this.WaterPlacement == LiquidPlacement.NotAllowed)
						{
							return false;
						}
						if (this.WaterPlacement == LiquidPlacement.OnlyInFullLiquid && checkTile.liquid != 255)
						{
							return false;
						}
						break;
					case 1:
						if (checkStay && !this.LavaDeath)
						{
							break;
						}
						if (this.LavaPlacement == LiquidPlacement.NotAllowed)
						{
							return false;
						}
						if (this.LavaPlacement == LiquidPlacement.OnlyInFullLiquid && checkTile.liquid != 255)
						{
							return false;
						}
						break;
				}
			}
			else
			{
				switch (checkTile.liquidType())
				{
					case 0:
					case 2:
						if (this.WaterPlacement == LiquidPlacement.OnlyInFullLiquid || this.WaterPlacement == LiquidPlacement.OnlyInLiquid)
						{
							return false;
						}
						break;
					case 1:
						if (this.LavaPlacement == LiquidPlacement.OnlyInFullLiquid || this.LavaPlacement == LiquidPlacement.OnlyInLiquid)
						{
							return false;
						}
						break;
				}
			}
			return true;
		}

		public bool isValidTileAnchor(int type)
		{
			int[] array;
			int[] array2;
			if (this._anchorTiles == null)
			{
				array = null;
				array2 = null;
			}
			else
			{
				array = this._anchorTiles.tileValid;
				array2 = this._anchorTiles.tileInvalid;
			}
			if (array2 != null)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					if (type == array2[i])
					{
						return false;
					}
				}
			}
			if (array == null)
			{
				return true;
			}
			for (int j = 0; j < array.Length; j++)
			{
				if (type == array[j])
				{
					return true;
				}
			}
			return false;
		}

		public bool isValidWallAnchor(int type)
		{
			int[] array;
			if (this._anchorTiles == null)
			{
				array = null;
			}
			else
			{
				array = this._anchorTiles.wallValid;
			}
			if (array == null)
			{
				return type != 0;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (type == array[i])
				{
					return true;
				}
			}
			return false;
		}

		public bool isValidAlternateAnchor(int type)
		{
			if (this._anchorTiles == null)
			{
				return false;
			}
			int[] tileAlternates = this._anchorTiles.tileAlternates;
			if (tileAlternates == null)
			{
				return false;
			}
			for (int i = 0; i < tileAlternates.Length; i++)
			{
				if (type == tileAlternates[i])
				{
					return true;
				}
			}
			return false;
		}

		public int CalculatePlacementStyle(int style, int alternate, int random)
		{
			int num = style * this.StyleMultiplier;
			num += this.Style;
			if (random >= 0)
			{
				num += random;
			}
			return num;
		}

		private static void addBaseTile(out TileObjectData baseTile)
		{
			TileObjectData.newTile.Calculate();
			baseTile = TileObjectData.newTile;
			baseTile._parent = TileObjectData._baseObject;
			TileObjectData.newTile = new TileObjectData(TileObjectData._baseObject);
		}

		public static void addTile(int tileType)
		{
			TileObjectData.newTile.Calculate();
			TileObjectData._data[tileType] = TileObjectData.newTile;
			TileObjectData.newTile = new TileObjectData(TileObjectData._baseObject);
		}

		public static void addSubTile(int style)
		{
			TileObjectData.newSubTile.Calculate();
			List<TileObjectData> list;
			if (!TileObjectData.newTile._hasOwnSubTiles)
			{
				list = new List<TileObjectData>(style + 1);
				TileObjectData.newTile.SubTiles = list;
			}
			else
			{
				list = TileObjectData.newTile.SubTiles;
			}
			if (list.Count <= style)
			{
				for (int i = list.Count; i <= style; i++)
				{
					list.Add(null);
				}
			}
			TileObjectData.newSubTile._parent = TileObjectData.newTile;
			list[style] = TileObjectData.newSubTile;
			TileObjectData.newSubTile = new TileObjectData(TileObjectData._baseObject);
		}

		public static void addAlternate(int baseStyle)
		{
			TileObjectData.newAlternate.Calculate();
			if (!TileObjectData.newTile._hasOwnAlternates)
			{
				TileObjectData.newTile.Alternates = new List<TileObjectData>();
			}
			TileObjectData.newAlternate.Style = baseStyle;
			TileObjectData.newAlternate._parent = TileObjectData.newTile;
			TileObjectData.newTile.Alternates.Add(TileObjectData.newAlternate);
			TileObjectData.newAlternate = new TileObjectData(TileObjectData._baseObject);
		}

		public static void Initialize()
		{
			TileObjectData._baseObject = new TileObjectData(null);
			TileObjectData._baseObject.SetupBaseObject();
			TileObjectData._data = new List<TileObjectData>(467);
			for (int i = 0; i < 467; i++)
			{
				TileObjectData._data.Add(null);
			}
			TileObjectData.newTile = new TileObjectData(TileObjectData._baseObject);
			TileObjectData.newSubTile = new TileObjectData(TileObjectData._baseObject);
			TileObjectData.newAlternate = new TileObjectData(TileObjectData._baseObject);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 27;
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(13);
			TileObjectData.addTile(19);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 27;
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(427);
			for (int j = 435; j <= 439; j++)
			{
				TileObjectData.newTile.CoordinateHeights = new int[]
				{
					16
				};
				TileObjectData.newTile.CoordinateWidth = 16;
				TileObjectData.newTile.CoordinatePadding = 2;
				TileObjectData.newTile.StyleHorizontal = true;
				TileObjectData.newTile.StyleMultiplier = 27;
				TileObjectData.newTile.StyleWrapLimit = 27;
				TileObjectData.newTile.UsesCustomCanPlace = false;
				TileObjectData.newTile.LavaDeath = true;
				TileObjectData.addTile(j);
			}
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 8;
			TileObjectData.newTile.Origin = new Point16(1, 7);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.HookPlaceOverride = new PlacementHook(new Func<int, int, int, int, int, int>(WorldGen.PlaceXmasTree_Direct), -1, 0, true);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.addTile(171);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.EmptyTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				38
			};
			TileObjectData.newTile.CoordinateWidth = 32;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = -20;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawFlipHorizontal = true;
			TileObjectData.addBaseTile(out TileObjectData.StyleDye);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleDye);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleDye);
			TileObjectData arg_3B1_0 = TileObjectData.newSubTile;
			int[] anchorValidWalls = new int[1];
			arg_3B1_0.AnchorValidWalls = anchorValidWalls;
			TileObjectData.addSubTile(3);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleDye);
			TileObjectData arg_3DA_0 = TileObjectData.newSubTile;
			int[] anchorValidWalls2 = new int[1];
			arg_3DA_0.AnchorValidWalls = anchorValidWalls2;
			TileObjectData.addSubTile(4);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleDye);
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.OnlyInFullLiquid;
			TileObjectData.addSubTile(5);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleDye);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				80
			};
			TileObjectData.newSubTile.AnchorLeft = new AnchorData(AnchorType.EmptyTile, 1, 1);
			TileObjectData.newSubTile.AnchorRight = new AnchorData(AnchorType.EmptyTile, 1, 1);
			TileObjectData.addSubTile(6);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleDye);
			TileObjectData.newSubTile.DrawYOffset = -6;
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.newSubTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newSubTile.Width, 0);
			TileObjectData.newSubTile.AnchorBottom = new AnchorData(AnchorType.EmptyTile, TileObjectData.newSubTile.Width, 0);
			TileObjectData.addSubTile(7);
			TileObjectData.addTile(227);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.addTile(10);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 2);
			TileObjectData.addAlternate(0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 1);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 2);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.AnchorBottom = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.addAlternate(1);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(19);
			TileObjectData.addTile(11);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				18,
				16,
				16,
				16,
				18
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			for (int k = 1; k < 5; k++)
			{
				TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
				TileObjectData.newAlternate.Origin = new Point16(0, k);
				TileObjectData.addAlternate(0);
			}
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			for (int l = 1; l < 5; l++)
			{
				TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
				TileObjectData.newAlternate.Origin = new Point16(0, l);
				TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
				TileObjectData.addAlternate(1);
			}
			TileObjectData.addTile(388);
			TileObjectData.newTile.FullCopyFrom(388);
			TileObjectData.addTile(389);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addBaseTile(out TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(13);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(25);
			TileObjectData.addTile(33);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.addTile(49);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.addTile(372);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 5;
			TileObjectData.addTile(50);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(78);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.DrawYOffset = -4;
			TileObjectData.addTile(174);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addBaseTile(out TileObjectData.Style1xX);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(23);
			TileObjectData.addTile(93);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.Height = 6;
			TileObjectData.newTile.Origin = new Point16(0, 5);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.addTile(92);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1xX);
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(453);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style1x2Top);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.addTile(270);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.addTile(271);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(32);
			TileObjectData.addTile(42);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.StyleWrapLimit = 111;
			TileObjectData.addTile(91);
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addBaseTile(out TileObjectData.Style4x2);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(25);
			TileObjectData.addTile(90);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, -2);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(8);
			TileObjectData.addTile(79);
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop, 2, 1);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(209);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addBaseTile(out TileObjectData.StyleSmallCage);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(285);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(286);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(298);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(299);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(310);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(339);
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(3, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style6x3);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(275);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(276);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(413);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(414);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(277);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(278);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(279);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(280);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(281);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(296);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(297);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(309);
			TileObjectData.newTile.Width = 5;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 3);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style5x4);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
			TileObjectData.addTile(464);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
			TileObjectData.addTile(466);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style2x1);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(29);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(103);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(462);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				18
			};
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(14);
			TileObjectData.addTile(18);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				18
			};
			TileObjectData.addTile(16);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(134);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Height, 0);
			TileObjectData.addTile(387);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.addAlternate(3);
			TileObjectData.addTile(443);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addBaseTile(out TileObjectData.Style2xX);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(1, 3);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(207);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				18
			};
			TileObjectData.addTile(410);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(349);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.addTile(337);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorBottom = default(AnchorData);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(465);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.addTile(320);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.addTile(456);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(TETrainingDummy.Hook_AfterPlacement), -1, 0, false);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(378);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleWrapLimit = 55;
			TileObjectData.addTile(105);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Height = 5;
			TileObjectData.newTile.Origin = new Point16(0, 4);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(17);
			TileObjectData.addTile(104);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(128);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.newTile.Origin = new Point16(0, 2);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(269);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addBaseTile(out TileObjectData.Style3x3);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(32);
			TileObjectData.addTile(34);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Origin = new Point16(2, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(454);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style3x2);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newSubTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(13);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newSubTile.Height = 1;
			TileObjectData.newSubTile.Origin = new Point16(1, 0);
			TileObjectData.newSubTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.addSubTile(25);
			TileObjectData.addTile(14);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[]
			{
				127
			};
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(9);
			TileObjectData.addTile(88);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addTile(237);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(244);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.addTile(26);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.addTile(86);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(377);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(15);
			TileObjectData.addTile(87);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(10);
			TileObjectData.addTile(89);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(114);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(186);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(187);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.WaterDeath = false;
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(1);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.WaterDeath = false;
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(4);
			TileObjectData.addTile(215);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(217);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(218);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.addTile(17);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(77);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(133);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.addTile(405);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.addTile(235);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(1, 3);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style3x4);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(4);
			TileObjectData.addTile(101);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.addTile(102);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x4);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(463);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style2x2);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.HookCheck = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.FindEmptyChest), -1, 0, true);
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(Chest.AfterPlacement_Hook), -1, 0, false);
			TileObjectData.newTile.AnchorInvalidTiles = new int[]
			{
				127
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(21);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.AnchorInvalidTiles = new int[]
			{
				127
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(441);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.StyleWrapLimit = 6;
			TileObjectData.newTile.StyleMultiplier = 6;
			TileObjectData.newTile.RandomStyleRange = 6;
			TileObjectData.newTile.AnchorValidTiles = new int[]
			{
				2,
				109
			};
			TileObjectData.addTile(254);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(96);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.RandomStyleRange = 5;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(457);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(139);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.RandomStyleRange = 9;
			TileObjectData.addTile(35);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.addTile(95);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.addTile(126);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.addTile(444);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.addTile(98);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(13);
			TileObjectData.addTile(172);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(94);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(411);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(97);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(99);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(25);
			TileObjectData.addTile(100);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(125);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(173);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(287);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(319);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(287);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(376);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(138);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(142);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(143);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(282);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(288);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(289);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(290);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(291);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(292);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(293);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(294);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(295);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(316);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(317);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(318);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(360);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(358);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.addTile(359);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(361);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(362);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(363);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(364);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(391);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(392);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(393);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSmallCage);
			TileObjectData.addTile(394);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(287);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.addTile(335);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(354);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(355);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
			TileObjectData.addTile(356);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
			TileObjectData.newTile.AnchorLeft = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newTile.AnchorRight = new AnchorData(AnchorType.SolidTile, 1, 1);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.addTile(386);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(2);
			TileObjectData.addTile(132);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(3);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(4);
			TileObjectData.addTile(55);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(3);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(4);
			TileObjectData.addTile(425);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(85);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(TEItemFrame.Hook_AfterPlacement), -1, 0, true);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 0);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(1, 0);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 2, 0);
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(3);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = Point16.Zero;
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.newAlternate.AnchorBottom = AnchorData.Empty;
			TileObjectData.addAlternate(4);
			TileObjectData.addTile(395);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 2);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style3x3);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.addTile(106);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(212);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(219);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(220);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(228);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(231);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(243);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(247);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(283);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(300);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(301);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(302);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(303);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(304);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(305);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(306);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(307);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(308);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.addTile(406);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.addTile(452);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(412);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(455);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style1x2);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleWrapLimit = 2;
			TileObjectData.newTile.StyleMultiplier = 2;
			TileObjectData.newTile.CoordinatePaddingFix = new Point16(0, 2);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(16);
			TileObjectData.addTile(15);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				20
			};
			TileObjectData.addTile(216);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.addTile(390);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
			TileObjectData.addTile(338);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style1x1);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(420);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorAlternateTiles = new int[]
			{
				420,
				419
			};
			TileObjectData.newAlternate.UsesCustomCanPlace = true;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Origin = new Point16(0, 1);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				419
			};
			TileObjectData.addTile(419);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(new Func<int, int, int, int, int, int>(TELogicSensor.Hook_AfterPlacement), -1, 0, true);
			TileObjectData.addTile(423);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(424);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(445);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(429);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.EmptyTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				26
			};
			TileObjectData.newTile.CoordinateWidth = 24;
			TileObjectData.newTile.DrawYOffset = -8;
			TileObjectData.newTile.RandomStyleRange = 6;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(81);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				18
			};
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(135);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				18
			};
			TileObjectData.newTile.CoordinatePadding = 0;
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(428);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.RandomStyleRange = 2;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(141);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(144);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(210);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.addTile(239);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 7;
			TileObjectData.addTile(36);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.StyleMultiplier = 3;
			TileObjectData.newTile.StyleWrapLimit = 3;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.CoordinateWidth = 20;
			TileObjectData.newTile.DrawYOffset = -2;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Table | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(324);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.FlattenAnchors = true;
			TileObjectData.addBaseTile(out TileObjectData.StyleSwitch);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleSwitch);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				124
			};
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				124
			};
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleSwitch);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(3);
			TileObjectData.addTile(136);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.FlattenAnchors = true;
			TileObjectData.newTile.UsesCustomCanPlace = false;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.DrawStepDown = 2;
			TileObjectData.newTile.CoordinateWidth = 20;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleMultiplier = 6;
			TileObjectData.newTile.StyleWrapLimit = 6;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.WaterDeath = true;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.StyleTorch);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				124
			};
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				124
			};
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.StyleTorch);
			TileObjectData.newAlternate.AnchorWall = true;
			TileObjectData.addAlternate(0);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.WaterDeath = false;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(8);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.newTile);
			TileObjectData.newSubTile.LinkedAlternates = true;
			TileObjectData.newSubTile.WaterDeath = false;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(11);
			TileObjectData.addTile(4);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(0, 0);
			TileObjectData.newTile.FlattenAnchors = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.DrawStepDown = 2;
			TileObjectData.newTile.CoordinateWidth = 20;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.WaterDeath = false;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newTile.StyleWrapLimit = 4;
			TileObjectData.newTile.StyleMultiplier = 4;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
			TileObjectData.newAlternate.DrawStepDown = -4;
			TileObjectData.addAlternate(1);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorLeft = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				124
			};
			TileObjectData.addAlternate(2);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorRight = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.Tree | AnchorType.AlternateTile, TileObjectData.newTile.Height, 0);
			TileObjectData.newAlternate.AnchorAlternateTiles = new int[]
			{
				124
			};
			TileObjectData.addAlternate(3);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.addTile(442);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = Point16.Zero;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				20
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.DrawYOffset = -1;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.AlternateTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.addBaseTile(out TileObjectData.StyleAlch);
			TileObjectData.newTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newTile.AnchorValidTiles = new int[]
			{
				2,
				109
			};
			TileObjectData.newTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				60
			};
			TileObjectData.newSubTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.addSubTile(1);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				0,
				59
			};
			TileObjectData.newSubTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.addSubTile(2);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				199,
				203,
				25,
				23
			};
			TileObjectData.newSubTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.addSubTile(3);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				53,
				116
			};
			TileObjectData.newSubTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(4);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				57
			};
			TileObjectData.newSubTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.newSubTile.LavaPlacement = LiquidPlacement.Allowed;
			TileObjectData.newSubTile.LavaDeath = false;
			TileObjectData.addSubTile(5);
			TileObjectData.newSubTile.CopyFrom(TileObjectData.StyleAlch);
			TileObjectData.newSubTile.AnchorValidTiles = new int[]
			{
				147,
				161,
				163,
				164,
				200
			};
			TileObjectData.newSubTile.AnchorAlternateTiles = new int[]
			{
				78
			};
			TileObjectData.newSubTile.WaterPlacement = LiquidPlacement.Allowed;
			TileObjectData.addSubTile(6);
			TileObjectData.addTile(82);
			TileObjectData.newTile.FullCopyFrom(82);
			TileObjectData.addTile(83);
			TileObjectData.newTile.FullCopyFrom(83);
			TileObjectData.addTile(84);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addBaseTile(out TileObjectData.Style3x3Wall);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(240);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(440);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Direction = TileObjectDirection.PlaceLeft;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.LavaDeath = false;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.Direction = TileObjectDirection.PlaceRight;
			TileObjectData.addAlternate(1);
			TileObjectData.addTile(334);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(245);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16
			};
			TileObjectData.addTile(246);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Width = 4;
			TileObjectData.newTile.Height = 3;
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.RandomStyleRange = 9;
			TileObjectData.addTile(241);
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.Width = 6;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(2, 2);
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				16
			};
			TileObjectData.newTile.StyleWrapLimit = 27;
			TileObjectData.addTile(242);
			TileObjectData.newTile.Width = 2;
			TileObjectData.newTile.Height = 4;
			TileObjectData.newTile.Origin = new Point16(0, 3);
			TileObjectData.newTile.AnchorWall = true;
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				16,
				16,
				18
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData arg_4A74_0 = TileObjectData.newTile;
			int[] anchorValidWalls3 = new int[1];
			arg_4A74_0.AnchorValidWalls = anchorValidWalls3;
			TileObjectData.newTile.AnchorValidTiles = new int[]
			{
				2,
				109
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.addTile(27);
			TileObjectData.newTile.Width = 1;
			TileObjectData.newTile.Height = 2;
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[]
			{
				16,
				18
			};
			TileObjectData.newTile.CoordinateWidth = 16;
			TileObjectData.newTile.CoordinatePadding = 2;
			TileObjectData.newTile.AnchorValidTiles = new int[]
			{
				2
			};
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.DrawFlipHorizontal = true;
			TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
			TileObjectData.newTile.LavaDeath = true;
			TileObjectData.newTile.RandomStyleRange = 3;
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				147
			};
			TileObjectData.addAlternate(3);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				60
			};
			TileObjectData.addAlternate(6);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				23
			};
			TileObjectData.addAlternate(9);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				199
			};
			TileObjectData.addAlternate(12);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				109
			};
			TileObjectData.addAlternate(15);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				53
			};
			TileObjectData.addAlternate(18);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				116
			};
			TileObjectData.addAlternate(21);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				234
			};
			TileObjectData.addAlternate(24);
			TileObjectData.newAlternate.CopyFrom(TileObjectData.newTile);
			TileObjectData.newAlternate.AnchorValidTiles = new int[]
			{
				112
			};
			TileObjectData.addAlternate(27);
			TileObjectData.addTile(20);
		}

		public static bool CustomPlace(int type, int style)
		{
			if (type < 0 || type >= TileObjectData._data.Count || style < 0)
			{
				return false;
			}
			TileObjectData tileObjectData = TileObjectData._data[type];
			if (tileObjectData == null)
			{
				return false;
			}
			List<TileObjectData> subTiles = tileObjectData.SubTiles;
			if (subTiles != null && style < subTiles.Count)
			{
				TileObjectData tileObjectData2 = subTiles[style];
				if (tileObjectData2 != null)
				{
					return tileObjectData2._usesCustomCanPlace;
				}
			}
			return tileObjectData._usesCustomCanPlace;
		}

		public static bool CheckLiquidPlacement(int type, int style, Tile checkTile)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData != null)
			{
				return tileData.LiquidPlace(checkTile);
			}
			return TileObjectData.LiquidPlace(type, checkTile);
		}

		public static bool LiquidPlace(int type, Tile checkTile)
		{
			if (checkTile == null)
			{
				return false;
			}
			if (checkTile.liquid > 0)
			{
				switch (checkTile.liquidType())
				{
					case 0:
					case 2:
						if (Main.tileWaterDeath[type])
						{
							return false;
						}
						break;
					case 1:
						if (Main.tileLavaDeath[type])
						{
							return false;
						}
						break;
				}
			}
			return true;
		}

		public static bool CheckWaterDeath(int type, int style)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData == null)
			{
				return Main.tileWaterDeath[type];
			}
			return tileData.WaterDeath;
		}

		public static bool CheckWaterDeath(Tile checkTile)
		{
			if (!checkTile.active())
			{
				return false;
			}
			TileObjectData tileData = TileObjectData.GetTileData(checkTile);
			if (tileData == null)
			{
				return Main.tileWaterDeath[(int)checkTile.type];
			}
			return tileData.WaterDeath;
		}

		public static bool CheckLavaDeath(int type, int style)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData == null)
			{
				return Main.tileLavaDeath[type];
			}
			return tileData.LavaDeath;
		}

		public static bool CheckLavaDeath(Tile checkTile)
		{
			if (!checkTile.active())
			{
				return false;
			}
			TileObjectData tileData = TileObjectData.GetTileData(checkTile);
			if (tileData == null)
			{
				return Main.tileLavaDeath[(int)checkTile.type];
			}
			return tileData.LavaDeath;
		}

		public static int PlatformFrameWidth()
		{
			return TileObjectData._data[19].CoordinateFullWidth;
		}

		public static TileObjectData GetTileData(int type, int style, int alternate = 0)
		{
			if (type < 0 || type >= TileObjectData._data.Count)
			{
				throw new ArgumentOutOfRangeException("Function called with a bad type argument");
			}
			if (style < 0)
			{
				throw new ArgumentOutOfRangeException("Function called with a bad style argument");
			}
			TileObjectData tileObjectData = TileObjectData._data[type];
			if (tileObjectData == null)
			{
				return null;
			}
			List<TileObjectData> subTiles = tileObjectData.SubTiles;
			if (subTiles != null && style < subTiles.Count)
			{
				TileObjectData tileObjectData2 = subTiles[style];
				if (tileObjectData2 != null)
				{
					tileObjectData = tileObjectData2;
				}
			}
			alternate--;
			List<TileObjectData> alternates = tileObjectData.Alternates;
			if (alternates != null && alternate >= 0 && alternate < alternates.Count)
			{
				TileObjectData tileObjectData3 = alternates[alternate];
				if (tileObjectData3 != null)
				{
					tileObjectData = tileObjectData3;
				}
			}
			return tileObjectData;
		}

		public static TileObjectData GetTileData(Tile getTile)
		{
			if (getTile == null || !getTile.active())
			{
				return null;
			}
			int type = (int)getTile.type;
			if (type < 0 || type >= TileObjectData._data.Count)
			{
				throw new ArgumentOutOfRangeException("Function called with a bad tile type");
			}
			TileObjectData tileObjectData = TileObjectData._data[type];
			if (tileObjectData == null)
			{
				return null;
			}
			int num = (int)getTile.frameX / tileObjectData.CoordinateFullWidth;
			int num2 = (int)getTile.frameY / tileObjectData.CoordinateFullHeight;
			int num3 = tileObjectData.StyleWrapLimit;
			if (num3 == 0)
			{
				num3 = 1;
			}
			int num4;
			if (tileObjectData.StyleHorizontal)
			{
				num4 = num2 * num3 + num;
			}
			else
			{
				num4 = num * num3 + num2;
			}
			int num5 = num4 / tileObjectData.StyleMultiplier;
			int num6 = num4 % tileObjectData.StyleMultiplier;
			if (tileObjectData.SubTiles != null && num5 >= 0 && num5 < tileObjectData.SubTiles.Count)
			{
				TileObjectData tileObjectData2 = tileObjectData.SubTiles[num5];
				if (tileObjectData2 != null)
				{
					tileObjectData = tileObjectData2;
				}
			}
			if (tileObjectData._alternates != null)
			{
				for (int i = 0; i < tileObjectData.Alternates.Count; i++)
				{
					TileObjectData tileObjectData3 = tileObjectData.Alternates[i];
					if (tileObjectData3 != null && num6 >= tileObjectData3.Style && num6 <= tileObjectData3.Style + tileObjectData3.RandomStyleRange)
					{
						return tileObjectData3;
					}
				}
			}
			return tileObjectData;
		}

		public static void SyncObjectPlacement(int tileX, int tileY, int type, int style, int dir)
		{
			NetMessage.SendData(17, -1, -1, "", 1, (float)tileX, (float)tileY, (float)type, style, 0, 0);
			TileObjectData.GetTileData(type, style, 0);
		}

		public static bool CallPostPlacementPlayerHook(int tileX, int tileY, int type, int style, int dir, TileObject data)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, data.alternate);
			if (tileData == null || tileData._placementHooks == null || tileData._placementHooks.postPlaceMyPlayer.hook == null)
			{
				return false;
			}
			PlacementHook postPlaceMyPlayer = tileData._placementHooks.postPlaceMyPlayer;
			if (postPlaceMyPlayer.processedCoordinates)
			{
				tileX -= (int)tileData.Origin.X;
				tileY -= (int)tileData.Origin.Y;
			}
			return postPlaceMyPlayer.hook(tileX, tileY, type, style, dir) == postPlaceMyPlayer.badReturn;
		}

		public static void OriginToTopLeft(int type, int style, ref Point16 baseCoords)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style, 0);
			if (tileData == null)
			{
				return;
			}
			baseCoords = new Point16((int)(baseCoords.X - tileData.Origin.X), (int)(baseCoords.Y - tileData.Origin.Y));
		}

		public static int GetTileStyle(Tile getTile)
		{
			if (getTile == null || !getTile.active())
			{
				return -1;
			}
			int type = (int)getTile.type;
			if (type < 0 || type >= TileObjectData._data.Count)
			{
				throw new ArgumentOutOfRangeException("Function called with a bad tile type");
			}
			TileObjectData tileObjectData = TileObjectData._data[type];
			if (tileObjectData == null)
			{
				return -1;
			}
			int num = (int)getTile.frameX / tileObjectData.CoordinateFullWidth;
			int num2 = (int)getTile.frameY / tileObjectData.CoordinateFullHeight;
			int num3 = tileObjectData.StyleWrapLimit;
			if (num3 == 0)
			{
				num3 = 1;
			}
			int num4;
			if (tileObjectData.StyleHorizontal)
			{
				num4 = num2 * num3 + num;
			}
			else
			{
				num4 = num * num3 + num2;
			}
			int num5 = num4 / tileObjectData.StyleMultiplier;
			int num6 = num4 % tileObjectData.StyleMultiplier;
			return num5;
		}

		public static void GetTileInfo(Tile getTile, ref int style, ref int alternate)
		{
			if (getTile == null || !getTile.active())
			{
				return;
			}
			int type = (int)getTile.type;
			if (type < 0 || type >= TileObjectData._data.Count)
			{
				throw new ArgumentOutOfRangeException("Function called with a bad tile type");
			}
			TileObjectData tileObjectData = TileObjectData._data[type];
			if (tileObjectData == null)
			{
				return;
			}
			int num = (int)getTile.frameX / tileObjectData.CoordinateFullWidth;
			int num2 = (int)getTile.frameY / tileObjectData.CoordinateFullHeight;
			int num3 = tileObjectData.StyleWrapLimit;
			if (num3 == 0)
			{
				num3 = 1;
			}
			int num4;
			if (tileObjectData.StyleHorizontal)
			{
				num4 = num2 * num3 + num;
			}
			else
			{
				num4 = num * num3 + num2;
			}
			int num5 = num4 / tileObjectData.StyleMultiplier;
			int num6 = num4 % tileObjectData.StyleMultiplier;
			style = num5;
			if (tileObjectData._alternates != null)
			{
				alternate = num6;
			}
			return;
		}
	}
}
