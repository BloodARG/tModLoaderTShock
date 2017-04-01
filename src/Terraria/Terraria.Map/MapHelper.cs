using Ionic.Zlib;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Terraria.IO;
using Terraria.Social;
using Terraria.Utilities;

namespace Terraria.Map
{
	public static class MapHelper
	{
		private struct OldMapHelper
		{
			public byte misc;

			public byte misc2;

			public bool active()
			{
				return (this.misc & 1) == 1;
			}

			public bool water()
			{
				return (this.misc & 2) == 2;
			}

			public bool lava()
			{
				return (this.misc & 4) == 4;
			}

			public bool honey()
			{
				return (this.misc2 & 64) == 64;
			}

			public bool changed()
			{
				return (this.misc & 8) == 8;
			}

			public bool wall()
			{
				return (this.misc & 16) == 16;
			}

			public byte option()
			{
				byte b = 0;
				if ((this.misc & 32) == 32)
				{
					b += 1;
				}
				if ((this.misc & 64) == 64)
				{
					b += 2;
				}
				if ((this.misc & 128) == 128)
				{
					b += 4;
				}
				if ((this.misc2 & 1) == 1)
				{
					b += 8;
				}
				return b;
			}

			public byte color()
			{
				return (byte)((this.misc2 & 30) >> 1);
			}
		}

		public const int drawLoopMilliseconds = 5;

		private const int HeaderEmpty = 0;

		private const int HeaderTile = 1;

		private const int HeaderWall = 2;

		private const int HeaderWater = 3;

		private const int HeaderLava = 4;

		private const int HeaderHoney = 5;

		private const int HeaderHeavenAndHell = 6;

		private const int HeaderBackground = 7;

		private const int maxTileOptions = 12;

		private const int maxWallOptions = 2;

		private const int maxLiquidTypes = 3;

		private const int maxSkyGradients = 256;

		private const int maxDirtGradients = 256;

		private const int maxRockGradients = 256;

		public static int maxUpdateTile = 1000;

		public static int numUpdateTile = 0;

		public static short[] updateTileX = new short[MapHelper.maxUpdateTile];

		public static short[] updateTileY = new short[MapHelper.maxUpdateTile];

		private static bool saveLock = false;

		private static object padlock = new object();

		public static int[] tileOptionCounts;

		public static int[] wallOptionCounts;

		public static ushort[] tileLookup;

		public static ushort[] wallLookup;

		private static ushort tilePosition;

		private static ushort wallPosition;

		private static ushort liquidPosition;

		private static ushort skyPosition;

		private static ushort dirtPosition;

		private static ushort rockPosition;

		private static ushort hellPosition;

		private static Color[] colorLookup;

		private static ushort[] snowTypes;

		private static ushort wallRangeStart;

		private static ushort wallRangeEnd;

