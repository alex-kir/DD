//
//  DDScene.cs
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

#if !NETFX_CORE
using System.IO;
#else
using WindowsStore.Compatibility.IO;
#endif

public class DDScene : DDNode
{

    bool adjusted = false;

    public DDScene()
        : base()
    {
        AnchorPoint = DDVector.LeftBottom;
        Size = DDDirector.Instance.WinSize;
        Position = DDVector.Zero; // ignored for root node like scene.
    }

    public void AdjustWidth(float width)
    {
        if (adjusted)
            throw new Exception("Scene already adjusted");
        adjusted = true;

        {
            var wsz = DDDirector.Instance.WinSize;
            this.Size = (wsz.Width > wsz.Height) ? new DDVector(wsz.Height, wsz.Width) : wsz;
            this.Size = this.Size * (width / this.Size.Width);
        }

        Action correctScaleRotateAnchor = () =>
        {
            var wsz = DDDirector.Instance.WinSize;
            if (wsz.Width > wsz.Height)
            {
                this.Rotation = -90;
                this.AnchorPoint = DDVector.RightBottom;
                Scale = DDMath.MinXOrY(new DDVector(wsz.Height, wsz.Width) / this.Size);
            }
            else
            {
                this.Rotation = 0;
                this.AnchorPoint = DDVector.LeftBottom;
                Scale = DDMath.MinXOrY(wsz / this.Size);
            }
        };

        correctScaleRotateAnchor();

        this.RunAction(new DDUpdate(() =>
        {
            correctScaleRotateAnchor();
        }));
    }

    public void AdjustHeight(float height)
    {
        if (adjusted)
            throw new Exception("Scene already adjusted");
        adjusted = true;

        {
            var wsz = DDDirector.Instance.WinSize;
            this.Size = (wsz.Width < wsz.Height) ? new DDVector(wsz.Height, wsz.Width) : wsz;
            this.Size = this.Size * (height / this.Size.Height);
        }

        Action correctScaleRotateAnchor = () =>
        {
            var wsz = DDDirector.Instance.WinSize;
            if (wsz.Width < wsz.Height)
            {
                this.Rotation = -90;
                this.AnchorPoint = DDVector.RightBottom;
                Scale = DDMath.MinXOrY(new DDVector(wsz.Height, wsz.Width) / this.Size);
            }
            else
            {
                this.Rotation = 0;
                this.AnchorPoint = DDVector.LeftBottom;
                Scale = DDMath.MinXOrY(wsz / this.Size);
            }
        };

        correctScaleRotateAnchor();

        this.RunAction(new DDUpdate(() =>
        {
            correctScaleRotateAnchor();
        }));
    }

    public static List<DDNode> LoadCbbLevel(string levelName)
    {
        List<DDNode> ret = new List<DDNode>();

        var bytes = DDFile.GetBytes(levelName);
        Dictionary<object, object> plist = DDXml.DecodePlist(bytes);

        var nodeGraph = plist["nodeGraph"] as Dictionary<object, object>;
        var children = nodeGraph["children"] as List<object>;
        foreach (Dictionary<object, object> child in children)
        {
            var properties = child["properties"] as List<object>;
            string displayFrame = null;
            DDVector position = DDVector.Zero;
            float rotation = 0;
            foreach (Dictionary<object, object> property in properties)
            {
                var name = property["name"] as string;
                var values = property["value"] as List<object>;

                if (name == "displayFrame")
                {
                    displayFrame = values[1].ToString();
                }
                else if (name == "position")
                {
                    position = new DDVector(DDXml.GetFloat(values, 0), DDXml.GetFloat(values, 1));
                }
                else if (name == "rotation")
                {
                    rotation = DDXml.GetFloat(property, "value", 0);
                }
            }

            if (displayFrame != null)
            {
                var node = new DDSprite(displayFrame);
                node.Position = position;// this.Size * DDVector.LeftTop + position * (DDVector.Horizontal + -DDVector.Vertical);
                node.Rotation = -rotation;
                ret.Add(node);
            }
        }
        return ret;
    }

    public static List<DDNode> LoadGleedLevel(string fileName, string layerName)
    {
        List<DDNode> ret = new List<DDNode>();

        var root = DDXml.DecodeXml(DDFile.GetBytes(fileName));

        var items = from node1 in root.AllNodes
                    where node1.Name == "Layer" && node1.Attributes["Name"] == layerName
                    from node2 in node1.AllNodes
                    where node2.Name == "Item"
                    select node2;

        foreach (var item in items.ToList())
        {
            var positionNode = item.ChildNodes.Find(it => it.Name == "Position");
            var posxNode = positionNode.ChildNodes.Find(it => it.Name == "X");
            var posyNode = positionNode.ChildNodes.Find(it => it.Name == "Y");

            var rotationNode = item.ChildNodes.Find(it => it.Name == "Rotation");

            var scaleNode = item.ChildNodes.Find(it => it.Name == "Scale");
            var scalexNode = scaleNode.ChildNodes.Find(it => it.Name == "X");
            var scaleyNode = scaleNode.ChildNodes.Find(it => it.Name == "Y");

            var flipHorizontallyNode = item.ChildNodes.Find(it => it.Name == "FlipHorizontally");
            var flipVerticallyNode = item.ChildNodes.Find(it => it.Name == "FlipVertically");

            var filenameNode = item.ChildNodes.Find(it => it.Name == "texture_filename");

            string filenameInner = filenameNode.InnerText.Replace('\\', Path.DirectorySeparatorChar);
            string textureName = System.IO.Path.GetFileNameWithoutExtension(filenameInner);

            var sprite = new DDSprite(textureName);
            sprite.Position = new DDVector(
                float.Parse(posxNode.InnerText, System.Globalization.NumberStyles.Any),
                -float.Parse(posyNode.InnerText, System.Globalization.NumberStyles.Any));

            sprite.ScaleX = float.Parse(scalexNode.InnerText, System.Globalization.NumberStyles.Any);
            if (flipHorizontallyNode.InnerText == "true")
                sprite.ScaleX = -sprite.ScaleX;
            sprite.ScaleY = float.Parse(scaleyNode.InnerText, System.Globalization.NumberStyles.Any);
            if (flipVerticallyNode.InnerText == "true")
                sprite.ScaleY = -sprite.ScaleY;

            sprite.Rotation = DDMath.RadianToDegrees(-float.Parse(rotationNode.InnerText, System.Globalization.NumberStyles.Any));

            ret.Add(sprite);
        }

        return ret;
    }

}