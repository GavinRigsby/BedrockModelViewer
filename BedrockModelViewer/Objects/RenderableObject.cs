using BedrockModelViewer.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;

namespace BedrockModelViewer.Objects
{
    public class RenderableObject
    {

        public string textureName = "MissingTexture.png";
        public Vector3 position = new(0,0,0);
        public List<Vector3> Vertices = new();
        public List<Vector3> CenteredVertices { get; private set; } = new();
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
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string resourceDirectory = Path.Combine(appDirectory, "Resources");
            string textureFile = Path.Combine(resourceDirectory, textureName);
            Directory.CreateDirectory(resourceDirectory);
            File.Copy(texturePath, textureFile, true);
        }

        // Positions the model based on currently set postion
        public void SetPosition()
        {
            SetPosition(this.position);
        }
        
        // Postitions the model based on supplied position
        public void SetPosition(Vector3 position)
        {
            if (CenteredVertices.Count > 0)
            {
                this.Vertices = RenderTools.PostitionModel(position, CenteredVertices);
                this.position = position;
                BuildModel();
            }
            //List<Vector3> newVerts = new List<Vector3>();
            //foreach (Vector3 v in Vertices)
            //{
            //    Vector3 newVert = new Vector3(
            //        v.X + position.X,
            //        v.Y + position.Y,
            //        v.Z + position.Z);
            //    newVerts.Add(newVert);
            //}
            //this.PostitionedVertices = newVerts;
            //this.position = position;
            //BuildModel();
        }

        // Builds the objects to render the model
        public void BuildModel()
        {
            if (UVs.Count != Vertices.Count)
            {
                Debug.WriteLine("We Got Issues");
            }

            VAO = new VAO();

            List<Vector3> renderVerts = Vertices;
            VertexVBO = new VBO(renderVerts);
            VAO.LinkToVAO(0, 3, VertexVBO);
            TextureVBO = new VBO(UVs);
            VAO.LinkToVAO(1, 2, TextureVBO);
            IBO = new IBO(Indices);
            texture = new Texture(textureName);
        }

        public void OffsetModel(Vector3 offset)
        {
            for (int i = 0; i < Vertices.Count; i++)
            {
                Vertices[i] = Vertices[i] + offset;
            }
            // May need to update position
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
            this.CenteredVertices = Vectors;
            this.UVs = UVs;
            this.Indices = Indicies;
            SetPosition();
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
