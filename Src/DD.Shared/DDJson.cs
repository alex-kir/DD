//
//  DDJson.cs
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
using System.Collections;
using System.Globalization;
using System.Text;
using System.Collections.Generic;

#if !NETFX_CORE
using System.Reflection;
#else
using WindowsStore.Compatibility.Reflection;
#endif

/// <summary>
/// This class implements a JSON protocol and used for serializing\deserializing data. 
/// </summary>
public class DDJson
{
    public const int TOKEN_NONE = 0;
    public const int TOKEN_CURLY_OPEN = 1;
    public const int TOKEN_CURLY_CLOSE = 2;
    public const int TOKEN_SQUARED_OPEN = 3;
    public const int TOKEN_SQUARED_CLOSE = 4;
    public const int TOKEN_COLON = 5;
    public const int TOKEN_COMMA = 6;
    public const int TOKEN_STRING = 7;
    public const int TOKEN_NUMBER = 8;
    public const int TOKEN_TRUE = 9;
    public const int TOKEN_FALSE = 10;
    public const int TOKEN_NULL = 11;

    private const int BUILDER_CAPACITY = 2000;

    /// <summary>
    /// On decoding, this value holds the position at which the parse failed (-1 = no error).
    /// </summary>
    protected int lastErrorIndex = -1;
    protected string lastDecode = "";

    public static object Decode(string json)
    {
        return new DDJson().JsonDecode(json);
    }

    public static string Encode(object json)
    {
        return new DDJson().JsonEncode(json);
    }

    /// <summary>
    /// Parses the string json into a value
    /// </summary>
    /// <param name="json">A JSON string.</param>
    /// <returns>An ArrayList, a Hashtable, a double, a string, null, true, or false</returns>
    public object JsonDecode(string json)
    {
        // save the string for debug information
        lastDecode = json;

