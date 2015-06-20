//
//  DDFlashClip.cs
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
using System.Globalization;

public class DDFlashClip : DDNode
{
    public class DDFlashMatrixTo : DDIntervalAnimation
    {
        float _a1;
        float _b1;
        float _c1;
        float _d1;
        float _x1;
        float _y1;

        float _a2;
        float _b2;
        float _c2;
        float _d2;
        float _x2;
        float _y2;

        float _da;
        float _db;
        float _dc;
        float _dd;
        float _dx;
        float _dy;

        protected DDVector _startPosition;
        protected DDVector _delta;

        public DDFlashMatrixTo(float duration, float a, float b, float c, float d, float x, float y)
            : base(duration)
        {
            _a2 = a;
            _b2 = b;
            _c2 = c;
            _d2 = d;
            _x2 = x;
            _y2 = y;
        }

        protected override void Start(DDNode target)
        {
            base.Start(target);

            DDFlashClip.PosRotScaleToAbcdxy(target.Position, target.RotationXY, target.ScaleXY,
                out _a1, out _b1, out _c1, out _d1, out _x1, out _y1);

            _da = _a2 - _a1;
            _db = _b2 - _b1;
            _dc = _c2 - _c1;
            _dd = _d2 - _d1;
            _dx = _x2 - _x1;
            _dy = _y2 - _y1;
        }

        protected override void Update(DDNode target, float t)
        {
            float a = _a1 + _da * t;
            float b = _b1 + _db * t;
            float c = _c1 + _dc * t;
            float d = _d1 + _dd * t;
            float x = _x1 + _dx * t;
            float y = _y1 + _dy * t;

            DDVector position, rotation, scale;
            DDFlashClip.AbcdToTrs(a, b, c, d, x, y, out position, out rotation, out scale);

            target.Position = position;
            target.RotationXY = rotation;
            target.ScaleXY = scale;
        }
    }


    private static Dictionary<string, DDXml.XmlNode> xmlCache = new Dictionary<string, DDXml.XmlNode>();

    DDRectangle? bounds = null;
	
	class FlashLayer
	{
		public string Name;
		public float Duration;
		public DDNode RootNode;
        public List<KeyValuePair<DDNode, DDIntervalAnimation>> AllNodesActions;
	}

    Dictionary<string, FlashLayer> layers = new Dictionary<string, FlashLayer>();
	FlashLayer currentLayer = null;
	
    public DDFlashClip(string name)
    {
        if (!xmlCache.ContainsKey(name))
            xmlCache[name] = DDXml.DecodeXml(DDFile.GetBytes(name), System.Text.Encoding.UTF8);

        var xml = xmlCache[name];
        var document = xml.AllNodes.First(it => it.Name == "DOMDocument");
        foreach (var ch in LoadFlashObject(document, xml, true))
            Children.Add(ch);

        this.Size = this.GetBounds().Size;
        this.Origin = this.GetBounds().LeftBottom;
    }

    public DDRectangle GetBounds()
    {
        if (!bounds.HasValue)
        {
            DDRectangle ret = new DDRectangle();
            bool first = true;
            var corners = new DDVector[] { DDVector.LeftBottom, DDVector.LeftTop, DDVector.RightBottom, DDVector.RightTop };
            var tr = WorldToNodeTransform();
			foreach (var child in Children.AllRecursive)
            {
                if (child.Size != DDVector.Zero)
                {
                    foreach (var corner in corners)
                    {
                        var world = child.NodeToWorldTransform() * (corner * child.Size);
                        var pt = tr * world;
                        if (first)
                        {
                            first = false;
                            ret = new DDRectangle(pt, pt);
                        }
                        else
                        {
                            ret.Extends(pt);
                        }
                    }
                }
            }
            bounds = ret;
        }
        return bounds.Value;
    }

    public float GetClipDuration()
    {
		return layers.Values.DDMax(it => it.Duration);
    }

    public float GetClipDuration(string name)
    {
        return layers[name].Duration;
    }