		public static void Initialize()
		{
			Color[][] array = new Color[467][];
			for (int i = 0; i < 467; i++)
			{
				array[i] = new Color[12];
			}
			Color color = new Color(151, 107, 75);
			array[0][0] = color;
			array[5][0] = color;
			array[30][0] = color;
			array[191][0] = color;
			array[272][0] = new Color(121, 119, 101);
			color = new Color(128, 128, 128);
			array[1][0] = color;
			array[38][0] = color;
			array[48][0] = color;
			array[130][0] = color;
			array[138][0] = color;
			array[273][0] = color;
			array[283][0] = color;
			array[2][0] = new Color(28, 216, 94);
			color = new Color(26, 196, 84);
			array[3][0] = color;
			array[192][0] = color;
			array[73][0] = new Color(27, 197, 109);
			array[52][0] = new Color(23, 177, 76);
			array[353][0] = new Color(28, 216, 94);
			array[20][0] = new Color(163, 116, 81);
			array[6][0] = new Color(140, 101, 80);
			color = new Color(150, 67, 22);
			array[7][0] = color;
			array[47][0] = color;
			array[284][0] = color;
			color = new Color(185, 164, 23);
			array[8][0] = color;
			array[45][0] = color;
			color = new Color(185, 194, 195);
			array[9][0] = color;
			array[46][0] = color;
			color = new Color(98, 95, 167);
			array[22][0] = color;
			array[140][0] = color;
			array[23][0] = new Color(141, 137, 223);
			array[24][0] = new Color(122, 116, 218);
			array[25][0] = new Color(109, 90, 128);
			array[37][0] = new Color(104, 86, 84);
			array[39][0] = new Color(181, 62, 59);
			array[40][0] = new Color(146, 81, 68);
			array[41][0] = new Color(66, 84, 109);
			array[43][0] = new Color(84, 100, 63);
			array[44][0] = new Color(107, 68, 99);
			array[53][0] = new Color(186, 168, 84);
			color = new Color(190, 171, 94);
			array[151][0] = color;
			array[154][0] = color;
			array[274][0] = color;
			array[328][0] = new Color(200, 246, 254);
			array[329][0] = new Color(15, 15, 15);
			array[54][0] = new Color(200, 246, 254);
			array[56][0] = new Color(43, 40, 84);
			array[75][0] = new Color(26, 26, 26);
			array[57][0] = new Color(68, 68, 76);
			color = new Color(142, 66, 66);
			array[58][0] = color;
			array[76][0] = color;
			color = new Color(92, 68, 73);
			array[59][0] = color;
			array[120][0] = color;
			array[60][0] = new Color(143, 215, 29);
			array[61][0] = new Color(135, 196, 26);
			array[74][0] = new Color(96, 197, 27);
			array[62][0] = new Color(121, 176, 24);
			array[233][0] = new Color(107, 182, 29);
			array[63][0] = new Color(110, 140, 182);
			array[64][0] = new Color(196, 96, 114);
			array[65][0] = new Color(56, 150, 97);
			array[66][0] = new Color(160, 118, 58);
			array[67][0] = new Color(140, 58, 166);
			array[68][0] = new Color(125, 191, 197);
			array[70][0] = new Color(93, 127, 255);
			color = new Color(182, 175, 130);
			array[71][0] = color;
			array[72][0] = color;
			array[190][0] = color;
			color = new Color(73, 120, 17);
			array[80][0] = color;
			array[188][0] = color;
			color = new Color(11, 80, 143);
			array[107][0] = color;
			array[121][0] = color;
			color = new Color(91, 169, 169);
			array[108][0] = color;
			array[122][0] = color;
			color = new Color(128, 26, 52);
			array[111][0] = color;
			array[150][0] = color;
			array[109][0] = new Color(78, 193, 227);
			array[110][0] = new Color(48, 186, 135);
			array[113][0] = new Color(48, 208, 234);
			array[115][0] = new Color(33, 171, 207);
			array[112][0] = new Color(103, 98, 122);
			color = new Color(238, 225, 218);
			array[116][0] = color;
			array[118][0] = color;
			array[117][0] = new Color(181, 172, 190);
			array[119][0] = new Color(107, 92, 108);
			array[123][0] = new Color(106, 107, 118);
			array[124][0] = new Color(73, 51, 36);
			array[131][0] = new Color(52, 52, 52);
			array[145][0] = new Color(192, 30, 30);
			array[146][0] = new Color(43, 192, 30);
			color = new Color(211, 236, 241);
			array[147][0] = color;
			array[148][0] = color;
			array[152][0] = new Color(128, 133, 184);
			array[153][0] = new Color(239, 141, 126);
			array[155][0] = new Color(131, 162, 161);
			array[156][0] = new Color(170, 171, 157);
			array[157][0] = new Color(104, 100, 126);
			color = new Color(145, 81, 85);
			array[158][0] = color;
			array[232][0] = color;
			array[159][0] = new Color(148, 133, 98);
			array[160][0] = new Color(200, 0, 0);
			array[160][1] = new Color(0, 200, 0);
			array[160][2] = new Color(0, 0, 200);
			array[161][0] = new Color(144, 195, 232);
			array[162][0] = new Color(184, 219, 240);
			array[163][0] = new Color(174, 145, 214);
			array[164][0] = new Color(218, 182, 204);
			array[170][0] = new Color(27, 109, 69);
			array[171][0] = new Color(33, 135, 85);
			color = new Color(129, 125, 93);
			array[166][0] = color;
			array[175][0] = color;
			array[167][0] = new Color(62, 82, 114);
			color = new Color(132, 157, 127);
			array[168][0] = color;
			array[176][0] = color;
			color = new Color(152, 171, 198);
			array[169][0] = color;
			array[177][0] = color;
			array[179][0] = new Color(49, 134, 114);
			array[180][0] = new Color(126, 134, 49);
			array[181][0] = new Color(134, 59, 49);
			array[182][0] = new Color(43, 86, 140);
			array[183][0] = new Color(121, 49, 134);
			array[381][0] = new Color(254, 121, 2);
			array[189][0] = new Color(223, 255, 255);
			array[193][0] = new Color(56, 121, 255);
			array[194][0] = new Color(157, 157, 107);
			array[195][0] = new Color(134, 22, 34);
			array[196][0] = new Color(147, 144, 178);
			array[197][0] = new Color(97, 200, 225);
			array[198][0] = new Color(62, 61, 52);
			array[199][0] = new Color(208, 80, 80);
			array[201][0] = new Color(203, 61, 64);
			array[205][0] = new Color(186, 50, 52);
			array[200][0] = new Color(216, 152, 144);
			array[202][0] = new Color(213, 178, 28);
			array[203][0] = new Color(128, 44, 45);
			array[204][0] = new Color(125, 55, 65);
			array[206][0] = new Color(124, 175, 201);
			array[208][0] = new Color(88, 105, 118);
			array[211][0] = new Color(191, 233, 115);
			array[213][0] = new Color(137, 120, 67);
			array[214][0] = new Color(103, 103, 103);
			array[221][0] = new Color(239, 90, 50);
			array[222][0] = new Color(231, 96, 228);
			array[223][0] = new Color(57, 85, 101);
			array[224][0] = new Color(107, 132, 139);
			array[225][0] = new Color(227, 125, 22);
			array[226][0] = new Color(141, 56, 0);
			array[229][0] = new Color(255, 156, 12);
			array[230][0] = new Color(131, 79, 13);
			array[234][0] = new Color(53, 44, 41);
			array[235][0] = new Color(214, 184, 46);
			array[236][0] = new Color(149, 232, 87);
			array[237][0] = new Color(255, 241, 51);
			array[238][0] = new Color(225, 128, 206);
			array[243][0] = new Color(198, 196, 170);
			array[248][0] = new Color(219, 71, 38);
			array[249][0] = new Color(235, 38, 231);
			array[250][0] = new Color(86, 85, 92);
			array[251][0] = new Color(235, 150, 23);
			array[252][0] = new Color(153, 131, 44);
			array[253][0] = new Color(57, 48, 97);
			array[254][0] = new Color(248, 158, 92);
			array[255][0] = new Color(107, 49, 154);
			array[256][0] = new Color(154, 148, 49);
			array[257][0] = new Color(49, 49, 154);
			array[258][0] = new Color(49, 154, 68);
			array[259][0] = new Color(154, 49, 77);
			array[260][0] = new Color(85, 89, 118);
			array[261][0] = new Color(154, 83, 49);
			array[262][0] = new Color(221, 79, 255);
			array[263][0] = new Color(250, 255, 79);
			array[264][0] = new Color(79, 102, 255);
			array[265][0] = new Color(79, 255, 89);
			array[266][0] = new Color(255, 79, 79);
			array[267][0] = new Color(240, 240, 247);
			array[268][0] = new Color(255, 145, 79);
			array[287][0] = new Color(79, 128, 17);
			color = new Color(122, 217, 232);
			array[275][0] = color;
			array[276][0] = color;
			array[277][0] = color;
			array[278][0] = color;
			array[279][0] = color;
			array[280][0] = color;
			array[281][0] = color;
			array[282][0] = color;
			array[285][0] = color;
			array[286][0] = color;
			array[288][0] = color;
			array[289][0] = color;
			array[290][0] = color;
			array[291][0] = color;
			array[292][0] = color;
			array[293][0] = color;
			array[294][0] = color;
			array[295][0] = color;
			array[296][0] = color;
			array[297][0] = color;
			array[298][0] = color;
			array[299][0] = color;
			array[309][0] = color;
			array[310][0] = color;
			array[413][0] = color;
			array[339][0] = color;
			array[358][0] = color;
			array[359][0] = color;
			array[360][0] = color;
			array[361][0] = color;
			array[362][0] = color;
			array[363][0] = color;
			array[364][0] = color;
			array[391][0] = color;
			array[392][0] = color;
			array[393][0] = color;
			array[394][0] = color;
			array[414][0] = color;
			array[408][0] = new Color(85, 83, 82);
			array[409][0] = new Color(85, 83, 82);
			array[415][0] = new Color(249, 75, 7);
			array[416][0] = new Color(0, 160, 170);
			array[417][0] = new Color(160, 87, 234);
			array[418][0] = new Color(22, 173, 254);
			array[311][0] = new Color(117, 61, 25);
			array[312][0] = new Color(204, 93, 73);
			array[313][0] = new Color(87, 150, 154);
			array[4][0] = new Color(253, 221, 3);
			array[4][1] = new Color(253, 221, 3);
			color = new Color(253, 221, 3);
			array[93][0] = color;
			array[33][0] = color;
			array[174][0] = color;
			array[100][0] = color;
			array[98][0] = color;
			array[173][0] = color;
			color = new Color(119, 105, 79);
			array[11][0] = color;
			array[10][0] = color;
			color = new Color(191, 142, 111);
			array[14][0] = color;
			array[15][0] = color;
			array[18][0] = color;
			array[19][0] = color;
			array[55][0] = color;
			array[79][0] = color;
			array[86][0] = color;
			array[87][0] = color;
			array[88][0] = color;
			array[89][0] = color;
			array[94][0] = color;
			array[101][0] = color;
			array[104][0] = color;
			array[106][0] = color;
			array[114][0] = color;
			array[128][0] = color;
			array[139][0] = color;
			array[172][0] = color;
			array[216][0] = color;
			array[269][0] = color;
			array[334][0] = color;
			array[377][0] = color;
			array[380][0] = color;
			array[395][0] = color;
			array[12][0] = new Color(174, 24, 69);
			array[13][0] = new Color(133, 213, 247);
			color = new Color(144, 148, 144);
			array[17][0] = color;
			array[90][0] = color;
			array[96][0] = color;
			array[97][0] = color;
			array[99][0] = color;
			array[132][0] = color;
			array[142][0] = color;
			array[143][0] = color;
			array[144][0] = color;
			array[207][0] = color;
			array[209][0] = color;
			array[212][0] = color;
			array[217][0] = color;
			array[218][0] = color;
			array[219][0] = color;
			array[220][0] = color;
			array[228][0] = color;
			array[300][0] = color;
			array[301][0] = color;
			array[302][0] = color;
			array[303][0] = color;
			array[304][0] = color;
			array[305][0] = color;
			array[306][0] = color;
			array[307][0] = color;
			array[308][0] = color;
			array[349][0] = new Color(144, 148, 144);
			array[105][0] = new Color(144, 148, 144);
			array[105][1] = new Color(177, 92, 31);
			array[105][2] = new Color(201, 188, 170);
			array[137][0] = new Color(144, 148, 144);
			array[137][1] = new Color(141, 56, 0);
			array[16][0] = new Color(140, 130, 116);
			array[26][0] = new Color(119, 101, 125);
			array[26][1] = new Color(214, 127, 133);
			array[36][0] = new Color(230, 89, 92);
			array[28][0] = new Color(151, 79, 80);
			array[28][1] = new Color(90, 139, 140);
			array[28][2] = new Color(192, 136, 70);
			array[28][3] = new Color(203, 185, 151);
			array[28][4] = new Color(73, 56, 41);
			array[28][5] = new Color(148, 159, 67);
			array[28][6] = new Color(138, 172, 67);
			array[28][7] = new Color(226, 122, 47);
			array[28][8] = new Color(198, 87, 93);
			array[29][0] = new Color(175, 105, 128);
			array[51][0] = new Color(192, 202, 203);
			array[31][0] = new Color(141, 120, 168);
			array[31][1] = new Color(212, 105, 105);
			array[32][0] = new Color(151, 135, 183);
			array[42][0] = new Color(251, 235, 127);
			array[50][0] = new Color(170, 48, 114);
			array[85][0] = new Color(192, 192, 192);
			array[69][0] = new Color(190, 150, 92);
			array[77][0] = new Color(238, 85, 70);
			array[81][0] = new Color(245, 133, 191);
			array[78][0] = new Color(121, 110, 97);
			array[141][0] = new Color(192, 59, 59);
			array[129][0] = new Color(255, 117, 224);
			array[126][0] = new Color(159, 209, 229);
			array[125][0] = new Color(141, 175, 255);
			array[103][0] = new Color(141, 98, 77);
			array[95][0] = new Color(255, 162, 31);
			array[92][0] = new Color(213, 229, 237);
			array[91][0] = new Color(13, 88, 130);
			array[215][0] = new Color(254, 121, 2);
			array[316][0] = new Color(157, 176, 226);
			array[317][0] = new Color(118, 227, 129);
			array[318][0] = new Color(227, 118, 215);
			array[319][0] = new Color(96, 68, 48);
			array[320][0] = new Color(203, 185, 151);
			array[321][0] = new Color(96, 77, 64);
			array[322][0] = new Color(198, 170, 104);
			array[149][0] = new Color(220, 50, 50);
			array[149][1] = new Color(0, 220, 50);
			array[149][2] = new Color(50, 50, 220);
			array[133][0] = new Color(231, 53, 56);
			array[133][1] = new Color(192, 189, 221);
			array[134][0] = new Color(166, 187, 153);
			array[134][1] = new Color(241, 129, 249);
			array[102][0] = new Color(229, 212, 73);
			array[49][0] = new Color(89, 201, 255);
			array[35][0] = new Color(226, 145, 30);
			array[34][0] = new Color(235, 166, 135);
			array[136][0] = new Color(213, 203, 204);
			array[231][0] = new Color(224, 194, 101);
			array[239][0] = new Color(224, 194, 101);
			array[240][0] = new Color(120, 85, 60);
			array[240][1] = new Color(99, 50, 30);
			array[240][2] = new Color(153, 153, 117);
			array[240][3] = new Color(112, 84, 56);
			array[240][4] = new Color(234, 231, 226);
			array[241][0] = new Color(77, 74, 72);
			array[244][0] = new Color(200, 245, 253);
			color = new Color(99, 50, 30);
			array[242][0] = color;
			array[245][0] = color;
			array[246][0] = color;
			array[242][1] = new Color(185, 142, 97);
			array[247][0] = new Color(140, 150, 150);
			array[271][0] = new Color(107, 250, 255);
			array[270][0] = new Color(187, 255, 107);
			array[314][0] = new Color(181, 164, 125);
			array[324][0] = new Color(228, 213, 173);
			array[351][0] = new Color(31, 31, 31);
			array[424][0] = new Color(146, 155, 187);
			array[429][0] = new Color(220, 220, 220);
			array[445][0] = new Color(240, 240, 240);
			array[21][0] = new Color(174, 129, 92);
			array[21][1] = new Color(233, 207, 94);
			array[21][2] = new Color(137, 128, 200);
			array[21][3] = new Color(160, 160, 160);
			array[21][4] = new Color(106, 210, 255);
			array[441][0] = new Color(174, 129, 92);
			array[441][1] = new Color(233, 207, 94);
			array[441][2] = new Color(137, 128, 200);
			array[441][3] = new Color(160, 160, 160);
			array[441][4] = new Color(106, 210, 255);
			array[27][0] = new Color(54, 154, 54);
			array[27][1] = new Color(226, 196, 49);
			color = new Color(246, 197, 26);
			array[82][0] = color;
			array[83][0] = color;
			array[84][0] = color;
			color = new Color(76, 150, 216);
			array[82][1] = color;
			array[83][1] = color;
			array[84][1] = color;
			color = new Color(185, 214, 42);
			array[82][2] = color;
			array[83][2] = color;
			array[84][2] = color;
			color = new Color(167, 203, 37);
			array[82][3] = color;
			array[83][3] = color;
			array[84][3] = color;
			color = new Color(72, 145, 125);
			array[82][4] = color;
			array[83][4] = color;
			array[84][4] = color;
			color = new Color(177, 69, 49);
			array[82][5] = color;
			array[83][5] = color;
			array[84][5] = color;
			color = new Color(40, 152, 240);
			array[82][6] = color;
			array[83][6] = color;
			array[84][6] = color;
			array[165][0] = new Color(115, 173, 229);
			array[165][1] = new Color(100, 100, 100);
			array[165][2] = new Color(152, 152, 152);
			array[165][3] = new Color(227, 125, 22);
			array[178][0] = new Color(208, 94, 201);
			array[178][1] = new Color(233, 146, 69);
			array[178][2] = new Color(71, 146, 251);
			array[178][3] = new Color(60, 226, 133);
			array[178][4] = new Color(250, 30, 71);
			array[178][5] = new Color(166, 176, 204);
			array[178][6] = new Color(255, 217, 120);
			array[184][0] = new Color(29, 106, 88);
			array[184][1] = new Color(94, 100, 36);
			array[184][2] = new Color(96, 44, 40);
			array[184][3] = new Color(34, 63, 102);
			array[184][4] = new Color(79, 35, 95);
			array[184][5] = new Color(253, 62, 3);
			color = new Color(99, 99, 99);
			array[185][0] = color;
			array[186][0] = color;
			array[187][0] = color;
			color = new Color(114, 81, 56);
			array[185][1] = color;
			array[186][1] = color;
			array[187][1] = color;
			color = new Color(133, 133, 101);
			array[185][2] = color;
			array[186][2] = color;
			array[187][2] = color;
			color = new Color(151, 200, 211);
			array[185][3] = color;
			array[186][3] = color;
			array[187][3] = color;
			color = new Color(177, 183, 161);
			array[185][4] = color;
			array[186][4] = color;
			array[187][4] = color;
			color = new Color(134, 114, 38);
			array[185][5] = color;
			array[186][5] = color;
			array[187][5] = color;
			color = new Color(82, 62, 66);
			array[185][6] = color;
			array[186][6] = color;
			array[187][6] = color;
			color = new Color(143, 117, 121);
			array[185][7] = color;
			array[186][7] = color;
			array[187][7] = color;
			color = new Color(177, 92, 31);
			array[185][8] = color;
			array[186][8] = color;
			array[187][8] = color;
			color = new Color(85, 73, 87);
			array[185][9] = color;
			array[186][9] = color;
			array[187][9] = color;
			array[227][0] = new Color(74, 197, 155);
			array[227][1] = new Color(54, 153, 88);
			array[227][2] = new Color(63, 126, 207);
			array[227][3] = new Color(240, 180, 4);
			array[227][4] = new Color(45, 68, 168);
			array[227][5] = new Color(61, 92, 0);
			array[227][6] = new Color(216, 112, 152);
			array[227][7] = new Color(200, 40, 24);
			array[227][8] = new Color(113, 45, 133);
			array[227][9] = new Color(235, 137, 2);
			array[227][10] = new Color(41, 152, 135);
			array[227][11] = new Color(198, 19, 78);
			array[373][0] = new Color(9, 61, 191);
			array[374][0] = new Color(253, 32, 3);
			array[375][0] = new Color(255, 156, 12);
			array[461][0] = new Color(255, 222, 100);
			array[323][0] = new Color(182, 141, 86);
			array[325][0] = new Color(129, 125, 93);
			array[326][0] = new Color(9, 61, 191);
			array[327][0] = new Color(253, 32, 3);
			array[330][0] = new Color(226, 118, 76);
			array[331][0] = new Color(161, 172, 173);
			array[332][0] = new Color(204, 181, 72);
			array[333][0] = new Color(190, 190, 178);
			array[335][0] = new Color(217, 174, 137);
			array[336][0] = new Color(253, 62, 3);
			array[337][0] = new Color(144, 148, 144);
			array[338][0] = new Color(85, 255, 160);
			array[315][0] = new Color(235, 114, 80);
			array[340][0] = new Color(96, 248, 2);
			array[341][0] = new Color(105, 74, 202);
			array[342][0] = new Color(29, 240, 255);
			array[343][0] = new Color(254, 202, 80);
			array[344][0] = new Color(131, 252, 245);
			array[345][0] = new Color(255, 156, 12);
			array[346][0] = new Color(149, 212, 89);
			array[347][0] = new Color(236, 74, 79);
			array[348][0] = new Color(44, 26, 233);
			array[350][0] = new Color(55, 97, 155);
			array[352][0] = new Color(238, 97, 94);
			array[354][0] = new Color(141, 107, 89);
			array[355][0] = new Color(141, 107, 89);
			array[463][0] = new Color(155, 214, 240);
			array[464][0] = new Color(233, 183, 128);
			array[465][0] = new Color(51, 84, 195);
			array[466][0] = new Color(205, 153, 73);
			array[356][0] = new Color(233, 203, 24);
			array[357][0] = new Color(168, 178, 204);
			array[367][0] = new Color(168, 178, 204);
			array[365][0] = new Color(146, 136, 205);
			array[366][0] = new Color(223, 232, 233);
			array[368][0] = new Color(50, 46, 104);
			array[369][0] = new Color(50, 46, 104);
			array[370][0] = new Color(127, 116, 194);
			array[372][0] = new Color(252, 128, 201);
			array[371][0] = new Color(249, 101, 189);
			array[376][0] = new Color(160, 120, 92);
			array[378][0] = new Color(160, 120, 100);
			array[379][0] = new Color(251, 209, 240);
			array[382][0] = new Color(28, 216, 94);
			array[383][0] = new Color(221, 136, 144);
			array[384][0] = new Color(131, 206, 12);
			array[385][0] = new Color(87, 21, 144);
			array[386][0] = new Color(127, 92, 69);
			array[387][0] = new Color(127, 92, 69);
			array[388][0] = new Color(127, 92, 69);
			array[389][0] = new Color(127, 92, 69);
			array[390][0] = new Color(253, 32, 3);
			array[397][0] = new Color(212, 192, 100);
			array[396][0] = new Color(198, 124, 78);
			array[398][0] = new Color(100, 82, 126);
			array[399][0] = new Color(77, 76, 66);
			array[400][0] = new Color(96, 68, 117);
			array[401][0] = new Color(68, 60, 51);
			array[402][0] = new Color(174, 168, 186);
			array[403][0] = new Color(205, 152, 186);
			array[404][0] = new Color(140, 84, 60);
			array[405][0] = new Color(140, 140, 140);
			array[406][0] = new Color(120, 120, 120);
			array[407][0] = new Color(255, 227, 132);
			array[411][0] = new Color(227, 46, 46);
			array[421][0] = new Color(65, 75, 90);
			array[422][0] = new Color(65, 75, 90);
			array[425][0] = new Color(146, 155, 187);
			array[426][0] = new Color(168, 38, 47);
			array[430][0] = new Color(39, 168, 96);
			array[431][0] = new Color(39, 94, 168);
			array[432][0] = new Color(242, 221, 100);
			array[433][0] = new Color(224, 100, 242);
			array[434][0] = new Color(197, 193, 216);
			array[427][0] = new Color(183, 53, 62);
			array[435][0] = new Color(54, 183, 111);
			array[436][0] = new Color(54, 109, 183);
			array[437][0] = new Color(255, 236, 115);
			array[438][0] = new Color(239, 115, 255);
			array[439][0] = new Color(212, 208, 231);
			array[440][0] = new Color(238, 51, 53);
			array[440][1] = new Color(13, 107, 216);
			array[440][2] = new Color(33, 184, 115);
			array[440][3] = new Color(255, 221, 62);
			array[440][4] = new Color(165, 0, 236);
			array[440][5] = new Color(223, 230, 238);
			array[440][6] = new Color(207, 101, 0);
			array[419][0] = new Color(88, 95, 114);
			array[419][1] = new Color(214, 225, 236);
			array[419][2] = new Color(25, 131, 205);
			array[423][0] = new Color(245, 197, 1);
			array[423][1] = new Color(185, 0, 224);
			array[423][2] = new Color(58, 240, 111);
			array[423][3] = new Color(50, 107, 197);
			array[423][4] = new Color(253, 91, 3);
			array[423][5] = new Color(254, 194, 20);
			array[423][6] = new Color(174, 195, 215);
			array[420][0] = new Color(99, 255, 107);
			array[420][1] = new Color(99, 255, 107);
			array[420][4] = new Color(99, 255, 107);
			array[420][2] = new Color(218, 2, 5);
			array[420][3] = new Color(218, 2, 5);
			array[420][5] = new Color(218, 2, 5);
			array[410][0] = new Color(75, 139, 166);
			array[412][0] = new Color(75, 139, 166);
			array[443][0] = new Color(144, 148, 144);
			array[442][0] = new Color(3, 144, 201);
			array[444][0] = new Color(191, 176, 124);
			array[446][0] = new Color(255, 66, 152);
			array[447][0] = new Color(179, 132, 255);
			array[448][0] = new Color(0, 206, 180);
			array[449][0] = new Color(91, 186, 240);
			array[450][0] = new Color(92, 240, 91);
			array[451][0] = new Color(240, 91, 147);
			array[452][0] = new Color(255, 150, 181);
			array[453][0] = new Color(179, 132, 255);
			array[453][1] = new Color(0, 206, 180);
			array[453][2] = new Color(255, 66, 152);
			array[454][0] = new Color(174, 16, 176);
			array[455][0] = new Color(48, 225, 110);
			array[456][0] = new Color(179, 132, 255);
			array[457][0] = new Color(150, 164, 206);
			array[457][1] = new Color(255, 132, 184);
			array[457][2] = new Color(74, 255, 232);
			array[457][3] = new Color(215, 159, 255);
			array[457][4] = new Color(229, 219, 234);
			array[458][0] = new Color(211, 198, 111);
			array[459][0] = new Color(190, 223, 232);
			array[460][0] = new Color(141, 163, 181);
			array[462][0] = new Color(231, 178, 28);
			Color[] array2 = new Color[]
			{
				new Color(9, 61, 191),
				new Color(253, 32, 3),
				new Color(254, 194, 20)
			};
			Color[][] array3 = new Color[231][];
			for (int j = 0; j < 231; j++)
			{
				array3[j] = new Color[2];
			}
			array3[158][0] = new Color(107, 49, 154);
			array3[163][0] = new Color(154, 148, 49);
			array3[162][0] = new Color(49, 49, 154);
			array3[160][0] = new Color(49, 154, 68);
			array3[161][0] = new Color(154, 49, 77);
			array3[159][0] = new Color(85, 89, 118);
			array3[157][0] = new Color(154, 83, 49);
			array3[154][0] = new Color(221, 79, 255);
			array3[166][0] = new Color(250, 255, 79);
			array3[165][0] = new Color(79, 102, 255);
			array3[156][0] = new Color(79, 255, 89);
			array3[164][0] = new Color(255, 79, 79);
			array3[155][0] = new Color(240, 240, 247);
			array3[153][0] = new Color(255, 145, 79);
			array3[169][0] = new Color(5, 5, 5);
			array3[224][0] = new Color(57, 55, 52);
			array3[225][0] = new Color(68, 68, 68);
			array3[226][0] = new Color(148, 138, 74);
			array3[227][0] = new Color(95, 137, 191);
			array3[170][0] = new Color(59, 39, 22);
			array3[171][0] = new Color(59, 39, 22);
			color = new Color(52, 52, 52);
			array3[1][0] = color;
			array3[53][0] = color;
			array3[52][0] = color;
			array3[51][0] = color;
			array3[50][0] = color;
			array3[49][0] = color;
			array3[48][0] = color;
			array3[44][0] = color;
			array3[5][0] = color;
			color = new Color(88, 61, 46);
			array3[2][0] = color;
			array3[16][0] = color;
			array3[59][0] = color;
			array3[3][0] = new Color(61, 58, 78);
			array3[4][0] = new Color(73, 51, 36);
			array3[6][0] = new Color(91, 30, 30);
			color = new Color(27, 31, 42);
			array3[7][0] = color;
			array3[17][0] = color;
			color = new Color(32, 40, 45);
			array3[94][0] = color;
			array3[100][0] = color;
			color = new Color(44, 41, 50);
			array3[95][0] = color;
			array3[101][0] = color;
			color = new Color(31, 39, 26);
			array3[8][0] = color;
			array3[18][0] = color;
			color = new Color(36, 45, 44);
			array3[98][0] = color;
			array3[104][0] = color;
			color = new Color(38, 49, 50);
			array3[99][0] = color;
			array3[105][0] = color;
			color = new Color(41, 28, 36);
			array3[9][0] = color;
			array3[19][0] = color;
			color = new Color(72, 50, 77);
			array3[96][0] = color;
			array3[102][0] = color;
			color = new Color(78, 50, 69);
			array3[97][0] = color;
			array3[103][0] = color;
			array3[10][0] = new Color(74, 62, 12);
			array3[11][0] = new Color(46, 56, 59);
			array3[12][0] = new Color(75, 32, 11);
			array3[13][0] = new Color(67, 37, 37);
			color = new Color(15, 15, 15);
			array3[14][0] = color;
			array3[20][0] = color;
			array3[15][0] = new Color(52, 43, 45);
			array3[22][0] = new Color(113, 99, 99);
			array3[23][0] = new Color(38, 38, 43);
			array3[24][0] = new Color(53, 39, 41);
			array3[25][0] = new Color(11, 35, 62);
			array3[26][0] = new Color(21, 63, 70);
			array3[27][0] = new Color(88, 61, 46);
			array3[27][1] = new Color(52, 52, 52);
			array3[28][0] = new Color(81, 84, 101);
			array3[29][0] = new Color(88, 23, 23);
			array3[30][0] = new Color(28, 88, 23);
			array3[31][0] = new Color(78, 87, 99);
			color = new Color(69, 67, 41);
			array3[34][0] = color;
			array3[37][0] = color;
			array3[32][0] = new Color(86, 17, 40);
			array3[33][0] = new Color(49, 47, 83);
			array3[35][0] = new Color(51, 51, 70);
			array3[36][0] = new Color(87, 59, 55);
			array3[38][0] = new Color(49, 57, 49);
			array3[39][0] = new Color(78, 79, 73);
			array3[45][0] = new Color(60, 59, 51);
			array3[46][0] = new Color(48, 57, 47);
			array3[47][0] = new Color(71, 77, 85);
			array3[40][0] = new Color(85, 102, 103);
			array3[41][0] = new Color(52, 50, 62);
			array3[42][0] = new Color(71, 42, 44);
			array3[43][0] = new Color(73, 66, 50);
			array3[54][0] = new Color(40, 56, 50);
			array3[55][0] = new Color(49, 48, 36);
			array3[56][0] = new Color(43, 33, 32);
			array3[57][0] = new Color(31, 40, 49);
			array3[58][0] = new Color(48, 35, 52);
			array3[60][0] = new Color(1, 52, 20);
			array3[61][0] = new Color(55, 39, 26);
			array3[62][0] = new Color(39, 33, 26);
			array3[69][0] = new Color(43, 42, 68);
			array3[70][0] = new Color(30, 70, 80);
			color = new Color(30, 80, 48);
			array3[63][0] = color;
			array3[65][0] = color;
			array3[66][0] = color;
			array3[68][0] = color;
			color = new Color(53, 80, 30);
			array3[64][0] = color;
			array3[67][0] = color;
			array3[78][0] = new Color(63, 39, 26);
			array3[71][0] = new Color(78, 105, 135);
			array3[72][0] = new Color(52, 84, 12);
			array3[73][0] = new Color(190, 204, 223);
			color = new Color(64, 62, 80);
			array3[74][0] = color;
			array3[80][0] = color;
			array3[75][0] = new Color(65, 65, 35);
			array3[76][0] = new Color(20, 46, 104);
			array3[77][0] = new Color(61, 13, 16);
			array3[79][0] = new Color(51, 47, 96);
			array3[81][0] = new Color(101, 51, 51);
			array3[82][0] = new Color(77, 64, 34);
			array3[83][0] = new Color(62, 38, 41);
			array3[84][0] = new Color(48, 78, 93);
			array3[85][0] = new Color(54, 63, 69);
			color = new Color(138, 73, 38);
			array3[86][0] = color;
			array3[108][0] = color;
			color = new Color(50, 15, 8);
			array3[87][0] = color;
			array3[112][0] = color;
			array3[109][0] = new Color(94, 25, 17);
			array3[110][0] = new Color(125, 36, 122);
			array3[111][0] = new Color(51, 35, 27);
			array3[113][0] = new Color(135, 58, 0);
			array3[114][0] = new Color(65, 52, 15);
			array3[115][0] = new Color(39, 42, 51);
			array3[116][0] = new Color(89, 26, 27);
			array3[117][0] = new Color(126, 123, 115);
			array3[118][0] = new Color(8, 50, 19);
			array3[119][0] = new Color(95, 21, 24);
			array3[120][0] = new Color(17, 31, 65);
			array3[121][0] = new Color(192, 173, 143);
			array3[122][0] = new Color(114, 114, 131);
			array3[123][0] = new Color(136, 119, 7);
			array3[124][0] = new Color(8, 72, 3);
			array3[125][0] = new Color(117, 132, 82);
			array3[126][0] = new Color(100, 102, 114);
			array3[127][0] = new Color(30, 118, 226);
			array3[128][0] = new Color(93, 6, 102);
			array3[129][0] = new Color(64, 40, 169);
			array3[130][0] = new Color(39, 34, 180);
			array3[131][0] = new Color(87, 94, 125);
			array3[132][0] = new Color(6, 6, 6);
			array3[133][0] = new Color(69, 72, 186);
			array3[134][0] = new Color(130, 62, 16);
			array3[135][0] = new Color(22, 123, 163);
			array3[136][0] = new Color(40, 86, 151);
			array3[137][0] = new Color(183, 75, 15);
			array3[138][0] = new Color(83, 80, 100);
			array3[139][0] = new Color(115, 65, 68);
			array3[140][0] = new Color(119, 108, 81);
			array3[141][0] = new Color(59, 67, 71);
			array3[142][0] = new Color(17, 172, 143);
			array3[143][0] = new Color(90, 112, 105);
			array3[144][0] = new Color(62, 28, 87);
			array3[146][0] = new Color(120, 59, 19);
			array3[147][0] = new Color(59, 59, 59);
			array3[148][0] = new Color(229, 218, 161);
			array3[149][0] = new Color(73, 59, 50);
			array3[151][0] = new Color(102, 75, 34);
			array3[167][0] = new Color(70, 68, 51);
			array3[172][0] = new Color(163, 96, 0);
			array3[173][0] = new Color(94, 163, 46);
			array3[174][0] = new Color(117, 32, 59);
			array3[175][0] = new Color(20, 11, 203);
			array3[176][0] = new Color(74, 69, 88);
			array3[177][0] = new Color(60, 30, 30);
			array3[183][0] = new Color(111, 117, 135);
			array3[179][0] = new Color(111, 117, 135);
			array3[178][0] = new Color(111, 117, 135);
			array3[184][0] = new Color(25, 23, 54);
			array3[181][0] = new Color(25, 23, 54);
			array3[180][0] = new Color(25, 23, 54);
			array3[182][0] = new Color(74, 71, 129);
			array3[185][0] = new Color(52, 52, 52);
			array3[186][0] = new Color(38, 9, 66);
			array3[216][0] = new Color(158, 100, 64);
			array3[217][0] = new Color(62, 45, 75);
			array3[218][0] = new Color(57, 14, 12);
			array3[219][0] = new Color(96, 72, 133);
			array3[187][0] = new Color(149, 80, 51);
			array3[220][0] = new Color(67, 55, 80);
			array3[221][0] = new Color(64, 37, 29);
			array3[222][0] = new Color(70, 51, 91);
			array3[188][0] = new Color(82, 63, 80);
			array3[189][0] = new Color(65, 61, 77);
			array3[190][0] = new Color(64, 65, 92);
			array3[191][0] = new Color(76, 53, 84);
			array3[192][0] = new Color(144, 67, 52);
			array3[193][0] = new Color(149, 48, 48);
			array3[194][0] = new Color(111, 32, 36);
			array3[195][0] = new Color(147, 48, 55);
			array3[196][0] = new Color(97, 67, 51);
			array3[197][0] = new Color(112, 80, 62);
			array3[198][0] = new Color(88, 61, 46);
			array3[199][0] = new Color(127, 94, 76);
			array3[200][0] = new Color(143, 50, 123);
			array3[201][0] = new Color(136, 120, 131);
			array3[202][0] = new Color(219, 92, 143);
			array3[203][0] = new Color(113, 64, 150);
			array3[204][0] = new Color(74, 67, 60);
			array3[205][0] = new Color(60, 78, 59);
			array3[206][0] = new Color(0, 54, 21);
			array3[207][0] = new Color(74, 97, 72);
			array3[208][0] = new Color(40, 37, 35);
			array3[209][0] = new Color(77, 63, 66);
			array3[210][0] = new Color(111, 6, 6);
			array3[211][0] = new Color(88, 67, 59);
			array3[212][0] = new Color(88, 87, 80);
			array3[213][0] = new Color(71, 71, 67);
			array3[214][0] = new Color(76, 52, 60);
			array3[215][0] = new Color(89, 48, 59);
			array3[223][0] = new Color(51, 18, 4);
			array3[228][0] = new Color(160, 2, 75);
			array3[229][0] = new Color(100, 55, 164);
			array3[230][0] = new Color(0, 117, 101);
			Color[] array4 = new Color[256];
			Color color2 = new Color(50, 40, 255);
			Color color3 = new Color(145, 185, 255);
			for (int k = 0; k < array4.Length; k++)
			{
				float num = (float)k / (float)array4.Length;
				float num2 = 1f - num;
				array4[k] = new Color((int)((byte)((float)color2.R * num2 + (float)color3.R * num)), (int)((byte)((float)color2.G * num2 + (float)color3.G * num)), (int)((byte)((float)color2.B * num2 + (float)color3.B * num)));
			}
			Color[] array5 = new Color[256];
			Color color4 = new Color(88, 61, 46);
			Color color5 = new Color(37, 78, 123);
			for (int l = 0; l < array5.Length; l++)
			{
				float num3 = (float)l / 255f;
				float num4 = 1f - num3;
				array5[l] = new Color((int)((byte)((float)color4.R * num4 + (float)color5.R * num3)), (int)((byte)((float)color4.G * num4 + (float)color5.G * num3)), (int)((byte)((float)color4.B * num4 + (float)color5.B * num3)));
			}
			Color[] array6 = new Color[256];
			Color color6 = new Color(74, 67, 60);
			color5 = new Color(53, 70, 97);
			for (int m = 0; m < array6.Length; m++)
			{
				float num5 = (float)m / 255f;
				float num6 = 1f - num5;
				array6[m] = new Color((int)((byte)((float)color6.R * num6 + (float)color5.R * num5)), (int)((byte)((float)color6.G * num6 + (float)color5.G * num5)), (int)((byte)((float)color6.B * num6 + (float)color5.B * num5)));
			}
			Color color7 = new Color(50, 44, 38);
			int num7 = 0;
			MapHelper.tileOptionCounts = new int[467];
			for (int n = 0; n < 467; n++)
			{
				Color[] array7 = array[n];
				int num8 = 0;
				while (num8 < 12 && !(array7[num8] == Color.Transparent))
				{
					num8++;
				}
				MapHelper.tileOptionCounts[n] = num8;
				num7 += num8;
			}
			MapHelper.wallOptionCounts = new int[231];
			for (int num9 = 0; num9 < 231; num9++)
			{
				Color[] array8 = array3[num9];
				int num10 = 0;
				while (num10 < 2 && !(array8[num10] == Color.Transparent))
				{
					num10++;
				}
				MapHelper.wallOptionCounts[num9] = num10;
				num7 += num10;
			}
			num7 += 773;
			MapHelper.colorLookup = new Color[num7];
			MapHelper.colorLookup[0] = Color.Transparent;
			ushort num11 = 1;
			MapHelper.tilePosition = num11;
			MapHelper.tileLookup = new ushort[467];
			for (int num12 = 0; num12 < 467; num12++)
			{
				if (MapHelper.tileOptionCounts[num12] > 0)
				{
					Color[] arg_5F4E_0 = array[num12];
					MapHelper.tileLookup[num12] = num11;
					for (int num13 = 0; num13 < MapHelper.tileOptionCounts[num12]; num13++)
					{
						MapHelper.colorLookup[(int)num11] = array[num12][num13];
						num11 += 1;
					}
				}
				else
				{
					MapHelper.tileLookup[num12] = 0;
				}
			}
			MapHelper.wallPosition = num11;
			MapHelper.wallLookup = new ushort[231];
			MapHelper.wallRangeStart = num11;
			for (int num14 = 0; num14 < 231; num14++)
			{
				if (MapHelper.wallOptionCounts[num14] > 0)
				{
					Color[] arg_5FE3_0 = array3[num14];
					MapHelper.wallLookup[num14] = num11;
					for (int num15 = 0; num15 < MapHelper.wallOptionCounts[num14]; num15++)
					{
						MapHelper.colorLookup[(int)num11] = array3[num14][num15];
						num11 += 1;
					}
				}
				else
				{
					MapHelper.wallLookup[num14] = 0;
				}
			}
			MapHelper.wallRangeEnd = num11;
			MapHelper.liquidPosition = num11;
			for (int num16 = 0; num16 < 3; num16++)
			{
				MapHelper.colorLookup[(int)num11] = array2[num16];
				num11 += 1;
			}
			MapHelper.skyPosition = num11;
			for (int num17 = 0; num17 < 256; num17++)
			{
				MapHelper.colorLookup[(int)num11] = array4[num17];
				num11 += 1;
			}
			MapHelper.dirtPosition = num11;
			for (int num18 = 0; num18 < 256; num18++)
			{
				MapHelper.colorLookup[(int)num11] = array5[num18];
				num11 += 1;
			}
			MapHelper.rockPosition = num11;
			for (int num19 = 0; num19 < 256; num19++)
			{
				MapHelper.colorLookup[(int)num11] = array6[num19];
				num11 += 1;
			}
			MapHelper.hellPosition = num11;
			MapHelper.colorLookup[(int)num11] = color7;
			MapHelper.snowTypes = new ushort[6];
			MapHelper.snowTypes[0] = MapHelper.tileLookup[147];
			MapHelper.snowTypes[1] = MapHelper.tileLookup[161];
			MapHelper.snowTypes[2] = MapHelper.tileLookup[162];
			MapHelper.snowTypes[3] = MapHelper.tileLookup[163];
			MapHelper.snowTypes[4] = MapHelper.tileLookup[164];
			MapHelper.snowTypes[5] = MapHelper.tileLookup[200];
		}

