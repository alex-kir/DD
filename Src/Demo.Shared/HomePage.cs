using System;

namespace Demo.Shared
{
    public class HomePage : DDNavigationView.Page
    {
        DDVerticalFlowView flowView;
        DDScrollView scrollView;

        public HomePage():base(500, 500)
        {
            this.SubViews.Add(new DDScrollView(Size.Width, Size.Height)
                {
                    Position = Size * DDVector.CenterMiddle,
                    ContentView = (flowView = new DDVerticalFlowView(Size.Width, Size.Height)
                        {
                            AutoresizingMask = DDView.Autoresizing.Bottom | DDView.Autoresizing.Width,
                        }),
                }.Assign(ref scrollView));

            for (int i = 0; i < 100; i++)
            {
                var view = MyScene.GetView("Какой то русский текст ({0})".Format(i), () => {
                    NavigationView.PushView(new HomePage());
                });
                flowView.SubViews.Add(view);
            }

//            scrollView.
        }
    }
}