        if (json != null)
        {
            char[] charArray = json.ToCharArray();
            int index = 0;
            bool success = true;
            object value = ParseValue(charArray, ref index, ref success);
            if (success)
            {
                lastErrorIndex = -1;
            }
            else
            {
                lastErrorIndex = index;
            }
            return value;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Converts a Hashtable / ArrayList object into a JSON string
    /// </summary>
    /// <param name="json">A Hashtable / ArrayList</param>
    /// <returns>A JSON encoded string, or null if object 'json' is not serializable</returns>
    public string JsonEncode(object json)
    {
        StringBuilder builder = new StringBuilder(BUILDER_CAPACITY);
        bool success = SerializeValue(json, builder);
        return (success ? builder.ToString() : null);
    }

    /// <summary>
    /// On decoding, this function returns the position at which the parse failed (-1 = no error).
    /// </summary>
    /// <returns></returns>
    public bool LastDecodeSuccessful()
    {
        return (lastErrorIndex == -1);
    }

    /// <summary>
    /// On decoding, this function returns the position at which the parse failed (-1 = no error).
    /// </summary>
    /// <returns></returns>
    public int GetLastErrorIndex()
    {
        return lastErrorIndex;
    }

    /// <summary>
    /// If a decoding error occurred, this function returns a piece of the JSON string 
    /// at which the error took place. To ease debugging.
    /// </summary>
    /// <returns></returns>
    public string GetLastErrorSnippet()
    {
        if (lastErrorIndex == -1)
        {
            return "";
        }
        else
        {
            int startIndex = lastErrorIndex - 5;
            int endIndex = lastErrorIndex + 15;
            if (startIndex < 0)
            {
                startIndex = 0;
            }
            if (endIndex >= lastDecode.Length)
            {
                endIndex = lastDecode.Length - 1;
            }

            return lastDecode.Substring(startIndex, endIndex - startIndex + 1);
        }
    }

    protected Dictionary<object, object> ParseObject(char[] json, ref int index)
    {
        Dictionary<object, object> dict = new Dictionary<object, object>();
        int token;

        // {
        NextToken(json, ref index);

        bool done = false;
        while (!done)
        {
            token = LookAhead(json, index);
            if (token == DDJson.TOKEN_NONE)
            {
                return null;
            }
            else if (token == DDJson.TOKEN_COMMA)
            {
                NextToken(json, ref index);
            }
            else if (token == DDJson.TOKEN_CURLY_CLOSE)
            {
                NextToken(json, ref index);
                return dict;
            }
            else
            {

                // name
                string name = ParseString(json, ref index);
                if (name == null)
                {
                    return null;
                }

                // :
                token = NextToken(json, ref index);
                if (token != DDJson.TOKEN_COLON)
                {
                    return null;
                }

                // value
                bool success = true;
                object value = ParseValue(json, ref index, ref success);
                if (!success)
                {
                    return null;
                }

                dict[name] = value;
            }
        }

        return dict;
    }

    protected List<object> ParseArray(char[] json, ref int index)
    {
        List<object> list = new List<object>();

        // [
        NextToken(json, ref index);

        bool done = false;
        while (!done)
        {
            int token = LookAhead(json, index);
            if (token == DDJson.TOKEN_NONE)
            {
                return null;
            }
            else if (token == DDJson.TOKEN_COMMA)
            {
                NextToken(json, ref index);
            }
            else if (token == DDJson.TOKEN_SQUARED_CLOSE)
            {
                NextToken(json, ref index);
                break;
            }
            else
            {
                bool success = true;
                object value = ParseValue(json, ref index, ref success);
                if (!success)
                {
                    return null;
                }

                list.Add(value);
            }
        }

        return list;
    }

    protected object ParseValue(char[] json, ref int index, ref bool success)
    {
        switch (LookAhead(json, index))
        {
            case DDJson.TOKEN_STRING:
                return ParseString(json, ref index);
            case DDJson.TOKEN_NUMBER:
                return ParseNumber(json, ref index);
            case DDJson.TOKEN_CURLY_OPEN:
                return ParseObject(json, ref index);
            case DDJson.TOKEN_SQUARED_OPEN:
                return ParseArray(json, ref index);
            case DDJson.TOKEN_TRUE:
                NextToken(json, ref index);
                return Boolean.Parse("TRUE");
            case DDJson.TOKEN_FALSE:
                NextToken(json, ref index);
                return Boolean.Parse("FALSE");
            case DDJson.TOKEN_NULL:
                NextToken(json, ref index);
                return null;
            case DDJson.TOKEN_NONE:
                break;
        }

        success = false;
        return null;
    }

    protected string ParseString(char[] json, ref int index)
    {
        StringBuilder s = new StringBuilder(BUILDER_CAPACITY);
        char c;

        EatWhitespace(json, ref index);

        // "
        c = json[index++];

        bool complete = false;
        while (!complete)
        {

            if (index == json.Length)
            {
                break;
            }

            c = json[index++];
            if (c == '"')
            {
                complete = true;
                break;
            }
            else if (c == '\\')
            {

                if (index == json.Length)
                {
                    break;
                }
                c = json[index++];
                if (c == '"')
                {
                    s.Append('"');
                }
                else if (c == '\\')
                {
                    s.Append('\\');
                }
                else if (c == '/')
                {
                    s.Append('/');
                }
                else if (c == 'b')
                {
                    s.Append('\b');
                }
                else if (c == 'f')
                {
                    s.Append('\f');
                }
                else if (c == 'n')
                {
                    s.Append('\n');
                }
                else if (c == 'r')
                {
                    s.Append('\r');
                }
                else if (c == 't')
                {
                    s.Append('\t');
                }
                else if (c == 'u')
                {
                    int remainingLength = json.Length - index;
                    if (remainingLength >= 4)
                    {
                        // fetch the next 4 chars
                        char[] unicodeCharArray = new char[4];
                        Array.Copy(json, index, unicodeCharArray, 0, 4);
                        // parse the 32 bit hex into an integer codepoint
                        uint codePoint = UInt32.Parse(new string(unicodeCharArray), NumberStyles.HexNumber);
                        // convert the integer codepoint to a unicode char and add to string
                        s.Append(Char.ConvertFromUtf32((int)codePoint));
                        // skip 4 chars
                        index += 4;
                    }
                    else
                    {
                        break;
                    }
                }

            }
            else
            {
                s.Append(c);
            }

        }

        if (!complete)
        {
            return null;
        }

        return s.ToString();
    }

    protected double ParseNumber(char[] json, ref int index)
    {
        EatWhitespace(json, ref index);

        int lastIndex = GetLastIndexOfNumber(json, index);
        int charLength = (lastIndex - index) + 1;
        char[] numberCharArray = new char[charLength];

        Array.Copy(json, index, numberCharArray, 0, charLength);
        index = lastIndex + 1;
        return Double.Parse(new string(numberCharArray), CultureInfo.InvariantCulture);
    }

    protected int GetLastIndexOfNumber(char[] json, int index)
    {
        int lastIndex;
        for (lastIndex = index; lastIndex < json.Length; lastIndex++)
        {
            if ("0123456789+-.eE".IndexOf(json[lastIndex]) == -1)
            {
                break;
            }
        }
        return lastIndex - 1;
    }

    protected void EatWhitespace(char[] json, ref int index)
    {
        for (; index < json.Length; index++)
        {
            if (" \t\n\r".IndexOf(json[index]) == -1)
            {
                break;
            }
        }
    }

    protected int LookAhead(char[] json, int index)
    {
        int saveIndex = index;
        return NextToken(json, ref saveIndex);
    }

    protected int NextToken(char[] json, ref int index)
    {
        EatWhitespace(json, ref index);

        if (index == json.Length)
        {
            return DDJson.TOKEN_NONE;
        }

        char c = json[index];
        index++;
        switch (c)
        {
            case '{':
                return DDJson.TOKEN_CURLY_OPEN;
            case '}':
                return DDJson.TOKEN_CURLY_CLOSE;
            case '[':
                return DDJson.TOKEN_SQUARED_OPEN;
            case ']':
                return DDJson.TOKEN_SQUARED_CLOSE;
            case ',':
                return DDJson.TOKEN_COMMA;
            case '"':
                return DDJson.TOKEN_STRING;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
            case '-':
                return DDJson.TOKEN_NUMBER;
            case ':':
                return DDJson.TOKEN_COLON;
        }
        index--;

        int remainingLength = json.Length - index;

        // false
        if (remainingLength >= 5)
        {
            if (json[index] == 'f' &&
                json[index + 1] == 'a' &&
                json[index + 2] == 'l' &&
                json[index + 3] == 's' &&
                json[index + 4] == 'e')
            {
                index += 5;
                return DDJson.TOKEN_FALSE;
            }
        }

        // true
        if (remainingLength >= 4)
        {
            if (json[index] == 't' &&
                json[index + 1] == 'r' &&
                json[index + 2] == 'u' &&
                json[index + 3] == 'e')
            {
                index += 4;
                return DDJson.TOKEN_TRUE;
            }
        }

        // null
        if (remainingLength >= 4)
        {
            if (json[index] == 'n' &&
                json[index + 1] == 'u' &&
                json[index + 2] == 'l' &&
                json[index + 3] == 'l')
            {
                index += 4;
                return DDJson.TOKEN_NULL;
            }
        }

        return DDJson.TOKEN_NONE;
    }

    protected bool SerializeObjectOrArray(object objectOrArray, StringBuilder builder)
    {
        if (objectOrArray is Dictionary<object, object>)
        {
            return SerializeObject((Dictionary<object, object>)objectOrArray, builder);
        }
        else if (objectOrArray is List<object>)
        {
            return SerializeArray((List<object>)objectOrArray, builder);
        }
        else
        {
            return false;
        }
    }

    protected bool SerializeObject(IDictionary anObject, StringBuilder builder)
    {
        builder.Append("{");

        IDictionaryEnumerator e = anObject.GetEnumerator();
        bool first = true;
        while (e.MoveNext())
        {
            string key = e.Key.ToString();
            object value = e.Value;

            if (!first)
            {
                builder.Append(", ");
            }

            SerializeString(key, builder);
            builder.Append(":");
            if (!SerializeValue(value, builder))
            {
                return false;
            }

            first = false;
        }

        builder.Append("}");
        return true;
    }

    protected bool SerializeArray(ICollection anArray, StringBuilder builder)
    {
        builder.Append("[");

        bool first = true;
        //		int i = 0;
        foreach (object value in anArray)
        {
            //		for (int i = 0; i < anArray.Count; i++) {
            //	object value = anArray[i];

            if (!first)
            {
                builder.Append(", ");
            }

            if (!SerializeValue(value, builder))
            {
                return false;
            }

            first = false;
        }

        builder.Append("]");
        return true;
    }

    protected bool SerializeValue(object value, StringBuilder builder)
    {
        if (value is string)
        {
            SerializeString((string)value, builder);
        }
        else if (value is IDictionary)
        {
            SerializeObject((IDictionary)value, builder);
        }
        else if (value is ICollection)
        {
            SerializeArray((ICollection)value, builder);
        }
        else if (IsNumeric(value))
        {
            SerializeNumber(Convert.ToDouble(value), builder);
        }
        else if ((value is Boolean) && ((Boolean)value == true))
        {
            builder.Append("true");
        }
        else if ((value is Boolean) && ((Boolean)value == false))
        {
            builder.Append("false");
        }
        else if (value == null)
        {
            builder.Append("null");
        }
        else
        {
            return false;
        }
        return true;
    }

    protected void SerializeString(string aString, StringBuilder builder)
    {
        builder.Append("\"");

        char[] charArray = aString.ToCharArray();
        for (int i = 0; i < charArray.Length; i++)
        {
            char c = charArray[i];
            if (c == '"')
            {
                builder.Append("\\\"");
            }
            else if (c == '\\')
            {
                builder.Append("\\\\");
            }
            else if (c == '\b')
            {
                builder.Append("\\b");
            }
            else if (c == '\f')
            {
                builder.Append("\\f");
            }
            else if (c == '\n')
            {
                builder.Append("\\n");
            }
            else if (c == '\r')
            {
                builder.Append("\\r");
            }
            else if (c == '\t')
            {
                builder.Append("\\t");
            }
            else
            {
                int codepoint = Convert.ToInt32(c);
                if ((codepoint >= 32) && (codepoint <= 126))
                {
                    builder.Append(c);
                }
                else
                {
                    builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                }
            }
        }

        builder.Append("\"");
    }

    protected void SerializeNumber(double number, StringBuilder builder)
    {
        builder.Append(Convert.ToString(number, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Determines if a given object is numeric in any way
    /// (can be integer, double, etc). C# has no pretty way to do this.
    /// </summary>
    protected bool IsNumeric(object o)
    {
        try
        {
            Double.Parse(o.ToString());
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }

    #region DecodeJson<T>

	public static T Decode<T>(string json) where T : class
	{
		object obj = new DDJson().JsonDecode(json);
		return (T)DecodeObject(obj, typeof(T));
	}

    public static T DecodeJson<T>(string json) where T : class
    {
        object obj = new DDJson().JsonDecode(json);
        return (T)DecodeObject(obj, typeof(T));
    }

    public static T DecodeOrNull<T>(string json) where T : class
    {
        try
        {
            object obj = new DDJson().JsonDecode(json);
            var ret = (T)DecodeObject(obj, typeof(T));
			return ret;
        }
        catch
        {
            return null;
        }
    }

    private static object DecodeObject(object source, Type type)
    {
#if !NETFX_CORE
		bool isGeneric = type.IsGenericType;
#else
		bool isGeneric = type.IsGenericType();
#endif		
		if (isGeneric && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            var dictionary = (source as IDictionary);
            if (dictionary != null)
            {
                Type keyType = type.GetGenericArguments()[0];
                Type valueType = type.GetGenericArguments()[1];
                var ret = (IDictionary)Activator.CreateInstance(type);
                foreach (var key in dictionary.Keys)
                {
                    ret.Add(DecodeObject(key, keyType), DecodeObject(dictionary[key], valueType));
                }
                return ret;
            }
        }
        //-----------------------------------------
		if (isGeneric && type.GetGenericTypeDefinition() == typeof(List<>))
        {
            var list = source as IEnumerable;
            if (list != null)
            {
                Type itemType = type.GetGenericArguments()[0];
                var ret = (IList)Activator.CreateInstance(type);
                foreach (object item in list)
                {
                    ret.Add(DecodeObject(item, itemType));
                }
                return ret;
            }
        }
        //-----------------------------------------
		if (type == typeof(string))
        {
            var str = source as string;
            if (str != null)
            {
                return str;
            }
        }
        //-----------------------------------------
		if (type == typeof(int))
        {
            var number1 = source as string;
            if (number1 != null)
            {
                return int.Parse(number1);
            }
            var number2 = source as double?;
            if (number2.HasValue)
            {
                return (int)number2.Value;
            }
        }
        //-----------------------------------------

        throw new ArgumentException("AKJson.DecodeObject() " + source.GetType().Name + " != " + type.Name, "type");
        
    }

    #endregion

}