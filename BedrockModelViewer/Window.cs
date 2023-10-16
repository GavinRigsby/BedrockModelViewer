using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using StbImageSharp;
using OpenTK.Windowing.GraphicsLibraryFramework;
using BedrockModelViewer.Graphics;
using OpenTK.Compute.OpenCL;

namespace BedrockModelViewer
{
    public class Window : GameWindow
    {

        private string _texturePath;
        private string _modelPath;
        private string _imageOutput;
        private ShaderProgram shaderProgram;
        private Color4 _backColor = Color4.BlueViolet;
        private VBO vertexBufferObject;
        private VAO vertexArrayObject;
        private IBO elementBufferObject;
        private Texture texture;
        private VBO textureVBO;
        private int width;
        private int height;
        private float yRot = 0f;
        private float xRot = 0f;
        private float zRot = 0f;
        private Camera camera;
        private bool focused = false;
        private ModelObject gameModel;
        private ModelObject testModel;

        //"origin": [ -4.0, 6.0, -2.0 ],
        //"size": [ 8, 12, 4 ],
        //"uv": [ 16, 16 ]

        private List<Vector3> vertices = new List<Vector3>()
        {
            // front face
            new Vector3(-4f, 18f, 2f), // topleft vert
            new Vector3(4f, 18f, 2f), // topright vert
            new Vector3(4f, 6f, 2f), // bottomright vert
            new Vector3(-4f, 6f, 2f), // bottomleft vert
            // right face
            new Vector3(4f, 18f, 2f), // topleft vert
            new Vector3(4f, 18f, -2f), // topright vert
            new Vector3(4f, 6f, -2f), // bottomright vert
            new Vector3(4f, 6f, 2f), // bottomleft vert
            // back face
            new Vector3(4f, 18f, -2f), // topleft vert
            new Vector3(-4f, 18f, -2f), // topright vert
            new Vector3(-4f, 6f, -2f), // bottomright vert
            new Vector3(4f, 6f, -2f), // bottomleft vert
            // left face
            new Vector3(-4f, 18f, -2f), // topleft vert
            new Vector3(-4f, 18f, 2f), // topright vert
            new Vector3(-4f, 6f, 2f), // bottomright vert
            new Vector3(-4f, 6f, -2f), // bottomleft vert
            // top face
            new Vector3(-4f, 18f, -2f), // topleft vert
            new Vector3(4f, 18f, -2f), // topright vert
            new Vector3(4f, 18f, 2f), // bottomright vert
            new Vector3(-4f, 18f, 2f), // bottomleft vert
            // bottom face
            new Vector3(-4f, 6f, 2f), // topleft vert
            new Vector3(4f, 6f, 2f), // topright vert
            new Vector3(4f, 6f, -2f), // bottomright vert
            new Vector3(-4f, 6f, -2f), // bottomleft vert

            // FACE
            // front
            new Vector3(-4f, 26f, 4f),
            new Vector3(4f, 26f, 4f),
            new Vector3(4f, 18f, 4f),
            new Vector3(-4f, 18f, 4f),

            //right
            new Vector3(4f, 26f, 4f),
            new Vector3(4f, 26f, -4f),
            new Vector3(4f, 18f, -4f),
            new Vector3(4f, 18f, 4f),

            new Vector3(4f, 26f, -4f),
            new Vector3(-4f, 26f, -4f),
            new Vector3(-4f, 18f, -4f),
            new Vector3(4f, 18f, -4f),

            new Vector3(-4f, 26f, -4f),
            new Vector3(-4f, 26f, 4f),
            new Vector3(-4f, 18f, 4f),
            new Vector3(-4f, 18f, -4f),

            new Vector3(-4f, 26f, -4f),
            new Vector3(4f, 26f, -4f),
            new Vector3(4f, 26f, 4f),
            new Vector3(-4f, 26f, 4f),

            new Vector3(-4f, 18f, 4f),
            new Vector3(4f, 18f, 4f),
            new Vector3(4f, 18f, -4f),
            new Vector3(-4f, 18f, -4f),
        };

