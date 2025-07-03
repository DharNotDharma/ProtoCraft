using System;
using StbImageSharp;
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
         -0.5f,  0.5f, 0f,  // top-left
          0.5f,  0.5f, 0f,  // top-right
          0.5f, -0.5f, 0f,  // bottom-right
         -0.5f, -0.5f, 0f   // bottom-left
         };

        float[] texCoords = {
            0f, 1f, // top-left
            1f, 1f, // top-right
            1f, 0f, // bottom-right
            0f, 0f  // bottom-left
        };

        uint[] indices = {
            0, 1, 2, // top triangle
            2, 3, 0  // bottom triangle
        };
        //Render Pipeline vars
        int vao;
        int shaderProgram;
        int vbo;
        int ebo; //element buffer object for indices
        int textureID; //texture ID for the texture we will load
        int textureVBO; //texture VBO for texture coordinates

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

                 // need to bind vao
                GL.BindVertexArray(vao);
                
                vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
                
               // put the vertex VBo

                // Slot 0  
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao, 0);

               GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // unbind the VBO

                //Texture Coordinates
                textureVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Length * sizeof(float), texCoords, BufferUsageHint.StaticDraw);
                
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexAttribArray(1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // unbind the VBO
            
                //Load shader program
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);

                ebo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);


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

                // ---- TEXTURE LOADING ----
                textureID = GL.GenTexture();
                // activate the texture unit
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureID);

                // texture parameters
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                // load the texture image
                StbImage.stbi_set_flip_vertically_on_load(1); // flip the image vertically
                ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/tnt_top.png"), ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);
                // unbind the texture
                GL.BindTexture(TextureTarget.Texture2D, 0);

        }     


        protected override void OnUnload()
            {
                base.OnUnload();
                //Delete the buffers and shader program
                GL.DeleteVertexArray(vao);
                GL.DeleteBuffer(vbo);
                GL.DeleteBuffer(ebo);
                GL.DeleteTexture(textureID);
                GL.DeleteProgram(shaderProgram);
            }
        protected override void OnRenderFrame(FrameEventArgs args)
            {
                
                GL.ClearColor(0.5f, 0.2f, 1f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                //The triangle
                GL.UseProgram(shaderProgram);
                int texUniformLocation = GL.GetUniformLocation(shaderProgram, "texture0");
                GL.Uniform1(texUniformLocation, 0); // Bind sampler 'texture0' to texture unit 0
                
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, textureID);
                GL.BindVertexArray(vao);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
                    
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
