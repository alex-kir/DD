//
//  DDXml.cs
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
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

//using System.Xml;

public class DDXml
{
    public class XmlNode
    {
        public string Name { get; private set; }
        private Dictionary<string, string> _attributes;
        public Dictionary<string, string> Attributes
        {
            get
            {
                if (_attributes == null)
                    _attributes = new Dictionary<string, string>();
                return _attributes;
            }
        }
        public string GetAttribute(string attrName, string df)
        {
            if (_attributes != null && _attributes.ContainsKey(attrName))
                return _attributes[attrName];
            return df;
            
        }

        public string InnerText { get; set; }
        public List<XmlNode> ChildNodes { get; private set; }
        
        public IEnumerable<XmlNode> AllNodes
        {
            get
            {
                yield return this;
                foreach (var child in ChildNodes)
                {
                    foreach (var node in child.AllNodes)
                    {
                        yield return node;
                    }
                }
            }
        }

        public XmlNode(string name, Dictionary<string, string> attrs)
        {
            Name = name;
            _attributes = attrs;
            ChildNodes = new List<XmlNode>();
        }
    };

    public static Dictionary<object, object> DecodePlist(byte[] bytes)
    {
        var node = DecodeXml(bytes);
        var dict = node.ChildNodes[0].ChildNodes[0]; // Select(/plist/dict)
        return ParseDictionary(dict.ChildNodes);
    }

    public static XmlNode DecodeXml(byte[] bytes, Encoding encoding = null)
    {
		encoding = encoding ?? Encoding.UTF8;
		
        Stack<XmlNode> nodes = new Stack<XmlNode>();
        nodes.Push(new XmlNode("", new Dictionary<string,string>()));

        foreach (var tag in EnumerateXmlTags(bytes, encoding))
        {
            if (tag.StartsWith("<"))
            {
                // tag

                if (tag.StartsWith("<?"))
                {
                }
                else if (tag.StartsWith("<!"))
                {
                }
                else if (tag.StartsWith("</"))
                {
                    nodes.Pop();
                }
                else
                {
                    var trimmed = tag.Trim('<', '/', '>');
                    var entries = EnumerateTagBody(trimmed).ToArray();
                    var name = entries[0];

                    Dictionary<string, string> attrs = null;
                    for (int i = 1; i + 1 < entries.Length; i += 2)
                    {
                        if (attrs == null)
                            attrs = new Dictionary<string, string>();
                        attrs[entries[i]] = entries[i + 1];
                    }

                    if (tag.EndsWith("/>"))
                    {
                        nodes.First().ChildNodes.Add(new XmlNode(name, attrs));
                    }
                    else
                    {
                        var node = new XmlNode(name, attrs);
                        nodes.First().ChildNodes.Add(node);
                        nodes.Push(node);
                    }
                }
            }
            else
            {
                // text
                nodes.First().InnerText += tag;
            }
        }
        return nodes.Last();
    }

    private static IEnumerable<string> EnumerateTagBody(string body)
    {
        var chars = new[] { ' ', '\t', '\r', '\n' };
        int index1 = 0;
        int index2 = 0;

        Action skipping = () =>
        {
            while (index1 < body.Length)
            {
                if (!chars.Contains(body[index1]))
                    break;
                index1++;
            }
        };

        // skip

        skipping();

        // name
        
        index2 = body.IndexOfAny(chars, index1);
        if (index2 == -1)
            index2 = body.Length - 1;
        else
            index2 = index2 - 1;

        yield return body.Substring(index1, index2 - index1 + 1);


        while (true)
        {
            // skip
            index1 = index2 + 1;
            skipping();

            // attr name

            index2 = body.IndexOf('=', index1);
            if (index2 == -1)
                yield break;

            yield return body.Substring(index1, index2 - index1);

            // attr value

            index1 = index2 + 1;

            if (body[index1] == '"')
            {
                index2 = body.IndexOf('"', index1 + 1);
                if (index2 == -1)
                    yield break;
                yield return body.Substring(index1 + 1, index2 - index1 - 1);
            }
            else
            {
                index2 = body.IndexOfAny(chars, index1);
                if (index2 == -1)
                    yield return body.Substring(index1);
                else
                    yield return body.Substring(index1, index2 - index1 + 1);
            }
        }
    }