		public static int TileToLookup(int tileType, int option)
		{
			int num = (int)MapHelper.tileLookup[tileType];
			return num + option;
		}

		public static int LookupCount()
		{
			return MapHelper.colorLookup.Length;
		}

		private static void MapColor(ushort type, ref Color oldColor, byte colorType)
		{
			Color color = WorldGen.paintColor((int)colorType);
			float num = (float)oldColor.R / 255f;
			float num2 = (float)oldColor.G / 255f;
			float num3 = (float)oldColor.B / 255f;
			if (num2 > num)
			{
				num = num2;
			}
			if (num3 > num)
			{
				float num4 = num;
				num = num3;
				num3 = num4;
			}
			if (colorType == 29)
			{
				float num5 = num3 * 0.3f;
				oldColor.R = (byte)((float)color.R * num5);
				oldColor.G = (byte)((float)color.G * num5);
				oldColor.B = (byte)((float)color.B * num5);
				return;
			}
			if (colorType != 30)
			{
				float num6 = num;
				oldColor.R = (byte)((float)color.R * num6);
				oldColor.G = (byte)((float)color.G * num6);
				oldColor.B = (byte)((float)color.B * num6);
				return;
			}
			if (type >= MapHelper.wallRangeStart && type <= MapHelper.wallRangeEnd)
			{
				oldColor.R = (byte)((float)(255 - oldColor.R) * 0.5f);
				oldColor.G = (byte)((float)(255 - oldColor.G) * 0.5f);
				oldColor.B = (byte)((float)(255 - oldColor.B) * 0.5f);
				return;
			}
			oldColor.R = (byte)(255 - oldColor.R);
			oldColor.G = (byte)(255 - oldColor.G);
			oldColor.B = (byte)(255 - oldColor.B);
		}