        List<Vector2> texCoords = new List<Vector2>()
        {
            // =======  BODY =========
            new Vector2(20/64f, 12/32f),
            new Vector2(28/64f, 12/32f),
            new Vector2(28/64f, 0/32f),
            new Vector2(20/64f, 0/32f),

            new Vector2(28/64f, 12/32f),
            new Vector2(32/64f, 12/32f),
            new Vector2(32/64f, 0/32f),
            new Vector2(28/64f, 0/32f),

            new Vector2(32/64f, 12/32f),
            new Vector2(40/64f, 12/32f),
            new Vector2(40/64f, 0/32f),
            new Vector2(32/64f, 0/32f),

            new Vector2(16/64f, 12/32f),
            new Vector2(20/64f, 12/32f),
            new Vector2(20/64f, 0/32f),
            new Vector2(16/64f, 0/32f),

            new Vector2(20/64f, 16/32f),
            new Vector2(28/64f, 16/32f),
            new Vector2(28/64f, 12/32f),
            new Vector2(20/64f, 12/32f),

            new Vector2(28/64f, 16/32f),
            new Vector2(36/64f, 16/32f),
            new Vector2(36/64f, 12/32f),
            new Vector2(28/64f, 12/32f),

            // =======  HEAD =========
            new Vector2(1/8f, 3/4f),
            new Vector2(2/8f, 3/4f),
            new Vector2(2/8f, 2/4f),
            new Vector2(1/8f, 2/4f),

            new Vector2(2/8f, 3/4f),
            new Vector2(3/8f, 3/4f),
            new Vector2(3/8f, 2/4f),
            new Vector2(2/8f, 2/4f),

            new Vector2(3/8f, 3/4f),
            new Vector2(4/8f, 3/4f),
            new Vector2(4/8f, 2/4f),
            new Vector2(3/8f, 2/4f),

            new Vector2(0f, 3/4f),
            new Vector2(1/8f, 3/4f),
            new Vector2(1/8f, 2/4f),
            new Vector2(0f, 2/4f),

            new Vector2(1/8f, 4/4f),
            new Vector2(2/8f, 4/4f),
            new Vector2(2/8f, 3/4f),
            new Vector2(1/8f, 3/4f),

            new Vector2(2/8f, 4/4f),
            new Vector2(3/8f, 4/4f),
            new Vector2(3/8f, 3/4f),
            new Vector2(2/8f, 3/4f),
        };

        List<uint> indices = new List<uint>
        {
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
            22, 23, 20,

            // HEAD
            24, 25, 26,
            26, 27, 24,

            28, 29, 30, 
            30, 31, 28,

            32, 33, 34, 
            34, 35, 32, 

            36, 37, 38,
            38, 39, 36, 

            40, 41, 42,
            42, 43, 40,

            44, 45, 46,
            46, 47, 44,
        };



        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, string Texture, string Model, string output) : base(gameWindowSettings, nativeWindowSettings)
        {
            _texturePath = Texture;
            _modelPath = Model;
            _imageOutput = output;
            width = Size.X;
            height = Size.Y;
        }

        // Loads in initial values 
        protected override void OnLoad()
        {
            base.OnLoad();
            File.Copy(_texturePath, "Resources/texture.png", true);


            gameModel = new ModelObject(new Vector3(10f, 0f, 0f), _modelPath, _texturePath);

            //testModel = new ModelObject(new Vector3(-10,0,0), _modelPath, _texturePath);
            //// generate the vertex buffer object
            //vertexArrayObject = new VAO();

            ////// generate a buffer
            //vertexBufferObject = new VBO(vertices);

            //vertexArrayObject.LinkToVAO(0, 3, vertexBufferObject);

            //textureVBO = new VBO(texCoords);
            //vertexArrayObject.LinkToVAO(1, 2, textureVBO);

            //elementBufferObject = new IBO(indices);

            shaderProgram = new ShaderProgram("default.vert", "default.frag");

            //texture = new Texture("texture.png");

            // Enable 3D
            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(width, height, Vector3.Zero);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0,0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {

            // Clear the Color and Depth Buffers
            GL.ClearColor(_backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();
            Matrix4 translation = Matrix4.CreateTranslation(0f, 0f, -10f);

            model *= translation;

            int modelLocation = GL.GetUniformLocation(shaderProgram.ID, "model");
            int viewLocation = GL.GetUniformLocation(shaderProgram.ID, "view");
            int projectionLocation = GL.GetUniformLocation(shaderProgram.ID, "projection");

            GL.UniformMatrix4(modelLocation, true, ref model);
            GL.UniformMatrix4(viewLocation, true, ref view);
            GL.UniformMatrix4(projectionLocation, true, ref projection);

            shaderProgram.Bind();
            //vertexArrayObject.Bind();
            //elementBufferObject.Bind();
            //texture.Bind();
            //GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);



            gameModel.Render(shaderProgram);

            
            //testModel.Render(shaderProgram);

            SwapBuffers();

            base.OnRenderFrame(e);

        }

        // Cleanup when closing
        protected override void OnUnload()
        {
            base.OnUnload();
            gameModel.Delete();
            shaderProgram.Delete();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            //Debug.WriteLine($"X: {camera.position.X}, Y: {camera.position.Y}, Z: {camera.position.Z}");

            MouseState mouse = MouseState;
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.LeftAlt))
            {
                focused = false;
                CursorState = CursorState.Normal;
            }

            if (mouse.IsButtonDown(MouseButton.Left))
            {
                Box2i windowBounds = this.Bounds;
                if (mouse.X >= windowBounds.Min.X && mouse.X <= windowBounds.Max.X && mouse.Y >= windowBounds.Min.Y && mouse.Y <= windowBounds.Max.Y) {
                    focused = true;
                    CursorState = CursorState.Grabbed;
                }
            }

            base.OnUpdateFrame(e);

            camera.Update(input, mouse, focused, e);
        }
    }
}
