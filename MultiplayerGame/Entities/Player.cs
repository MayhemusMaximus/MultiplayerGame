using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MultiplayerGame
{
    public class Player : Sprite
    {
        #region Constructors and Destructors

        internal Player(long id, Texture2D texture, EntityState simulationState) :base(id, texture, simulationState)
        {

        }

        #endregion Constructors and Destructors

        #region Public Properites



        #endregion Public Properites

        #region Public Methods and Operators

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion Public Methods and Operators
    }
}
