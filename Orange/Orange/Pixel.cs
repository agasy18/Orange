using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orange
{
    public class Pixel
    {
        public Pixel()
        {

        }
        public Pixel(int r, int g, int b, int a = 0)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
            A = (byte)a;
        }
        public Pixel(byte r, byte g, byte b, byte a = 0)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public Pixel(Pixel pixel)
        {
            R = pixel.R;
            G = pixel.G;
            B = pixel.B;
            A = pixel.A;
        }

        public byte R;
        public byte G;
        public byte B;
        public byte A;

   

        public override int GetHashCode()
        {
            return R << 24 + G << 16 + B << 8 + A;
        }
        public override string ToString()
        {
            return "{" + R + "," + G + "," + B + "," + A + "}";
        }


        public static Pixel operator +(Pixel p1, Pixel p2)
        {
            return new Pixel(p1.R + p2.R, p1.G + p2.G, p1.B + p2.B, Math.Max(p1.A, p2.A));
        }
        public static Pixel operator -(Pixel p1, Pixel p2)
        {
            return new Pixel(p1.R - p2.R, p1.G - p2.G, p1.B - p2.B, Math.Max(p1.A, p2.A));
        }
        public static bool operator ==(Pixel p1, Pixel p2)
        {

            return (!object.Equals(p1, null) && !object.Equals(p1, null) && p1.R == p2.R && p1.G == p2.G && p1.B == p2.B) || (object.Equals(p1, null) && !object.Equals(p1, null));
        }
        public static bool operator !=(Pixel p1, Pixel p2)
        {
            return !(p1 == p2);
        }
        public override bool Equals(object obj)
        {
            return this == (Pixel)obj;
        }



        public bool EqualsByRounding(Pixel p,byte rounding)
        {
            return EqualsByRounding(R, p.R, rounding) && EqualsByRounding(G, p.G, rounding) && EqualsByRounding(B, p.B, rounding);
        }

        private bool EqualsByRounding(byte a, byte b, byte d)
        {
            if (a>b)
            {
                return a - b <= d;
            }
            return b - a <= d;
        }

    }

}
