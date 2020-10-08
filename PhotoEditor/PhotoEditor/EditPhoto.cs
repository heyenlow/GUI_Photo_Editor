using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoEditor
{
    public partial class EditPhoto : Form
    {
        Bitmap BeforeTransfromation;
        Bitmap transformedBitmap;
        ProgressBar ProgressBarDialog = new ProgressBar();
        public static bool CancelEdit = false;
        private async Task InvertColors()
        {
            BeforeTransfromation = (Bitmap)transformedBitmap.Clone();
            await Task.Run(() =>
            {
                CancelEdit = false;
                Invoke((Action)delegate ()
                {
                    ProgressBarDialog.Show();
                    ProgressBarDialog.ProgressBarLimits(0, transformedBitmap.Height);
                });
                int i = 0;
                for (int y = 0; y < transformedBitmap.Height && !CancelEdit; y++)
                {
                    for (int x = 0; x < transformedBitmap.Width && !CancelEdit; x++)
                    {
                        var color = transformedBitmap.GetPixel(x, y);
                        int newRed = Math.Abs(color.R - 255);
                        int newGreen = Math.Abs(color.G - 255);
                        int newBlue = Math.Abs(color.B - 255);
                        Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                        transformedBitmap.SetPixel(x, y, newColor);
                  
                    }
                    i++;
                    Invoke((Action)delegate ()
                    {
                        ProgressBarDialog.updateProgress(i);
                    });
                }
                if (CancelEdit)
                {
                    
                    Console.WriteLine("Cancel Here");
                    Invoke((Action)delegate ()
                    {
                        this.ImageBox.Image = BeforeTransfromation;
                        transformedBitmap = BeforeTransfromation;
                        ProgressBarDialog.Close();
                    });

                }
                ProgressBarDialog = new ProgressBar();
            });
        }
        private async Task AlterColors(Color chosenColor)
        {
            BeforeTransfromation = (Bitmap)transformedBitmap.Clone();
            await Task.Run(() =>
            {
                CancelEdit = false;
                Invoke((Action)delegate ()
                {
                    ProgressBarDialog.Show();
                    ProgressBarDialog.ProgressBarLimits(0, transformedBitmap.Height);
                });
                int i = 0;
                for (int y = 0; y < transformedBitmap.Height && !CancelEdit; y++)
                    {
                    for (int x = 0; x < transformedBitmap.Width && !CancelEdit; x++)
                        //6
                    {
                        var color = transformedBitmap.GetPixel(x, y);
                        int ave = (color.R + color.G + color.B) / 3;
                        double percent = ave / 255.0;
                        int newRed = Convert.ToInt32(chosenColor.R * percent);
                        int newGreen = Convert.ToInt32(chosenColor.G * percent);
                        int newBlue = Convert.ToInt32(chosenColor.B * percent);
                        var newColor = Color.FromArgb(newRed, newGreen, newBlue);
                        transformedBitmap.SetPixel(x, y, newColor);
                    }
                    i++;
                    Invoke((Action)delegate ()
                    {
                        ProgressBarDialog.updateProgress(i);
                    });
                }
                if (CancelEdit)
                {
                    this.ImageBox.Image = BeforeTransfromation;
                    transformedBitmap = BeforeTransfromation;
                    Console.WriteLine("Here");
                    Invoke((Action)delegate ()
                    {
                        ProgressBarDialog.Close();
                    });

                }
                ProgressBarDialog = new ProgressBar();
            });
        }
        // brightness is a value between 0 – 100. Values < 50 makes the image darker, > 50 makes lighter
        private async Task ChangeBrightness(int brightness)
        {
            BeforeTransfromation = (Bitmap)transformedBitmap.Clone();
            await Task.Run(() =>
            {
                CancelEdit = false;
                Invoke((Action)delegate ()
                {
                    ProgressBarDialog.Show();
                    ProgressBarDialog.ProgressBarLimits(0, transformedBitmap.Height);
                });
                int i = 0;
                // Calculate amount to change RGB
                int amount = Convert.ToInt32(2 * (50 - brightness) * 0.01 * 255);
                for (int y = 0; y < transformedBitmap.Height && !CancelEdit; y++)
                {
                    for (int x = 0; x < transformedBitmap.Width && !CancelEdit; x++)
                    {
                        var color = transformedBitmap.GetPixel(x, y);
                        int newRed = Math.Max(0, Math.Min(color.R - amount, 255));
                        int newGreen = Math.Max(0, Math.Min(color.G - amount, 255));
                        int newBlue = Math.Max(0, Math.Min(color.B - amount, 255));
                        var newColor = Color.FromArgb(newRed, newGreen, newBlue);
                        transformedBitmap.SetPixel(x, y, newColor);
                    }
                    i++;
                    Invoke((Action)delegate ()
                    {
                        ProgressBarDialog.updateProgress(i);
                    });
                }

            if (CancelEdit)
            {
                this.ImageBox.Image = BeforeTransfromation;
                transformedBitmap = BeforeTransfromation;
                Console.WriteLine("Here");
                Invoke((Action)delegate ()
                {
                    ProgressBarDialog.Close();
                });

            }
            ProgressBarDialog = new ProgressBar();
        });
        }

        public EditPhoto(FileInfo file)
        {
            InitializeComponent();
            transformedBitmap = new Bitmap(file.FullName);
            BeforeTransfromation = new Bitmap(file.FullName);
            this.ImageBox.Image = transformedBitmap;
            this.Text = file.Name;
            //this.ImageBox.ImageLocation = file.FullName;
        }

        private void EditPhoto_Load(object sender, EventArgs e)
        {

        }

        private void InvertButton_Click(object sender, EventArgs e)
        {
            InvertButton_ClickAsync(sender, e);
        }

        private async void InvertButton_ClickAsync(object sender, EventArgs e)
        {
            this.Enabled = false;
            await InvertColors();
            this.Enabled = true;
            this.Refresh();
        }

        private void ImageBox_Click(object sender, EventArgs e)
        {

        }

        private async void ColorButton_Click(object sender, EventArgs e)
        {
            ColorDialog ColorDialog1 = new ColorDialog();
            if(ColorDialog1.ShowDialog() == DialogResult.OK)
            {
                this.Enabled = false;
                await AlterColors(ColorDialog1.Color);
                this.Enabled = true;
                this.Refresh();
            }
        }

        private async void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.Enabled = false;
            await ChangeBrightness(trackBar1.Value);
            this.Enabled = true;
            this.Refresh();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                ImageBox.Image.Save(ImageBox.Name, ImageFormat.Jpeg);
            }
            catch (Exception)
            {
                Console.WriteLine("Saving Error");
            }
            Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
