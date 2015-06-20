//
//  DDUserData.cs
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


public class DDUserData
{
    #region Ints

    Dictionary<string, int> _ints = null;

    public int GetInt(string key, int v)
    {
        if (_ints != null && _ints.ContainsKey(key))
            return _ints[key];
        return v;
    }

    public void SetInt(string key, int v)
    {
        if (_ints == null)
            _ints = new Dictionary<string, int>();
        _ints[key] = v;
    }

    #endregion

    #region Floats

    Dictionary<string, float> _floats = null;

    public float GetFloat(string key, float v)
    {
        if (_floats != null && _floats.ContainsKey(key))
            return _floats[key];
        return v;
    }

    public void SetFloat(string key, float v)
    {
        if (_floats == null)
            _floats = new Dictionary<string, float>();
        _floats[key] = v;
    }

    #endregion

    #region Strings

    Dictionary<string, string> _strings = null;

    public string GetString(string key, string v)
    {
        if (_strings != null && _strings.ContainsKey(key))
            return _strings[key];
        return v;
    }

    public void SetString(string key, string v)
    {
        if (_strings == null)
            _strings = new Dictionary<string, string>();
        _strings[key] = v;
    }

    #endregion

    #region Objects

    Dictionary<string, object> _objects = null;

    public object GetObject(string key, object v)
    {
        if (_objects != null && _objects.ContainsKey(key))
            return _objects[key];
        return v;
    }

    public void SetObject(string key, object v)
    {
        if (_objects == null)
            _objects = new Dictionary<string, object>();
        _objects[key] = v;
    }

    #endregion

    #region Functions

    Dictionary<string, Action> _functions = null;

    public void CallFunction(string key)
    {
        if (_functions != null && _functions.ContainsKey(key))
            _functions[key]();
    }

    public void SetFunction(string key, Action v)
    {
        if (_functions == null)
            _functions = new Dictionary<string, Action>();
        _functions[key] = v;
    }

    #endregion
}