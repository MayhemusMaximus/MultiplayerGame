using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MultiplayerGame
{
    public class Sprite
    {
        #region fields

        #endregion Fields

        #region Constructors and Destructors

        internal Sprite(long id, Texture2D texture, EntityState simulationState)
        {
            this.Id = id;
            this.Texture = texture;

            this.SimulationState = simulationState;
            this.DisplayState = (EntityState)simulationState.Clone();
            this.PrevDisplayState = (EntityState)simulationState.Clone();
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public Rectangle Bounds
        {
            get
            {
                return new Rectangle((int)this.SimulationState.Position.X, (int)this.SimulationState.Position.Y, 32, 32);
            }
        }

        public Vector2 Center
        {
            get
            {
                return this.SimulationState.Position + new Vector2(32 / 2, 32 / 2);
            }
        }

        public EntityState DisplayState { get; set; }

        public bool EnableSmoothing { get; set; }

        public long Id { get; set; }

        public double LastUpdateTime { get; set; }

        public EntityState PrevDisplayState { get; set; }

        public EntityState SimulationState { get; set; }

        public Texture2D Texture { get; set; }

        #endregion Public Properties

        #region Public Methods and Operators

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Texture, DisplayState.Position, Color.White);
        }

        public virtual void Update(GameTime gameTime)
        {
            var elapsedSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.SimulationState.Position += this.SimulationState.Velocity * elapsedSeconds;

            this.DisplayState = (EntityState)this.SimulationState.Clone();
        }

        #endregion Public Methods and Operators

        #region Methods



        #endregion Methods
    }
}
