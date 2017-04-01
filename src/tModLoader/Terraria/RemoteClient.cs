using Microsoft.Xna.Framework;
using System;
using Terraria.Localization;
using Terraria.Net.Sockets;
using TerrariaApi.Server;

namespace Terraria
{
    public class RemoteClient
    {
        public ISocket Socket;
        public int Id;
        public string Name = "Anonymous";
        public string ClientUUID = "";
        public bool IsActive;
        public bool PendingTermination;
        public bool IsAnnouncementCompleted;
        public bool IsReading;
        public int State;
        public int TimeOutTimer;
        public string StatusText = "";
        public string StatusText2;
        public int StatusCount;
        public int StatusMax;
        public bool[,] TileSections = new bool[Main.maxTilesX / 200 + 1, Main.maxTilesY / 150 + 1];
        public byte[] ReadBuffer;
        public float SpamProjectile;
        public float SpamAddBlock;
        public float SpamDeleteBlock;
        public float SpamWater;
        public float SpamProjectileMax = 100f;
        public float SpamAddBlockMax = 100f;
        public float SpamDeleteBlockMax = 500f;
        public float SpamWaterMax = 50f;
        public RemoteClient()
        {
        }

        public bool IsConnected()
        {
            return this.Socket != null && this.Socket.IsConnected();
        }

        public void SpamUpdate()
        {
            if (!Netplay.spamCheck)
            {
                this.SpamProjectile = 0f;
                this.SpamDeleteBlock = 0f;
                this.SpamAddBlock = 0f;
                this.SpamWater = 0f;
                return;
            }
            if (this.SpamProjectile > this.SpamProjectileMax)
            {
                NetMessage.BootPlayer(this.Id, Language.GetTextValue("Net.CheatingProjectileSpam"));
            }
            if (this.SpamAddBlock > this.SpamAddBlockMax)
            {
                NetMessage.BootPlayer(this.Id, Language.GetTextValue("Net.CheatingTileSpam"));
            }
            if (this.SpamDeleteBlock > this.SpamDeleteBlockMax)
            {
                NetMessage.BootPlayer(this.Id, Language.GetTextValue("Net.CheatingTileRemovalSpam"));
            }
            if (this.SpamWater > this.SpamWaterMax)
            {
                NetMessage.BootPlayer(this.Id, Language.GetTextValue("Net.CheatingLiquidSpam"));
            }
            this.SpamProjectile -= 0.4f;
            if (this.SpamProjectile < 0f)
            {
                this.SpamProjectile = 0f;
            }
            this.SpamAddBlock -= 0.3f;
            if (this.SpamAddBlock < 0f)
            {
                this.SpamAddBlock = 0f;
            }
            this.SpamDeleteBlock -= 5f;
            if (this.SpamDeleteBlock < 0f)
            {
                this.SpamDeleteBlock = 0f;
            }
            this.SpamWater -= 0.2f;
            if (this.SpamWater < 0f)
            {
                this.SpamWater = 0f;
            }
        }

        public void SpamClear()
        {
            this.SpamProjectile = 0f;
            this.SpamAddBlock = 0f;
            this.SpamDeleteBlock = 0f;
            this.SpamWater = 0f;
        }

