using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;

namespace BMP_to_Carray2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            button1.Dock = DockStyle.Fill;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private string GenerateConstByteArrayDeclaration(byte[] array, string variableName, Bitmap bmp)
        {
            var builder = new StringBuilder();

            // Add dimensions comment
            builder.AppendLine($"// Dimensions: {bmp.Width}x{bmp.Height}");

            builder.AppendLine($"const uint8_t {variableName}[] =");
            builder.AppendLine("{");

            int bytesPerRow = (bmp.Width + 7) / 8; // Round up to the nearest byte

            for (int i = 0; i < array.Length; i++)
            {
                builder.Append($"0x{array[i]:X2}, ");
                if ((i + 1) % bytesPerRow == 0) builder.AppendLine();
            }

            builder.AppendLine("};");
            return builder.ToString();
        }

        private byte[] ConvertBitmapToByteArray(Bitmap bmp)
        {
            int bytesPerRow = (bmp.Width + 7) / 8; // Round up to the nearest byte
            byte[] formattedData = new byte[bytesPerRow * bmp.Height];

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Stride is the width of a single row of the image in bytes, taking into account any padding added to align rows to 4 bytes
            int stride = bmpData.Stride;

            unsafe
            {
                byte* ptr = (byte*)bmpData.Scan0;
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bytesPerRow; x++)
                    {
                        formattedData[y * bytesPerRow + x] = ptr[y * stride + x];
                    }
                }
            }

            bmp.UnlockBits(bmpData);
            return formattedData;
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length > 0)
            {
                string bmpPath = files[0];
                var bmp = new Bitmap(bmpPath);
                var bmp1bpp = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format1bppIndexed);
                var byteArray = ConvertBitmapToByteArray(bmp1bpp);
                var arrayName = Path.GetFileNameWithoutExtension(bmpPath);
                arrayName = arrayName.Replace(" ", "_");
                var byteArrayString = GenerateConstByteArrayDeclaration(byteArray, arrayName, bmp);
                textBox1.Text = GenerateConstByteArrayDeclaration(byteArray, arrayName, bmp1bpp);
                textBox1.Visible = true;
                button1.Dock = DockStyle.Bottom;
                button1.Text = "Drag New Image Here";
                button1.BackgroundImage = null;
                button2.Visible = true;
                textBox1.Dock = DockStyle.Fill;


            }
        }

        private void button1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Clipboard.SetText(textBox1.Text);
        }
    }
}