    public void SetVisibleLayer(string name)
    {
        foreach (var nameLayer in this.layers)
        {
			nameLayer.Value.RootNode.Visible = (nameLayer.Key == name);
        }
    }

    #region Parsing

    private List<DDNode> LoadFlashObject(DDXml.XmlNode root, DDXml.XmlNode xml, bool isTop)
    {
        List<DDNode> ret = new List<DDNode>();

        foreach (var layer in root.AllNodes.Where(it => it.Name == "DOMLayer").Reverse())
        {
            string layerName = layer.GetAttribute("name", "?");
			
			if (isTop)
			{
				currentLayer = new FlashLayer {
					Name = layerName,
                    AllNodesActions = new List<KeyValuePair<DDNode, DDIntervalAnimation>>()
                };
				layers[layerName] = currentLayer;
			}

			
            var frames = layer.AllNodes.Where(it => it.Name == "DOMFrame").OrderBy(it => int.Parse(it.GetAttribute("index", "0"))).ToList();

            string libraryItemName = null;
            string libraryItemType = null;

            foreach (var frame in frames)
            {
                var elements = frame.ChildNodes.Find(it => it.Name == "elements");
                var xnode = elements.ChildNodes.FirstOrDefault();
                if (xnode != null)
                {
                    var libName = xnode.GetAttribute("libraryItemName", "?");
                    libraryItemType = libraryItemType ?? xnode.Name;
                    libraryItemName = libraryItemName ?? libName;

                    if (libraryItemType != xnode.Name || libraryItemName != libName)
                    {
                        DDDebug.Log("[DDFlashClip], ", root.GetAttribute("name", "?"), " / ", layerName, " layer contains different symbols, ", libraryItemName, " -> ", libName);
                    }
                }
            }

            if (libraryItemName == null || libraryItemType == null)
            {
                DDDebug.Log("[DDFlashClip], ", root.GetAttribute("name", "?"), " / ", layerName, " layer is empty");
                continue;
            }

            List<DDNode> layerNodes = new List<DDNode>();

            foreach (var xnode in frames.First().ChildNodes.Find(it => it.Name == "elements").ChildNodes)
            {
                try
                {
                    string name = xnode.GetAttribute("libraryItemName", "?");

                    float a, b, c, d, x, y;
                    ExtractMatrix(xnode, out a, out b, out c, out d, out x, out y);

                    DDVector position, scale, rotation;
                    AbcdToTrs(a, b, c, d, x, y, out position, out rotation, out scale);

                    DDColor color;
                    ExtractColor(xnode, out color);
                    DDNode layerNode = null;
                    if (xnode.Name == "DOMSymbolInstance")
                    {
                        layerNode = new DDNode();
                        var item = xml.ChildNodes.FirstOrDefault(it => it.Name == "DOMSymbolItem" && it.GetAttribute("name", "") == name);
                        if (item == null)
                        {
                            DDDebug.Log("[DDFlashClip], " + "DOMSymbolItem name='", name, "' not found (try end with)");
                            item = xml.ChildNodes.First(it => it.Name == "DOMSymbolItem" && it.GetAttribute("name", "").EndsWith("/" + name));
                        }
                        foreach (var ch in LoadFlashObject(item, xml, false))
                        {
							layerNode.Children.Add(ch);
                        }
                    }
                    else if (xnode.Name == "DOMBitmapInstance")
                    {
                        layerNode = new DDSprite(name);
                    }
                    else
                    {
                        DDDebug.Log("[DDFlashClip], ", root.GetAttribute("name", "?"), " / ", layerName, " / ", xnode.Name, "=", name, " node is unknown type");
                        continue;
                    }

                    layerNode.AnchorPoint = DDVector.LeftTop;
                    layerNode.Position = position;
                    layerNode.ScaleXY = scale;
                    layerNode.RotationXY = rotation;
                    layerNode.Color = color;
                    layerNodes.Add(layerNode);
                }
                catch (Exception ex) { DDDebug.Log(ex); }
            }

            DDIntervalAnimation action = null;
            if (frames.Count > 1)
            {
                var frame1 = frames[0];
                var xnode1 = frame1.AllNodes.FirstOrDefault(it => it.Name == "DOMSymbolInstance" || it.Name == "DOMBitmapInstance");

                if (xnode1 == null)
                {
                    DDDebug.Log("[DDFlashClip], ", root.GetAttribute("name", "?"), " / ", layerName, ", first frame is empty, ", frame1.GetAttribute("index", "?"));
                    action = new DDDelayTime(0);
                }
                else
                {
                    float duration = 0f / 24f;

                    float a, b, c, d, x, y;
                    ExtractMatrix(xnode1, out a, out b, out c, out d, out x, out y);

                    DDColor color;
                    ExtractColor(xnode1, out color);

                    action = new DDFlashMatrixTo(duration, a, b, c, d, x, y) & new DDColorTo(duration, color);
                }
            }

            for (int frameIndex = 1; frameIndex < frames.Count; frameIndex++)
            {
                var frame1 = frames[frameIndex - 1];
                var frame2 = frames[frameIndex];

                var xnode1 = frame1.AllNodes.FirstOrDefault(it => it.Name == "DOMSymbolInstance" || it.Name == "DOMBitmapInstance");
                var xnode2 = frame2.AllNodes.FirstOrDefault(it => it.Name == "DOMSymbolInstance" || it.Name == "DOMBitmapInstance");

                float duration = float.Parse(frame1.GetAttribute("duration", "1")) / 24f;

                if (xnode1 == null || xnode2 == null)
                {
                    DDDebug.Log("[DDFlashClip], ", root.GetAttribute("name", "?"), " / ", layer.GetAttribute("name", "?"), ", empty frames ", frame1.GetAttribute("index", "?"), "->", frame2.GetAttribute("index", "?"));
                    action = action + new DDDelayTime(duration);
                }
                else
                {
                    float a, b, c, d, x, y;
                    ExtractMatrix(xnode2, out a, out b, out c, out d, out x, out y);

                    DDColor color;
                    ExtractColor(xnode2, out color);

                    action = action + (new DDFlashMatrixTo(duration, a, b, c, d, x, y) & new DDColorTo(duration, color));
                }
            }

            foreach (var layerNode in layerNodes)
            {
                if (action != null)
                {
                    currentLayer.AllNodesActions.Add(new KeyValuePair<DDNode, DDIntervalAnimation>(layerNode, action));
                }
                ret.Add(layerNode);
                if (isTop)
                {
                    float duration = frames.DDSum(frame => float.Parse(frame.GetAttribute("duration", "1")) / 24f);
					layers[layerName].Duration = duration;
					layers[layerName].RootNode = layerNode;
                }
            }
        }
        return ret;
    }

