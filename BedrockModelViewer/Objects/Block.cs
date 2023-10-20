using BedrockModelViewer.Graphics;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BedrockModelViewer.Objects.RectangularPrism;

namespace BedrockModelViewer.Objects
{
    internal class Block : RenderableObject
    {
        public Vector3 position;

        public List<Vector2> fullUV = new List<Vector2>
        {
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(1f, 0f),
            new Vector2(0f, 0f),
        };

        public Block(Vector3 position, string texturePath) : base(position, texturePath)
        {
            GenerateModel();
        }

        public Block(Vector3 position) : base(position)
        {
            GenerateModel();
        }

        public override void GenerateModel()
        {
            List<Vector3> v = new List<Vector3>()
            {
                    new Vector3(0f, 1f, 1f), // topleft vert
                    new Vector3(1f, 1f, 1f), // topright vert
                    new Vector3(1f, 0f, 1f), // bottomright vert
                    new Vector3(0f, 0f, 1f), // bottomleft vert
                    new Vector3(1f, 1f, 1f), // topleft vert
                    new Vector3(1f, 1f, 0f), // topright vert
                    new Vector3(1f, 0f, 0f), // bottomright vert
                    new Vector3(1f, 0f, 1f), // bottomleft vert
                    new Vector3(1f, 1f, 0f), // topleft vert
                    new Vector3(0f, 1f, 0f), // topright vert
                    new Vector3(0f, 0f, 0f), // bottomright vert
                    new Vector3(1f, 0f, 0f), // bottomleft vert
                    new Vector3(0f, 1f, 0f), // topleft vert
                    new Vector3(0f, 1f, 1f), // topright vert
                    new Vector3(0f, 0f, 1f), // bottomright vert
                    new Vector3(0f, 0f, 0f), // bottomleft vert
                    new Vector3(0f, 1f, 0f), // topleft vert
                    new Vector3(1f, 1f, 0f), // topright vert
                    new Vector3(1f, 1f, 1f), // bottomright vert
                    new Vector3(0f, 1f, 1f), // bottomleft vert
                    new Vector3(0f, 0f, 1f), // topleft vert
                    new Vector3(1f, 0f, 1f), // topright vert
                    new Vector3(1f, 0f, 0f), // bottomright vert
                    new Vector3(0f, 0f, 0f), // bottomleft vert
            };
            // Scale 
            v = RenderTools.ScaleToBlock(v);

            List<Vector2> u = new List<Vector2>()
            {
                // front
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // right
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // back
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // left
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // top
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // bottom
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

            };

            List<uint> i = new List<uint>
            {
                // front
                0, 1, 2,
                2, 3, 0,

                // right
                4, 5, 6,
                6, 7, 4,

                // back
                8, 9, 10,
                10, 11, 8,

                // left
                12, 13, 14,
                14, 15, 12,

                // top 
                16, 17, 18,
                18, 19, 16,

                // bottom
                20, 21, 22,
                22, 23, 20,
            };

            SetData(v, u, i);
        }

        public List<Vector2> GetUVCoordinates()
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < 6; i++)
            {
                result.AddRange(fullUV);
            }
            return result;
        }
    }
}
