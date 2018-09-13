using Microsoft.Xna.Framework;
namespace MG
{
    public struct Logo:IScreen
    {
        internal Color c;
        public void Draw(GameTime gt){G.Gdm.GraphicsDevice.Clear(c);}
        public void LoadContent(){}
        public void Update(GameTime gt){c=gt.IsRunningSlowly?Color.Red:Color.GreenYellow;}
    }
}