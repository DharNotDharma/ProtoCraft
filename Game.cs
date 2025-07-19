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


        List<Vector3> vertices = new List<Vector3>()
        {
        // front face
            new Vector3(-0.5f, 0.5f, 0.5f), // topleft vert
            new Vector3(0.5f, 0.5f, 0.5f), // topright vert
            new Vector3(0.5f, -0.5f, 0.5f), // bottomright vert
            new Vector3(-0.5f, -0.5f, 0.5f), // bottomleft vert
            // right face
            new Vector3(0.5f, 0.5f, 0.5f), // topleft vert
            new Vector3(0.5f, 0.5f, -0.5f), // topright vert
            new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
            new Vector3(0.5f, -0.5f, 0.5f), // bottomleft vert
            // back face
            new Vector3(0.5f, 0.5f, -0.5f), // topleft vert
            new Vector3(-0.5f, 0.5f, -0.5f), // topright vert
            new Vector3(-0.5f, -0.5f, -0.5f), // bottomright vert
            new Vector3(0.5f, -0.5f, -0.5f), // bottomleft vert
            // left face
            new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
            new Vector3(-0.5f, 0.5f, 0.5f), // topright vert
            new Vector3(-0.5f, -0.5f, 0.5f), // bottomright vert
            new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
            // top face
            new Vector3(-0.5f, 0.5f, -0.5f), // topleft vert
            new Vector3(0.5f, 0.5f, -0.5f), // topright vert
            new Vector3(0.5f, 0.5f, 0.5f), // bottomright vert
            new Vector3(-0.5f, 0.5f, 0.5f), // bottomleft vert
            // bottom face
            new Vector3(-0.5f, -0.5f, 0.5f), // topleft vert
            new Vector3(0.5f, -0.5f, 0.5f), // topright vert
            new Vector3(0.5f, -0.5f, -0.5f), // bottomright vert
            new Vector3(-0.5f, -0.5f, -0.5f), // bottomleft vert
        };


        List<Vector2> texCoords = new List<Vector2>()
        {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),

            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        };
        uint[] indices = {
            // first face
            // top triangle
            0, 1, 2,
            // bottom triangle
            2, 3, 0,

            4, 5, 6,
            6, 7, 4,

            8, 9, 10,
            10, 11, 8,

            12, 13, 14,
            14, 15, 12,

            16, 17, 18,
            18, 19, 16,

            20, 21, 22,
            22, 23, 20
        };
        //Render Pipeline vars
        int vao;
        int shaderProgram;
        int vbo;
        int ebo; //element buffer object for indices
        int textureID; //texture ID for the texture we will load
        int textureVBO; //texture VBO for texture coordinates

        // Camera vars
        Camera camera;

        //tranformation matrix variables
        float yRot = 0f;

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
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
                
               // put the vertex VBo

                // Slot 0  
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.EnableVertexArrayAttrib(vao, 0);

               GL.BindBuffer(BufferTarget.ArrayBuffer, 0); // unbind the VBO

                //Texture Coordinates
                textureVBO = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, textureVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, texCoords.Count * Vector2.SizeInBytes, texCoords.ToArray(), BufferUsageHint.StaticDraw);
                
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
                ImageResult dirtTexture = ImageResult.FromStream(File.OpenRead("../../../Textures/dirtTex.png"), ColorComponents.RedGreenBlueAlpha);

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, dirtTexture.Width, dirtTexture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dirtTexture.Data);
                // unbind the texture
                GL.BindTexture(TextureTarget.Texture2D, 0);
                
                GL.Enable(EnableCap.DepthTest); // Enable depth testing for 3D rendering

                camera = new Camera(width, height, Vector3.Zero);
                CursorState = CursorState.Grabbed; // Lock the cursor to the center of the window

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
                

                // transformation matrix
                Matrix4 model = Matrix4.Identity;
                Matrix4 view = camera.GetViewMatrix();
                Matrix4 projection = camera.GetProjectionMatrix();

                model = Matrix4.CreateRotationY(yRot);
                yRot += 0.0005f; // Increment rotation angle for animation

                Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -3f);

                model *= translation;
                
                int modelLocation = GL.GetUniformLocation(shaderProgram, "model");
                int viewLocation = GL.GetUniformLocation(shaderProgram, "view");
                int projectionLocation = GL.GetUniformLocation(shaderProgram, "projection");

                GL.UniformMatrix4(modelLocation, false, ref model);
                GL.UniformMatrix4(viewLocation, false, ref view);
                GL.UniformMatrix4(projectionLocation, false, ref projection);
                
                GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
                //GL.DrawArrays(PrimitiveType.Triangles, 0, 3);
            
                Context.SwapBuffers();
                base.OnRenderFrame(args);

        }
        protected override void OnUpdateFrame(FrameEventArgs args)
            {
                MouseState mouse = MouseState;
                KeyboardState input = KeyboardState;
            
                base.OnUpdateFrame(args);
                camera.Update(input, mouse, args);
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
