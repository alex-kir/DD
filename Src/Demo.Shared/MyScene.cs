using System;

namespace Demo.Shared
{
    public class MyScene : DDWindow
    {
        DDNavigationView navigationView = null;
        DDVerticalFlowView flowView = null;
        public MyScene() : base()
        {
            DDFont.Default = new DDFont("roboto_fnt");

            this.RootView = navigationView = new DDNavigationView(Size.Width, Size.Height){
                Position = Size * DDVector.CenterMiddle,
                SubViews = {
                    new DDScrollView(Size.Width, Size.Height)
                    {
                        Position = Size * DDVector.CenterMiddle,
                        ContentView = (flowView = new DDVerticalFlowView(Size.Width, Size.Height){
                            AutoresizingMask = DDView.Autoresizing.Bottom | DDView.Autoresizing.Width,
//                            Position = Size * DDVector.CenterMiddle,
                        })
                        ,
                    },
                }
            };

            flowView.SubViews.Add(new DDTextView("11111 111 1111 111111", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("2 22222 2222 22222", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("33333 3 333 333333 333", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("Какой то русский текст", Size.Width, 72){ BackgroundColor = DDColor.Gray });
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));
            flowView.SubViews.Add(new DDTextView("44 44444 444 4444444 44 4 444 ", Size.Width, 72));

//            var spr = Children.Add(new DDSprite("block"));
//            spr.Color = DDColor.White;
//            spr.Position = this.Size * DDVector.CenterMiddle;
//
//            spr.StartAction(aa => aa.Repeat(aa.MoveTo(3, 100, 100).EaseNurbs(0, 0, 1, 1) + aa.MoveTo(3, 300, 300).EaseNurbs(0, 0, 1, 1)));
        }
    }
}

