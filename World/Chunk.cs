using ProtoCraft.Graphics;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtoCraft.World
{
    internal class Chunk
    {
        public List<Vector3> chunkVerts;
        public List<Vector2> chunkUVs;
        public List<uint> chunkIndices;

        const int SIZE = 16;
        const int HEIGHT = 32;
        public Vector3 position;

        public uint indexCount;

        VAO chunkVAO;
        VBO chunkVertexVBO;
        VBO chunkUVVBO;
        IBO chunkIBO;

        Texture texture;
        public Chunk(Vector3 postition)
        {
            this.position = postition;

            chunkVerts = new List<Vector3>();
            chunkUVs = new List<Vector2>();
            chunkIndices = new List<uint>();

            GenBlocks();
            BuildChunk();
        }

        public void GenChunk() { } // generate the data
        public void GenBlocks()
            {
                int chunkWidth = 16;   // X
                int chunkDepth = 16;   // Z
                int chunkHeight = 8;   // Y

                for (int x = 0; x < chunkWidth; x++)
                {
                    for (int z = 0; z < chunkDepth; z++)
                    {
                        // Make the step/stairstep shape:
                        int surfaceHeight = (x + z) / 4 + 2; // You can tweak the math for more/less steep

                        for (int y = 0; y <= surfaceHeight; y++)
                        {
                            BlockType type = (y == surfaceHeight) ? BlockType.GRASS : BlockType.DIRT;
                            Block block = new Block(new Vector3(x, y, z), type);

                            // Render all 6 faces for every block (simple, works for now)
                            foreach (Faces face in Enum.GetValues(typeof(Faces)))
                            {
                                var faceData = block.GetFace(face);
                                chunkVerts.AddRange(faceData.vertices);
                                chunkUVs.AddRange(faceData.uv);
                            }

                            AddIndices(6); // 6 faces per block
                        }
                    }
                }
            }
        public void AddIndices(int amtFaces)
        {
            for(int i = 0; i < amtFaces; i++)
            {
                chunkIndices.Add(0 + indexCount);
                chunkIndices.Add(1 + indexCount);
                chunkIndices.Add(2 + indexCount);
                chunkIndices.Add(2 + indexCount);
                chunkIndices.Add(3 + indexCount);
                chunkIndices.Add(0 + indexCount);

                indexCount += 4;
            }
        }
        public void BuildChunk() {
            chunkVAO = new VAO();
            chunkVAO.Bind();

            chunkVertexVBO = new VBO(chunkVerts);
            chunkVertexVBO.Bind();
            chunkVAO.LinkToVAO(0, 3, chunkVertexVBO);

            chunkUVVBO = new VBO(chunkUVs);
            chunkUVVBO.Bind();
            chunkVAO.LinkToVAO(1, 2, chunkUVVBO);

            chunkIBO = new IBO(chunkIndices);

            texture = new Texture("atlas.png");
        } // take data and process it for rendering
        public void Render(ShaderProgram program) // drawing the chunk
        {
            program.Bind();
            chunkVAO.Bind();
            chunkIBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, chunkIndices.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Delete()
        {
            chunkVAO.Delete();
            chunkVertexVBO.Delete();
            chunkUVVBO.Delete();
            chunkIBO.Delete();
            texture.Delete();
        }
    }
}
