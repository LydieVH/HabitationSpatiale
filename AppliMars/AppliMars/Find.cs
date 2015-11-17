using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Input;
using System.IO;
using System.Collections.Generic;

namespace AppliMars {
    public partial class Find : Form {
        public string nextImg = "0";
        int nbIllustrationDone = 0;
        public int time = 0;

        List<double> dilation = new List<double>();
        Graphics g;
        public Image img;
        Point mouseDown;
        // Coordonnées de l'image quand on déplace avec la souris
        int startx = 0, starty = 0, _ticks;
        // Coordonnées réelles de l'image
        int imgx = 0, imgy = 0;
        int deltaX = 0, deltaY = 0, deltaWheelX = 0, deltaWheelY = 0, order = 0;
        bool mousepressed = false;
        float zoom = 1, zoomMin, zoomMax = 2, rightX = 0, rightY = 0;

        public Find() : this(0, new int[] { 0, 0, 0 }) { }

        public Find(int o) : this(o, new int[] { 0, 0, 0 }) { }

        public Find(int o, int[] l) {
            InitializeComponent();

            int sum = 0;
            foreach (int i in l) {
                sum += i;
            }

            string imagefilename = "";
            if (nbIllustrationDone == 0) {
                imagefilename = @"nanedi.jpg";
            } 

            img = Image.FromFile(imagefilename);
            g = this.CreateGraphics();

            // Fit whole image
            zoom = Math.Min(
               ((float)pictureBox.Height / (float)img.Height) * (img.VerticalResolution / g.DpiY),
               ((float)pictureBox.Width / (float)img.Width) * (img.HorizontalResolution / g.DpiX)
            );
            // Fit width
            zoom = ((float)pictureBox.Width / (float)img.Width) * (img.HorizontalResolution / g.DpiX);
            zoomMin = zoom;
            pictureBox.Paint += new PaintEventHandler(imageBox_Paint);

            Console.WriteLine(this.ClientSize.Width);
            Console.WriteLine(this.ClientSize.Height);
            Console.WriteLine(order);

        }


        private void pictureBox_MouseMove(object sender, EventArgs e) {
            System.Windows.Forms.MouseEventArgs mouse = e as System.Windows.Forms.MouseEventArgs;

            if (mouse.Button == MouseButtons.Left) {
                Point mousePosNow = mouse.Location;

                // Distance dont la souris a été bougé depuis qu'on a appuyé sur le bouton
                deltaX = mousePosNow.X - mouseDown.X;
                deltaY = mousePosNow.Y - mouseDown.Y;

                /*
                La taille de la pictureBox ne bouge pas. On n'y a donc que la taille de l'image qui change et qui dépasse par moment de la pictureBox.
                Sur l'axe X on a l'équation : imgx*zoom + pictureBox.Width + rightX*zoom = img.Width*zoom
                Sur l'axe Y on a l'équation : imgy*zoom + pictureBox.Height + rightY*zoom = img.Height*zoom
                */

                // Comme tempoImgx et tempoImgy seront négatifs quand l'image depassera, on les met positives pour que les équations ci-dessus soient bonnes
                int tempoImgx = (int)(startx + (deltaX / zoom)) * -1;
                int tempoImgy = (int)(starty + (deltaY / zoom)) * -1;
                rightX = ((img.Width * zoom) - pictureBox.Width - (tempoImgx * zoom)) / zoom;
                rightY = ((img.Height * zoom) - pictureBox.Height - (tempoImgy * zoom)) / zoom;

                /*
                Pour ne pas qu'il y ait une marge en haut de l'image il faut imgy<=0
                Pour ne pas qu'il y ait une marge à gauche de l'image il faut imgx<=0
                Pour ne pas qu'il y ait une marge en bas de l'image il faut rightY<=0
                Pour ne pas qu'il y ait une marge à droite de l'image il faut rightX<=0
                */

                if (imgx <= 0 && rightX >= 0) {
                    imgx = (int)(startx + (deltaX / zoom));
                } else if (imgx > 0 && rightX >= 0) { // Pour qu'il n'y ait pas de marge à gauche
                    imgx = 0;
                    deltaX = 0;
                    startx = 0;
                } else if (imgx <= 0 && rightX < 0) { // Pour qu'il n'y ait pas de marge à droite
                    startx = imgx = (int)((img.Width * zoom - pictureBox.Width) / zoom * -1); // On prend l'équation (1) =  imgx*zoom + pictureBox.Width + rightX*zoom = img.Width*zoom	       Avec rightX = 0
                    rightX = 0;
                }

                if (imgy <= 0 && rightY >= 0) { // Pour qu'il n'y ait pas de marge en haut
                    imgy = (int)(starty + (deltaY / zoom));
                } else if (imgy > 0 && rightY >= 0) {
                    imgy = 0;
                    deltaY = 0;
                    starty = 0;
                } else if (imgy <= 0 && rightY < 0) { // Pour qu'il n'y ait pas de marge en bas
                    starty = imgy = (int)((img.Height * zoom - pictureBox.Height) / zoom * -1); // On prend l'équation (2) = imgy*zoom + pictureBox.Height + rightY*zoom = img.Height*zoom		  Avec rightY = 0
                    rightY = 0;
                }

                if (imgx <= 0 && imgy <= 0 && rightX >= 0 && rightY >= 0) {// Ne rafraichir l'image que s'il n'y a pas de marges

                    pictureBox.Refresh();
                }
            }
        }