		public static Color GetMapTileXnaColor(ref MapTile tile)
		{
			Color result = MapHelper.colorLookup[(int)tile.Type];
			byte color = tile.Color;
			if (color > 0)
			{
				MapHelper.MapColor(tile.Type, ref result, color);
			}
			if (tile.Light == 255)
			{
				return result;
			}
			float num = (float)tile.Light / 255f;
			result.R = (byte)((float)result.R * num);
			result.G = (byte)((float)result.G * num);
			result.B = (byte)((float)result.B * num);
			return result;
		}

		public static MapTile CreateMapTile(int i, int j, byte Light)
		{
			Tile tile = Main.tile[i, j];
			if (tile == null)
			{
				tile = (Main.tile[i, j] = new Tile());
			}
			int num = 0;
			int num2 = (int)Light;
			ushort type = Main.Map[i, j].Type;
			int num3 = 0;
			int num4 = 0;
			if (tile.active())
			{
				int type2 = (int)tile.type;
				num3 = (int)MapHelper.tileLookup[type2];
				if (type2 == 51 && (i + j) % 2 == 0)
				{
					num3 = 0;
				}
				if (num3 != 0)
				{
					if (type2 == 160)
					{
						num = 0;
					}
					else
					{
						num = (int)tile.color();
					}
					int num5 = type2;
					int num6;
					if (num5 <= 165)
					{
						if (num5 <= 84)
						{
							if (num5 <= 21)
							{
								if (num5 == 4)
								{
									if (tile.frameX < 66)
									{
									}
									num4 = 0;
									goto IL_A02;
								}
								if (num5 != 21)
								{
									goto IL_9FF;
								}
							}
							else
							{
								switch (num5)
								{
								case 26:
									if (tile.frameX >= 54)
									{
										num4 = 1;
										goto IL_A02;
									}
									num4 = 0;
									goto IL_A02;
								case 27:
									if (tile.frameY < 34)
									{
										num4 = 1;
										goto IL_A02;
									}
									num4 = 0;
									goto IL_A02;
								case 28:
									if (tile.frameY < 144)
									{
										num4 = 0;
										goto IL_A02;
									}
									if (tile.frameY < 252)
									{
										num4 = 1;
										goto IL_A02;
									}
									if (tile.frameY < 360 || (tile.frameY > 900 && tile.frameY < 1008))
									{
										num4 = 2;
										goto IL_A02;
									}
									if (tile.frameY < 468)
									{
										num4 = 3;
										goto IL_A02;
									}
									if (tile.frameY < 576)
									{
										num4 = 4;
										goto IL_A02;
									}
									if (tile.frameY < 684)
									{
										num4 = 5;
										goto IL_A02;
									}
									if (tile.frameY < 792)
									{
										num4 = 6;
										goto IL_A02;
									}
									if (tile.frameY < 898)
									{
										num4 = 8;
										goto IL_A02;
									}
									if (tile.frameY < 1006)
									{
										num4 = 7;
										goto IL_A02;
									}
									if (tile.frameY < 1114)
									{
										num4 = 0;
										goto IL_A02;
									}
									if (tile.frameY < 1222)
									{
										num4 = 3;
										goto IL_A02;
									}
									num4 = 7;
									goto IL_A02;
								case 29:
								case 30:
									goto IL_9FF;
								case 31:
									if (tile.frameX >= 36)
									{
										num4 = 1;
										goto IL_A02;
									}
									num4 = 0;
									goto IL_A02;
								default:
									switch (num5)
									{
									case 82:
									case 83:
									case 84:
										if (tile.frameX < 18)
										{
											num4 = 0;
											goto IL_A02;
										}
										if (tile.frameX < 36)
										{
											num4 = 1;
											goto IL_A02;
										}
										if (tile.frameX < 54)
										{
											num4 = 2;
											goto IL_A02;
										}
										if (tile.frameX < 72)
										{
											num4 = 3;
											goto IL_A02;
										}
										if (tile.frameX < 90)
										{
											num4 = 4;
											goto IL_A02;
										}
										if (tile.frameX < 108)
										{
											num4 = 5;
											goto IL_A02;
										}
										num4 = 6;
										goto IL_A02;
									default:
										goto IL_9FF;
									}
									break;
								}
							}
						}
						else if (num5 <= 137)
						{
							if (num5 != 105)
							{
								switch (num5)
								{
								case 133:
									if (tile.frameX < 52)
									{
										num4 = 0;
										goto IL_A02;
									}
									num4 = 1;
									goto IL_A02;
								case 134:
									if (tile.frameX < 28)
									{
										num4 = 0;
										goto IL_A02;
									}
									num4 = 1;
									goto IL_A02;
								case 135:
								case 136:
									goto IL_9FF;
								case 137:
									if (tile.frameY == 0)
									{
										num4 = 0;
										goto IL_A02;
									}
									num4 = 1;
									goto IL_A02;
								default:
									goto IL_9FF;
								}
							}
							else
							{
								if (tile.frameX >= 1548 && tile.frameX <= 1654)
								{
									num4 = 1;
									goto IL_A02;
								}
								if (tile.frameX >= 1656 && tile.frameX <= 1798)
								{
									num4 = 2;
									goto IL_A02;
								}
								num4 = 0;
								goto IL_A02;
							}
						}
						else
						{
							if (num5 == 149)
							{
								num4 = j % 3;
								goto IL_A02;
							}
							if (num5 == 160)
							{
								num4 = j % 3;
								goto IL_A02;
							}
							if (num5 != 165)
							{
								goto IL_9FF;
							}
							if (tile.frameX < 54)
							{
								num4 = 0;
								goto IL_A02;
							}
							if (tile.frameX < 106)
							{
								num4 = 1;
								goto IL_A02;
							}
							if (tile.frameX >= 216)
							{
								num4 = 1;
								goto IL_A02;
							}
							if (tile.frameX < 162)
							{
								num4 = 2;
								goto IL_A02;
							}
							num4 = 3;
							goto IL_A02;
						}
					}
					else if (num5 <= 242)
					{
						if (num5 <= 187)
						{
							if (num5 != 178)
							{
								switch (num5)
								{
								case 184:
									if (tile.frameX < 22)
									{
										num4 = 0;
										goto IL_A02;
									}
									if (tile.frameX < 44)
									{
										num4 = 1;
										goto IL_A02;
									}
									if (tile.frameX < 66)
									{
										num4 = 2;
										goto IL_A02;
									}
									if (tile.frameX < 88)
									{
										num4 = 3;
										goto IL_A02;
									}
									if (tile.frameX < 110)
									{
										num4 = 4;
										goto IL_A02;
									}
									num4 = 5;
									goto IL_A02;
								case 185:
									if (tile.frameY < 18)
									{
										num6 = (int)(tile.frameX / 18);
										if (num6 < 6 || num6 == 28 || num6 == 29 || num6 == 30 || num6 == 31 || num6 == 32)
										{
											num4 = 0;
											goto IL_A02;
										}
										if (num6 < 12 || num6 == 33 || num6 == 34 || num6 == 35)
										{
											num4 = 1;
											goto IL_A02;
										}
										if (num6 < 28)
										{
											num4 = 2;
											goto IL_A02;
										}
										if (num6 < 48)
										{
											num4 = 3;
											goto IL_A02;
										}
										if (num6 < 54)
										{
											num4 = 4;
											goto IL_A02;
										}
										goto IL_A02;
									}
									else
									{
										num6 = (int)(tile.frameX / 36);
										if (num6 < 6 || num6 == 19 || num6 == 20 || num6 == 21 || num6 == 22 || num6 == 23 || num6 == 24 || num6 == 33 || num6 == 38 || num6 == 39 || num6 == 40)
										{
											num4 = 0;
											goto IL_A02;
										}
										if (num6 < 16)
										{
											num4 = 2;
											goto IL_A02;
										}
										if (num6 < 19 || num6 == 31 || num6 == 32)
										{
											num4 = 1;
											goto IL_A02;
										}
										if (num6 < 31)
										{
											num4 = 3;
											goto IL_A02;
										}
										if (num6 < 38)
										{
											num4 = 4;
											goto IL_A02;
										}
										goto IL_A02;
									}
									break;
								case 186:
									num6 = (int)(tile.frameX / 54);
									if (num6 < 7)
									{
										num4 = 2;
										goto IL_A02;
									}
									if (num6 < 22 || num6 == 33 || num6 == 34 || num6 == 35)
									{
										num4 = 0;
										goto IL_A02;
									}
									if (num6 < 25)
									{
										num4 = 1;
										goto IL_A02;
									}
									if (num6 == 25)
									{
										num4 = 5;
										goto IL_A02;
									}
									if (num6 < 32)
									{
										num4 = 3;
										goto IL_A02;
									}
									goto IL_A02;
								case 187:
									num6 = (int)(tile.frameX / 54);
									if (num6 < 3 || num6 == 14 || num6 == 15 || num6 == 16)
									{
										num4 = 0;
										goto IL_A02;
									}
									if (num6 < 6)
									{
										num4 = 6;
										goto IL_A02;
									}
									if (num6 < 9)
									{
										num4 = 7;
										goto IL_A02;
									}
									if (num6 < 14)
									{
										num4 = 4;
										goto IL_A02;
									}
									if (num6 < 18)
									{
										num4 = 4;
										goto IL_A02;
									}
									if (num6 < 23)
									{
										num4 = 8;
										goto IL_A02;
									}
									if (num6 < 25)
									{
										num4 = 0;
										goto IL_A02;
									}
									if (num6 < 29)
									{
										num4 = 1;
										goto IL_A02;
									}
									goto IL_A02;
								default:
									goto IL_9FF;
								}
							}
							else
							{
								if (tile.frameX < 18)
								{
									num4 = 0;
									goto IL_A02;
								}
								if (tile.frameX < 36)
								{
									num4 = 1;
									goto IL_A02;
								}
								if (tile.frameX < 54)
								{
									num4 = 2;
									goto IL_A02;
								}
								if (tile.frameX < 72)
								{
									num4 = 3;
									goto IL_A02;
								}
								if (tile.frameX < 90)
								{
									num4 = 4;
									goto IL_A02;
								}
								if (tile.frameX < 108)
								{
									num4 = 5;
									goto IL_A02;
								}
								num4 = 6;
								goto IL_A02;
							}
						}
						else
						{
							if (num5 == 227)
							{
								num4 = (int)(tile.frameX / 34);
								goto IL_A02;
							}
							switch (num5)
							{
							case 240:
							{
								num6 = (int)(tile.frameX / 54);
								int num7 = (int)(tile.frameY / 54);
								num6 += num7 * 36;
								if ((num6 >= 0 && num6 <= 11) || (num6 >= 47 && num6 <= 53))
								{
									num4 = 0;
									goto IL_A02;
								}
								if (num6 >= 12 && num6 <= 15)
								{
									num4 = 1;
									goto IL_A02;
								}
								if (num6 == 16 || num6 == 17)
								{
									num4 = 2;
									goto IL_A02;
								}
								if (num6 >= 18 && num6 <= 35)
								{
									num4 = 1;
									goto IL_A02;
								}
								if (num6 >= 41 && num6 <= 45)
								{
									num4 = 3;
									goto IL_A02;
								}
								if (num6 == 46)
								{
									num4 = 4;
									goto IL_A02;
								}
								goto IL_A02;
							}
							case 241:
								goto IL_9FF;
							case 242:
								num6 = (int)(tile.frameY / 72);
								if (num6 >= 22 && num6 <= 24)
								{
									num4 = 1;
									goto IL_A02;
								}
								num4 = 0;
								goto IL_A02;
							default:
								goto IL_9FF;
							}
						}
					}
					else if (num5 <= 428)
					{
						switch (num5)
						{
						case 419:
							num6 = (int)(tile.frameX / 18);
							if (num6 > 2)
							{
								num6 = 2;
							}
							num4 = num6;
							goto IL_A02;
						case 420:
							num6 = (int)(tile.frameY / 18);
							if (num6 > 5)
							{
								num6 = 5;
							}
							num4 = num6;
							goto IL_A02;
						case 421:
						case 422:
							goto IL_9FF;
						case 423:
							num6 = (int)(tile.frameY / 18);
							if (num6 > 6)
							{
								num6 = 6;
							}
							num4 = num6;
							goto IL_A02;
						default:
							if (num5 != 428)
							{
								goto IL_9FF;
							}
							num6 = (int)(tile.frameY / 18);
							if (num6 > 3)
							{
								num6 = 3;
							}
							num4 = num6;
							goto IL_A02;
						}
					}
					else
					{
						switch (num5)
						{
						case 440:
							num6 = (int)(tile.frameX / 54);
							if (num6 > 6)
							{
								num6 = 6;
							}
							num4 = num6;
							goto IL_A02;
						case 441:
							break;
						default:
							if (num5 == 453)
							{
								num6 = (int)(tile.frameX / 36);
								if (num6 > 2)
								{
									num6 = 2;
								}
								num4 = num6;
								goto IL_A02;
							}
							if (num5 != 457)
							{
								goto IL_9FF;
							}
							num6 = (int)(tile.frameX / 36);
							if (num6 > 4)
							{
								num6 = 4;
							}
							num4 = num6;
							goto IL_A02;
						}
					}
					num6 = (int)(tile.frameX / 36);
					if (num6 == 1 || num6 == 2 || num6 == 10 || num6 == 13 || num6 == 15)
					{
						num4 = 1;
						goto IL_A02;
					}
					if (num6 == 3 || num6 == 4)
					{
						num4 = 2;
						goto IL_A02;
					}
					if (num6 == 6)
					{
						num4 = 3;
						goto IL_A02;
					}
					if (num6 == 11 || num6 == 17)
					{
						num4 = 4;
						goto IL_A02;
					}
					num4 = 0;
					goto IL_A02;
					IL_9FF:
					num4 = 0;
				}
			}
			IL_A02:
			if (num3 == 0)
			{
				if (tile.liquid > 32)
				{
					int num8 = (int)tile.liquidType();
					num3 = (int)MapHelper.liquidPosition + num8;
				}
				else if (tile.wall > 0)
				{
					int wall = (int)tile.wall;
					num3 = (int)MapHelper.wallLookup[wall];
					num = (int)tile.wallColor();
					int num9 = wall;
					if (num9 <= 27)
					{
						if (num9 != 21)
						{
							if (num9 != 27)
							{
								goto IL_A97;
							}
							num4 = i % 2;
							goto IL_A9A;
						}
					}
					else
					{
						switch (num9)
						{
						case 88:
						case 89:
						case 90:
						case 91:
						case 92:
						case 93:
							break;
						default:
							if (num9 != 168)
							{
								goto IL_A97;
							}
							break;
						}
					}
					num = 0;
					goto IL_A9A;
					IL_A97:
					num4 = 0;
				}
			}
			IL_A9A:
			if (num3 == 0)
			{
				if ((double)j < Main.worldSurface)
				{
					int num10 = (int)((byte)(255.0 * ((double)j / Main.worldSurface)));
					num3 = (int)MapHelper.skyPosition + num10;
					num2 = 255;
					num = 0;
				}
				else if (j < Main.maxTilesY - 200)
				{
					num = 0;
					bool flag = type < MapHelper.dirtPosition || type >= MapHelper.hellPosition;
					byte b = 0;
					float num11 = Main.screenPosition.X / 16f - 5f;
					float num12 = (Main.screenPosition.X + (float)Main.screenWidth) / 16f + 5f;
					float num13 = Main.screenPosition.Y / 16f - 5f;
					float num14 = (Main.screenPosition.Y + (float)Main.screenHeight) / 16f + 5f;
					if (((float)i < num11 || (float)i > num12 || (float)j < num13 || (float)j > num14) && i > 40 && i < Main.maxTilesX - 40 && j > 40 && j < Main.maxTilesY - 40 && flag)
					{
						for (int k = i - 36; k <= i + 30; k += 10)
						{
							for (int l = j - 36; l <= j + 30; l += 10)
							{
								for (int m = 0; m < MapHelper.snowTypes.Length; m++)
								{
									if (MapHelper.snowTypes[m] == type)
									{
										b = 255;
										k = i + 31;
										l = j + 31;
										break;
									}
								}
							}
						}
					}
					else
					{
						float num15 = (float)Main.snowTiles / 1000f;
						num15 *= 255f;
						if (num15 > 255f)
						{
							num15 = 255f;
						}
						b = (byte)num15;
					}
					if ((double)j < Main.rockLayer)
					{
						num3 = (int)(MapHelper.dirtPosition + (ushort)b);
					}
					else
					{
						num3 = (int)(MapHelper.rockPosition + (ushort)b);
					}
				}
				else
				{
					num3 = (int)MapHelper.hellPosition;
				}
			}
			int num16 = num3 + num4;
			return MapTile.Create((ushort)num16, (byte)num2, (byte)num);
		}

