using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Lidgren.Network;

namespace MultiplayerGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Constants and Fields

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        INetworkManager networkManager;
        PlayerManager playerManager;
        InputManager inputManager;

        Texture2D player;
        Rectangle playerPos = new Rectangle(100, 100, 32, 32);

        #endregion Constants and Fields

        #region Constructors and Destructors

        public Game1(INetworkManager networkManager)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.networkManager = networkManager;
        }

        #endregion Constructors and Destructors

        #region Properties

        private bool IsHost
        {
            get
            {
                return this.networkManager is ServerNetworkManager;
            }
        }

        #endregion Properties

        #region Methods

        #endregion Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.networkManager.Connect();

            if(this.IsHost)
            {
                // Event handler here...

                //this.enemyManager.EnemySpawned +=
                //    (sender, e) => this.networkManager.SendMessage(new EnemySpawnedMessage(e.Enemy));
            }

            this.inputManager = new InputManager(this);
            this.playerManager = new PlayerManager(this.inputManager, this.IsHost);
            this.playerManager.PlayerStateChanged += (sender, e) => this.networkManager.SendMessage(new UpdatePlayerStateMessage(e.Player));

            this.Components.Add(inputManager);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            this.player = Content.Load<Texture2D>("Rectangle");

            this.playerManager.LoadContent(this.Content);

            if(this.IsHost)
            {
                this.playerManager.AddPlayer(true);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            this.networkManager.Disconnect();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            // TODO: Add your update logic here
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            this.ProcessNetworkMessages();

            this.playerManager.Update(gameTime);

            //if(IsHost)
            //    HandleServerInput(gameTime);
            //else
            //    HandleClientInput(gameTime);

            base.Update(gameTime);
        }

        //private void HandleClientInput(GameTime gameTime)
        //{
        //    KeyboardState currentKeyboardState = Keyboard.GetState();

        //    Vector2 velocity = Vector2.Zero;

        //    if (currentKeyboardState.IsKeyDown(Keys.Right))
        //        velocity.X++;
        //    if (currentKeyboardState.IsKeyDown(Keys.Left))
        //        velocity.X--;
        //    if (currentKeyboardState.IsKeyDown(Keys.Up))
        //        velocity.Y--;
        //    if (currentKeyboardState.IsKeyDown(Keys.Down))
        //        velocity.Y++;

        //    Vector2 topLeft = new Vector2(playerPos.X, playerPos.Y);
        //    topLeft = topLeft + velocity;
        //    playerPos = new Rectangle((int)topLeft.X, (int)topLeft.Y, 32, 32);
        //}

        //private void HandleServerInput(GameTime gameTime)
        //{

        //}

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            this.spriteBatch.Begin();

            //this.spriteBatch.Draw(player, playerPos, Color.White);
            playerManager.Draw(spriteBatch);

            this.spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleUpdatePlayerStateMessage(NetIncomingMessage im)
        {
            var message = new UpdatePlayerStateMessage(im);

            var timeDelay = (float)(NetTime.Now - im.SenderConnection.GetLocalTime(message.MessageTime));

            Player player = this.playerManager.GetPlayer(message.Id)
                ??
                this.playerManager.AddPlayer(
                message.Id, message.Position, message.Velocity, false);

            if(player.LastUpdateTime < message.MessageTime)
            {
                player.SimulationState.Position = message.Position += message.Velocity * timeDelay;
                player.SimulationState.Velocity = message.Velocity;

                player.LastUpdateTime = message.MessageTime;
            }
        }

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while((im = this.networkManager.ReadMessage()) != null)
            {
                switch(im.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        Console.WriteLine(im.ReadString());
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                if (!this.IsHost)
                                {
                                    var message = new UpdatePlayerStateMessage(im.SenderConnection.RemoteHailMessage);
                                    this.playerManager.AddPlayer(message.Id, message.Position, message.Velocity, true);
                                    Console.WriteLine("Connected to {0}", im.SenderEndpoint);
                                }
                                else
                                    Console.WriteLine("{0} Connected", im.SenderEndpoint);
                                break;
                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine(this.IsHost ? "{0} Disconnected" : "Disconnected from {0}", im.SenderEndpoint);
                                break;
                            case NetConnectionStatus.RespondedAwaitingApproval:
                                NetOutgoingMessage hailMessage = this.networkManager.CreateMessage();
                                new UpdatePlayerStateMessage(this.playerManager.AddPlayer(false)).Encode(hailMessage);
                                im.SenderConnection.Approve(hailMessage);
                                break;
                        }
                        break;
                    case NetIncomingMessageType.Data:
                        var gameMessageType = (GameMessageTypes)im.ReadByte();
                        switch (gameMessageType)
                        {
                            case GameMessageTypes.UpdatePlayerState:
                                this.HandleUpdatePlayerStateMessage(im);
                                break;

                        }
                        break;
                }
                this.networkManager.Recycle(im);
            }
        }
    }
}
