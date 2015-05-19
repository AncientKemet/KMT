using Server.Model.Content.Spawns;
using Server.Model.ContentHandling;
using Server.Servers;
#if SERVER
using Code.Core.Client.UI;

using Server.Model.ContentHandling.Player;

using UnityEngine;
using Code.Code.Libaries.Net.Packets;
using Server.Model.Entities.Groups;
using Server.Model.Extensions.PlayerExtensions;
using Server.Model.Extensions.PlayerExtensions.UIHelpers;
using Server.Model.Extensions.UnitExts;

namespace Server.Model.Entities.Human
{

    public class Player : Human
    {
        private ServerClient _client;

        /// <summary>
        /// Gets or Sets the ServerClient extension for this Player, do not use the Set, as it's already implemented.
        /// </summary>
        public ServerClient Client
        {
            get { return _client; }
            set
            {
                _client = value;
            }
        }

        public PlayerInput PlayerInput { get; private set; }
        public ClientUI ClientUi { get { return Client.UI; }}
        public PlayerParty Party { get; set; }
        public PlayerUDP PlayerUdp { get; private set; }

        /// <summary>
        /// Password that the client used to connect.
        /// </summary>
        public string Password { get; set; }

        public override void Awake()
        {
            PlayerInput = AddExt<PlayerInput>();
            PlayerUdp = AddExt<PlayerUDP>();
            base.Awake();
        }
        
        /// <summary>
        /// Is called when the player enters a world.
        /// </summary>
        /// <param name="world"></param>
        public void OnEnteredWorld(World world)
        {
            EnterWorldPacket enterWorldPacket = new EnterWorldPacket();

            enterWorldPacket.worldId = world.Map.name;
            enterWorldPacket.myUnitID = ID;
            enterWorldPacket.Position = Movement.Position;

            Client.ConnectionHandler.SendPacket(enterWorldPacket);

            ClientUi.Open(InterfaceType.ActionBars);
            ClientUi.Open(InterfaceType.ChatPanel);
            ClientUi.Open(InterfaceType.Chat);
            ClientUi.Open(InterfaceType.StatsBars);
            ClientUi.Open(InterfaceType.LowerLeftMenu);
            ClientUi.Open(InterfaceType.UnitSelectionInterface);

            GetExt<UnitInventory>().ListeningPlayers.Add(this);
            GetExt<UnitInventory>().RefreshFull();
        }

        public void SetupNewPlayer(WorldServer worldServer)
        {
            Movement.Teleport(ServerSpawnManager.Instance(worldServer.World).PlayerSpawns.Find(spawn => spawn.type == PlayerSpawn.Type.Default).transform.position);
            worldServer.World.AddEntity(this);
            ClientUi.CreateCharacterInterface.Opened = true;

            /*Movement.Teleport(new Vector3(512, 10, 512));
            ClientUi.Open(InterfaceType.ActionBars);
            ClientUi.Open(InterfaceType.ChatPanel);
            ClientUi.Open(InterfaceType.Chat);
            ClientUi.Open(InterfaceType.StatsBars);
            ClientUi.Open(InterfaceType.LowerLeftMenu);
            ClientUi.Open(InterfaceType.UnitSelectionInterface);*/
            
            PlayerFeed.AddFeed(this, "Joined Ancient Kemet", "I've logged in to Ancient Kemet for the first time!", PlayerFeed.FeedRarity.Normal);
        }
    }
}

#endif
