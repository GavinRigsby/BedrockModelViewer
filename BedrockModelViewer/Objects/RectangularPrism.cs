using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockModelViewer.Objects
{
    public class RectangularPrism
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

            public List<Vector2> GetUVCoordinates()
            {
                List<Vector2> result = new List<Vector2>();
                result.AddRange(GetFaceUV(Faces.FRONT));
                result.AddRange(GetFaceUV(Faces.RIGHT));
                result.AddRange(GetFaceUV(Faces.BACK));
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
                //Debug.WriteLine($"W: {width}, H: {height}");
                Vector2 TopLeft = ConvertCoordinates(UVOffsets[face]);
                Vector2 TopRight = ConvertCoordinates(UVOffsets[face] + new Vector2(width, 0));
                Vector2 BottomRight = ConvertCoordinates(UVOffsets[face] + new Vector2(width, height));
                Vector2 BottomLeft = ConvertCoordinates(UVOffsets[face] + new Vector2(0, height));
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

        public class VertData
        {
            private Vector3 Size;
            private Vector3 Position;

            public List<Vector3> GetVerticies()
            {
                List<Vector3> result = new List<Vector3>();
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
                    transformedVertices.Add(vert + Position);
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
            }
        }

        public List<Vector3> Vertices { get; private set; }

        public List<Vector2> UVs { get; private set; }

        public List<uint> Indicies { get; private set; } = new List<uint>();


        public Vector3 Origin { get; set; }
        public Vector3 Size { get; set; }
        public Vector2 TextureSize { get; set; }
        public Vector2 TextureOrigin { get; set; }


        public void AddIndices()
        {
            Indicies.Clear();
            uint indexCount = 0;
            for (int i = 0; i < 6; i++)
            {
                Indicies.Add(0 + indexCount);
                Indicies.Add(1 + indexCount);
                Indicies.Add(2 + indexCount);
                Indicies.Add(2 + indexCount);
                Indicies.Add(3 + indexCount);
                Indicies.Add(0 + indexCount);

                indexCount += 4;
            }
        }

        public RectangularPrism(Vector3 origin, Vector3 size, Vector2 textureSize, Vector2 textureOrigin)
        {
            this.Origin = origin;
            this.Size = size;
            this.TextureSize = textureSize;
            this.TextureOrigin = textureOrigin;

            GenerateModel();
        }

        public void GenerateModel()
        {
            UVData data = new(TextureOrigin, Size, TextureSize);
            VertData vertData = new VertData(Size, Origin);

            Vertices = vertData.GetVerticies();
            UVs = data.GetUVCoordinates();
            AddIndices();
        }
    }
}
