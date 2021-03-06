﻿//
//  FadeInAnimation.cs
//
//  Author:
//       Benito Palacios Sánchez <benito356@gmail.com>
//
//  Copyright (c) 2015 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace NinoPatcher
{
    public class Fade : AnimationElement
    {
        private Image image;
        private float alpha;

		private int count;
        private float fadeIncrement;
        private int fadeChangeTick;

        public Fade(int delay, int duration, int steps, Point position,
            int fadeChangeTick, float fadeIncrement, Image image, float startAlpha = 0.0f)
            : base(delay, duration, steps, position)
        {
            this.image = image;
            this.fadeIncrement  = fadeIncrement;
            this.fadeChangeTick = fadeChangeTick;
            this.alpha = startAlpha;
			count = 0;
        }

        public override void Draw(Graphics g)
        {
            ColorMatrix cm = new ColorMatrix();
            cm.Matrix33 = alpha;

            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            Rectangle outputRectangle = new Rectangle(Position, image.Size);
            g.DrawImage(image,
                outputRectangle,
                0, 0, image.Width, image.Height,
                GraphicsUnit.Pixel, ia);
        }

        public override void Update()
        {
            if (fadeChangeTick != -1 && (count != 0 && (count % fadeChangeTick) == 0))
				fadeIncrement *= -1;
			
			count++;
            alpha += fadeIncrement;

            if (alpha > 1) alpha = 1;
            if (alpha < 0) alpha = 0;
        }
    }
}

