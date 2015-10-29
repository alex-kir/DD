//
//  DDDirector.cs
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
using Android.Widget;



#if DD_PLATFORM_ANDROID

using System;
using System.Linq;
using OpenTK.Graphics.ES11;
using Android.Views;
using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Content;

//using TOpenglView = DDDirector.Opengl2View;
using TOpenglView = DDDirector.GLView;

public partial class DDDirector
{
    public FrameLayout RootLayout { get; private set; }
    TOpenglView _glView;

    Func<DDScene> _initialScene;
    public Android.App.Activity Activity { get; private set; }
    DDRenderer renderer;


    public class ActivityResultArgs
    {
        public readonly int RequestCode;
        public readonly Result ResultCode;
        public readonly Intent Data;

        public ActivityResultArgs(int requestCode, Result resultCode, Intent data)
        {
            RequestCode = requestCode;
            ResultCode = resultCode;
            Data = data;
        }

        public override string ToString()
        {
            return string.Format("[ActivityResultArgs: RequestCode={0}, ResultCode={1}, Data={2}]", RequestCode, ResultCode, Data);
        }
    };

    public event Action<ActivityResultArgs> ActivityResult;

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

    public void OnCreate(Activity activity1, Bundle bundle, ScreenOrientation orientation, int frameRate, DDRenderer renderer, Func<DDScene> scene)
    {
        this.renderer = renderer;
        this.Activity = activity1;
        this.FrameRate = frameRate;
        this.Activity.RequestWindowFeature(WindowFeatures.NoTitle);
        this.Activity.Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
        this.Activity.RequestedOrientation = orientation;

        _initialScene = scene;
        DPI = DDUtils.GetDPI();
        DDTouchDispatcher.Instance.GetHashCode();

        RootLayout = new FrameLayout(this.Activity);
        _glView = new TOpenglView(this.Activity, this);
        RootLayout.AddView(_glView, new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
        this.Activity.SetContentView(RootLayout);


    }

    public void OnPause(Activity activity1)
    {
        _glView.Pause();
    }

    public void OnResume(Activity activity1)
    {
        _glView.Resume();
    }

    public void OnActivityResult(int requestCode, Result resultCode, Intent data)
    {
        if (ActivityResult != null)
            ActivityResult(new ActivityResultArgs(requestCode, resultCode, data));
    }

    void SetInitialScene()
    {
        if (_initialScene != null)
        {
            SetScene(DDDirector.Instance._initialScene());
            _initialScene = null;
        }
    }

    void DrawScene()
    {
        OnDraw(renderer);
    }
}

#endif