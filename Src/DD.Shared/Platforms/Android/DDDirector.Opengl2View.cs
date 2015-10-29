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


public partial class DDDirector
{
    internal class Opengl2View : AndroidGameView
    {
        readonly DDDirector director;
//        ffShader shader = null;
//        ffFrame currentFrame = new ffFrame();

        int viewportWidth, viewportHeight;

        public Opengl2View(Context context, DDDirector director)
            : base(context)
        {
            this.director = director;
        }

        public Opengl2View (IntPtr handle, Android.Runtime.JniHandleOwnership transfer)
            : base (handle, transfer) // required for xamarin internals
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            director.UpdateWinSize();
            director.SetInitialScene();

//            GL.Enable(EnableCap.Texture2D);           //Enable Texture Mapping ( NEW )
//            GL.ShadeModel(All.Smooth);

//            GL.Enable(EnableCap.Blend);
//            GL.BlendFunc(BlendingFactorSrc.SrcColor, BlendingFactorDest.OneMinusSrcAlpha);

            Run();
        }

//        public void Release()
//        {
//            GGUtils.Delete(ref shader, it => it.Dispose());
//            GGUtils.Delete(ref currentFrame, it => it.Dispose());
//        }

        // This method is called everytime the context needs
        // to be recreated. Use it to set any egl-specific settings
        // prior to context creation
        protected override void CreateFrameBuffer ()
        {
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

//        private OpenTK.Matrix4 GetOrthoMatrix(ffFrame frame)
//        {
//            var orthoMatrix = OpenTK.Matrix4.Identity;
//            float s1 = frame.Height / (float)frame.Width;
//            float s2 = (float)(viewportHeight / (float)viewportWidth);
//            float sx = s2 / s1;
//            float sy = s1 / s2;
//            if (sy < 1)
//                orthoMatrix.M22 = sy;
//            else if (sx < 1)
//                orthoMatrix.M11 = sx;
//            return orthoMatrix;
//        }

       

        // this is called whenever android raises the SurfaceChanged event
        protected override void OnResize (EventArgs e)
        {
            DDDebug.Trace(Width, Height);

            viewportWidth = Width;
            viewportHeight = Height;

            director.UpdateWinSize();

            // the surface change event makes your context
            // not be current, so be sure to make it current again
            //            MakeCurrent ();
            //            Render (null);
//            if (currentFrame.GetPlanesCount() > 0)
//                Render(currentFrame);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            DDDebug.Trace();
//            base.OnRenderFrame(e);
            try {
                MakeCurrent(); // fix: EglSwapBuffers failed with error 12301
            }
            catch {
                // on rotation MakeCurrent may throw exception
                return;
            }

            float t = Math.Min(Math.Max((float)e.Time, 1f / director.FrameRate), 1f);
            director.OnTick(t);

            GL.ClearColor(0, 0.5f, 0, 1);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Viewport(0, 0, viewportWidth, viewportHeight);

            director.DrawScene();
//            shader.UseInRender();

            GL.Finish();
            SwapBuffers();
        }

//        public void Rerender()
//        {
//            GGDirector.Instance.PostOnMainThread(() => {
//                if (currentFrame.GetPlanesCount() > 0)
//                    Render(currentFrame);
//            }, TimeSpan.FromSeconds(0.1)).DoNotAwait();
//        }
//
//        public void Render (ffFrame frame)
//        {
//            //            await GGDirector.UIScheduler;
//
//            GGDirector.UIFactory.StartNew(() => {
//                try {
//                    MakeCurrent(); // fix: EglSwapBuffers failed with error 12301
//                }
//                catch {
//                    // on rotation MakeCurrent may throw exception
//                    return;
//                }
//
//                GL.ClearColor(0, 0, 0, 1);
//                GL.Clear(ClearBufferMask.ColorBufferBit);
//
//                GL.Viewport(0, 0, viewportWidth, viewportHeight);
//
//
//                //                if (frame != null) {
//                currentFrame.Swap(frame);
//                if (shader == null || shader.frameType != currentFrame.Type)
//                    shader = new ffShader(currentFrame.Type);
//
//                var orthoMatrix = GetOrthoMatrix(currentFrame);
//                shader.SetMatrix(orthoMatrix);
//                shader.ActivateFrame(currentFrame);
//                shader.UseInRender();
//                //                    GGDebug.Trace(frame.PTS);
//                //                }
//
//                GL.Finish();
//
//                SwapBuffers();
//            }).LogException().Wait();
//        }

    }
}


#endif
