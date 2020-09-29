﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoEditor
{
    public partial class EditPhoto : Form
    {
        Bitmap transformedBitmap;

        private void InvertColors()
        {
            for (int y = 0; y < transformedBitmap.Height; y++)
            {
                for (int x = 0; x < transformedBitmap.Width; x++)
                {
                    var color = transformedBitmap.GetPixel(x, y);
                    int newRed = Math.Abs(color.R - 255);
                    int newGreen = Math.Abs(color.G - 255);
                    int newBlue = Math.Abs(color.B - 255);
                    Color newColor = Color.FromArgb(newRed, newGreen, newBlue);
                    transformedBitmap.SetPixel(x, y, newColor);
                }
            }
        }
        private void AlterColors(Color chosenColor)
        {
            for (int y = 0; y < transformedBitmap.Height; y++)
            {
                for (int x = 0; x < transformedBitmap.Width; x++)
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
            }
        }
        // brightness is a value between 0 – 100. Values < 50 makes the image darker, > 50 makes lighter
        private void ChangeBrightness(int brightness)
        {
            // Calculate amount to change RGB
            int amount = Convert.ToInt32(2 * (50 - brightness) * 0.01 * 255);
            for (int y = 0; y < transformedBitmap.Height; y++)
            {
                for (int x = 0; x < transformedBitmap.Width; x++)
                {
                    var color = transformedBitmap.GetPixel(x, y);
                    int newRed = Math.Max(0, Math.Min(color.R - amount, 255));
                    int newGreen = Math.Max(0, Math.Min(color.G - amount, 255));
                    int newBlue = Math.Max(0, Math.Min(color.B - amount, 255));
                    var newColor = Color.FromArgb(newRed, newGreen, newBlue);
                    transformedBitmap.SetPixel(x, y, newColor);
                }
            }
        }

        public EditPhoto(FileInfo file)
        {
            InitializeComponent();
            transformedBitmap = new Bitmap(file.FullName);
            this.ImageBox.Image = transformedBitmap;
            this.Text = file.Name;
            //this.ImageBox.ImageLocation = file.FullName;
        }

        private void EditPhoto_Load(object sender, EventArgs e)
        {

        }

        private void InvertButton_Click(object sender, EventArgs e)
        {
            InvertColors();
            this.Refresh();
        }
    }
}