        public static void CheckSection(int playerIndex, Vector2 position, int fluff = 1)
        {
            int sectionX = Netplay.GetSectionX((int)(position.X / 16f));
            int sectionY = Netplay.GetSectionY((int)(position.Y / 16f));
            int num = 0;
            for (int i = sectionX - fluff; i < sectionX + fluff + 1; i++)
            {
                for (int j = sectionY - fluff; j < sectionY + fluff + 1; j++)
                {
                    if (i >= 0 && i < Main.maxSectionsX && j >= 0 && j < Main.maxSectionsY && !Netplay.Clients[playerIndex].TileSections[i, j])
                    {
                        num++;
                    }
                }
            }
            if (num > 0)
            {
                int num2 = num;
                NetMessage.SendData(9, playerIndex, -1, Lang.inter[44], num2, 0f, 0f, 0f, 0, 0, 0);
                Netplay.Clients[playerIndex].StatusText2 = Language.GetTextValue("Net.IsReceivingTileData");
                Netplay.Clients[playerIndex].StatusMax += num2;
                for (int k = sectionX - fluff; k < sectionX + fluff + 1; k++)
                {
                    for (int l = sectionY - fluff; l < sectionY + fluff + 1; l++)
                    {
                        if (k >= 0 && k < Main.maxSectionsX && l >= 0 && l < Main.maxSectionsY && !Netplay.Clients[playerIndex].TileSections[k, l])
                        {
                            NetMessage.SendSection(playerIndex, k, l, false);
                            NetMessage.SendData(11, playerIndex, -1, "", k, (float)l, (float)k, (float)l, 0, 0, 0);
                        }
                    }
                }
            }
        }

        public bool SectionRange(int size, int firstX, int firstY)
        {
            for (int i = 0; i < 4; i++)
            {
                int num = firstX;
                int num2 = firstY;
                if (i == 1)
                {
                    num += size;
                }
                if (i == 2)
                {
                    num2 += size;
                }
                if (i == 3)
                {
                    num += size;
                    num2 += size;
                }
                int sectionX = Netplay.GetSectionX(num);
                int sectionY = Netplay.GetSectionY(num2);
                if (this.TileSections[sectionX, sectionY])
                {
                    return true;
                }
            }
            return false;
        }

        public void ResetSections()
        {
            for (int i = 0; i < Main.maxSectionsX; i++)
            {
                for (int j = 0; j < Main.maxSectionsY; j++)
                {
                    this.TileSections[i, j] = false;
                }
            }
        }

        public void Reset()
        {
            ServerApi.Hooks.InvokeServerSocketReset(this);
            this.ResetSections();
            if (this.Id < 255)
            {
                Main.player[this.Id] = new Player();
            }
            this.TimeOutTimer = 0;
            this.StatusCount = 0;
            this.StatusMax = 0;
            this.StatusText2 = "";
            this.StatusText = "";
            this.ClientUUID = "";
            this.State = 0;
            this.IsReading = false;
            this.PendingTermination = false;
            this.SpamClear();
            this.IsActive = false;
            NetMessage.buffer[this.Id].Reset();
            if (this.Socket != null)
            {
                this.Socket.Close();
            }
        }
        //public void Reset()
        //{

        //    ServerApi.Hooks.InvokeServerSocketReset(this);
        //    this.ResetSections();
        //    if (this.Id < 255)
        //    {
        //        Main.player[this.Id] = new Player();
        //    }
        //    this.TimeOutTimer = 0;
        //    this.StatusCount = 0;
        //    this.StatusMax = 0;
        //    this.StatusText2 = "";
        //    this.StatusText = "";
        //    this.ClientUUID = "";
        //    this.State = 0;
        //    this.IsReading = false;
        //    this.PendingTermination = false;
        //    this.SpamClear();
        //    this.IsActive = false;
        //    NetMessage.buffer[this.Id].Reset();
        //    this.Socket.Close();
        //}

        public void ServerWriteCallBack(object state)
        {
            NetMessage.buffer[this.Id].spamCount--;
            if (this.StatusMax > 0)
            {
                this.StatusCount++;
            }
        }

        public void ServerReadCallBack(object state, int length)
        {
            if (!Netplay.disconnect)
            {
                if (length == 0)
                {
                    this.PendingTermination = true;
                }
                else
                {
                    if (Main.ignoreErrors)
                    {
                        try
                        {
                            NetMessage.RecieveBytes(this.ReadBuffer, length, this.Id);
                            goto IL_45;
                        }
                        catch
                        {
                            goto IL_45;
                        }
                    }
                    NetMessage.RecieveBytes(this.ReadBuffer, length, this.Id);
                }
            }
        IL_45:
            this.IsReading = false;
        }
    }
}
