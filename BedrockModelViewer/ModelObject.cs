using BedrockModelViewer.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockModelViewer
{
    internal class ModelObject
    {
        public List<Vector3> modelVerts;
        public List<Vector2> modelUVs;
        public List<uint> modelIndices;

        public Vector3 position = new();
        public uint indexCount;

        VAO modelVAO;
        VBO modelVertexVBO;
        VBO modelTextureVBO;
        IBO modelIBO;

        public Texture texture { get; set; }

        public ModelObject(Vector3 position, string modelPath, string texturePath)
        {
            ModelInfo info = new ModelData(modelPath, texturePath).model;

            modelVerts = info.Vertices;
            modelUVs = info.UVs;
            modelIndices = info.Indices;

            SetPosition(position);
        }

        public void ManualData(List<Vector3> Vectors, List<Vector2> UVs, List<uint> Indicies)
        {
            modelVerts = Vectors;
            modelUVs = UVs;
            modelIndices = Indicies;

            SetPosition(position);
        }

        public void SetPosition(Vector3 newPosition)
        {
            List<Vector3> newVerts = new List<Vector3>();
            foreach (Vector3 v in modelVerts)
            {
                Vector3 newVert = new Vector3(v.X - position.X, v.Y - position.Y, v.Z - position.Z);
                newVert = new Vector3(newVert.X + newPosition.X, newVert.Y + newPosition.Y, newVert.Z + newPosition.Z);
                newVerts.Add(newVert);
            }
            this.modelVerts = newVerts;
            this.position = newPosition;
            BuildModel();
        }

        public void Render(ShaderProgram program) // drawing the chunk
        {
            program.Bind();
            modelVAO.Bind();
            modelIBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, modelIndices.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void BuildModel()
        {
            modelVAO = new VAO();
            
            modelVertexVBO = new VBO(modelVerts);
            modelVAO.LinkToVAO(0, 3, modelVertexVBO);
            modelTextureVBO = new VBO(modelUVs);
            modelVAO.LinkToVAO(1, 2, modelVertexVBO);
            modelIBO = new IBO(modelIndices);
            texture = new Texture("texture.png");
            modelVAO.Bind();
            modelVertexVBO.Bind();
            modelTextureVBO.Bind();
            

            

            
        }
    }
}
