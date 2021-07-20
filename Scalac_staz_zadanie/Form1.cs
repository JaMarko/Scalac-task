using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Scalac_staz_zadanie
{
    public partial class Form1 : Form
    {
        Bitmap loaded_bitmap;
        public Form1()
        {
            InitializeComponent();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            inputBox.Enabled = false;
            outputBox.Enabled = false;
            label3.Visible = false;
            string[] loaded_Files = Directory.GetFiles(@inputBox.Text);
            process_images(loaded_Files);
        }

        private void process_images(string[] files)
        {
            foreach (string file in files)
            {
                pixel_defining(file);   //defines brightness of every pixel
                brightness = count_brightness(frame_pixels, mid_pixels, weights);   //counts overall brightness of a picture
                str_brightness = bright_check(brightness);  //checks whether the picture is "dark" or "bright"
                save_images(file, brightness, str_brightness);  //saves the image to new directory with changed name
            }
            label3.Text = "processing is done!";
            label3.Visible = true;
            button1.Enabled = true;
            inputBox.Enabled = true;
            outputBox.Enabled = true;
        }

        private void save_images(string file, int brightness, string str_brightness)
        {
            file_name = Path.GetFileName(file);
            dot_position = file_name.IndexOf(".");
            file_name = file_name.Insert(dot_position, "_" + str_brightness + "_" + brightness);    //composes final name of the file
            string directory = outputBox.Text + "\\" + file_name;
            loaded_image.Save(directory);
        }

        private string bright_check(int br)
        {
            if (br > cut_off)
                return "dark";
            else
                return "bright";
        }

        private void pixel_defining(string file)
        {
            loaded_image = Image.FromFile(file);
            loaded_bitmap = new Bitmap(loaded_image);
            for (int x = 0; x < loaded_bitmap.Width; x++)
            {
                for (int y = 0; y < loaded_bitmap.Height; y++)
                {
                    Color pixel_color = loaded_bitmap.GetPixel(x, y);
                    if (is_frame(loaded_bitmap, x, y))  //checks whether the pixel is on the edge or in the middle of picture (different importance)
                    {
                        frame_pixels.Add(pixel_color.GetBrightness());
                        weights.Add(frame_weight);
                    }
                    else
                    {
                        mid_pixels.Add(pixel_color.GetBrightness());
                        weights.Add(mid_weight);
                    }
                }
            }
        }

        private bool is_frame(Bitmap pic, int width, int height)
        {
            if (width > pic.Width * frame_ratio && width < pic.Width - (pic.Width * frame_ratio))
            {
                if (height > pic.Height * frame_ratio && height < pic.Height - (pic.Height * frame_ratio))
                    return false;
                else
                    return true;
            }
            else return true;
        }

        private int count_brightness(List<double> frame, List<double> mid, List<double> weights)
        {
            foreach(double x in frame)
            {
                num += x * frame_weight;
            }
            frame.Clear();
            foreach (double x in mid)
            {
                num += x * mid_weight;
            }
            mid.Clear();
            foreach (double x in weights)
            {
                den += x;
            }
            weights.Clear();
            if (den != 0)
                brightness = 100 - (int)(100 * (num / den));
            else
                brightness = 0;
            num = 0;
            den = 0;
            return brightness;
        }
    }
}
