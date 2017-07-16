using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TestGame1
{
    class Ship
    {
        Vector2 mPosition = Vector2.Zero;
        Vector2 mVelocity = Vector2.Zero;
        Texture2D mTex;
        Vector2 mAccel = new Vector2(0.4f);
        Vector2 mDecel = new Vector2(1.2f);
        float mMaxSpeed = 12f;


 
        public void SetTexture(Texture2D tex)
        {
            mTex = tex;
        }

        public void ProcessInputVector(Vector2 accelVector)
        {
            // Normalise and get our real accel vector
            accelVector.Normalize();
            if (float.IsNaN(accelVector.X) || float.IsNaN(accelVector.Y))
            {
                accelVector = Vector2.Zero;
            }

            accelVector *= mAccel;

            // Damp motion from previously, if no accel
            mVelocity /= mDecel;
           // if (accelVector == Vector2.Zero)
           // {
           // mVelocity.Normalize();
           //mVelocity += Vector2.Negate(mVelocity) * mDecel;
           // }

            // Add acceleration from input
            mVelocity += accelVector;

            // Clamp max speed
            if (mVelocity.Length() > mMaxSpeed)
            {
                mVelocity.Normalize();
                mVelocity *= mMaxSpeed;
            }

            // Update position
            mPosition += mVelocity;

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            spriteBatch.Draw(mTex, mPosition, Color.White);
        }
    }

}
