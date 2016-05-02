//
//  DDDirector.Opengl2View.cs
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

#if DD_PLATFORM_ANDROID

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Android.Content;
using Android.Graphics;
using Android.Opengl;
using Android.Util;
using Android.Views;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.ES20;
using OpenTK.Platform.Android;

namespace DD.Graphics
{
    internal class DDGraphicsAndroidGameView : AndroidGameView
    {
        readonly DDDirector director;

        int viewportWidth, viewportHeight;

        public DDGraphicsAndroidGameView(Context context, DDDirector director)
            : base(context)
        {
            this.director = director;
        }

        public DDGraphicsAndroidGameView (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base (handle, transfer) // required for xamarin internals
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            DDDebug.Trace();

            base.OnLoad(e);
            director.UpdateVideoInfo(Size, DDUtils.GetDPI(), 60);
            director.LoadInitialScene();

            MakeCurrent();
            DDGraphics.SetupGL();

            Run();
        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer ()
        {
            DDDebug.Trace();
            this.ContextRenderingApi = GLVersion.ES2;

            // the default GraphicsMode that is set consists of (16, 16, 0, 0, 2, false)
            try {
                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex) {
                ex.LogException();
            }

            // this is a graphics setting that sets everything to the lowest mode possible so
            // the device returns a reliable graphics setting.
            try {
                GraphicsMode = new AndroidGraphicsMode(0, 0, 0, 0, 0, false);

                // if you don't call this, the context won't be created
                base.CreateFrameBuffer();
                return;
            }
            catch (Exception ex) {
                ex.LogException();
            }
            throw new Exception("Can't load egl, aborting");

        }

        // this is called whenever android raises the SurfaceChanged event
        protected override void OnResize (EventArgs e)
        {
            DDDebug.Trace(Width, Height);

            viewportWidth = Width;
            viewportHeight = Height;

            director.UpdateVideoInfo(this.Size, director.DPI, 0);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            using (var measure = DDDebug.Measure("OnUpdateFrame")) {
//            DDDebug.Trace(e.Time);
                base.OnUpdateFrame(e);
                float t = Math.Min(1f, (float)e.Time);
                director.OnTick(t);
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            using (var measure = DDDebug.Measure("OnRenderFrame")) {
//            DDDebug.Trace();
                try {
                    MakeCurrent(); // fix: EglSwapBuffers failed with error 12301
                }
                catch (Exception ex) {
                    ex.LogException();
                    // on rotation MakeCurrent may throw exception
                    return;
                }

                GL.ClearColor(0, 0.5f, 0, 1);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Viewport(0, 0, viewportWidth, viewportHeight);

                director.DrawScene();

                GL.Finish();
                SwapBuffers();
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            bool ret = false;
            for (int i = 0, n = e.PointerCount; i < n; i++)
            {
                int id = e.GetPointerId(i);
                float x = e.GetX(i);
                float y = Height - e.GetY(i);
                //DDDebug.Log("action = ", e.Action, "/", e.ActionMasked, "; id = ", id, "; x = ", x, "; y = ", y);
                if (e.ActionMasked == MotionEventActions.Down)
                {
                    DDTouchDispatcher.Instance.OnTouch(id, x, y, DDTouchPhase.Began);
                    ret = true;
                }
                else if (e.Action == MotionEventActions.Move)
                {
                    DDTouchDispatcher.Instance.OnTouch(id, x, y, DDTouchPhase.Moved);
                    ret = true;
                }
                else if (e.Action == MotionEventActions.Up)
                {
                    DDTouchDispatcher.Instance.OnTouch(id, x, y, DDTouchPhase.Ended);
                    ret = true;
                }
            }

            // -----------------------------------------

            var touches = Enumerable.Range(0, e.PointerCount)
                .Select(it => {
                    int id = e.GetPointerId(it);
                    float x = e.GetX(it);
                    float y = Height - e.GetY(it);

                    var touch = new DDTouch();
                    touch.Position = new DDVector(x,y);
                    touch.Finger = id;

                    if (e.ActionMasked == MotionEventActions.Down)
                        touch.Phase = DDTouchPhase.Began;
                    else if(e.Action == MotionEventActions.Move)
                        touch.Phase = DDTouchPhase.Moved;
                    else if (e.Action == MotionEventActions.Up)
                        touch.Phase = DDTouchPhase.Ended;
                    return touch;
                }).ToArray();

            DDTouchDispatcher.Instance.OnTouches(touches);

            ret = true;
            return ret;// base.OnTouchEvent(e);
        }

    }
}


#endif
