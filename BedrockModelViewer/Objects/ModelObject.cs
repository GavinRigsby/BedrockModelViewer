using BedrockModelViewer.Graphics;
using BedrockModelViewer.Objects;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockModelViewer
{
    internal class ModelObject : RenderableObject
    {

        public uint indexCount;




        public ModelObject(Vector3 position, string modelPath, string texturePath)
        {
            textureName = Path.GetFileName(texturePath);
            File.Copy(texturePath, "Resources/"+textureName, true);

            ModelInfo info = new ModelData(modelPath, texturePath).model;

            Vertices = info.Vertices;
            UVs = info.UVs;
            Indices = info.Indices;

            SetPosition(position);
        }
    }
}
