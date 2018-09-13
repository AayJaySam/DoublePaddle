using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace MG
{
    public static class GPMenuFunctions
    {
        public static Color[] buttonColor = new Color[] { Color.Black, Color.DeepPink, Color.Red, Color.Orange };
        public static Color MenuColor = Color.Gray;
        public static SpriteFont Sf;
        public static Texture2D MenuTexture;
        public static void FitTextCentered(string Text,Rectangle bounds,SpriteFont s,out Vector2 v,out float Scale)
        {
            var size = s.MeasureString(Text);
            var xs = bounds.Width / size.X;
            var ys = bounds.Height / size.Y;
            Scale = xs < ys ? xs : ys;
            int Swidth = (int)System.Math.Round(size.X * Scale);
            int Shigh = (int)System.Math.Round(size.Y * Scale);
            Swidth = (bounds.Width - Swidth) / 2;
            Shigh = (bounds.Height - Shigh) / 2;
            v = new Vector2(Swidth + bounds.X, Shigh + bounds.Y);
        }
        public static float ScaleToFit(string Text,Rectangle bounds,SpriteFont s)
        {
            var size = s.MeasureString(Text);
            var xs = bounds.Width / size.X;
            var ys = bounds.Height / size.Y;
            return xs < ys ? xs : ys;
        }
        public static Texture2D ColorToButtonTexture(int Height,Color[] cs,out Rectangle Source)
        {
            var h4 = 4 * Height;
            var ButtonSprite = new Texture2D(G.Gdm.GraphicsDevice, 1, h4);
            Source = new Rectangle(0, 0, 1, Height);
            Color[] xx = new Color[h4];
            for (int x = 0; x < h4; x++) { xx[x] = cs[x / Height]; }
            ButtonSprite.SetData<Color>(xx);
            return ButtonSprite;
        }
    }
    internal enum MenuButtonState : byte { Idle = 0, OnButton = 1, ButtonPressed = 2, ButtonReleased = 3, Disabled=7 }
    public struct SimpleMenuButton
    {
        internal string Text;
        internal SpriteFont Sf;
        internal Texture2D ButtonSprite;
        internal MenuButtonState Currentstate;
        internal Rectangle Dest, Source;
        internal float Scale;
        internal Vector2 TextPosition;
        public SimpleMenuButton(string t, int Width, int Height, int Posx, int Posy)
        {
            var h4 = 4 * Height;
            ButtonSprite = new Texture2D(G.Gdm.GraphicsDevice, 1, h4);
            Dest = new Rectangle(Posx, Posy, Width, Height);
            Source = new Rectangle(0, 0, 1, Height);
            Color[] xx = new Color[h4];
            for (int x = 0; x < h4; x++) { xx[x] = GPMenuFunctions.buttonColor[x / Height]; }
            ButtonSprite.SetData<Color>(xx);
            Sf = GPMenuFunctions.Sf;
            Text = t;
            Currentstate = MenuButtonState.Idle;
            GPMenuFunctions.FitTextCentered(Text, Dest, Sf, out TextPosition, out Scale);
        }
        ///<summary>To be used with GamePad and NOT with mouse</summary>
        internal void Update(MenuButtonState s) { Source.Y = (int)s%4 * Source.Height; }
        ///<summary>To be used with Mouse and NOT with gamepad</summary>
        internal void UpdateMouse()
        {
            if (Currentstate != MenuButtonState.Disabled)
            {
                var M = Mouse.GetState();
                var i = Inside(M.Position);
                var m = M.LeftButton == ButtonState.Pressed;
                var b = Currentstate == MenuButtonState.ButtonPressed;
                if (i)
                {
                    if (m) { Currentstate = MenuButtonState.ButtonPressed; }
                    else if (b) { Currentstate = MenuButtonState.ButtonReleased; }
                    else { Currentstate = MenuButtonState.OnButton; }
                }
                else if (b && m) { Currentstate = MenuButtonState.ButtonPressed; }
                else { Currentstate = MenuButtonState.Idle; }
            }
            Update(Currentstate);
        }
        bool Inside(Point p) => p.X >= Dest.X && p.X <= Dest.X + Dest.Width && p.Y >= Dest.Y && p.Y <= Dest.Y + Dest.Height;
        public void Draw(SpriteBatch Sb, Color c)
        {
            Sb.Draw(ButtonSprite, Dest, Source, Color.White);
            Sb.DrawString(Sf, Text, TextPosition, c, 0, Vector2.Zero, Scale, SpriteEffects.None, 0);
        }
    }
}