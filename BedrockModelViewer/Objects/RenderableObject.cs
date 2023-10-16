using BedrockModelViewer.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace BedrockModelViewer.Objects
{
    public class RenderableObject
    {

        public string textureName = "MissingTexture.png";
        public Vector3 position = new(0,0,0);
        public List<Vector3> Vertices = new();
        private List<Vector3> PostitionedVertices;
        public List<Vector2> UVs = new();
        public List<uint> Indices = new();
        private Texture texture { get; set; }
        private VAO VAO;
        private VBO VertexVBO;
        private VBO TextureVBO;
        private IBO IBO;

        // Method used by inheriting class to populate Vertices, UVs, and Indices
        public virtual void GenerateModel()
        {
        }

        public RenderableObject() { }

        public RenderableObject(Vector3 position)
        {
            this.position = position;
        }

        public RenderableObject(Vector3 position, string texturePath)
        {
            this.position = position;
            textureName = Path.GetFileName(texturePath);
            File.Copy(texturePath, "Resources/" + textureName, true);
        }
        
        // Positions the model based on currently set postion
        public void SetPostion()
        {
            SetPosition(this.position);
        }
        
        // Postitions the model based on supplied position
        public void SetPosition(Vector3 position)
        {
            List<Vector3> newVerts = new List<Vector3>();
            foreach (Vector3 v in Vertices)
            {
                Vector3 newVert = new Vector3(
                    v.X + position.X,
                    v.Y + position.Y,
                    v.Z + position.Z);
                newVerts.Add(newVert);
            }
            this.PostitionedVertices = newVerts;
            this.position = position;
            BuildModel();
        }

        // Builds the objects to render the model
        public void BuildModel()
        {
            VAO = new VAO();

            List<Vector3> renderVerts = Vertices;
            if (PostitionedVertices != null)
            {
                renderVerts = PostitionedVertices;
            }
            VertexVBO = new VBO(renderVerts);
            VAO.LinkToVAO(0, 3, VertexVBO);
            TextureVBO = new VBO(UVs);
            VAO.LinkToVAO(1, 2, TextureVBO);
            IBO = new IBO(Indices);
            texture = new Texture(textureName);
        }

        // Remove all data (cleanup)
        public void Delete()
        {
            VAO.Delete();
            VertexVBO.Delete();
            TextureVBO.Delete();
            IBO.Delete();
        }
        
        // Allows user to manually supply Vertices, UVs, and Indices for model
        public void SetData(List<Vector3> Vectors, List<Vector2> UVs, List<uint> Indicies)
        {
            this.Vertices = Vectors;
            this.UVs = UVs;
            this.Indices = Indicies;
            SetPosition(position);
        }

        // Renders the modle to the screen
        public void Render(ShaderProgram program) // drawing the chunk
        {
            program.Bind();
            VAO.Bind();
            IBO.Bind();
            texture.Bind();
            GL.DrawElements(PrimitiveType.Triangles, Indices.Count, DrawElementsType.UnsignedInt, 0);
        }

        public void Inflate(float inflate)
        {
            List<Vector3> result = new();
            foreach (var vert in Vertices)
            {
                float xInflate = vert.X < 0 ? -inflate : inflate;
                float yInflate = vert.Y < 0 ? -inflate : inflate;
                float zInflate = vert.Z < 0 ? -inflate : inflate;

                result.Add(new Vector3(vert.X + xInflate, vert.Y + yInflate, vert.Z + zInflate));
            }
            Vertices = result;
        }
    }
}