    private static void PosRotScaleToAbcdxy(DDVector position, DDVector rotation, DDVector scale,
        out float a, out float b, out float c, out float d, out float x, out float y)
    {
        float rx = rotation.X / 180f * DDMath.PI;
        float ry = rotation.Y / 180f * DDMath.PI;

        float sx = scale.X;
        float sy = scale.Y;

        a = DDMath.Cos(ry) * sx;
        b = -DDMath.Sin(ry) * sx;
        c = DDMath.Sin(rx) * sy;
        d = DDMath.Cos(rx) * sy;

        x = position.X;
        y = -position.Y;
    }

    private static void AbcdToTrs(float a, float b, float c, float d, float x, float y,
        out DDVector position, out DDVector rotation, out DDVector scale)
    {
        position = new DDVector(x, -y);

        scale = new DDVector(DDMath.Sqrt(a * a + b * b), DDMath.Sqrt(c * c + d * d));
        if (a < 0) scale.X = -scale.X;
        if (d < 0) scale.Y = -scale.Y;

        rotation = new DDVector();
        rotation.X = -DDMath.Atan(-c / d);
        rotation.Y = -DDMath.Atan(b / a);
        rotation = rotation * 180 / DDMath.PI;
    }

    private static void ExtractMatrix(DDXml.XmlNode xnode, out float a, out float b, out float c, out float d, out float x, out float y)
    {
        var xmatrix = xnode.AllNodes.FirstOrDefault(it => it.Name == "Matrix");
        if (xmatrix != null)
        {
            x = float.Parse(xmatrix.GetAttribute("tx", "0"), NumberStyles.Any);
            y = float.Parse(xmatrix.GetAttribute("ty", "0"), NumberStyles.Any);

            a = float.Parse(xmatrix.GetAttribute("a", "1"), NumberStyles.Any);
            b = float.Parse(xmatrix.GetAttribute("b", "0"), NumberStyles.Any);
            c = float.Parse(xmatrix.GetAttribute("c", "0"), NumberStyles.Any);
            d = float.Parse(xmatrix.GetAttribute("d", "1"), NumberStyles.Any);
        }
        else
        {
            x = 0;
            y = 0;

            a = 1;
            b = 0;
            c = 0;
            d = 1;
        }
    }

