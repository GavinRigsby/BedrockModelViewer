using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockModelViewer.Objects
{
    public class RectangularPrism : RenderableObject
    {
        public enum Faces
        {
            FRONT,
            BACK,
            LEFT,
            RIGHT,
            TOP,
            BOTTOM
        }

        public class UVData
        {
            public Dictionary<Faces, Vector2> UVOffsets;
            private Vector2 origin;
            private Vector3 size;
            private Vector2 textureSize;
            private bool isPlane;
            public List<Vector2> GetUVCoordinates()
            {
                List<Vector2> result = new List<Vector2>();
                if (isPlane)
                {
                    result.AddRange(GetFaceUV(Faces.LEFT));
                    result.AddRange(GetFaceUV(Faces.RIGHT));
                    return result;
                }
                result.AddRange(GetFaceUV(Faces.BACK));
                result.AddRange(GetFaceUV(Faces.RIGHT));
                result.AddRange(GetFaceUV(Faces.FRONT));
                result.AddRange(GetFaceUV(Faces.LEFT));
                result.AddRange(GetFaceUV(Faces.TOP));
                result.AddRange(GetFaceUV(Faces.BOTTOM));
                return result;
            }

            public List<Vector2> GetFaceUV(Faces face)
            {
                UVOffsets = new() {
                    { Faces.FRONT, origin + new Vector2(size.Z, size.Z)},
                    { Faces.BACK, origin + new Vector2(size.Z + size.X + size.Z, size.Z)},
                    { Faces.LEFT, origin + new Vector2(0, size.Z)},
                    { Faces.RIGHT, origin + new Vector2(size.Z + size.X, size.Z)},
                    { Faces.TOP, origin + new Vector2(size.Z, 0)},
                    { Faces.BOTTOM, origin + new Vector2(size.Z + size.X, 0)},
                };

                float width = 0;
                float height = 0;

                if (face == Faces.TOP || face == Faces.BOTTOM)
                {
                    height = size.Z;
                    width = size.X;
                }
                else
                {
                    height = size.Y;
                }

                if (face == Faces.LEFT || face == Faces.RIGHT)
                {
                    width = size.Z;
                }
                else
                {
                    width = size.X;
                }

                Vector2 TopLeft;
                Vector2 TopRight;
                Vector2 BottomRight;
                Vector2 BottomLeft;

                if (face == Faces.TOP || face == Faces.BOTTOM)
                {
                    //Debug.WriteLine($"W: {width}, H: {height}");
                    TopLeft = ConvertCoordinates(UVOffsets[face]);
                    TopRight = ConvertCoordinates(UVOffsets[face] + new Vector2(width, 0));
                    BottomRight = ConvertCoordinates(UVOffsets[face] + new Vector2(width, height));
                    BottomLeft = ConvertCoordinates(UVOffsets[face] + new Vector2(0, height));
                }
                else
                {
                    TopRight = ConvertCoordinates(UVOffsets[face]);
                    TopLeft = ConvertCoordinates(UVOffsets[face] + new Vector2(width, 0));
                    BottomLeft = ConvertCoordinates(UVOffsets[face] + new Vector2(width, height));
                    BottomRight = ConvertCoordinates(UVOffsets[face] + new Vector2(0, height));
                }
                //Debug.WriteLine($"{face} UV: {TopLeft.X}f/{textureSize.X},{TopLeft.Y}f/{textureSize.Y}  {TopRight.X}f/{textureSize.X},{TopRight.Y}f/{textureSize.Y} {BottomRight.X}f/{textureSize.X},{BottomRight.Y}f/{textureSize.Y} {BottomLeft.X}f/{textureSize.X},{BottomLeft.Y}f/{textureSize.Y}");


                List<Vector2> uvCoorinates = new List<Vector2>()
                { TopLeft, TopRight, BottomRight, BottomLeft };
                uvCoorinates = NormalizeUV(uvCoorinates);
                return uvCoorinates;
            }

            public UVData(Vector2 origin, Vector3 size, Vector2 textureSize)
            {
                this.origin = origin;
                this.size = size;
                this.textureSize = textureSize;
                CheckForPlane(size);
            }

            private void CheckForPlane(Vector3 size)
            {
                if (size.X < .05f)
                {
                    isPlane = true;
                }
            }

            private Vector2 ConvertCoordinates(Vector2 coords)
            {
                return new Vector2(coords.X, textureSize.Y - coords.Y);
            }

            private List<Vector2> NormalizeUV(List<Vector2> textureCoordinates)
            {
                List<Vector2> uvCoordinates = new List<Vector2>();
                foreach (Vector2 coodinates in textureCoordinates)
                {
                    Vector2 coords = coodinates;
                    Vector2 Scalar = new Vector2(1f / textureSize.X, 1f / textureSize.Y);

                    uvCoordinates.Add((coords.X * Scalar.X, coords.Y * Scalar.Y));
                }

                //Debug.WriteLine($"UV Norm: {uvCoordinates[0].X},{uvCoordinates[0].Y}  {uvCoordinates[1].X},{uvCoordinates[1].Y} {uvCoordinates[2].X},{uvCoordinates[2].Y} {uvCoordinates[3].X},{uvCoordinates[3].Y}");
                //Debug.WriteLine($"UV Norm: {uvCoordinates[0].X},{uvCoordinates[0].Y}  {uvCoordinates[1].X},{uvCoordinates[1].Y} {uvCoordinates[2].X},{uvCoordinates[2].Y} {uvCoordinates[3].X},{uvCoordinates[3].Y}");

                return uvCoordinates;
            }
        }

        // Rotates prism with its own value
        public void Rotate()
        {
            Rotate(rotation, pivot);
        }

        // Used to rotate prism if parent or higher family is rotated
        public void Rotate(Vector3 rotation, Vector3 pivot)
        {
            if (rotation == (0, 0, 0)) { return; }

            float xRotInRadians = MathHelper.DegreesToRadians(rotation.X);
            float yRotInRadians = -MathHelper.DegreesToRadians(rotation.Y);
            float zRotInRadians = -MathHelper.DegreesToRadians(rotation.Z);

            Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, xRotInRadians);
            Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, yRotInRadians);
            Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, zRotInRadians);

            for (int i = 0; i < Vertices.Count; i++)
            {
                Vector3 vertex = Vertices[i];

                Vector3 translatedVertex = vertex - (pivot * (1, 1, -1));

                // Apply the rotations in the order: x, y, z
                Vector3 rotatedVertex = Vector3.Transform(Vector3.Transform(Vector3.Transform(translatedVertex, xRotation), yRotation), zRotation);

                rotatedVertex += (pivot * (1, 1, -1));

                // Replace the original vertex with the rotated one
                Vertices[i] = rotatedVertex;
            }
        }

        public class VertData
        {
            private Vector3 Size;
            private Vector3 Position;
            private bool isPlane;
            public List<Vector3> GetVerticies()
            {
                List<Vector3> result = new List<Vector3>();
                if (isPlane)
                {
                    result.AddRange(GetFaceVerticies(Faces.RIGHT));
                    result.AddRange(GetFaceVerticies(Faces.LEFT));
                    return result;
                }

                result.AddRange(GetFaceVerticies(Faces.FRONT));
                result.AddRange(GetFaceVerticies(Faces.RIGHT));
                result.AddRange(GetFaceVerticies(Faces.BACK));
                result.AddRange(GetFaceVerticies(Faces.LEFT));
                result.AddRange(GetFaceVerticies(Faces.TOP));
                result.AddRange(GetFaceVerticies(Faces.BOTTOM));
                return result;
            }

            public List<Vector3> GetFaceVerticies(Faces face)
            {
                List<Vector3> defaultVerts = rawVertexData[face];
                List<Vector3> Verticies = new();
                foreach (Vector3 vert in defaultVerts)
                {
                    Verticies.Add(new Vector3(Size.X * vert.X, Size.Y * vert.Y, Size.Z * vert.Z));
                }
                return AddTransformedVertices(Verticies);
            }

            public List<Vector3> AddTransformedVertices(List<Vector3> vertices)
            {
                List<Vector3> transformedVertices = new List<Vector3>();
                foreach (var vert in vertices)
                {
                    transformedVertices.Add((vert + Position) * new Vector3(1, 1, -1));
                }
                return transformedVertices;
            }

            private static readonly Dictionary<Faces, List<Vector3>> rawVertexData = new Dictionary<Faces, List<Vector3>>
            {
                {Faces.FRONT, new List<Vector3>()
                {
                    new Vector3(0f, 1f, 1f), // topleft vert
                    new Vector3(1f, 1f, 1f), // topright vert
                    new Vector3(1f, 0f, 1f), // bottomright vert
                    new Vector3(0f, 0f, 1f), // bottomleft vert
                } },
                {Faces.BACK, new List<Vector3>()
                {
                    new Vector3(1f, 1f, 0f), // topleft vert
                    new Vector3(0f, 1f, 0f), // topright vert
                    new Vector3(0f, 0f, 0f), // bottomright vert
                    new Vector3(1f, 0f, 0f), // bottomleft vert
                } },
                {Faces.LEFT, new List<Vector3>()
                {
                    new Vector3(0f, 1f, 0f), // topleft vert
                    new Vector3(0f, 1f, 1f), // topright vert
                    new Vector3(0f, 0f, 1f), // bottomright vert
                    new Vector3(0f, 0f, 0f), // bottomleft vert
                } },
                {Faces.RIGHT, new List<Vector3>()
                {
                    new Vector3(1f, 1f, 1f), // topleft vert
                    new Vector3(1f, 1f, 0f), // topright vert
                    new Vector3(1f, 0f, 0f), // bottomright vert
                    new Vector3(1f, 0f, 1f), // bottomleft vert
                } },
                {Faces.TOP, new List<Vector3>()
                {
                    new Vector3(0f, 1f, 0f), // topleft vert
                    new Vector3(1f, 1f, 0f), // topright vert
                    new Vector3(1f, 1f, 1f), // bottomright vert
                    new Vector3(0f, 1f, 1f), // bottomleft vert
                } },
                {Faces.BOTTOM, new List<Vector3>()
                {
                    new Vector3(0f, 0f, 1f), // topleft vert
                    new Vector3(1f, 0f, 1f), // topright vert
                    new Vector3(1f, 0f, 0f), // bottomright vert
                    new Vector3(0f, 0f, 0f), // bottomleft vert
                } },
            };

            public VertData(Vector3 size, Vector3 position)
            {
                this.Size = size;
                this.Position = position;

                CheckForPlane(size);
            }

            private void CheckForPlane(Vector3 size)
            {
                if (Size.X < .05f)
                {
                    isPlane = true;
                }
            }
        }

        public Vector3 Origin { get; set; }
        public Vector3 Size { get; set; }
        public Vector2 TextureSize { get; set; }
        public Vector2 TextureOrigin { get; set; }

        public Vector3 pivot { get; set; }

        public Vector3 rotation { get; set; }

        public List<RectangularPrism> Children { get; set; }


        public void AddIndices(int faces = 6)
        {
            Indices.Clear();
            uint indexCount = 0;
            for (int i = 0; i < faces; i++)
            {
                Indices.Add(0 + indexCount);
                Indices.Add(1 + indexCount);
                Indices.Add(2 + indexCount);
                Indices.Add(2 + indexCount);
                Indices.Add(3 + indexCount);
                Indices.Add(0 + indexCount);
                indexCount += 4;
            }
        }

        public RectangularPrism(Vector3 origin, Vector3 size, Vector2 textureSize, Vector2 textureOrigin): base (origin)
        {
            this.Origin = origin;
            this.Size = size;
            this.TextureSize = textureSize;
            this.TextureOrigin = textureOrigin;

            GenerateModel();
        }

        public override void GenerateModel()
        {
            UVData data = new(TextureOrigin, Size, TextureSize);
            VertData vertData = new VertData(Size, Origin);

            Vertices = vertData.GetVerticies();
            UVs = data.GetUVCoordinates();

            if (Vertices.Count != UVs.Count)
            {
                Debug.WriteLine("WE GOT ISSUES");
            }

            if (Size.X < .05f)
            {
                AddIndices(2);
            }
            else {
                AddIndices();
            }
            
        }
    }
}
