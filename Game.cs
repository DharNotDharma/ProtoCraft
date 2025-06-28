using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;


namespace ProtoCraft
{
    public class Game : GameWindow
    {

        float[] vertices = {
          0.0f,  0.5f, 0.0f,   // top
          0.5f,  0.0f, 0.0f,   // right
          0.0f, -0.5f, 0.0f,   // bottom
          -0.5f,  0.0f, 0.0f    // left
         };

        // VAO, VBO, EBO, and Shader Program variables
        //Render Pipeline vars
        int vao;
        int shaderProgram;

        // everything involving height and width should be in here 
        int width, height;
        public Game(int width, int height) : base(GameWindowSettings.Default, NativeWindowSettings.Default)
        {
            

                this.width = width;
                this.height = height;

                CenterWindow(new Vector2i(width, height));

        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }
        protected override void OnLoad()
            {
                base.OnLoad();

                vao = GL.GenVertexArray();
                
                int vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
                // need to bind vao
                GL.BindVertexArray(vao);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao, 0);
                //Load shader program
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);

                //Shader Program
                shaderProgram = GL.CreateProgram();
                int vertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vertexShader, LoadShaderSource("Default.vert"));    
                GL.CompileShader(vertexShader);    

                int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fragmentShader, LoadShaderSource("Default.frag"));
                GL.CompileShader(fragmentShader);

                GL.AttachShader(shaderProgram, vertexShader);
                GL.AttachShader(shaderProgram, fragmentShader);

                GL.LinkProgram(shaderProgram);  

                //delete shaders after linking
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);
        }     


            protected override void OnUnload()
            {
                base.OnUnload();
            }
        protected override void OnRenderFrame(FrameEventArgs args)
            {
                
                GL.ClearColor(0.5f, 0.2f, 1f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //The triangle
                GL.UseProgram(shaderProgram);
                GL.BindVertexArray(vao);
                GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
                    
                Context.SwapBuffers();
                base.OnRenderFrame(args);

        }
        protected override void OnUpdateFrame(FrameEventArgs args)
            {
                base.OnUpdateFrame(args);

                if (KeyboardState.IsKeyDown(Keys.Escape))
                {
                    Close();
                }
            }
        public static string LoadShaderSource(string filePath)
        {
            string shaderSource = "";

            try
            {
                using (StreamReader reader = new StreamReader("../../../Shaders/" + filePath))
                {
                    shaderSource = reader.ReadToEnd();
                }
                // Console.WriteLine(shaderSource);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to load shader source file: " + e.Message);
            }

            return shaderSource;
        }

    }
    
}
