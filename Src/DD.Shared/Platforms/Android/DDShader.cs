using System;
using OpenTK.Graphics.ES20;



// this class is combination of shader and mesh (two triangles);
public class DDShader : IDisposable
{
    private readonly float[] Vertices = new float[]
    {
        1, -1,
        1,  1,
        -1,  1,
        -1, -1,
    };

    const int VerticesStride = sizeof(float) * 2;
    const int VerticesCount = 4;

    private readonly float[] UVs = new float[] {
        1, 1,
        1, 0,
        0, 0,
        0, 1
    };
    const int UVsStride = sizeof(float) * 2;

    private readonly float[] Colors = new float[] {
        1, 1, 1, 1,
        1, 1, 1, 1,
        1, 1, 1, 1,
        1, 1, 1, 1,
    };

    const int ColorsStride = sizeof(float) * 4;

    private readonly ushort[] Indices = new ushort[] { 
        0, 1, 2, 
        2, 3, 0 
    };


    private const string VertexShaderString =  
        @"
            attribute vec4 Position;
            attribute vec4 SourceColor;

            uniform mat4 OrthographicMatrix;
        
            varying vec4 DestinationColor;

            attribute vec2 TexCoordIn;
            varying vec2 TexCoordOut;

            void main(void) { 
                DestinationColor = SourceColor;
                gl_Position = Position * OrthographicMatrix;
                TexCoordOut = TexCoordIn;
            }
        ";

    // -------------------------------------------

//    private const string FragmentShader_Yuv =
//        @"
//            varying lowp vec4 DestinationColor;
//
//            varying lowp vec2 TexCoordOut;
//            uniform sampler2D Texture0;
//            uniform sampler2D Texture1;
//            uniform sampler2D Texture2;
//
//            void main(void) {
//                highp float y = texture2D(Texture0, TexCoordOut).r;
//                highp float u = texture2D(Texture1, TexCoordOut).r - 0.5;
//                highp float v = texture2D(Texture2, TexCoordOut).r - 0.5;
//
//                highp float r = y               + 1.13983 * v;
//                highp float g = y - 0.39465 * u - 0.58060 * v;
//                highp float b = y + 2.03211 * u;
//
//                gl_FragColor = vec4(r, g, b, 1.0);
//            }
//        ";
//
//
//    private const string FragmentShader_Yuv420v =
//        @"
//            varying lowp vec4 DestinationColor;
//
//            varying lowp vec2 TexCoordOut;
//            uniform sampler2D Texture0;
//            uniform sampler2D Texture1;
//
//            void main(void) {
//                highp float y = texture2D(Texture0, TexCoordOut).r;
//                y = (y - 0.0625) * 1.163;
//                highp vec4 uv = texture2D(Texture1, TexCoordOut).rgba;
//                highp float u = (uv.r * 0.9375 + uv.g * 0.0625) - 0.5;
//                highp float v = (uv.b * 0.9375 + uv.a * 0.0625) - 0.5;
//
//                highp float r = y +             1.402 * u;
//                highp float g = y - 0.344 * v - 0.714 * u;
//                highp float b = y + 1.772 * v;
//
//                gl_FragColor = vec4(r, g, b, 1.0);
//            }
//        ";

//    private const string FragmentShader_Empty =
//        @"
//            varying lowp vec4 DestinationColor;
//
//            varying lowp vec2 TexCoordOut;
//            uniform sampler2D Texture0;
//            uniform sampler2D Texture1;
//
//            void main(void) {
//                gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
//            }
//        ";

    private const string FragmentShader_Default =
        @"
            varying lowp vec4 DestinationColor;

            varying lowp vec2 TexCoordOut;
            uniform sampler2D Texture0;
            /* uniform sampler2D Texture1; */

            void main(void) {

                highp vec4 tex = texture2D(Texture0, TexCoordOut).rgba;
                /* highp vec4 result = vec4(1.0) * gl_FragColor + vec4(1.0 - gl_FragColor.a) * tc; */

                gl_FragColor = tex;//vec4(tex.rgb, tex.a);//vec4(tex.a, 0, 0, 1);//vec4(tex.rgb/tex.a, tex.a); /* DestinationColor * vec4(1 - tc.a) + vec4(tc.a) * tc; */
//                gl_FragColor = vec4(tex.r, tex.g, tex.b, 1);
//                gl_FragColor = vec4(1, 1, 1, tex.r);

                /* gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0); */
            }
        ";
    


    int _programHandle;


    public uint _positionSlot;
    public uint _colorSlot;
    public uint _texCoordSlot;

    int _orthoMatrixSlot;

    int []_textureUniforms = new int[1];
//    int [] _glTextures = new int[3];

//    public readonly ffFrame.Types frameType;

    public DDShader()
    {
//        this.frameType = frameType;

        CompileShaders();

        const int verSize = 8 * sizeof(float);// 7 * sizeof(float);
        GL.VertexAttribPointer(_positionSlot, 2, VertexAttribPointerType.Float, false, verSize, 0);
        GL.VertexAttribPointer(_colorSlot, 4, VertexAttribPointerType.Float, false, verSize, 2 * sizeof(float));
//            GL.VertexAttribPointer(_texCoordSlot, 2, VertexAttribPointerType.Float, false, verSize, 5 * sizeof(float));
        GL.VertexAttribPointer(_texCoordSlot, 2, VertexAttribPointerType.Float, false, verSize, 6 * sizeof(float));

//        for (int i = 0; i < _glTextures.Length; i++)
//            _glTextures[i] = CreateTexture();

    }
        
