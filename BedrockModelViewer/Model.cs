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
    internal class Model
    {
        public List<Vector3> modelVerts;
        public List<Vector2> modelUVs;
        public List<uint> modelIndices;

        public Vector3 position;
        public uint indexCount;

        VAO modelVAO;
        VBO modelVertexVBO;
        VBO modelTextureVBO;
        IBO modelIBO;

        Texture texture;

        public Model(Vector3 position, string modelPath, string texturePath)
        {
            this.position = position;

            ModelInfo info = new ModelData(modelPath, texturePath).model;

            modelVerts = info.Vertices;
            modelUVs = info.UVs;
            modelIndices = info.Indices;

            File.Copy(texturePath, "Resources/texture.png", true);

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
            modelVAO.Bind();

            modelVertexVBO = new VBO(modelVerts);
            modelVertexVBO.Bind();
            modelVAO.LinkToVAO(0, 3, modelVertexVBO);

            modelTextureVBO = new VBO(modelUVs);
            modelTextureVBO.Bind();
            modelVAO.LinkToVAO(1, 2, modelVertexVBO);

            modelIBO = new IBO(modelIndices);

            texture = new Texture("texture.png");
        }
    }
}