        private void imageBox_MouseDown(object sender, EventArgs e) {
            System.Windows.Forms.MouseEventArgs mouse = e as System.Windows.Forms.MouseEventArgs;

            if (mouse.Button == MouseButtons.Left) {
                if (!mousepressed) {
                    mousepressed = true;
                    mouseDown = mouse.Location;
                    startx = imgx;
                    starty = imgy;
                }
            }
        }

        private void imageBox_MouseUp(object sender, EventArgs e) {
            mousepressed = false;
        }

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e) {
            float oldzoom = zoom;

            if (e.Delta > 0) {
                if (zoom > zoomMax) {
                    return;
                }

                zoom += 0.1F;
            } else if (e.Delta < 0) {
                if (zoom <= zoomMin)
                    return;
                zoom = Math.Max(zoom - 0.1F, 0.01F);
            }

            System.Windows.Forms.MouseEventArgs mouse = e as System.Windows.Forms.MouseEventArgs;
            Point mousePosNow = mouse.Location;

            int x = mousePosNow.X - pictureBox.Location.X;    // Localisation de la souris sur la pictureframe-+
            int y = mousePosNow.Y - pictureBox.Location.Y;

            int oldimagex = (int)(x / oldzoom);  // Position de l'image maintenant
            int oldimagey = (int)(y / oldzoom);

            int newimagex = (int)(x / zoom);     // Position de l'image avec le nouveau zoom 
            int newimagey = (int)(y / zoom);

            deltaWheelX = newimagex - oldimagex;
            deltaWheelY = newimagey - oldimagey;

            imgx = deltaWheelX + imgx;
            imgy = deltaWheelY + imgy;
            if (imgx > 0) {
                imgx = 0;
            }
            if (imgy > 0) {
                imgy = 0;
            }
            rightX = ((img.Width * zoom) - pictureBox.Width - (imgx * zoom)) / zoom;
            rightY = ((img.Height * zoom) - pictureBox.Height - (imgy * zoom)) / zoom;

            if (((-1 * imgx * zoom) + pictureBox.ClientSize.Width) > (img.Width * zoom)) {
                imgx = (int)((img.Width * zoom - pictureBox.Width) / zoom * -1);
            }
            if (((-1 * imgy * zoom) + pictureBox.ClientSize.Height) > (img.Height * zoom)) {
                imgy = (int)((img.Height * zoom - pictureBox.Height) / zoom * -1);
            }

            pictureBox.Refresh();

        }

        private void imageBox_Paint(object sender, PaintEventArgs e) {
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.ScaleTransform(zoom, zoom);
            e.Graphics.DrawImage(img, imgx, imgy);

        }

        private void realTime_Tick(object sender, EventArgs e) {
            _ticks++;
            this.Text = _ticks.ToString();

            if (_ticks == 10) {
                this.Text = "Done";
                realTime.Stop();
            }
        }


        private void Find_FormClosed(object sender, FormClosedEventArgs e) {
            Application.Exit();
        }

    }
}