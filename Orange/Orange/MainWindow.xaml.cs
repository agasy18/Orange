using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;



namespace Orange
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 



    public partial class MainWindow : Window
    {


        Pixel[] sourcePixels;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculeteImageCapacity()
        {
            try
            {
                var infoBase = (byte)baseSlider.Value;
                var rounding = (byte)roundingSlider.Value;
                ImagePixelCapacity = OrangeAlgorithm.CalculeteMinCapacity(sourcePixels, infoBase, rounding);
                int byteCount = ImagePixelCapacity * 3 / ((int)Math.Log(255, infoBase) + 1);
                imageCapacityLabel.Content = byteCount;
            }
            catch
            {
                imageCapacityLabel.Content = "Undefind";
            }
        }


        void ReloadOutPicture(Pixel[] destPixels)
        {
            var len = image.PixelHeight * image.PixelWidth;
            var destPixelsBin = new byte[len * 4];
            for (int i = 0; i < len; i++)
            {
                var p = destPixels[i];
                destPixelsBin[4 * i + 0] = p.R;
                destPixelsBin[4 * i + 1] = p.G;
                destPixelsBin[4 * i + 2] = p.B;
                destPixelsBin[4 * i + 3] = p.A;
            }
            outImage.WritePixels(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight), destPixelsBin, image.PixelWidth * 4, 0);
        }


        public BitmapImage image { get; set; }

        public WriteableBitmap outImage { get; set; }


        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                baseValueText.Content = ((int)baseSlider.Value).ToString();
                roundingValueText.Content = ((int)roundingSlider.Value).ToString();
                CalculeteImageCapacity();
            }
            catch { }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                textSizeLabel.Content = infoTextBox.Text.Length.ToString();
            }
            catch { }
        }
        public int ImagePixelCapacity { get; set; }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            if (ImagePixelCapacity > 1)
            {
                var text = infoTextBox.Text;
                byte[] bText = System.Text.Encoding.ASCII.GetBytes(text);
                var newBase = (byte)baseSlider.Value;
                var bInfo = Rebase(bText, newBase);
                var saveInfoSize = bInfo.Length;
                var hInfoSize = 1 + (saveInfoSize - 1) / 3 + 1;
                var hInfo = new Pixel[hInfoSize];
                hInfo[0] = new Pixel(newBase / 3, newBase / 3, newBase - 2 * (newBase / 3));
                CopyToPixel(bInfo, hInfo, saveInfoSize, 1);

                var destPixels = new Pixel[sourcePixels.Length];
                for (int i = 0; i < destPixels.Length; i++)
                {
                    destPixels[i] = new Pixel(sourcePixels[i]);
                }
                
                var size = OrangeAlgorithm.Write(destPixels, hInfo, (byte)roundingSlider.Value);
                int contLen = ((int)Math.Log(255, newBase) + 1);
                savedSizeLabel.Content = size * 3 / contLen;
                ReloadOutPicture(destPixels);
            }
        }

        private void CopyToPixel(byte[] arr, Pixel[] pixels, int length, int offset)
        {
            var fLen = length / 3;

            for (int i = 0; i < fLen; i++)
            {
                pixels[i + offset] = new Pixel(arr[3 * i + 0], arr[3 * i + 1], arr[3 * i + 2]);
            }
            int k = fLen * 3;
            if (k < length)
            {
                pixels[fLen + offset] = new Pixel();
                pixels[fLen + offset].R = arr[k++];
            }
            if (k < length)
            {
                pixels[fLen + offset].G = arr[k++];
            }
        }

        private byte[] Rebase(byte[] info, byte newBase)
        {
            int contLen = ((int)Math.Log(255, newBase) + 1);
            var res = new byte[contLen * info.Length];
            for (int i = 0; i < info.Length; i++)
            {
                Rebase(info[i], newBase, res, contLen * i, contLen);
            }
            return res;
        }

        private void Rebase(byte info, byte newBase, byte[] dest, int offset, int len)
        {
            for (int i = len - 1; i >= 0; i--)
            {
                var m = info % newBase;
                info /= newBase;
                dest[i + offset] = (byte)m;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "OrangeImage";
            dlg.DefaultExt = ".png";
            dlg.Filter = "Portable Network Graphic  (.png)|*.png";

            if (dlg.ShowDialog() == true)
            {
                string filename = dlg.FileName;
                if (filename != string.Empty)
                {
                    Apply_Click(null, null);
                    using (FileStream stream5 = new FileStream(filename, FileMode.Create))
                    {
                        PngBitmapEncoder encoder5 = new PngBitmapEncoder();
                        encoder5.Frames.Add(BitmapFrame.Create(outImage));
                        encoder5.Save(stream5);
                        stream5.Close();
                    }
                }
            }

        }

        private void ReadOpen_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                ReadWindow w = new ReadWindow();
                w.FileUri = new Uri(op.FileName);
                w.Show();
            }
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog op = new Microsoft.Win32.OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                Open(new Uri(op.FileName));
            }
        }

        void Open(Uri uri)
        {
            image = new BitmapImage(uri);
            sourceImage.Source = image;
            var len = image.PixelHeight * image.PixelWidth;
            sourcePixels = new Pixel[len];
            var sourcePixelsBin = new byte[len * 4];
            image.CopyPixels(sourcePixelsBin, image.PixelWidth * 4, 0);
            for (int i = 0; i < len; i++)
            {
                sourcePixels[i] = new Pixel(sourcePixelsBin[4 * i + 0], sourcePixelsBin[4 * i + 1], sourcePixelsBin[4 * i + 2], sourcePixelsBin[4 * i + 3]);
            }
            outImage = new WriteableBitmap(image);
            destImage.Source = outImage;
            CalculeteImageCapacity();
        }


    }
}