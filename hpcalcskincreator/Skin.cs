using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace hpcalcskincreator
{
    public class Skin
    {
        public Rectangle Size = new Rectangle();
        public Rectangle Screen = new Rectangle();
        public Point[] border;
        public Key[] Keys = new Key[51];
        public Rectangle Maximized = new Rectangle();
        public Color ScreenForeground = Color.Black;
        public Color ScreenBackground = Color.White;
        public int[] Matrix = {320,240,320,0}; // still to be reverse-engineered
    }
    public class Key
    {
        public int ID;
        public char? KeyName = null;
        public Rectangle Position = new Rectangle();
        public int[] P1, P2, P3; // still to be reverse-engineered
        public override string ToString()
        {
            string str = "key=";
            if (this.KeyName != null)
                str += "\"" + this.KeyName + "\",";
            str += this.ID + "," + this.Position.Left + "," + this.Position.Top + "," + this.Position.Right + "," + this.Position.Bottom + ",";
            str += this.P1.ToString() + "," + this.P2.ToString() + "," + this.P3.ToString();
            return str;
        }
    }
}
