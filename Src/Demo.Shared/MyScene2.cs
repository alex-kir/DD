using System;

namespace Demo.Shared
{
    public class MyScene2 : DDScene
    {
        public MyScene2()
        {
            DDFont.Default = new DDFont("roboto_fnt");
            DDLabel label;

            this.Children.AddRange(new DDNode[]{
                (label = new DDLabel("Hello world"){
                    Position = this.Size * DDVector.CenterMiddle,
                    Animations = {
                        { "moving", (DDAnimations.MoveTo(1, 100, 200) + DDAnimations.MoveTo(1, 400, 200)).Repeat() },

                    }
                })
            });

            this.Animations.Add(DDAnimations.Delay(5) + DDAnimations.Exec(()=> label.Animations.Remove("moving")));
        }
    }
}

