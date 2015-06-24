//
//  DDDirector_Android.cs
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
using System.Linq;


#if DD_PLATFORM_ANDROID

using System;
using OpenTK.Graphics.ES11;
using Android.Views;
using Android.App;
using Android.OS;
using Android.Content.PM;

public partial class DDDirector
{
    class GLView : OpenTK.Platform.Android.AndroidGameView
    {
        DDRenderer renderer;

        public GLView(Android.Content.Context context, DDRenderer rndr)
            : base(context)
        {
            renderer = rndr;
        }

        // This gets called when the drawing surface is ready
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GL.Enable(All.Texture2D);			//Enable Texture Mapping ( NEW )
            GL.ShadeModel(All.Smooth); 			//Enable Smooth Shading
            //GL.ClearColor(0.0f, 0.0f, 0.0f, 0.5f); 	//Black Background
            //GL.ClearDepth(1.0f); 					//Depth Buffer Setup
            //GL.Enable(All.DepthTest); 			//Enables Depth Testing
            //GL.DepthFunc(All.Lequal); 			//The Type Of Depth Testing To Do

            GL.Hint(All.PerspectiveCorrectionHint, All.Nicest);

            //GL.BlendFunc(All.SrcAlpha, All.OneMinusSrcAlpha);
            GL.BlendFunc(All.One, All.OneMinusSrcAlpha);
            GL.Enable(All.Blend);

            DDDirector.Instance.UpdateWinSize();

            try
            {
                if (DDDirector.Instance._initialScene != null)
                {
                    DDDirector.Instance.SetScene(DDDirector.Instance._initialScene());
                    DDDirector.Instance._initialScene = null;
                }
            }
            catch (Exception ex)
            {
                DDDebug.LogException(ex);
                //System.Diagnostics.Debugger.Break();
            }

            // Run the render loop
            Run();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            DDDirector.Instance.UpdateWinSize();
        }

        // This gets called on each frame render
        protected override void OnRenderFrame(OpenTK.FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            float t = Math.Min(Math.Max((float)e.Time, 1f / DDDirector.Instance.FrameRate), 1f);
            DDDirector.Instance.OnTick(t);

            GL.MatrixMode(All.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, DDDirector.Instance.WinSize.Width, 0, DDDirector.Instance.WinSize.Height, -1.0f, 1.0f);

            GL.MatrixMode(All.Modelview);

            GL.ClearColor(0f, 0f, 0f, 1.0f);
            //GL.Clear((uint)All.ColorBufferBit);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.Disable(All.DepthTest);
            GL.Disable(All.Lighting);

            DDDirector.Instance.OnDraw(renderer);

            SwapBuffers();
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

    GLView _glView;
    Func<DDScene> _initialScene;
    public Android.App.Activity Activity { get; private set; }

    public void Quit()
    {
//        UnityEngine.Application.Quit();
    }

    public void StopAllAudioEffects()
    {
        //throw new NotImplementedException();
    }

    private void UpdateWinSize()
    {
        WinSize = _glView.Size;
    }

    public void OnCreate(Activity activity1, Bundle bundle, ScreenOrientation orientation, int frameRate, DDRenderer rndr, Func<DDScene> scene)
    {
        this.Activity = activity1;
        this.FrameRate = frameRate;
        this.Activity.RequestWindowFeature(WindowFeatures.NoTitle);
        this.Activity.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        this.Activity.RequestedOrientation = orientation;

        _glView = new GLView(this.Activity, rndr);
        this.Activity.SetContentView(_glView);

        DPI = DDUtils.GetDPI();
        DDTouchDispatcher.Instance.GetHashCode();
        _initialScene = scene;

    }

    public void OnPause(Activity activity1)
    {
        _glView.Pause();
    }

    public void OnResume(Activity activity1)
    {
        _glView.Resume();
    }
}

#endif