    private static IEnumerable<string> EnumerateXmlTags(byte [] bytes, Encoding encoding)
    {
        string str = encoding.GetString(bytes, 0, bytes.Length);
        int i = 0;
        int j;
        while (true)
        {
            j = str.IndexOf('<', i);
            if (j == -1)
            {
                yield return str.Substring(i);
                break;
            }
            yield return str.Substring(i, j - i);
            i = j;
            if (str.IndexOf("<!--", i, (str.Length - i) <= 5 ? (str.Length - i) : 5) == i)
            {
                j = str.IndexOf("-->", i);
            }
            else
            {
                j = str.IndexOf('>', i);
            }
            if (j == -1)
            {
                // possible error in xml;
                break;
            }
            yield return str.Substring(i, j - i + 1);
            i = j + 1;
        }
    }

    private static Dictionary<object, object> ParseDictionary(List<XmlNode> elements)
    {
        Dictionary<object, object> dict = new Dictionary<object, object>();
        for (int i = 0; i < elements.Count; i += 2)
        {
            var key = elements[i];
            XmlNode val = elements[i + 1];
            dict[key.InnerText] = ParseValue(val);
        }
        return dict;
    }

    private static List<object> ParseArray(List<XmlNode> elements)
    {
        List<object> list = new List<object>();
        foreach (XmlNode e in elements)
        {
            list.Add(ParseValue(e));
        }
        return list;
    }

    private static object ParseValue(XmlNode val)
    {
        switch (val.Name.ToString())
        {
            case "string":
                return val.InnerText;
            case "integer":
                return val.InnerText;//int.Parse(val.Value);
            case "real":
                return val.InnerText;//float.Parse(val.Value);
            case "true":
                return "true";//true;
            case "false":
                return "false";//false;
            case "dict":
                return ParseDictionary(val.ChildNodes);
            case "array":
                return ParseArray(val.ChildNodes);
            default:
                throw new ArgumentException("Unsupported");
        }
    }

#if false
    public static Dictionary<object, object> DecodePlist(byte[] bytes)
    {
        var doc = new XmlDocument();
        doc.Load(new MemoryStream(bytes));
        var dict = doc.SelectSingleNode("/plist/dict");
        return ParseDictionary(dict.ChildNodes);
    }

    private static Dictionary<object, object> ParseDictionary(XmlNodeList elements)
    {
        Dictionary<object, object> dict = new Dictionary<object, object>();
        for (int i = 0; i < elements.Count; i += 2)
        {
            var key = elements[i];
            XmlNode val = elements[i + 1];
            dict[key.InnerText] = ParseValue(val);
        }
        return dict;
    }

    private static List<object> ParseArray(XmlNodeList elements)
    {
        List<object> list = new List<object>();
        foreach (XmlNode e in elements)
        {
            list.Add(ParseValue(e));
        }
        return list;
    }

    private static object ParseValue(XmlNode val)
    {
        switch (val.Name.ToString())
        {
            case "string":
                return val.InnerText;
            case "integer":
                return val.InnerText;//int.Parse(val.Value);
            case "real":
                return val.InnerText;//float.Parse(val.Value);
            case "true":
                return "true";//true;
            case "false":
                return "false";//false;
            case "dict":
                return ParseDictionary(val.ChildNodes);
            case "array":
                return ParseArray(val.ChildNodes);
            default:
                throw new ArgumentException("Unsupported");
        }
    }
#endif
#if DD_PLATFORM_WIN32 && false
    public static Dictionary<object, object> DecodePlist(byte[] bytes)
    {
        XDocument doc = XDocument.Load(new MemoryStream(bytes));
        XElement plist = doc.Element("plist");
        XElement dict = plist.Element("dict");

        var dictElements = dict.Elements();
        Dictionary<object, object> ret = new Dictionary<object, object>();
        Parse(ret, dictElements);
        return ret;
    }

    private static void Parse(Dictionary<object, object> dict, IEnumerable<XElement> elements)
    {
        for (int i = 0; i < elements.Count(); i += 2)
        {
            XElement key = elements.ElementAt(i);
            XElement val = elements.ElementAt(i + 1);

            dict[key.Value] = ParseValue(val);
        }
    }

    private static List<object> ParseArray(IEnumerable<XElement> elements)
    {
        List<object> list = new List<object>();
        foreach (XElement e in elements)
        {
            list.Add(ParseValue(e));
        }

        return list;
    }

