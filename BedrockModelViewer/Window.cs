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
        private Model gameModel;


        private List<Vector3> vertices = new List<Vector3>()
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
            22, 23, 20
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

            gameModel = new Model(new Vector3(0f, 0f, 0f), _modelPath, _texturePath);

            // generate the vertex buffer object
            vertexArrayObject = new VAO();

            // generate a buffer
            vertexBufferObject = new VBO(vertices);

            vertexArrayObject.LinkToVAO(0, 3, vertexBufferObject);

            textureVBO = new VBO(texCoords);
            vertexArrayObject.LinkToVAO(1, 2, textureVBO);

            elementBufferObject = new IBO(indices);

            shaderProgram = new ShaderProgram("default.vert", "default.frag");

            texture = new Texture("texture.png");
                
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

            shaderProgram.Bind();
            vertexArrayObject.Bind();
            elementBufferObject.Bind();
            texture.Bind();

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

            gameModel.Render(shaderProgram);

            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, 0);


            SwapBuffers();

            base.OnRenderFrame(e);

        }

        // Cleanup when closing
        protected override void OnUnload()
        {
            base.OnUnload();

            vertexArrayObject.Delete();
            elementBufferObject.Delete();
            texture.Delete();
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
