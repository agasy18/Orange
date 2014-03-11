using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orange
{
    public static class OrangeAlgorithm
    {
        public static int Write(Pixel[] pixels, Pixel[] hInfo, byte rounding)
        {
    
            int j = 0;
            
            int li = 0;
            for (int i = 0; i < pixels.Length - 3 && j < hInfo.Length; i++)
            {
                var a = pixels[i + 0];
                var b = pixels[i + 1];
                var c = pixels[i + 2];
                var h = hInfo[j];
                if (a.R > 250 -h.R || a.G >250- h.G || a.B >250- h.B)
                {
                    continue;
                }
                if (b.R <3+ h.R || b.G <3+ h.G || b.B <3+ h.B)
                {
                    i++;
                    continue;
                }
                if (a.EqualsByRounding(b, rounding) && b.EqualsByRounding(c, rounding))
                {
                    
                    var pr = ((a.R + b.R) / 2);
                    var pg = ((a.G + b.G) / 2);
                    var pb = ((a.B + b.B) / 2);

                    c.R = (byte)pr;
                    c.G = (byte)pg;
                    c.B = (byte)pb;

                    a.R = (byte)(pr + h.R);
                    a.G = (byte)(pg + h.G);
                    a.B = (byte)(pb + h.B);

                    b.R = (byte)(pr - h.R);
                    b.G = (byte)(pg - h.G);
                    b.B = (byte)(pb - h.B);

                    Normalize(pixels, li, i - li);
                    i += 2;
                    li=i+1;
                    j++;
                }
            }

            Normalize(pixels, li,pixels.Length-li);

            return j;
        }



        private static void Normalize(Pixel [] pixels,int offset,int len)
        {
            for (int i =offset ; i <offset + len; i++)
            {
                Normalize(pixels, i);
            }
        }

        private static void Normalize(Pixel[] ps, int i)
        {
            int vaR = -1;
            int vaG = -1;
            int vaB = -1;

            int vbR = -1;
            int vbG = -1;
            int vbB = -1;

            int vcR = -1;
            int vcG = -1;
            int vcB = -1;

            //va
            if (i + 2 < ps.Length)
            {
                var b = ps[i + 1];
                var c = ps[i + 2];

                vaR = c.R + c.R - b.R;
                vaG = c.G + c.G - b.G;
                vaB = c.B + c.B - b.B;
            }


            //vb
            if (i - 1 > 0 && i + 1 < ps.Length)
            {
                var a = ps[i - 1];
                var c = ps[i + 1];

                vbR = c.R + c.R - a.R;
                vbG = c.G + c.G - a.G;
                vbB = c.B + c.B - a.B;
            }

            //vc
            if (i - 2 > 0)
            {
                var a = ps[i - 2];
                var b = ps[i - 1];

                vcR = (a.R + b.R) / 2;
                vcG = (a.G + b.G) / 2;
                vcB = (a.B + b.B) / 2;
            }

            

            for (int t = 0; t < 255; t++)
            {
                for (int k = 0; k < 3; k++)
                {
                    var p = ps[i];
                    var d = (t % 2 == 0) ? 1 : -1;
                    d *= t / 2;
                    int R = p.R;
                    int G = p.G;
                    int B = p.B;

                    if (k == 0)
                    {
                        R += d;
                    }
                    else if (k == 1)
                    {
                        G += d;
                    }
                    else
                    {
                        B += d;
                    }

                    var va = R == vaR && G == vaG && B == vaB;
                    var vb = R == vbR && G == vbG && B == vbB;
                    var vc = R == vcR && G == vcG && B == vcB;
                    var v = R < 0 || R > 255 || G < 0 || G > 255 || B < 0 || B > 255;
                    if (!(va || vb || vc || v))
                    {
                        if (d != 0)
                        {
                            p.R = (byte)R;
                            p.G = (byte)G;
                            p.B = (byte)B;
                        }
                        t = 10000;
                        break;
                    }
                }
                
            }
            
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcePixels"></param>
        /// <param name="infoBase"></param>
        /// <param name="rounding"></param>
        /// <returns>capasity by pixels</returns>
        public static int CalculeteMinCapacity(Pixel[] sourcePixels, byte infoBase, byte rounding)
        {
            int res = 0;
            for (int i = 0; i < sourcePixels.Length - 3; i++)
            {
                var a = sourcePixels[i + 0];
                var b = sourcePixels[i + 1];
                var c = sourcePixels[i + 2];
                byte maxUseValue = (byte)(250 - infoBase);
                if (a.R > maxUseValue || a.G > maxUseValue || a.B > maxUseValue)
                {
                    continue;
                }
                if (b.R <3+ infoBase || b.G <3+ infoBase || b.B <3+ infoBase)
                {
                    i++;
                    continue;
                }
                if (a.EqualsByRounding(b, rounding) && b.EqualsByRounding(c, rounding))
                {
                    res++;
                    i += 2;
                }
            }
            return  res;
        }


        public static List<Pixel> Read(Pixel[] pixels)
        {
            List<Pixel> info = new List<Pixel>();
            for (int i = 0; i < pixels.Length-3; i++)
            {
                var a = pixels[i + 0];
                var b = pixels[i + 1];
                var c = pixels[i + 2];
                var d1 = a - c;
                var d2 = c - b;
                if (d1 == d2)
                {
                    info.Add(d1);
                    i += 2;
                }
            }
            return info;
        }
    }
}
