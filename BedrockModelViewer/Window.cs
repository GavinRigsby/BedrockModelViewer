using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Diagnostics;
using StbImageSharp;
using OpenTK.Windowing.GraphicsLibraryFramework;
using BedrockModelViewer.Graphics;
using OpenTK.Compute.OpenCL;
using BedrockModelViewer.Objects;
using System.Drawing;
using System.Drawing.Imaging;

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
        private RenderTools texture;
        private VBO textureVBO;
        private int width;
        private int height;
        private float yRot = 0f;
        private float xRot = 0f;
        private float zRot = 0f;
        private Camera camera;
        private bool focused = false;
        private ModelObject gameModel;
        private Stair testStair;
        private Block testBlock;

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

        private Bitmap ExportImage()
        {

            Bitmap bmp = new Bitmap(this.ClientSize.X, this.ClientSize.Y);

            // Create a data buffer to read the OpenGL framebuffer into
            byte[] data = new byte[4 * width * height];

            // Read the framebuffer
            GL.ReadPixels(0, 0, width, height, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, data);

            // Lock the bits of the bitmap
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Copy the data into the bitmap
            System.Runtime.InteropServices.Marshal.Copy(data, 0, bmpData.Scan0, data.Length);

            // Unlock the bitmap
            bmp.UnlockBits(bmpData);

            // Flip the image vertically if needed
            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return bmp;
        }

        private List<Bitmap> ScreenShotImages = new();
        private Color4 secondaryColor = Color4.LimeGreen;

        public void SaveModelImage()
        {
            Bitmap Image = ExportImage();
            ScreenShotImages.Add(Image);
            Color4 tmp = _backColor;
            _backColor = secondaryColor;
            secondaryColor = tmp;

            if (ScreenShotImages.Count == 2)
            {
                Bitmap result = RemoveBackground(ScreenShotImages[0], ScreenShotImages[1]);
                string output = _imageOutput == null ? "result.png" : _imageOutput;
                result.Save(output);
                if (_imageOutput != null)
                {
                    Environment.Exit(0);
                }
            }
            //Image.Save("myfile.png", System.Drawing.Imaging.ImageFormat.Png);
        }

        private Bitmap RemoveBackground(Bitmap first, Bitmap second)
        {
            // Create a new bitmap for the output
            Bitmap outputImage = new Bitmap(first.Width, first.Height);

            for (int x = 0; x < first.Width; x++)
            {
                for (int y = 0; y < first.Height; y++)
                {
                    Color pixel1 = first.GetPixel(x, y);
                    Color pixel2 = second.GetPixel(x, y);

                    if (pixel1 == _backColor && pixel2 == secondaryColor)
                    {
                        outputImage.SetPixel(x, y, Color.Transparent);
                    }
                    else
                    {
                        outputImage.SetPixel(x, y, pixel2);
                    }
                }
            }

            // Save the modified image
            

            // Clean up resources
            first.Dispose();
            second.Dispose();

            return outputImage;
        }

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
            gameModel = new ModelObject(new Vector3(0f, 0f, 0f), _modelPath, _texturePath);
            shaderProgram = new ShaderProgram("default.vert", "default.frag");
            // Enable 3D
            GL.Enable(EnableCap.DepthTest);

            camera = new Camera(width, height, Vector3.Zero);
            camera.RotateAroundObject(gameModel, 55f);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0,0, e.Width, e.Height);
            this.width = e.Width;
            this.height = e.Height;
            camera.Resized(width, height);
        }

        private int RenderCount = 0;
        private float rotation = 0.0f;
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            // Clear the Color and Depth Buffers
            GL.ClearColor(_backColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //rotation += 0.0005f % 360;
            //camera.RotateAroundObject(gameModel, rotation);

            Matrix4 model = Matrix4.Identity;
            Matrix4 view = camera.GetViewMatrix();
            Matrix4 projection = camera.GetProjectionMatrix();

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
            //testStair.Render(shaderProgram);
            //testBlock.Render(shaderProgram);
            //testModel.Render(shaderProgram);

            SwapBuffers();
            base.OnRenderFrame(e);
            if (RenderCount > 10)
            {
                if (ScreenShotImages.Count < 2)
                {
                    SaveModelImage();
                    RenderCount = 0;
                }
            }
            else
            {
                RenderCount++;
            }
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