    private static void ExtractColor(DDXml.XmlNode xnode, out DDColor color)
    {
        var xcolor = xnode.AllNodes.FirstOrDefault(it => it.Name == "Color");
        if (xcolor != null)
        {
            float t = 1 - float.Parse(xcolor.GetAttribute("tintMultiplier", "0"), NumberStyles.Any);
            if (xcolor.GetAttribute("tintColor", "") != "")
            {
                DDDebug.Log("[DDFlashClip], " + xnode.GetAttribute("libraryItemName", "?") + ", tintColor used");
            }

            float r = float.Parse(xcolor.GetAttribute("redMultiplier", t.ToString()), NumberStyles.Any);
            float g = float.Parse(xcolor.GetAttribute("greenMultiplier", t.ToString()), NumberStyles.Any);
            float b = float.Parse(xcolor.GetAttribute("blueMultiplier", t.ToString()), NumberStyles.Any);

            float a = float.Parse(xcolor.GetAttribute("alphaMultiplier", "1"), NumberStyles.Any);

            color = new DDColor(r, g, b, a);
        }
        else
        {
            color = DDColor.White;
        }
    }

    #endregion

    #region Animations

    public class FlashClipAction : DDIntervalAnimation
    {
        public FlashClipAction(float duration)
            : base(duration)
        {
        }

        protected override void Start(DDNode target)
        {
            var clip = target as DDFlashClip;
            foreach (var layer in clip.layers.Values)
            {
                layer.RootNode.Visible = true;
                foreach (var nodeAction in layer.AllNodesActions)
                {
                    DDActionManager.Instance.RemoveAction(nodeAction.Value, nodeAction.Key);
                    DDAnimation.Start(nodeAction.Value, nodeAction.Key);
                }
            }
        }

        protected override void Update(DDNode target, float time01)
        {
            var clip = target as DDFlashClip;
            foreach (var layer in clip.layers.Values)
            {
                foreach (var nodeAction in layer.AllNodesActions)
                {
                    DDIntervalAnimation.Update(nodeAction.Value, nodeAction.Key, time01);
                }
            }
        }
    }

    public class FlashClipNamedAction : DDIntervalAnimation
    {
        string name;
        public FlashClipNamedAction(float duration, string name)
            : base(duration)
        {
            this.name = name;
        }

        protected override void Start(DDNode target)
        {
            var clip = target as DDFlashClip;
            foreach (var layer in clip.layers.Values)
            {
                if (layer.Name == name)
                {
                    layer.RootNode.Visible = true;
                    foreach (var nodeAction in layer.AllNodesActions)
                    {
                        DDAnimation.Start(nodeAction.Value, nodeAction.Key);
                    }
                }
                else
                {
                    layer.RootNode.Visible = false;
                    foreach (var nodeAction in layer.AllNodesActions)
                    {
                        DDActionManager.Instance.RemoveAction(nodeAction.Value, nodeAction.Key);
                    }
                }
            }
        }

        protected override void Update(DDNode target, float time01)
        {
            var clip = target as DDFlashClip;
            foreach (var nodeAction in clip.layers[name].AllNodesActions)
            {
                DDIntervalAnimation.Update(nodeAction.Value, nodeAction.Key, time01);
            }
        }
    }




    #endregion
}