		public static void SaveMap()
		{
			bool isCloudSave = Main.ActivePlayerFileData.IsCloudSave;
			if (isCloudSave && SocialAPI.Cloud == null)
			{
				return;
			}
			if (!Main.mapEnabled || MapHelper.saveLock)
			{
				return;
			}
			string text = Main.playerPathName.Substring(0, Main.playerPathName.Length - 4);
			lock (MapHelper.padlock)
			{
				try
				{
					MapHelper.saveLock = true;
					try
					{
						if (!isCloudSave)
						{
							Directory.CreateDirectory(text);
						}
					}
					catch
					{
					}
					text += Path.DirectorySeparatorChar;
					if (Main.ActiveWorldFileData.UseGuidAsMapName)
					{
						text = text + Main.ActiveWorldFileData.UniqueId.ToString() + ".map";
					}
					else
					{
						text = text + Main.worldID + ".map";
					}
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					bool flag2 = false;
					if (!Main.gameMenu)
					{
						flag2 = true;
					}
					using (MemoryStream memoryStream = new MemoryStream(4000))
					{
						using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
						{
							using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
							{
								int num = 0;
								byte[] array = new byte[16384];
								binaryWriter.Write(188);
								Main.MapFileMetadata.IncrementAndWrite(binaryWriter);
								binaryWriter.Write(Main.worldName);
								binaryWriter.Write(Main.worldID);
								binaryWriter.Write(Main.maxTilesY);
								binaryWriter.Write(Main.maxTilesX);
								binaryWriter.Write((short)467);
								binaryWriter.Write((short)231);
								binaryWriter.Write((short)3);
								binaryWriter.Write((short)256);
								binaryWriter.Write((short)256);
								binaryWriter.Write((short)256);
								byte b = 1;
								byte b2 = 0;
								int i;
								for (i = 0; i < 467; i++)
								{
									if (MapHelper.tileOptionCounts[i] != 1)
									{
										b2 |= b;
									}
									if (b == 128)
									{
										binaryWriter.Write(b2);
										b2 = 0;
										b = 1;
									}
									else
									{
										b = (byte)(b << 1);
									}
								}
								if (b != 1)
								{
									binaryWriter.Write(b2);
								}
								i = 0;
								b = 1;
								b2 = 0;
								while (i < 231)
								{
									if (MapHelper.wallOptionCounts[i] != 1)
									{
										b2 |= b;
									}
									if (b == 128)
									{
										binaryWriter.Write(b2);
										b2 = 0;
										b = 1;
									}
									else
									{
										b = (byte)(b << 1);
									}
									i++;
								}
								if (b != 1)
								{
									binaryWriter.Write(b2);
								}
								for (i = 0; i < 467; i++)
								{
									if (MapHelper.tileOptionCounts[i] != 1)
									{
										binaryWriter.Write((byte)MapHelper.tileOptionCounts[i]);
									}
								}
								for (i = 0; i < 231; i++)
								{
									if (MapHelper.wallOptionCounts[i] != 1)
									{
										binaryWriter.Write((byte)MapHelper.wallOptionCounts[i]);
									}
								}
								binaryWriter.Flush();
								for (int j = 0; j < Main.maxTilesY; j++)
								{
									if (!flag2)
									{
										float num2 = (float)j / (float)Main.maxTilesY;
										Main.statusText = string.Concat(new object[]
										{
											Lang.gen[66],
											" ",
											(int)(num2 * 100f + 1f),
											"%"
										});
									}
									for (int k = 0; k < Main.maxTilesX; k++)
									{
										MapTile mapTile = Main.Map[k, j];
										byte b4;
										byte b3 = b4 = 0;
										bool flag3 = true;
										bool flag4 = true;
										int num3 = 0;
										int num4 = 0;
										byte b5 = 0;
										int num5;
										ushort num6;
										int num7;
										if (mapTile.Light <= 18)
										{
											flag4 = false;
											flag3 = false;
											num5 = 0;
											num6 = 0;
											num7 = 0;
											int num8 = k + 1;
											int l = Main.maxTilesX - k - 1;
											while (l > 0)
											{
												if (Main.Map[num8, j].Light > 18)
												{
													break;
												}
												num7++;
												l--;
												num8++;
											}
										}
										else
										{
											b5 = mapTile.Color;
											num6 = mapTile.Type;
											if (num6 < MapHelper.wallPosition)
											{
												num5 = 1;
												num6 -= MapHelper.tilePosition;
											}
											else if (num6 < MapHelper.liquidPosition)
											{
												num5 = 2;
												num6 -= MapHelper.wallPosition;
											}
											else if (num6 < MapHelper.skyPosition)
											{
												num5 = (int)(3 + (num6 - MapHelper.liquidPosition));
												flag3 = false;
											}
											else if (num6 < MapHelper.dirtPosition)
											{
												num5 = 6;
												flag4 = false;
												flag3 = false;
											}
											else if (num6 < MapHelper.hellPosition)
											{
												num5 = 7;
												if (num6 < MapHelper.rockPosition)
												{
													num6 -= MapHelper.dirtPosition;
												}
												else
												{
													num6 -= MapHelper.rockPosition;
												}
											}
											else
											{
												num5 = 6;
												flag3 = false;
											}
											if (mapTile.Light == 255)
											{
												flag4 = false;
											}
											if (flag4)
											{
												num7 = 0;
												int num8 = k + 1;
												int l = Main.maxTilesX - k - 1;
												num3 = num8;
												while (l > 0)
												{
													MapTile mapTile2 = Main.Map[num8, j];
													if (!mapTile.EqualsWithoutLight(ref mapTile2))
													{
														num4 = num8;
														break;
													}
													l--;
													num7++;
													num8++;
												}
											}
											else
											{
												num7 = 0;
												int num8 = k + 1;
												int l = Main.maxTilesX - k - 1;
												while (l > 0)
												{
													MapTile mapTile3 = Main.Map[num8, j];
													if (!mapTile.Equals(ref mapTile3))
													{
														break;
													}
													l--;
													num7++;
													num8++;
												}
											}
										}
										if (b5 > 0)
										{
											b3 |= (byte)(b5 << 1);
										}
										if (b3 != 0)
										{
											b4 |= 1;
										}
										b4 |= (byte)(num5 << 1);
										if (flag3 && num6 > 255)
										{
											b4 |= 16;
										}
										if (flag4)
										{
											b4 |= 32;
										}
										if (num7 > 0)
										{
											if (num7 > 255)
											{
												b4 |= 128;
											}
											else
											{
												b4 |= 64;
											}
										}
										array[num] = b4;
										num++;
										if (b3 != 0)
										{
											array[num] = b3;
											num++;
										}
										if (flag3)
										{
											array[num] = (byte)num6;
											num++;
											if (num6 > 255)
											{
												array[num] = (byte)(num6 >> 8);
												num++;
											}
										}
										if (flag4)
										{
											array[num] = mapTile.Light;
											num++;
										}
										if (num7 > 0)
										{
											array[num] = (byte)num7;
											num++;
											if (num7 > 255)
											{
												array[num] = (byte)(num7 >> 8);
												num++;
											}
										}
										for (int m = num3; m < num4; m++)
										{
											array[num] = Main.Map[m, j].Light;
											num++;
										}
										k += num7;
										if (num >= 4096)
										{
											deflateStream.Write(array, 0, num);
											num = 0;
										}
									}
								}
								if (num > 0)
								{
									deflateStream.Write(array, 0, num);
								}
								deflateStream.Dispose();
								FileUtilities.WriteAllBytes(text, memoryStream.ToArray(), isCloudSave);
							}
						}
					}
				}
				catch (Exception value)
				{
					using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
					{
						streamWriter.WriteLine(DateTime.Now);
						streamWriter.WriteLine(value);
						streamWriter.WriteLine("");
					}
				}
				MapHelper.saveLock = false;
			}
		}

