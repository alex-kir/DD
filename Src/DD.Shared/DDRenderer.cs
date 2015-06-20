//
//  DDRenderer.cs
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
public partial class DDRenderer
{
	public class Quad
	{
		public DDMatrix Matrix;
		
        public DDVector _xy1;
        public DDVector _xy2;
        public DDVector _xy3;
        public DDVector _xy4;
		
		public DDRectangle _rect = new DDRectangle();
		
        public DDVector xy1;
        public DDVector uv1;
        public DDColor white_color1;
        public DDColor black_color1;

        public DDVector xy2;
        public DDVector uv2;
        public DDColor white_color2;
        public DDColor black_color2;

        public DDVector xy3;
        public DDVector uv3;
        public DDColor white_color3;
        public DDColor black_color3;

        public DDVector xy4;
        public DDVector uv4;
        public DDColor white_color4;
        public DDColor black_color4;
	}
}