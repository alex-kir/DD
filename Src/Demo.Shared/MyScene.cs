using System;

namespace Demo.Shared
{
    public class MyScene : DDScene
    {
        public MyScene()
        {
            var spr = Children.Add(new DDSprite("block"));
            spr.Color = DDColor.White;
            spr.Position = this.Size * DDVector.CenterMiddle;

            spr.StartAction(aa => aa.Repeat(aa.MoveTo(3, 100, 100).EaseNurbs(0, 0, 1, 1) + aa.MoveTo(3, 300, 300).EaseNurbs(0, 0, 1, 1)));
        }
    }
}

