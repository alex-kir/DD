//
//  DDDirector_Win32.cs
//
//  DD engine for 2d games and apps: https://code.google.com/p/dd-engine/
//
//  Copyright (c) 2013 - Alexander Kirienko
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//
#if DD_PLATFORM_WIN32

using System;
using System.Drawing;
using System.Windows.Forms;

public partial class DDDirector
{
    public partial class DDForm : Form
    {
        Timer timer = new Timer();

        public DDForm(int width, int height, float frameRate)
        {
            this.DoubleBuffered = true;
            this.ClientSize = new System.Drawing.Size(width, height);

            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;

            timer.Interval = (int)(1000 / frameRate);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Enabled = true;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            float interval = timer.Interval / 1000.0f;

            DDDirector.Instance.OnTick(interval);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.ScaleTransform(1.0f, -1.0f);
            g.TranslateTransform(0, -DDDirector.Instance.WinSize.Height);
            g.Clear(Color.Black);
            DDDirector.Instance.OnDraw(new DDRenderer(g));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                DDDirector.Instance.OnTouch(0, e.X, ClientSize.Height - e.Y, DDTouchPhase.Began);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                DDDirector.Instance.OnTouch(0, e.X, ClientSize.Height - e.Y, DDTouchPhase.Moved);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                DDDirector.Instance.OnTouch(0, e.X, ClientSize.Height - e.Y, DDTouchPhase.Ended);
        }

    }

    private DDForm _form;

    internal void OnMain(int width, int height, int frameRate, Func<DDScene> scene)
    {
        System.Windows.Forms.Application.EnableVisualStyles();
        System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
        WinSize = new DDVector(width, height);
        FrameRate = frameRate;
        _form = new DDForm(width, height, frameRate);

        SetScene(scene());

        System.Windows.Forms.Application.Run(_form);
        _form = null;
    }
}

#endif