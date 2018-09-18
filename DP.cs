using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace MG
{
    ///<summary>The Screen Interface for the Game</summary>
    public interface IScreen
    {
        void LoadContent();
        void Update(GameTime gt);
        void Draw(GameTime gt);
    }
    public class GameType : Game
    {
        IScreen Screen;
        IScreen LoadingScreen;
        bool Loading;
        System.Threading.Thread t;
        public GameType()
        {
            G.Gdm = new GraphicsDeviceManager(this);
            G.ThisGame=this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Loading=false;
            Screen=new GameScreen(Color.Black);
            LoadingScreen=new LoadingScreen();
        }
        protected override void Initialize(){base.Initialize();
            //G.Gdm.ToggleFullScreen();
            }
        protected override void LoadContent()
        {
            G.Sb = new SpriteBatch(GraphicsDevice);
            LoadingScreen.LoadContent();
            Screen.LoadContent();
        }
        protected override void Update(GameTime gameTime)
        {
            if(Loading)LoadingScreen.Update(gameTime);
            else Screen.Update(gameTime);
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            if(Loading)LoadingScreen.Draw(gameTime);
            else Screen.Draw(gameTime);
            base.Draw(gameTime);
        }
        ///<summary>Puts the loading Screen and Loads the Screen in a Thread</summary>
        internal void SetScreen(byte b)
        {
            Loading=true;
            switch(b)
            {
                case 0:Screen=new Logo();break;
                case 1:Screen=new MenuScreen();break;
                case 2:Screen=new GameScreen(Color.Black);break;
                default:break;
                //Add cases for Screens
            }
            Screen.LoadContent();
            Loading=false;
        }
    }
    ///<summary>The static Global Class for the program</summary>
    public static class G
    {
        ///<summary>The static Main() for the program</summary>
        [System.STAThread]
        static void Main()
        {
            var g=new GameType();
            g.Run();
        }
        ///<summary>The static GraphicsDeviceManager for the program</summary>
        internal static GraphicsDeviceManager Gdm;
        ///<summary>The static Spritebatch for the program</summary>
        internal static SpriteBatch Sb;
        ///<summary>The static self reference for the Game, to call SetScreen()</summary>
        internal static GameType ThisGame;
        internal static void Dimensions(out int Width,out int Height)
        {
            if(Gdm.IsFullScreen)
            {
                Height=Gdm.PreferredBackBufferHeight;
                Width=Gdm.PreferredBackBufferWidth;
            }
            else
            {
                var r=ThisGame.Window.ClientBounds;
                Width=r.Width;
                Height=r.Height;
            }
        }
    }
}