    private static object ParseValue(XElement val)
    {
        switch (val.Name.ToString())
        {
            case "string":
                return val.Value;
            case "integer":
                return val.Value;//int.Parse(val.Value);
            case "real":
                return val.Value;//float.Parse(val.Value);
            case "true":
                return "true";//true;
            case "false":
                return "false";//false;
            case "dict":
                Dictionary<object, object> plist = new Dictionary<object, object>();
                Parse(plist, val.Elements());
                return plist;
            case "array":
                List<object> list = ParseArray(val.Elements());
                return list;
            default:
                throw new ArgumentException("Unsupported");
        }
    }
#endif

    #region Safe Readers

    public static float GetFloat(object obj, int index, float df = 0)
    {
        float ret = df;
        var l = obj as List<object>;
        if (l != null && index >= 0 && index < l.Count)
        {
            if (!float.TryParse(l[index].ToString(), System.Globalization.NumberStyles.Any, null, out ret))
            {
                ret = df;
            }
        }
        return ret;
    }

    internal static float GetFloat(object obj, string key, int df)
    {
        float ret = df;
        var h = obj as Dictionary<object, object>;
        if (h != null && h.ContainsKey(key))
        {
            if (!float.TryParse(h[key].ToString(), System.Globalization.NumberStyles.Any, null, out ret))
            {
                ret = df;
            }
        }
        return ret;
    }

    internal static int GetInt(string value, int df)
    {
        int ret = df;
        if (!int.TryParse(value, System.Globalization.NumberStyles.Any, null, out ret))
        {
            ret = df;
        }
        return ret;
    }

    internal static string GetString(object obj, string key, string df = null)
    {
        var h = obj as Dictionary<object, object>;
        if (h != null && h.ContainsKey(key))
        {
            return (h[key] as string) ?? df;
        }
        return df;
    }

    internal static DDVector GetPlistVector(object obj, string key)
    {
        string s = DDXml.GetString(obj, key, null);
		s = s.Replace(" ", string.Empty);
        var x = int.Parse(DDXml.StringBetween(s, "{", ",", "0"));
        var y = int.Parse(DDXml.StringBetween(s, ",", "}", "0"));
        return new DDVector(x, y);
    }

    internal static DDRectangle GetPlistRectangle(object obj, string key)
    {
        string s = DDXml.GetString(obj, key, null); 
		s = s.Replace(" ", string.Empty);
        var s1 = DDXml.StringBetween(s, "{{", "},{", "0,0");
        var s2 = DDXml.StringBetween(s, "},{", "}}", "0,0");

        var x1 = int.Parse(DDXml.StringBefore(s1, ",", "0"));
        var y1 = int.Parse(DDXml.StringAfter(s1, ",", "0"));

        var x2 = int.Parse(DDXml.StringBefore(s2, ",", "0"));
        var y2 = int.Parse(DDXml.StringAfter(s2, ",", "0"));

        return new DDRectangle(x1, y1, x1 + x2, y1 + y2);
    }

    internal static List<object> GetList(object obj, string key)
    {
        var h = obj as Dictionary<object, object>;
        if (h != null && h.ContainsKey(key))
        {
            return h[key] as List<object>;
        }
        return null;
    }

    internal static Dictionary<object, object> GetDictionary(object obj, string key)
    {
        var h = obj as Dictionary<object, object>;
        if (h != null && h.ContainsKey(key))
        {
            return h[key] as Dictionary<object, object>;
        }
        return null;
    }

    #endregion

    #region String Parsing

    public static string StringBetween(string str, string start, string end, string df)
    {
        int n = str.IndexOf(start);
        if (n != -1)
        {
            int m = str.IndexOf(end, n + start.Length);
            if (m != -1)
            {
                return str.Substring(n + start.Length, m - n - start.Length);
            }
        }
        return df;
    }

    public static string StringAfter(string str, string start, string df)
    {
        int n = str.IndexOf(start);
        if (n != -1)
        {
            return str.Substring(n + start.Length);
        }
        return df;
    }

    public static string StringBefore(string str, string end, string df)
    {
        int m = str.IndexOf(end);
        if (m != -1)
        {
            return str.Substring(0, m);
        }
        return df;
    }

    #endregion
}