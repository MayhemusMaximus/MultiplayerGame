using System;
using System.Collections.Generic;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerGame
{
    public class PlayerManager
    {
        #region Constants and Fields

        private readonly InputManager inputManager;

        private readonly bool isHost;

        private readonly Dictionary<long, Player> players = new Dictionary<long, Player>();

        private static long playerIdCounter;

        private Player localPlayer;

        private Texture2D texture;

        private float playerSpeed = 160f;

        #endregion Constants and Fields

        #region Constructors and Destructors

        public PlayerManager(InputManager inputManager, bool isHost)
        {
            this.inputManager = inputManager;
            this.isHost = isHost;
        }

        #endregion Constructors and Destructors

        #region Public Events

        public event EventHandler<PlayerStateChangedArgs> PlayerStateChanged;

        #endregion Public Events

        #region Public Properties

        public IEnumerable<Player> Players
        {
            get
            {
                return this.players.Values;
            }
        }

        #endregion Public Properties

        #region Public Methods and Operators

        public Player AddPlayer(long id, Vector2 position, Vector2 velocity, bool isLocal)
        {
            if (this.players.ContainsKey(id))
            {
                return this.players[id];
            }

            var player = new Player(id, texture, new EntityState { Position = position, Velocity = velocity });

            this.players.Add(player.Id, player);

            if(isLocal)
            {
                this.localPlayer = player;
            }

            return player;
        }

        public Player AddPlayer(bool isLocal)
        {
            
            EntityState physicsState = this.SelectRandomEntityState();

            Player player = this.AddPlayer(Interlocked.Increment(ref playerIdCounter), physicsState.Position, physicsState.Velocity, isLocal);

            return player;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(Player player in this.Players)
            {
                player.Draw(spriteBatch);
            }
        }

        public Player GetPlayer(long id)
        {
            if (this.players.ContainsKey(id))
                return this.players[id];

            return null;
        }

        public void LoadContent(ContentManager contentManager)
        {
            this.texture = contentManager.Load<Texture2D>("Rectangle");
        }

        public bool PlayerIsLocal(Player player)
        {
            return this.localPlayer != null & this.localPlayer.Id == player.Id;
        }

        public void RemovePlayer(long id)
        {
            if (this.players.ContainsKey(id))
                this.players.Remove(id);
        }

        public EntityState SelectRandomEntityState()
        {
            var physicsState = new EntityState{Position = new Vector2(100,100)};
            return physicsState;
        }

        public void Update(GameTime gameTime)
        {
            if((this.localPlayer != null))
            {
                bool velocityChanged = this.HandlePlayerMovement();

                if(velocityChanged)
                {
                    this.OnPlayerStateChanged(this.localPlayer);
                }

                foreach (Player player in this.Players)
                {
                    player.Update(gameTime);
                }
            }

            // Adding health periodically here... ?
            
            //if(this.isHost && this.heartbeatTimer.Stopwatch(1000))
            //{
            //    foreach(Player player in this.Players)
            //    {
            //        this.OnPlayerStateChanged(player);
            //    }
            //}
        }

        #endregion Public Methods and Operators

        #region Methods

        protected void OnPlayerStateChanged(Player player)
        {
            EventHandler<PlayerStateChangedArgs> playerStateChanged = this.PlayerStateChanged;
            if(playerStateChanged != null)
            {
                playerStateChanged(this, new PlayerStateChangedArgs(player));
            }
        }

        private bool HandlePlayerMovement()
        {
            bool velocityChanged = false;

            this.localPlayer.SimulationState.Velocity = Vector2.Zero;

            if(this.inputManager.IsKeyDown(Keys.Up))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(0, -1);
                if (this.inputManager.IsKeyReleased(Keys.Up))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Up))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(this.localPlayer.SimulationState.Velocity.X, 0);
                velocityChanged = true;
            }

            if (this.inputManager.IsKeyDown(Keys.Down))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(0, 1);
                if (this.inputManager.IsKeyReleased(Keys.Down))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Down))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(this.localPlayer.SimulationState.Velocity.X, 0);
                velocityChanged = true;
            }

            if (this.inputManager.IsKeyDown(Keys.Left))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(-1, 0);
                if (this.inputManager.IsKeyReleased(Keys.Left))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Left))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(0, this.localPlayer.SimulationState.Velocity.Y);
                velocityChanged = true;
            }

            if (this.inputManager.IsKeyDown(Keys.Right))
            {
                this.localPlayer.SimulationState.Velocity += new Vector2(1, 0);
                if (this.inputManager.IsKeyReleased(Keys.Right))
                {
                    velocityChanged = true;
                }
            }

            if (this.inputManager.IsKeyPressed(Keys.Right))
            {
                this.localPlayer.SimulationState.Velocity = new Vector2(this.localPlayer.SimulationState.Velocity.X, 0);
                velocityChanged = true;
            }

            this.localPlayer.SimulationState.Velocity.Normalize();
            this.localPlayer.SimulationState.Velocity *= this.playerSpeed;

            return velocityChanged;
        }

        #endregion Methods
    }
}
