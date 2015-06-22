using System;

namespace Demo.Shared
{
    public class MyScene : DDWindow
    {
        DDNavigationView navigationView = null;

        public MyScene() : base()
        {
            DDFont.Default = new DDFont("roboto_fnt");

            this.RootView = navigationView = new DDNavigationView(Size.Width, Size.Height){
                Position = Size * DDVector.CenterMiddle,
            };
            navigationView.PushView(new HomePage());
//            var spr = Children.Add(new DDSprite("block"));
//            spr.Color = DDColor.White;
//            spr.Position = this.Size * DDVector.CenterMiddle;
//            spr.StartAction(aa => aa.Repeat(aa.MoveTo(3, 100, 100).EaseNurbs(0, 0, 1, 1) + aa.MoveTo(3, 300, 300).EaseNurbs(0, 0, 1, 1)));
        }

        public static DDView GetView(string text, Action action)
        {
            return new DDView(500, 72)
            {
                BackgroundColor = DDColor.Gray,
                SubViews =
                {
                    new DDImageView("block", 48, 48)
                    {
                        Position = new DDVector(36, 36),
                        AutoresizingMask = DDView.Autoresizing.Right | DDView.Autoresizing.Top | DDView.Autoresizing.Bottom,
                        BackgroundColor = DDColor.Green,
                    },
                    new DDTextView(text, 500 - 144, 48)
                    {
                        Position = new DDVector(250, 36),
                        AutoresizingMask = DDView.Autoresizing.Width | DDView.Autoresizing.Height,
                        TextAlign = DDVector.LeftMiddle,
                        BackgroundColor = DDColor.Red,
                    },
                    new DDButtonView("", 48, 48){
                        Position = new DDVector(500 - 36, 36),
                        AutoresizingMask = DDView.Autoresizing.Left | DDView.Autoresizing.Height,
                        Action = action,
                        BackgroundColor = DDColor.Magenta,
                    }
                }
            };
        }
    }
}

