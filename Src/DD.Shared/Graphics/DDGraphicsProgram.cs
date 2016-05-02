
#if DD_PLATFORM_IOS || true // || (DD_PLATFORM_ANDROID && false)

using System;
using OpenTK.Graphics.ES20;
using System.Collections.Generic;
using OpenTK;

namespace DD.Graphics
{
    public class DDGraphicsProgram : IDisposable
    {
        public static readonly string DefaultVertShaderCode = @"
            attribute vec4 position;
            attribute vec2 uv;
            attribute vec4 color_white;
            attribute vec4 color_black;

            uniform mat4 matrix;

            varying vec2 uv_var;
            varying vec4 color_white_var;
            varying vec4 color_black_var;

            void main()
            {
                gl_Position = matrix * position;
                uv_var = uv;
                color_white_var = color_white;
                color_black_var = color_black;
            }
        ";

        public static readonly string DefaultFragShaderCode = @"
            uniform sampler2D texture_0;

            varying mediump vec2 uv_var;
            varying mediump vec4 color_white_var;
            varying mediump vec4 color_black_var;

            void main()
            {
                gl_FragColor = mix(vec4(color_black_var.rgb, 0), color_white_var, texture2D(texture_0, uv_var));
            }
        ";



        int handle = 0;
        readonly Dictionary<string, int> uniforms = new Dictionary<string, int>();
        readonly Dictionary<string, int> attributes = new Dictionary<string, int>();

        public DDGraphicsProgram(string vertShaderCode, string fragShaderCode)
        {
            int vertShader = 0;
            int fragShader = 0;

            try
            {
                // Create shader program.
                handle = GL.CreateProgram();

                // Create and compile vertex shader.
                vertShader = CompileShader(ShaderType.VertexShader, vertShaderCode ?? DefaultVertShaderCode);
                fragShader = CompileShader(ShaderType.FragmentShader, fragShaderCode ?? DefaultFragShaderCode);

                // Attach vertex shader to program.
                GL.AttachShader(handle, vertShader);

                // Attach fragment shader to program.
                GL.AttachShader(handle, fragShader);

                // Bind attribute locations.
                // This needs to be done prior to linking.
                //                GL.BindAttribLocation(handle, (int)GLKVertexAttrib.Position, "position");
                //                GL.BindAttribLocation(handle, (int)GLKVertexAttrib.Normal, "normal");

                // Link program.
                if (!LinkProgram(handle))
                {
                    Console.WriteLine("Failed to link program: {0:x}", handle);

                    if (vertShader != 0)
                        GL.DeleteShader(vertShader);

                    if (fragShader != 0)
                        GL.DeleteShader(fragShader);

                    if (handle != 0)
                    {
                        GL.DeleteProgram(handle);
                        handle = 0;
                    }
                    throw new Exception("dd video: link program failed");
                }

                //                // Get uniform locations.
                //                uniforms["modelViewProjectionMatrix"] = GL.GetUniformLocation(handle, "modelViewProjectionMatrix");
                //                uniforms["normalMatrix"] = GL.GetUniformLocation(handle, "normalMatrix");
            }
            finally
            {
                // Release vertex and fragment shaders.
                if (vertShader != 0)
                {
                    GL.DetachShader(handle, vertShader);
                    GL.DeleteShader(vertShader);
                }

                if (fragShader != 0)
                {
                    GL.DetachShader(handle, fragShader);
                    GL.DeleteShader(fragShader);
                }

            }

        }

        public void Dispose()
        {
            if (handle != 0)
            {
                GL.DeleteProgram(handle);
                handle = 0;
            }
        }

        int CompileShader(ShaderType type, string src)
        {
            var shader = GL.CreateShader(type);
            GL.ShaderSource(shader, src);
            GL.CompileShader(shader);

            #if DEBUG
            int logLength = 0;
            GL.GetShader(shader, ShaderParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                Console.WriteLine("Shader compile log:\n{0}", GL.GetShaderInfoLog(shader));
            }
            #endif

            int status = 0;
            GL.GetShader(shader, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                GL.DeleteShader(shader);
                throw new Exception("dd video: " + type + " shader failed (" + status + "); \n" + src);
            }

            return shader;
        }

        bool LinkProgram(int prog)
        {
            GL.LinkProgram(prog);

            #if DEBUG
            int logLength = 0;
            GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            if (logLength > 0)
                Console.WriteLine("Program link log:\n{0}", GL.GetProgramInfoLog(prog));
            #endif

            int status = 0;
            GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            return status != 0;
        }

        bool ValidateProgram(int prog)
        {
            int logLength, status = 0;

            GL.ValidateProgram(prog);
            GL.GetProgram(prog, ProgramParameter.InfoLogLength, out logLength);
            if (logLength > 0)
            {
                var log = new System.Text.StringBuilder(logLength);
                GL.GetProgramInfoLog(prog, logLength, out logLength, log);
                Console.WriteLine("Program validate log:\n{0}", log);
            }

            GL.GetProgram(prog, ProgramParameter.LinkStatus, out status);
            return status != 0;
        }

        int GetAttributeLocation(string name)
        {
            if (!attributes.ContainsKey(name))
                attributes[name] = GL.GetAttribLocation(handle, name);
            return attributes[name];
        }

        int GetUniformLocation(string name)
        {
            if (!uniforms.ContainsKey(name))
                uniforms[name] = GL.GetUniformLocation(handle, name);
            return uniforms[name];
        }

        public void SetAttrib(string name, float [] value, int size, int stride)
        {
            var location = GetAttributeLocation(name);
            //            GL.VertexAttribPointer(_positionSlot, 2, VertexAttribPointerType.Float, false, verSize, 0);
            GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, stride, value);
            GL.EnableVertexAttribArray(location);
        }

        //        public void SetUniform(string name, float value)
        //        {
        //            var loc = GetUniformLocation(name);
        //        }

        public void SetUniform(string name, DDTexture value, int index)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + index);
            GL.BindTexture(TextureTarget.Texture2D, value.TextureId);
            GL.Uniform1(GetUniformLocation(name), index);
        }

        public void SetUniform(string name, Matrix4 matrix)
        {
//            GL.UniformMatrix4(GetUniformLocation(name), false, ref matrix);
            var m = matrix;
            GL.UniformMatrix4(GetUniformLocation(name), 1, false, new float[]{
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.M41, m.M42, m.M43, m.M44,
            });
        }

        public void SetUniform(string name, DDMatrix matrix)
        {
            GL.UniformMatrix4(GetUniformLocation(name), 1, false, (float[])matrix);
        }

        public void UseProgram()
        {
            GL.UseProgram(handle);
        }


    }    
}

#endif