//
//  DDDirector_iOS.cs
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
#if DD_PLATFORM_IOS

using System;
using MonoTouch.CoreAnimation;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;
using OpenTK;
//using OpenTK.Graphics.ES20;
using OpenTK.Platform.iPhoneOS;
using All1 = OpenTK.Graphics.ES11.All;
using GL1 = OpenTK.Graphics.ES11.GL;

public partial class DDDirector
{
	UIWindow window;
	OpenGLViewController viewController;

	public EAGLRenderingAPI ContextRenderingApi {
		get
		{
			return ((EAGLView)viewController.View).ContextRenderingApi;
		}
	}

	public bool OnFinishedLaunching(UIApplication app, NSDictionary options)
	{
		window = new UIWindow (UIScreen.MainScreen.Bounds);
		
		// load the appropriate UI, depending on whether the app is running on an iPhone or iPad
//		if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) {
//			viewController = new OpenGLViewController ("OpenGLViewController_iPhone", null);
//		} else {
//			viewController = new OpenGLViewController ("OpenGLViewController_iPad", null);
//		}

		viewController = new OpenGLViewController ();

		window.RootViewController = viewController;

		app.SetStatusBarHidden(true, false);

		// make the window visible
		window.MakeKeyAndVisible ();

		WinSize = new DDVector(UIScreen.MainScreen.Bounds.Size) * UIScreen.MainScreen.Scale;
		FrameRate = 60;
		return true;
	}

	[Register ("OpenGLViewController")]
	public partial class OpenGLViewController : UIViewController
	{
//		public OpenGLViewController (string nibName, NSBundle bundle) : base (nibName, bundle)
//		{
//		}
		
		new EAGLView View { get { return (EAGLView)base.View; } }

