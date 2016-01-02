using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MultiplayerGame
{
    public class InputManager : GameComponent
    {
        #region Constants and Fields

        private KeyboardState keyboardState;

        private KeyboardState lastKeyboardState;

        private MouseState lastMouseState;

        private MouseState mouseState;

        #endregion Constants and Fields

        #region Constructors and Destructors

        public InputManager(Game game) : base(game)
        {
            this.keyboardState = Keyboard.GetState();
            this.mouseState = Mouse.GetState();
        }

        #endregion Constructors and Destructors

        #region Public Properties

        public Vector2 MousePosition
        {
            get
            {
                return new Vector2(mouseState.X, mouseState.Y);
            }
        }

        #endregion Public Properties

        #region Public Methods and Operators

        public void Flush()
        {
            this.lastKeyboardState = this.keyboardState;
            this.lastMouseState = this.mouseState;
        }

        public bool IsKeyDown(Keys keytoTest)
        {
            return this.keyboardState.IsKeyDown(keytoTest);
        }

        public bool IsKeyPressed(Keys keyToTest)
        {
            return this.keyboardState.IsKeyUp(keyToTest) && this.lastKeyboardState.IsKeyDown(keyToTest);
        }

        public bool IsKeyReleased(Keys keyToTest)
        {
            return this.keyboardState.IsKeyDown(keyToTest) && this.lastKeyboardState.IsKeyUp(keyToTest);
        }

        public bool LeftButtonIsClicked()
        {
            return this.mouseState.LeftButton == ButtonState.Released
                && this.lastMouseState.LeftButton == ButtonState.Pressed;
        }

        public bool RightButtonIsClicked()
        {
            return this.mouseState.RightButton == ButtonState.Released
                && this.lastMouseState.RightButton == ButtonState.Pressed;
        }

        public override void Update(GameTime gameTime)
        {
            this.Flush();

            this.keyboardState = Keyboard.GetState(PlayerIndex.One);
            this.mouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        #endregion Public Methods and Operators
    }
}