		public static void LoadMapVersion1(BinaryReader fileIO, int release)
		{
			string a = fileIO.ReadString();
			int num = fileIO.ReadInt32();
			int num2 = fileIO.ReadInt32();
			int num3 = fileIO.ReadInt32();
			if (a != Main.worldName || num != Main.worldID || num3 != Main.maxTilesX || num2 != Main.maxTilesY)
			{
				throw new Exception("Map meta-data is invalid.");
			}
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				float num4 = (float)i / (float)Main.maxTilesX;
				Main.statusText = string.Concat(new object[]
				{
					Lang.gen[67],
					" ",
					(int)(num4 * 100f + 1f),
					"%"
				});
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					bool flag = fileIO.ReadBoolean();
					if (flag)
					{
						int num5;
						if (release > 77)
						{
							num5 = (int)fileIO.ReadUInt16();
						}
						else
						{
							num5 = (int)fileIO.ReadByte();
						}
						byte b = fileIO.ReadByte();
						MapHelper.OldMapHelper oldMapHelper;
						oldMapHelper.misc = fileIO.ReadByte();
						if (release >= 50)
						{
							oldMapHelper.misc2 = fileIO.ReadByte();
						}
						else
						{
							oldMapHelper.misc2 = 0;
						}
						bool flag2 = false;
						int num6 = (int)oldMapHelper.option();
						int num7;
						if (oldMapHelper.active())
						{
							num7 = num6 + (int)MapHelper.tileLookup[num5];
						}
						else if (oldMapHelper.water())
						{
							num7 = (int)MapHelper.liquidPosition;
						}
						else if (oldMapHelper.lava())
						{
							num7 = (int)(MapHelper.liquidPosition + 1);
						}
						else if (oldMapHelper.honey())
						{
							num7 = (int)(MapHelper.liquidPosition + 2);
						}
						else if (oldMapHelper.wall())
						{
							num7 = num6 + (int)MapHelper.wallLookup[num5];
						}
						else if ((double)j < Main.worldSurface)
						{
							flag2 = true;
							int num8 = (int)((byte)(256.0 * ((double)j / Main.worldSurface)));
							num7 = (int)MapHelper.skyPosition + num8;
						}
						else if ((double)j < Main.rockLayer)
						{
							flag2 = true;
							if (num5 > 255)
							{
								num5 = 255;
							}
							num7 = num5 + (int)MapHelper.dirtPosition;
						}
						else if (j < Main.maxTilesY - 200)
						{
							flag2 = true;
							if (num5 > 255)
							{
								num5 = 255;
							}
							num7 = num5 + (int)MapHelper.rockPosition;
						}
						else
						{
							num7 = (int)MapHelper.hellPosition;
						}
						MapTile mapTile = MapTile.Create((ushort)num7, b, 0);
						Main.Map.SetTile(i, j, ref mapTile);
						int k = (int)fileIO.ReadInt16();
						if (b == 255)
						{
							while (k > 0)
							{
								k--;
								j++;
								if (flag2)
								{
									if ((double)j < Main.worldSurface)
									{
										flag2 = true;
										int num9 = (int)((byte)(256.0 * ((double)j / Main.worldSurface)));
										num7 = (int)MapHelper.skyPosition + num9;
									}
									else if ((double)j < Main.rockLayer)
									{
										flag2 = true;
										num7 = num5 + (int)MapHelper.dirtPosition;
									}
									else if (j < Main.maxTilesY - 200)
									{
										flag2 = true;
										num7 = num5 + (int)MapHelper.rockPosition;
									}
									else
									{
										flag2 = true;
										num7 = (int)MapHelper.hellPosition;
									}
									mapTile.Type = (ushort)num7;
								}
								Main.Map.SetTile(i, j, ref mapTile);
							}
						}
						else
						{
							while (k > 0)
							{
								j++;
								k--;
								b = fileIO.ReadByte();
								if (b > 18)
								{
									mapTile.Light = b;
									if (flag2)
									{
										if ((double)j < Main.worldSurface)
										{
											flag2 = true;
											int num10 = (int)((byte)(256.0 * ((double)j / Main.worldSurface)));
											num7 = (int)MapHelper.skyPosition + num10;
										}
										else if ((double)j < Main.rockLayer)
										{
											flag2 = true;
											num7 = num5 + (int)MapHelper.dirtPosition;
										}
										else if (j < Main.maxTilesY - 200)
										{
											flag2 = true;
											num7 = num5 + (int)MapHelper.rockPosition;
										}
										else
										{
											flag2 = true;
											num7 = (int)MapHelper.hellPosition;
										}
										mapTile.Type = (ushort)num7;
									}
									Main.Map.SetTile(i, j, ref mapTile);
								}
							}
						}
					}
					else
					{
						int num11 = (int)fileIO.ReadInt16();
						j += num11;
					}
				}
			}
		}

		public static void LoadMapVersion2(BinaryReader fileIO, int release)
		{
			if (release >= 135)
			{
				Main.MapFileMetadata = FileMetadata.Read(fileIO, FileType.Map);
			}
			else
			{
				Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
			}
			string a = fileIO.ReadString();
			int num = fileIO.ReadInt32();
			int num2 = fileIO.ReadInt32();
			int num3 = fileIO.ReadInt32();
			if (a != Main.worldName || num != Main.worldID || num3 != Main.maxTilesX || num2 != Main.maxTilesY)
			{
				throw new Exception("Map meta-data is invalid.");
			}
			short num4 = fileIO.ReadInt16();
			short num5 = fileIO.ReadInt16();
			short num6 = fileIO.ReadInt16();
			short num7 = fileIO.ReadInt16();
			short num8 = fileIO.ReadInt16();
			short num9 = fileIO.ReadInt16();
			bool[] array = new bool[(int)num4];
			byte b = 0;
			byte b2 = 128;
			for (int i = 0; i < (int)num4; i++)
			{
				if (b2 == 128)
				{
					b = fileIO.ReadByte();
					b2 = 1;
				}
				else
				{
					b2 = (byte)(b2 << 1);
				}
				if ((b & b2) == b2)
				{
					array[i] = true;
				}
			}
			bool[] array2 = new bool[(int)num5];
			b = 0;
			b2 = 128;
			for (int i = 0; i < (int)num5; i++)
			{
				if (b2 == 128)
				{
					b = fileIO.ReadByte();
					b2 = 1;
				}
				else
				{
					b2 = (byte)(b2 << 1);
				}
				if ((b & b2) == b2)
				{
					array2[i] = true;
				}
			}
			byte[] array3 = new byte[(int)num4];
			ushort num10 = 0;
			for (int i = 0; i < (int)num4; i++)
			{
				if (array[i])
				{
					array3[i] = fileIO.ReadByte();
				}
				else
				{
					array3[i] = 1;
				}
				num10 += (ushort)array3[i];
			}
			byte[] array4 = new byte[(int)num5];
			ushort num11 = 0;
			for (int i = 0; i < (int)num5; i++)
			{
				if (array2[i])
				{
					array4[i] = fileIO.ReadByte();
				}
				else
				{
					array4[i] = 1;
				}
				num11 += (ushort)array4[i];
			}
			int num12 = (int)(num10 + num11 + (ushort)num6 + (ushort)num7 + (ushort)num8 + (ushort)num9 + 2);
			ushort[] array5 = new ushort[num12];
			array5[0] = 0;
			ushort num13 = 1;
			ushort num14 = 1;
			ushort num15 = num14;
			for (int i = 0; i < 467; i++)
			{
				if (i < (int)num4)
				{
					int num16 = (int)array3[i];
					int num17 = MapHelper.tileOptionCounts[i];
					for (int j = 0; j < num17; j++)
					{
						if (j < num16)
						{
							array5[(int)num14] = num13;
							num14 += 1;
						}
						num13 += 1;
					}
				}
				else
				{
					num13 += (ushort)MapHelper.tileOptionCounts[i];
				}
			}
			ushort num18 = num14;
			for (int i = 0; i < 231; i++)
			{
				if (i < (int)num5)
				{
					int num19 = (int)array4[i];
					int num20 = MapHelper.wallOptionCounts[i];
					for (int k = 0; k < num20; k++)
					{
						if (k < num19)
						{
							array5[(int)num14] = num13;
							num14 += 1;
						}
						num13 += 1;
					}
				}
				else
				{
					num13 += (ushort)MapHelper.wallOptionCounts[i];
				}
			}
			ushort num21 = num14;
			for (int i = 0; i < 3; i++)
			{
				if (i < (int)num6)
				{
					array5[(int)num14] = num13;
					num14 += 1;
				}
				num13 += 1;
			}
			ushort num22 = num14;
			for (int i = 0; i < 256; i++)
			{
				if (i < (int)num7)
				{
					array5[(int)num14] = num13;
					num14 += 1;
				}
				num13 += 1;
			}
			ushort num23 = num14;
			for (int i = 0; i < 256; i++)
			{
				if (i < (int)num8)
				{
					array5[(int)num14] = num13;
					num14 += 1;
				}
				num13 += 1;
			}
			ushort num24 = num14;
			for (int i = 0; i < 256; i++)
			{
				if (i < (int)num9)
				{
					array5[(int)num14] = num13;
					num14 += 1;
				}
				num13 += 1;
			}
			ushort num25 = num14;
			array5[(int)num14] = num13;
			BinaryReader binaryReader;
			if (release >= 93)
			{
				DeflateStream input = new DeflateStream(fileIO.BaseStream, CompressionMode.Decompress);
				binaryReader = new BinaryReader(input);
			}
			else
			{
				binaryReader = new BinaryReader(fileIO.BaseStream);
			}
			for (int l = 0; l < Main.maxTilesY; l++)
			{
				float num26 = (float)l / (float)Main.maxTilesY;
				Main.statusText = string.Concat(new object[]
				{
					Lang.gen[67],
					" ",
					(int)(num26 * 100f + 1f),
					"%"
				});
				for (int m = 0; m < Main.maxTilesX; m++)
				{
					byte b3 = binaryReader.ReadByte();
					byte b4;
					if ((b3 & 1) == 1)
					{
						b4 = binaryReader.ReadByte();
					}
					else
					{
						b4 = 0;
					}
					byte b5 = (byte)((b3 & 14) >> 1);
					bool flag;
					switch (b5)
					{
					case 0:
						flag = false;
						break;
					case 1:
					case 2:
					case 7:
						flag = true;
						break;
					case 3:
					case 4:
					case 5:
						flag = false;
						break;
					case 6:
						flag = false;
						break;
					default:
						flag = false;
						break;
					}
					ushort num27;
					if (flag)
					{
						if ((b3 & 16) == 16)
						{
							num27 = binaryReader.ReadUInt16();
						}
						else
						{
							num27 = (ushort)binaryReader.ReadByte();
						}
					}
					else
					{
						num27 = 0;
					}
					byte b6;
					if ((b3 & 32) == 32)
					{
						b6 = binaryReader.ReadByte();
					}
					else
					{
						b6 = 255;
					}
					int n;
					switch ((byte)((b3 & 192) >> 6))
					{
					case 0:
						n = 0;
						break;
					case 1:
						n = (int)binaryReader.ReadByte();
						break;
					case 2:
						n = (int)binaryReader.ReadInt16();
						break;
					default:
						n = 0;
						break;
					}
					if (b5 == 0)
					{
						m += n;
					}
					else
					{
						switch (b5)
						{
						case 1:
							num27 += num15;
							break;
						case 2:
							num27 += num18;
							break;
						case 3:
						case 4:
						case 5:
							num27 += (ushort)(num21 + (ushort)(b5 - 3));
							break;
						case 6:
							if ((double)l < Main.worldSurface)
							{
								ushort num28 = (ushort)((double)num7 * ((double)l / Main.worldSurface));
								num27 += (ushort)(num22 + num28);
							}
							else
							{
								num27 = num25;
							}
							break;
						case 7:
							if ((double)l < Main.rockLayer)
							{
								num27 += num23;
							}
							else
							{
								num27 += num24;
							}
							break;
						}
						MapTile mapTile = MapTile.Create(array5[(int)num27], b6, (byte)(b4 >> 1 & 31));
						Main.Map.SetTile(m, l, ref mapTile);
						if (b6 == 255)
						{
							while (n > 0)
							{
								m++;
								Main.Map.SetTile(m, l, ref mapTile);
								n--;
							}
						}
						else
						{
							while (n > 0)
							{
								m++;
								mapTile = mapTile.WithLight(binaryReader.ReadByte());
								Main.Map.SetTile(m, l, ref mapTile);
								n--;
							}
						}
					}
				}
			}
			binaryReader.Close();
		}
	}
}
