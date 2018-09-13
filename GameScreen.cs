using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
namespace MG
{
    internal enum GState:byte{Start,Play1H,Play2H,Pause,GameOver}
    internal struct GameScreen : IScreen
    {
        ///<summary>BackGround Color</summary>
        internal Color BGColor;
        ///<summary>Texture Most likely to be used by all.</summary>
        internal static Texture2D ballTexture;
        ///<summary>Home::Left:0,Right:1||Away::Up:2,Down:3,</summary>
        internal Board[] bd;
        ///<summary>All the balls in the game</summary>
        internal Ball[] bl;
        ///<summary>
        ///Borders at the corners of the screen.
        ///
        ///0:UpLeft || 1:DownLeft || 2:UpRight || 3:DownRight
        ///</summary>
        internal Rectangle[] bouncers;
        ///<summary>The two Players of the game</summary>
        internal Player Home,Away;
        ///<summary>The Length of array for balls</summary>
        byte MaxBalls;
        ///<summary>The present count of Balls</summary>
        byte CurrentBalls;
        ///<summary>Managing the states</summary>
        GState currentstate;
        ///<summary>Friction fot the balls and boards</summary>
        float GroundFriction;
        ///<summary>Dimensions for the window</summary>
        int Width,Height;
        ///<summary>balls generate from this Point.</summary>
        Vector2 MidScreen;
        ///<summary>The LengthUnit for the Game.</summary>
        static internal int LU;
        ///<summary>Pauses the Game.</summary>
        bool Pause;
        ///<summary>Randomizer for BallGenerate.</summary>
        System.Random r;
        ///<summary>Housechores variables.</summary>
        int i,j;
        ///<summary>Housechores variables.</summary>
        Vector2 ll,rr;
        float timer;
        ///<summary>Constructor! 'doh'!.</summary>
        public GameScreen(Color c,float GF=0.6f,byte MB=20)
        {
            r=new System.Random();
            BGColor=c;GroundFriction=GF;
            MaxBalls=MB;
            G.Dimensions(out Width,out Height);
            MidScreen=new Vector2(Width/2,Height/2);
            bouncers=new Rectangle[4];
            bl=new Ball[MaxBalls];
            bd=new Board[4];timer=i=j=0;
            currentstate=GState.Play1H;CurrentBalls=0;
            Pause=false;
            Player.PlayerInit(out Home,out Away);
            ll=rr=Vector2.Zero;
            G.Dimensions(out Width,out Height);
            AddBall();
        }
        void SwitchControl()
        {
            Home.Input^=Away.Input;
            Away.Input^=Home.Input;
            Home.Input=Home.Input^Away.Input;
        }
        ///<summary>Adds a ball in the game.</summary>
        void AddBall()
        {
            if(CurrentBalls<MaxBalls)
            {
                Vector2 iv;
                int i=r.Next(8);
                switch(i)
                {
                    case 0:
                        iv=new Vector2(1,0);
                        break;
                    case 1:
                        iv=new Vector2(1,1);
                        break;
                    case 2:
                        iv=new Vector2(0,1);
                        break;
                    case 3:
                        iv=new Vector2(-1,1);
                        break;
                    case 4:
                        iv=new Vector2(-1,0);
                        break;
                    case 5:
                        iv=new Vector2(-1,-1);
                        break;
                    case 6:
                        iv=new Vector2(0,-1);
                        break;
                    default:
                        iv=new Vector2(1,-1);
                        break;
                }
                iv*=3;
                bl[CurrentBalls++]=new Ball(MidScreen,iv);
            }
        }
        ///<summary>Removes a Ball.</summary>
        void RemoveBall(int index)
        {
            if(CurrentBalls>0){CurrentBalls--;bl[index]=bl[CurrentBalls];}
        }
        ///<summary>Collision Handler for Ball and Board/Bouncer.</summary>
        void BlBdColl(ref Ball xx,Rectangle rr)
        {
            var z=Collide(xx.x,rr);    //Check for Collision
            if(z!=0)
            {
                if(z==1)xx.v.X*=-1;                //Reflect X-Axis
                else xx.v.Y*=-1;                   //Reflect Y-Axis
                Vector2 add=new Vector2((xx.x.X+LU/2)-(rr.X+rr.Width/2)
                    ,(xx.x.Y+LU/2)-(rr.Y+rr.Height/2));
                add/=add.Length();
                Vector2.Add(ref xx.v,ref add,out xx.v);
                AddBall();
            }
        }
        ///<summary>Collision Handler for Bouncer and Board.</summary>
        void BdBncColl(int ibd,int bc)
        {
            if(bouncers[bc].Intersects(bd[ibd].bounds))
            {
                Vector2.Multiply(ref bd[ibd].v,-LU,out bd[ibd].v);
            }
        }
        byte Collide(Vector2 x,Rectangle r2)
        {
            if(x.X<r2.X){if(x.X+LU<r2.X)return 0;}
            else if(r2.X+r2.Width<x.X)return 0;
            if(x.Y<r2.Y){if(x.Y+LU<r2.Y)return 0;}
            else if(r2.Y+r2.Height<x.Y)return 0;
            float k1,k2;
            k1=x.X<r2.X?x.X+LU-r2.X:r2.X+r2.Width-x.X;
            k2=x.Y<r2.Y?x.Y+LU-r2.Y:r2.Y+r2.Height-x.Y;
            k1=k1<0?-k1:k1;
            k2=k2<0?-k2:k2;
            return (byte)(k1<k2?1:2);
        }
        public void LoadContent()
        {
            G.Dimensions(out Width,out Height);
            LU=Height/50;
            MidScreen=new Vector2(Width/2,Height/2);
            ballTexture=new Texture2D(G.Gdm.GraphicsDevice,LU,LU);
            var l=LU*LU;
            var cc=new Color[l];
            for(i=0;i<l;i++){cc[i]=Color.White;}
            ballTexture.SetData<Color>(cc);
            for(i=0;i<4;i++){bouncers[i]=new Rectangle(0,0,4*LU,4*LU);}
            bouncers[1].Y=bouncers[3].Y=Height-(4*LU);
            bouncers[2].X=bouncers[3].X=Width-(4*LU);
            bd[0]=new Board(new Vector2(0,MidScreen.Y),false,Color.Red);
            bd[1]=new Board(new Vector2(Width-LU,MidScreen.Y),false,Color.Red);
            bd[2]=new Board(new Vector2(MidScreen.X,0),true,Color.Blue);
            bd[3]=new Board(new Vector2(MidScreen.X,Height-LU),true,Color.Blue);
        }
        public void Update(GameTime gt)
        {
            Home.SendInput(out ll,out rr);
            bd[0].Update(ll,GroundFriction);
            bd[1].Update(rr,GroundFriction);
            Away.SendInput(out ll,out rr);
            bd[2].Update(ll,GroundFriction);
            bd[3].Update(rr,GroundFriction);
            for(i=0;i<CurrentBalls;i++)
            {
                bl[i].Update();                             //Update BallPosition
                for(int j=0;j<4;j++)
                {
                    BlBdColl(ref bl[i],bd[j].bounds);       //Check For Ball-Board Collision 
                    BlBdColl(ref bl[i],bouncers[j]);        //Check Ball-Bouncer Collision
                }
                while(i<CurrentBalls&&Inside(bl[i]))
                {RemoveBall(i);}                            //Remove if Outside Bounds
            }
            BdBncColl(0,0);                                 //Check Board-Bouncer Collision
            BdBncColl(0,1);
            BdBncColl(1,2);
            BdBncColl(1,3);
            BdBncColl(2,0);
            BdBncColl(2,2);
            BdBncColl(3,1);
            BdBncColl(3,3);                                 //UpTo Here
            var k=Keyboard.GetState();
            if(k.IsKeyDown(Keys.Space)){AddBall();}
            if(k.IsKeyDown(Keys.X)){G.ThisGame.Exit();}
        }
        bool Inside(Ball b)=>b.x.X<0||b.x.Y<0||b.x.X+LU>Width||b.x.Y+LU>Height;
        public void Draw(GameTime gt)
        {
            G.Gdm.GraphicsDevice.Clear(BGColor);
            G.Sb.Begin();
            for(i=0;i<CurrentBalls;i++){bl[i].Draw();}
            for(i=0;i<4;i++){bd[i].Draw();G.Sb.Draw(ballTexture,bouncers[i],Color.White);}
            G.Sb.End();
        }
    }
    ///<summary>The Entity playing the game and updating inputs</summary>
    internal struct Player
    {
        ///<summary>The Name of the player</summary>
        internal string Name;
        ///<summary>The input ID of the player
        ///
        ///0-3 : GamePad
        ///
        ///4 : Keyboard
        ///
        ///Input>4 : AI</summary>
        internal int Input;
        ///<summary>The Score Credited to the player</summary>
        internal ulong Score;
        ///<summary>i is the intput ID,</summary>
        internal void SendInput(out Vector2 a,out Vector2 b)
        {
            if(Input>4){a=b=Vector2.Zero;}
            else if(Input==4)
            {
                a=b=Vector2.Zero;
                var keys=Keyboard.GetState();
                if(keys.IsKeyDown(Keys.A)){a.X=-1;}
                else if(keys.IsKeyDown(Keys.D)){a.X=1;}
                if(keys.IsKeyDown(Keys.W)){a.Y=1;}
                else if(keys.IsKeyDown(Keys.S)){a.Y=-1;}
                if(keys.IsKeyDown(Keys.Left)){b.X=-1;}
                else if(keys.IsKeyDown(Keys.Right)){b.X=1;}
                if(keys.IsKeyDown(Keys.Up)){b.Y=1;}
                else if(keys.IsKeyDown(Keys.Down)){b.Y=-1;}
            }
            else
            {
                var state=GamePad.GetState(Input);
                a=state.ThumbSticks.Left;
                b=state.ThumbSticks.Right;
            }
        }
        internal static void PlayerInit
        (out Player P1,out Player P2,string a="Player 1",string b="Player 2")
        {
            int i,j;
            P1=P2=new Player();
            P1.Name=a;P2.Name=b;
            P1.Score=P2.Score=0;
            int[] input={5,5};
            for(i=j=0;i<4&&j<2;i++){if(GamePad.GetState(i).IsConnected){input[j++]=i;}}
            if(j<2)input[j++]=4;
            P1.Input=input[0];
            P2.Input=input[1];
        }
    }
    internal struct Board
    {
        internal Vector2 x,v;
        bool Horizontal;
        internal Rectangle bounds;
        Color c;
        public Board(Vector2 ix,bool H,Color cc)
        {
            v=Vector2.Zero;
            Horizontal=H;
            x=ix;
            c=cc;
            bounds=H?new Rectangle((int)x.X,(int)x.Y,8*GameScreen.LU,GameScreen.LU):
                new Rectangle((int)x.X,(int)x.Y,GameScreen.LU,GameScreen.LU*8);
        }
        internal void Update(Vector2 stick,float Friction)
        {
            stick*=GameScreen.LU;
            if(Horizontal){stick.Y=0;}
            else{stick.X=0;stick.Y*=-1;}
            var fv=Vector2.Multiply(v,-Friction);
            Vector2.Add(ref stick,ref fv,out fv);
            Vector2.Add(ref v,ref fv,out v);
            Vector2.Add(ref x,ref v,out x);
            bounds.X=(int)x.X;
            bounds.Y=(int)x.Y;
        }
        internal void Draw(){G.Sb.Draw(GameScreen.ballTexture,bounds,c);}
    }
    internal struct Ball
    {
        internal Vector2 x,v;
        public Ball(Vector2 ix,Vector2 iv){x=ix;v=iv;}
        public void Update(){Vector2.Add(ref x,ref v,out x);}
        public void Draw(){G.Sb.Draw(GameScreen.ballTexture,x,Color.White);}
    }
}