		public override void LoadView ()
		{
			base.View = new EAGLView ();
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillResignActiveNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StopAnimating ();
			}, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.DidBecomeActiveNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StartAnimating ();
			}, this);
			NSNotificationCenter.DefaultCenter.AddObserver (UIApplication.WillTerminateNotification, a => {
				if (IsViewLoaded && View.Window != null)
					View.StopAnimating ();
			}, this);
		}
		
		protected override void Dispose (bool disposing)
		{
			base.Dispose (disposing);
			
			NSNotificationCenter.DefaultCenter.RemoveObserver (this);
		}
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			View.StartAnimating ();
		}
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			View.StopAnimating ();
		}
	}

	[Register ("EAGLView")]
	public class EAGLView : iPhoneOSGameView
	{
		[Export("initWithCoder:")]
		public EAGLView (NSCoder coder) : base (coder)
		{
			LayerRetainsBacking = true;
			LayerColorFormat = EAGLColorFormat.RGBA8;
		}

		[Export("init")]
		public EAGLView () : base(System.Drawing.RectangleF.Empty)
		{
			LayerRetainsBacking = true;
			LayerColorFormat = EAGLColorFormat.RGBA8;
		}

		[Export ("layerClass")]
		public static new Class GetLayerClass ()
		{
			return iPhoneOSGameView.GetLayerClass ();
		}
		
		protected override void ConfigureLayer (CAEAGLLayer eaglLayer)
		{
			eaglLayer.Opaque = true;
		}
		
		protected override void CreateFrameBuffer ()
		{
//			try {
//				ContextRenderingApi = EAGLRenderingAPI.OpenGLES2;
//				base.CreateFrameBuffer ();
//			} catch (Exception) {
				ContextRenderingApi = EAGLRenderingAPI.OpenGLES1;
				base.CreateFrameBuffer ();
//			}
			
//			if (ContextRenderingApi == EAGLRenderingAPI.OpenGLES2)
//				LoadShaders ();
		}
		
		protected override void DestroyFrameBuffer ()
		{
			base.DestroyFrameBuffer ();
//			DestroyShaders ();
		}
		
		#region DisplayLink support
		
		int frameInterval;
		CADisplayLink displayLink;
		
		public bool IsAnimating { get; private set; }
		
		// How many display frames must pass between each time the display link fires.
		public int FrameInterval {
			get {
				return frameInterval;
			}
			set {
				if (value <= 0)
					throw new ArgumentException ();
				frameInterval = value;
				if (IsAnimating) {
					StopAnimating ();
					StartAnimating ();
				}
			}
		}
		
		public void StartAnimating ()
		{
			if (IsAnimating)
				return;
			
			CreateFrameBuffer ();
			CADisplayLink displayLink = UIScreen.MainScreen.CreateDisplayLink (this, new Selector ("drawFrame"));
			displayLink.FrameInterval = frameInterval;
			displayLink.AddToRunLoop (NSRunLoop.Current, NSRunLoop.NSDefaultRunLoopMode);
			this.displayLink = displayLink;
			
			IsAnimating = true;
		}
		
		public void StopAnimating ()
		{
			if (!IsAnimating)
				return;
			displayLink.Invalidate ();
			displayLink = null;
			DestroyFrameBuffer ();
			IsAnimating = false;
		}
		
		[Export ("drawFrame")]
		void DrawFrame ()
		{
			OnRenderFrame (new FrameEventArgs ());
		}
		
		#endregion
		
//		static float[] squareVertices = {
//			-0.5f, -0.33f,
//			0.5f, -0.33f,
//			-0.5f,  0.33f,
//			0.5f,  0.33f,
//		};
//		static byte[] squareColors = {
//			255, 255,   0, 255,
//			0,   255, 255, 255,
//			0,     0,   0,   0,
//			255,   0, 255, 255,
//		};
//		static float transY = 0.0f;
//		const int UNIFORM_TRANSLATE = 0;
//		const int UNIFORM_COUNT = 1;
//		int[] uniforms = new int [UNIFORM_COUNT];
//		const int ATTRIB_VERTEX = 0;
//		const int ATTRIB_COLOR = 1;
//		const int ATTRIB_COUNT = 2;
//		int program;
		
		protected override void OnRenderFrame (FrameEventArgs e)
		{
			base.OnRenderFrame (e);

			float t = Math.Min(Math.Max((float)e.Time, 1f / DDDirector.Instance.FrameRate), 1f);
			DDScheduler.Instance.OnTick(t);

			MakeCurrent ();



			GL1.Enable(All1.Texture2D);			//Enable Texture Mapping ( NEW )
			GL1.ShadeModel(All1.Smooth); 			//Enable Smooth Shading
			//GL.ClearColor(0.0f, 0.0f, 0.0f, 0.5f); 	//Black Background
			//GL.ClearDepth(1.0f); 					//Depth Buffer Setup
			//GL.Enable(All.DepthTest); 			//Enables Depth Testing
			//GL.DepthFunc(All.Lequal); 			//The Type Of Depth Testing To Do
			
			GL1.Hint(All1.PerspectiveCorrectionHint, All1.Nicest);
			
			//GL.BlendFunc(All.SrcAlpha, All.OneMinusSrcAlpha);
			GL1.BlendFunc(All1.One, All1.OneMinusSrcAlpha);
			GL1.Enable(All1.Blend);





			// Replace the implementation of this method to do your own custom drawing.
			GL1.Enable(All1.Texture2D);			//Enable Texture Mapping ( NEW )
			GL1.ClearColor (0.5f, 0.5f, 0.5f, 1.0f);
//			GL1.Clear(All1.ColorBufferBit);
			GL1.ShadeModel(All1.Smooth); 			//Enable Smooth Shading

//			if (ContextRenderingApi == EAGLRenderingAPI.OpenGLES2) {
				// Use shader program.
//				GL.UseProgram (program);
				
				// Update uniform value.
//				GL.Uniform1 (uniforms [UNIFORM_TRANSLATE], transY);
//				transY += 0.075f;
				
				// Update attribute values.
//				GL.VertexAttribPointer (ATTRIB_VERTEX, 2, VertexAttribPointerType.Float, false, 0, squareVertices);
//				GL.EnableVertexAttribArray (ATTRIB_VERTEX);
//				GL.VertexAttribPointer (ATTRIB_COLOR, 4, VertexAttribPointerType.UnsignedByte, true, 0, squareColors);
//				GL.EnableVertexAttribArray (ATTRIB_COLOR);
				
				// Validate program before drawing. This is a good check, but only really necessary in a debug build.
//				#if DEBUG
//				if (!ValidateProgram (program)) {
//					Console.WriteLine ("Failed to validate program {0:x}", program);
//					return;
//				}
//				#endif
//			} else {
//				GL1.MatrixMode (All1.Projection);
//				GL1.LoadIdentity ();
//				GL1.MatrixMode (All1.Modelview);
//				GL1.LoadIdentity ();
//				GL1.Translate (0.0f, (float)Math.Sin (transY) / 2.0f, 0.0f);
//				transY += 0.075f;
//				
//				GL1.VertexPointer (2, All1.Float, 0, squareVertices);
//				GL1.EnableClientState (All1.VertexArray);
//				GL1.ColorPointer (4, All1.UnsignedByte, 0, squareColors);
//				GL1.EnableClientState (All1.ColorArray);
//			}
			
//			GL.DrawArrays (BeginMode.TriangleStrip, 0, 4);

			GL1.MatrixMode(All1.Projection);
			GL1.LoadIdentity();
			GL1.Ortho(0, DDDirector.Instance.WinSize.Width, 0, DDDirector.Instance.WinSize.Height, -1.0f, 1.0f);
			
			GL1.MatrixMode(All1.Modelview);
			
			GL1.ClearColor(0.8f, 0.3f, 0.8f, 1.0f);
			GL1.Clear((uint)All1.ColorBufferBit);
			
			
			GL1.Disable(All1.Lighting);

			try
			{
				if (DDDirector.Instance.Scene != null)
					DDDirector.Instance.Scene.Visit ();
			}
			catch(Exception ex)
			{
				DDDebug.Log(ex);
			}
			SwapBuffers ();
		}
		
//		bool LoadShaders ()
//		{
//			int vertShader, fragShader;
//			
//			// Create shader program.
//			program = GL.CreateProgram ();
//			
//			// Create and compile vertex shader.
//			var vertShaderPathname = NSBundle.MainBundle.PathForResource ("Shader", "vsh");
//			if (!CompileShader (ShaderType.VertexShader, vertShaderPathname, out vertShader)) {
//				Console.WriteLine ("Failed to compile vertex shader");
//				return false;
//			}
//			
//			// Create and compile fragment shader.
//			var fragShaderPathname = NSBundle.MainBundle.PathForResource ("Shader", "fsh");
//			if (!CompileShader (ShaderType.FragmentShader, fragShaderPathname, out fragShader)) {
//				Console.WriteLine ("Failed to compile fragment shader");
//				return false;
//			}
//			
//			// Attach vertex shader to program.
//			GL.AttachShader (program, vertShader);
//			
//			// Attach fragment shader to program.
//			GL.AttachShader (program, fragShader);
//			
//			// Bind attribute locations.
//			// This needs to be done prior to linking.
//			GL.BindAttribLocation (program, ATTRIB_VERTEX, "position");
//			GL.BindAttribLocation (program, ATTRIB_COLOR, "color");
//			
//			// Link program.
//			if (!LinkProgram (program)) {
//				Console.WriteLine ("Failed to link program: {0:x}", program);
//				
//				if (vertShader != 0)
//					GL.DeleteShader (vertShader);
//				
//				if (fragShader != 0)
//					GL.DeleteShader (fragShader);
//				
//				if (program != 0) {
//					GL.DeleteProgram (program);
//					program = 0;
//				}
//				return false;
//			}
//			
//			// Get uniform locations.
//			uniforms [UNIFORM_TRANSLATE] = GL.GetUniformLocation (program, "translate");
//			
//			// Release vertex and fragment shaders.
//			if (vertShader != 0) {
//				GL.DetachShader (program, vertShader);
//				GL.DeleteShader (vertShader);
//			}
//			
//			if (fragShader != 0) {
//				GL.DetachShader (program, fragShader);
//				GL.DeleteShader (fragShader);
//			}
//			
//			return true;
//		}
		
//		void DestroyShaders ()
//		{
//			if (program != 0) {
//				GL.DeleteProgram (program);
//				program = 0;
//			}
//		}
		
		#region Shader utilities
		
//		static bool CompileShader (ShaderType type, string file, out int shader)
//		{
//			string src = System.IO.File.ReadAllText (file);
//			shader = GL.CreateShader (type);
//			GL.ShaderSource (shader, 1, new string[] { src }, (int[])null);
//			GL.CompileShader (shader);
//			
//			#if DEBUG
//			int logLength = 0;
//			GL.GetShader (shader, ShaderParameter.InfoLogLength, out logLength);
//			if (logLength > 0) {
//				var infoLog = new System.Text.StringBuilder ();
//				GL.GetShaderInfoLog (shader, logLength, out logLength, infoLog);
//				Console.WriteLine ("Shader compile log:\n{0}", infoLog);
//			}
//			#endif
//			int status = 0;
//			GL.GetShader (shader, ShaderParameter.CompileStatus, out status);
//			if (status == 0) {
//				GL.DeleteShader (shader);
//				return false;
//			}
//			
//			return true;
//		}
		
//		static bool LinkProgram (int prog)
//		{
//			GL.LinkProgram (prog);
//			
//			#if DEBUG
//			int logLength = 0;
//			GL.GetProgram (prog, ProgramParameter.InfoLogLength, out logLength);
//			if (logLength > 0) {
//				var infoLog = new System.Text.StringBuilder ();
//				GL.GetProgramInfoLog (prog, logLength, out logLength, infoLog);
//				Console.WriteLine ("Program link log:\n{0}", infoLog);
//			}
//			#endif
//			int status = 0;
//			GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
//			if (status == 0)
//				return false;
//			
//			return true;
//		}
		
//		static bool ValidateProgram (int prog)
//		{
//			GL.ValidateProgram (prog);
//			
//			int logLength = 0;
//			GL.GetProgram (prog, ProgramParameter.InfoLogLength, out logLength);
//			if (logLength > 0) {
//				var infoLog = new System.Text.StringBuilder ();
//				GL.GetProgramInfoLog (prog, logLength, out logLength, infoLog);
//				Console.WriteLine ("Program validate log:\n{0}", infoLog);
//			}
//			
//			int status = 0;
//			GL.GetProgram (prog, ProgramParameter.LinkStatus, out status);
//			if (status == 0)
//				return false;
//			
//			return true;
//		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			foreach (UITouch touch in touches)
			{
				var location = touch.LocationInView(this);
				float x = location.X * UIScreen.MainScreen.Scale;
				float y = (this.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
				DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, x, y, DDTouchPhase.Began);
			}
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			foreach (UITouch touch in touches)
			{
				var location = touch.LocationInView(this);
				float x = location.X * UIScreen.MainScreen.Scale;
				float y = (this.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
				DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, x, y, DDTouchPhase.Moved);
			}
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			foreach (UITouch touch in touches)
			{
				var location = touch.LocationInView(this);
				float x = location.X * UIScreen.MainScreen.Scale;
				float y = (this.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
				DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, x, y, DDTouchPhase.Ended);
			}
		}

		public override void TouchesCancelled (NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			foreach (UITouch touch in touches)
			{
				var location = touch.LocationInView(this);
				float x = location.X * UIScreen.MainScreen.Scale;
				float y = (this.Frame.Size.Height - location.Y) * UIScreen.MainScreen.Scale;
				DDTouchDispatcher.Instance.OnTouch((int)touch.Handle, x, y, DDTouchPhase.Ended);
			}
		}
		#endregion
	}
}

#endif