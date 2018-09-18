using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace CustomXNA
{
    public struct AnimatedSprite
    {
        internal Texture2D t;
        internal int[] xpos, ypos, next;
        internal int currentframe;
        internal Rectangle r1,r2;
        public AnimatedSprite(Texture2D tex,int[] x,int[] y,int[] n,int w,int h,int sw,int sh)
        {
            currentframe = 0;
            xpos = x;
            ypos = y;
            next = n;
            t = tex;
            r1 = new Rectangle(xpos[0],ypos[0],w,h);
            r2 = new Rectangle(0, 0, sw, sh);
        }
        public AnimatedSprite(Texture2D tex,int w,int h,int xend,int yend,int sw,int sh)
        {
            t = tex;
            currentframe = 0;
            int xlim = tex.Width / w;
            int size = (xlim * yend) - xlim + xend;
            xpos = new int[size];
            ypos = new int[size];
            next = new int[size];
            r1 = new Rectangle(xpos[0], ypos[0], w, h);
            r2 = new Rectangle(0, 0, sw, sh);
            int i, j=size-1, k;
            for(i=0;i<j;i++){next[i] = i + 1;}
            next[j] = 0;
            for (i=k=0;i<size;i++,k++)
            {
                k %= xlim;
                xpos[i] = k * w;
            }
            for (i = k = j = 0; i < size; i++, k++)
            {
                if (k >= xlim) { j++; k = 0; }
                ypos[i] = j * h;
            }
        }
        internal void GoToFrame(int f) { currentframe = f; }
        internal void Update()
        {
            currentframe = next[currentframe];
            r1.X = xpos[currentframe];
            r1.Y = ypos[currentframe];
        }
        internal void Draw(SpriteBatch Sb,Vector2 v)
        {
            r2.X = (int)v.X;
            r2.Y = (int)v.Y;
            Sb.Draw(t,r2,r1,Color.White);
        }
        internal void Draw(SpriteBatch Sb,Point p,Color cc)
        {
            r2.X = p.X;
            r2.Y = p.Y;
            Sb.Draw(t, r2, r1, cc);
        }
        internal void Draw(SpriteBatch Sb)
        {
            Sb.Draw(t, r2, r1, Color.White);
        }
        internal void Draw(SpriteBatch Sb, Color cc)
        {
            Sb.Draw(t,r1,r2,cc);
        }
    }
    public struct BigSprite
    {
        internal int currentframe;
        internal int[] nextTex;
        internal Texture2D[] frameimg;
        internal Rectangle Dest;
        public BigSprite(string s, int count, ContentManager c,int digits)
        {
            currentframe=0;
            frameimg=new Texture2D[count];
            Dest=new Rectangle(0,0,0,0);
            nextTex=new int[count];
            string k;
            for(int i=1;i<=count;i++)
            {
                k=i.ToString();
                while(k.Length<digits){k=string.Concat("0",k);}
                k=string.Concat(s,k);
                frameimg[i-1]=c.Load<Texture2D>(k);
                nextTex[i-1]=i;
            }
            nextTex[count-1]=count-1;
            Dest.Width=frameimg[0].Width;
            Dest.Height=frameimg[0].Height;
        }
        public void Update(){currentframe=nextTex[currentframe];}
        public void Draw(SpriteBatch Sb){Sb.Draw(frameimg[currentframe],Dest,Color.White);}
    }
}