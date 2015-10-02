using System;
using System.Collections.Generic;
using System.Linq;

public class DDText : DDNode
{
    private float textSize = 32;
    public float TextSize { get{ return textSize; } set { textSize = value; UpdateInnerLabels();} }

    private DDFont font = null;
    public DDFont Font{ get { return font ?? DDFont.Default; } set { font = value; UpdateInnerLabels(); } }

    private string text = "";
    public string Text { get { return text ?? ""; } set{ text = value; UpdateInnerLabels(); } }

    private int textXAlight = 0;
    public int TextXAlight{ get { return textXAlight; } set{ textXAlight = value; UpdateInnerLabels(); } }

    private int textYAlight = 0;
    public int TextYAlight{ get { return textYAlight; } set{ textYAlight = value; UpdateInnerLabels(); } }

    public override DDVector Size { get { return base.Size; } set { base.Size = value; UpdateInnerLabels(); } }

    public DDText()
    {
        
    }

    private void UpdateInnerLabels()
    {
        if (Children == null) // called from constructor of DDNode
            return;
        Children.Clear();
        var words = Text.Split(' ');
        if (words.Length == 0)
            return;

        var scale = TextSize / Font.LineHeight;
        float maxLineWidth = Size.Width / scale;
        var lines = new List<string>{ words[0] };

        for (int i = 1; i < words.Length; i++) {
            var newLine = lines.Last() + " " + words[i];
            var lineWidth = Font.MeasureString(newLine.TrimEnd()).Width;
            if (lineWidth < maxLineWidth) {
                lines[lines.Count - 1] = newLine;
            }
            else {
                lines.Add(words[i]);
            }
        }
        float x = TextXAlight == 0 ? Size.Width / 2 : TextXAlight == -1 ? 0 : Size.Width;
        float ax = TextXAlight == 0 ? 0.5f : TextXAlight == -1 ? 0 : 1;

        float y0 = Size.Height / 2 + (TextSize * (lines.Count - 1)) / 2;
        float y1 = Size.Height - TextSize / 2;
        float y_1 = TextSize * (lines.Count - 1);
        float y = TextYAlight == 0 ? y0 : TextYAlight == -1 ? y_1 : y1;
            
        for (int i = 0; i < lines.Count; i++) {
            this.Children.Add(new DDLabel(Font, lines[i].TrimEnd()){
                Position = new DDVector(x, y - i * TextSize),
                AnchorPoint = new DDVector(ax, 0.5f),
                Scale = scale,
            });
        }
    }
}

