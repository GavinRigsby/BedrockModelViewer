using OpenTK.Mathematics;

namespace BedrockModelViewer.Objects
{
    internal class ModelObject : RenderableObject
    {
        public ModelObject(Vector3 position, string modelPath, string texturePath) : base(position, texturePath)
        {
            textureName = Path.GetFileName(texturePath);
            File.Copy(texturePath, "Resources/" + textureName, true);

            ModelInfo info = new ModelData(modelPath, texturePath).model;

            SetData(info.Vertices, info.UVs, info.Indices);
        }
    }
}
