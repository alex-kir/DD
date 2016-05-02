using System;

#if DD_PLATFORM_IOS

using Foundation;
using GLKit;
using OpenGLES;
using OpenTK;
using OpenTK.Graphics.ES20;
using UIKit;

namespace DD.Graphics
{
    internal class DDGraphicsGameViewController : GLKViewController, IGLKViewDelegate
    {
        readonly DDDirector director;
        EAGLContext context;
        DateTime lastUpdateTime;

        public DDGraphicsGameViewController(DDDirector director)
        {
            this.director = director;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            context = new EAGLContext(EAGLRenderingAPI.OpenGLES2);
            if (context == null)
                DDDebug.Log("Failed to create ES context");

            var view = (GLKView)View;
            view.Context = context;
            view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;

            SetupGL();

            director.UpdateVideoInfo((DDVector)View.Bounds.Size * UIScreen.MainScreen.Scale, director.DPI, 0);//director.FrameRate);
            director.LoadInitialScene();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            TearDownGL();

            if (EAGLContext.CurrentContext == context)
                EAGLContext.SetCurrentContext(null);
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();

            if (IsViewLoaded && View.Window == null)
            {
                View = null;

                TearDownGL();

                if (EAGLContext.CurrentContext == context)
                {
                    EAGLContext.SetCurrentContext(null);
                }
            }

            // Dispose of any resources that can be recreated.
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        void SetupGL()
        {
            EAGLContext.SetCurrentContext(context);

            //            LoadShaders();

            //            effect = new GLKBaseEffect();
            //            effect.Light0.Enabled = true;
            //            effect.Light0.DiffuseColor = new Vector4(1.0f, 0.4f, 0.4f, 1.0f);

            //            GL.Enable(EnableCap.DepthTest);
//            GL.Disable(EnableCap.DepthTest);
//            GL.CullFace(CullFaceMode.FrontAndBack);
//            GL.Enable(EnableCap.Blend);
//            GL.BlendFunc(BlendingFactorSrc.SrcColor, BlendingFactorDest.OneMinusSrcAlpha);
            DD.Graphics.DDGraphics.SetupGL();

            //            GL.shadeShadeModel(All.Smooth);

            //            GL.Oes.GenVertexArrays(1, out vertexArray);
            //            GL.Oes.BindVertexArray(vertexArray);

            //            GL.GenBuffers(1, out vertexBuffer);
            //            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            //            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(cubeVertexData.Length * sizeof(float)), cubeVertexData, BufferUsage.StaticDraw);

            //            GL.EnableVertexAttribArray((int)GLKVertexAttrib.Position);
            //            GL.VertexAttribPointer((int)GLKVertexAttrib.Position, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(0));
            //            GL.EnableVertexAttribArray((int)GLKVertexAttrib.Normal);
            //            GL.VertexAttribPointer((int)GLKVertexAttrib.Normal, 3, VertexAttribPointerType.Float, false, 24, new IntPtr(12));

            //            GL.Oes.BindVertexArray(0);
        }

        void TearDownGL()
        {
            //            EAGLContext.SetCurrentContext(context);
            //            GL.DeleteBuffers(1, ref vertexBuffer);
            //            GL.Oes.DeleteVertexArrays(1, ref vertexArray);

            //            effect = null;

            //            if (program > 0)
            //            {
            //                GL.DeleteProgram(program);
            //                program = 0;
            //            }
        }

        public override void Update()
        {
            //            DDDebug.Trace();
            //            base.Update();
            var now = DateTime.Now;
            director.OnTick((float)((now - lastUpdateTime).TotalSeconds));
            lastUpdateTime = now;
        }



        void IGLKViewDelegate.DrawInRect(GLKView view, CoreGraphics.CGRect rect)
        {
            //            DDDebug.Trace();
            //            GL.ClearColor(0.65f, 0.65f, 0.65f, 1.0f);
            GL.ClearColor(0xFA / 255f, 0xCE / 255f, 0x8D / 255f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

//            var sz = director.ScreenResolution * 0.5f;
//            Matrix4 matrix1 = Matrix4.Scale(1 / sz.Width, 1 / sz.Height, 1) * Matrix4.CreateTranslation(-1, -1, -1);
//            DDMatrix matrix = new DDMatrix(new DDVector(-1, -1), DDVector.Zero, new DDVector(1 / sz.Width, 1 / sz.Height));

//            using(var m = DDDebug.MeasureOne("draw scene"))
                director.DrawScene();
        }

        public override void TouchesBegan (NSSet touches, UIEvent evt)
        {
            base.TouchesBegan (touches, evt);
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(View);
                var x = location.X * UIScreen.MainScreen.Scale;
                var y = (View.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
                DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, (float)x, (float)y, DDTouchPhase.Began);
            }
        }

        public override void TouchesMoved (NSSet touches, UIEvent evt)
        {
            base.TouchesMoved (touches, evt);
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(View);
                var x = location.X * UIScreen.MainScreen.Scale;
                var y = (View.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
                DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, (float)x, (float)y, DDTouchPhase.Moved);
            }
        }

        public override void TouchesEnded (NSSet touches, UIEvent evt)
        {
            base.TouchesEnded (touches, evt);
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(View);
                var x = location.X * UIScreen.MainScreen.Scale;
                var y = (View.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
                DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, (float)x, (float)y, DDTouchPhase.Ended);
            }
        }

        public override void TouchesCancelled (NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled (touches, evt);
            foreach (UITouch touch in touches)
            {
                var location = touch.LocationInView(View);
                var x = location.X * UIScreen.MainScreen.Scale;
                var y = (View.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
                DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, (float)x, (float)y, DDTouchPhase.Ended);
            }
        }
    }
}

#endif