    public void Dispose()
    {
        DDDebug.Trace();
//        DDUtils.Delete(ref _programHandle, 0, GL.DeleteProgram);
//
//        for (int i = 0; i < _glTextures.Length; i++)
//            DDUtils.Delete(ref _glTextures[i], 0, GL.DeleteTexture);
    }

    private int CreateTexture()
    {
        int texName = GL.GenTexture();

        GL.BindTexture(TextureTarget.Texture2D, texName);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return texName;
    }

    private bool CompileShader(ref int shaderHandle, ShaderType type, string shaderString)
    {
        int status;

        shaderHandle = GL.CreateShader(type);

        if (shaderHandle == 0)
        {
            throw new Exception("Failed to create shader " + type);
        }

        GL.ShaderSource(shaderHandle, shaderString);
        GL.CompileShader(shaderHandle);

        GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out status);

        return status == (int)All.True;
    }

    private void CompileShaders ()
    {
        int vertShader = 0;
        int fragShader = 0;

        if (!CompileShader (ref vertShader, ShaderType.VertexShader, VertexShaderString))
            DDDebug.Trace ("Failed to compile the vertex shader");

        var fragmentShader = "";
//        if (frameType == ffFrame.Types.YUV420v)
//            fragmentShader = FragmentShader_Yuv420v;
//        else if (frameType == ffFrame.Types.YUVp)
//            fragmentShader = FragmentShader_Yuv;
//        else {
//            throw new Exception("frameType(" + frameType + ") unknown");
//        }
        fragmentShader = FragmentShader_Default;

        if (!CompileShader (ref fragShader, ShaderType.FragmentShader, fragmentShader))
            DDDebug.Trace ("Failed to compile the fragment shader");

        _programHandle = GL.CreateProgram ();

        GL.AttachShader (_programHandle, vertShader);
        GL.AttachShader (_programHandle, fragShader);

        int status = 0;

        GL.LinkProgram (_programHandle);
        GL.GetProgram (_programHandle, ProgramParameter.LinkStatus, out status);
        if (status == (int)All.False) {
            var cause = GL.GetProgramInfoLog(_programHandle);
            throw new Exception("GL.LinkProgram:" + cause);
        }

        GL.ValidateProgram (_programHandle);
        GL.GetProgram (_programHandle, ProgramParameter.ValidateStatus, out status);
        if (status == (int)All.False) {
            var cause = GL.GetProgramInfoLog(_programHandle);
            throw new Exception("GL.ValidateProgram:" + cause);
        }


        GL.DeleteShader (vertShader);
        GL.DeleteShader (fragShader);

        _positionSlot = (uint)GL.GetAttribLocation(_programHandle, "Position");
        var tmp = GL.GetAttribLocation(_programHandle, "SourceColor");
        _colorSlot = tmp < 0 ? uint.MaxValue : (uint)tmp; // don't understand why it return -1 on some devices

        _orthoMatrixSlot = GL.GetUniformLocation(_programHandle, "OrthographicMatrix");

        GL.EnableVertexAttribArray(_positionSlot);
        GL.EnableVertexAttribArray(_colorSlot);

        _texCoordSlot = (uint)GL.GetAttribLocation(_programHandle, "TexCoordIn");
        GL.EnableVertexAttribArray(_texCoordSlot);

        for (int i = 0; i < _textureUniforms.Length; i++)
            _textureUniforms[i] = GL.GetUniformLocation(_programHandle, "Texture" + i);
    }


    public void SetMatrix(OpenTK.Matrix4 matrix)
    {
        GL.UniformMatrix4(_orthoMatrixSlot, false, ref matrix);
    }


    public void SetTexture(DDTexture tex)
    {
        int index = 0;
//        var plane = frame.GetPlane(index);

//        var internalPixetFormat = pixelType == PixelType.UnsignedShort4444 ? PixelInternalFormat.Rgba : PixelInternalFormat.Luminance;
//        var pixelFormat = pixelType == PixelType.UnsignedShort4444 ? PixelFormat.Rgba : PixelFormat.Luminance;

        GL.ActiveTexture(TextureUnit.Texture0 + index);
        GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);
        GL.Uniform1(_textureUniforms[index], index);

//        GL.BindTexture(TextureTarget.Texture2D, _glTextures[index]);
//        GL.Uniform1(_textureUniforms[index], index);
//        GL.TexImage2D(TextureTarget.Texture2D, 0,
//            internalPixetFormat,
//            plane.Width, plane.Height, 0,
//            pixelFormat, pixelType, plane.Bytes);
    }

    public void RenderMesh(float [] vertices, int [] indices, float [] uvs, float [] whiteColors, float [] blackColors)
    {
        GL.UseProgram(_programHandle);

        GL.VertexAttribPointer(_positionSlot, 2, VertexAttribPointerType.Float, false, VerticesStride, Vertices);
        GL.EnableVertexAttribArray(_positionSlot);

        GL.VertexAttribPointer(_colorSlot, 4, VertexAttribPointerType.Float, false, ColorsStride, Colors);
        GL.EnableVertexAttribArray(_colorSlot);

        GL.VertexAttribPointer(_texCoordSlot, 2, VertexAttribPointerType.Float, false, UVsStride, UVs);
        GL.EnableVertexAttribArray(_texCoordSlot);

        GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedShort, Indices);
    }
}

