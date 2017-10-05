using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Endeavour
{
    public class InputState
    {
        // Keyboard states used to determine key presses
        public KeyboardState mCurrentKeyboardState;
        public KeyboardState mPreviousKeyboardState;

        // Gamepad states used to determine button presses
        public GamePadState mCurrentGamePadState;
        public GamePadState mPreviousGamePadState;

        //Mouse states used to track Mouse button press
        public MouseState mCurrentMouseState;
        public MouseState mPreviousMouseState;

        public InputState()
        {
            mCurrentMouseState = new MouseState();
            mPreviousMouseState = mCurrentMouseState;

            mCurrentKeyboardState = new KeyboardState();
            mPreviousKeyboardState = mCurrentKeyboardState;

            mCurrentGamePadState = new GamePadState();
            mPreviousGamePadState = mCurrentGamePadState;
        }

        public void Update(GameTime gameTime)
        {
            mCurrentKeyboardState = Keyboard.GetState();
            mCurrentGamePadState = GamePad.GetState(PlayerIndex.One);
            mCurrentMouseState = Mouse.GetState();


        }

        public void UpdatePrevious()
        {
            mPreviousGamePadState = mCurrentGamePadState;
            mPreviousKeyboardState = mCurrentKeyboardState;
            mPreviousMouseState = mCurrentMouseState;
        }

        // helper methods
        public bool HasMouseMoved()
        {
            return mCurrentMouseState.Position != mPreviousMouseState.Position;
        }
    }

}
