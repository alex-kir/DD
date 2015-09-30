using System;

namespace Demo.Shared
{
    public class MyScene2 : DDScene
    {
        public MyScene2()
        {
//            AdjustWidth(600);

            DDFont.Default = new DDFont("roboto_fnt");
            DDLabel label;

            this.Children.AddRange(new DDNode[]{
                (label = new DDLabel("Hello world"){
                    Position = this.Size * DDVector.CenterMiddle,
                    Animations = {
                        { "moving", (DDAnimations.MoveTo(3, 200, 200).EaseNurbs(0, 0, 1, 1) + DDAnimations.MoveTo(3, 400, 200).EaseNurbs(0, 0, 1, 1)).Repeat() },

                    }
                })
            });

//            this.Animations.Add(DDAnimations.Delay(5) + DDAnimations.Exec(()=> label.Animations.Remove("moving")));
        }
    }
}

