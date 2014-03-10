using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Orange
{
    /// <summary>
    /// Interaction logic for ReadWindow.xaml
    /// </summary>
    public partial class ReadWindow : Window
    {
        public ReadWindow()
        {
            InitializeComponent();
        }

        void Open(Uri uri)
        {
            var image = new BitmapImage(uri);
            preImage.Source = image;
            var len = image.PixelHeight * image.PixelWidth;
            var pixels = new Pixel[len];
            var sourcePixelsBin = new byte[len * 4];
            image.CopyPixels(sourcePixelsBin, image.PixelWidth * 4, 0);
            for (int i = 0; i < len; i++)
            {
                pixels[i] = new Pixel(sourcePixelsBin[4 * i + 0], sourcePixelsBin[4 * i + 1], sourcePixelsBin[4 * i + 2], sourcePixelsBin[4 * i + 3]);
            }
            var pInfo = OrangeAlgorithm.Read(pixels);
            if (pInfo.Any())
            {
                var pBase = pInfo[0];
                var infoBase = pBase.R + pBase.G + pBase.B;
                baseLabel.Content = infoBase;
                var infoBinWithOldBase = ConvertToArray(pInfo, 1);
                byte[] infoBin = Unbase(infoBinWithOldBase, infoBase);
                textSizeLabel.Content = infoBin.Length;
                infoTextBox.Text = System.Text.Encoding.ASCII.GetString(infoBin);
            }
        }

        private byte[] Unbase(byte[] arr, int infoBase)
        {
            int contLen = ((int)Math.Log(255, infoBase) + 1);
            byte[] res = new byte[arr.Length / contLen];

            for (int i = 0; i < res.Length; i++)
            {
                int v = 0;
                for (int k = contLen - 1; k >= 0; k--)
                {
                    v += pow(infoBase, k) * arr[(i + 1) * contLen - k - 1];
                }
                res[i] = (byte)v;
            }
            return res;
        }

        int pow(int x, int n)
        {
            var res = 1;
            while (n-- > 0)
            {
                res *= x;
            }
            return res;
        }

        private byte[] ConvertToArray(List<Pixel> pInfo, int offset)
        {
            byte[] res = new byte[(pInfo.Count - offset) * 3];
            int i = 0;
            for (int k = offset; k < pInfo.Count; k++)
            {
                var p = pInfo[k];
                res[i++] = p.R;
                res[i++] = p.G;
                res[i++] = p.B;
            }
            return res;
        }

        public Uri FileUri { get; set; }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Open(FileUri);
        }
    }
}
