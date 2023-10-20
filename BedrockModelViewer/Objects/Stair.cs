

using BedrockModelViewer.Graphics;
using OpenTK.Mathematics;

namespace BedrockModelViewer.Objects
{
    public class Stair : RenderableObject
    {
        public Stair(Vector3 postition, string texturePath) : base(postition, texturePath) { 
            GenerateModel();
        }

        public override void GenerateModel()
        {
            List<Vector3> v = new List<Vector3>()
            {
                // front bottom face
                new Vector3(0f, .5f, 1f), // topleft vert
                new Vector3(1f, .5f, 1f), // topright vert
                new Vector3(1f, 0f, 1f), // bottomright vert
                new Vector3(0f, 0f, 1f), // bottomleft vert

                // front top face
                new Vector3(0f, 1f, .5f), // topleft vert
                new Vector3(1f, 1f, .5f), // topright vert
                new Vector3(1f, .5f, .5f), // bottomright vert
                new Vector3(0f, .5f, .5f), // bottomleft vert

                // right bottom face
                new Vector3(1f, .5f, 1f),  // topleft vert
                new Vector3(1f, .5f, 0f),
                new Vector3(1f, 0f, 0f),
                new Vector3(1f, 0f, 1f),

                // right top face
                new Vector3(1f, 1f, .5f),  // topleft vert
                new Vector3(1f, 1f, 0f),
                new Vector3(1f, .5f, 0f),
                new Vector3(1f, .5f, .5f),

                // back face
                new Vector3(1f, 1f, 0f),
                new Vector3(0f, 1f, 0f),
                new Vector3(0f, 0f, 0f),
                new Vector3(1f, 0f, 0f),

                // left bottom
                new Vector3(0f, .5f, 0f),
                new Vector3(0f, .5f, 1f),
                new Vector3(0f, 0f, 1f),
                new Vector3(0f, 0f, 0f),

                // left top
                new Vector3(0f, 1f, .5f),  // topleft vert
                new Vector3(0f, 1f, 0f),
                new Vector3(0f, .5f, 0f),
                new Vector3(0f, .5f, .5f),

                // top bottom
                new Vector3(0f, .5f, .5f),
                new Vector3(1f, .5f, .5f),
                new Vector3(1f, .5f, 1f),
                new Vector3(0f, .5f, 1f),

                // top top
                new Vector3(0f, 1f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(1f, 1f, .5f),
                new Vector3(0f, 1f, .5f),

                // bottom
                new Vector3(0f, 0f, 1f),
                new Vector3(1f, 0f, 1f),
                new Vector3(1f, 0f, 0f),
                new Vector3(0f, 0f, 0f),
            };
            // Scale 
            v = RenderTools.ScaleToBlock(v);

            List<Vector2> u = new List<Vector2>()
            {
                // front bottom face
                new Vector2(0f, .5f),
                new Vector2(1f, .5f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // front top face
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, .5f),
                new Vector2(0f, .5f),

                // right bottom face
                new Vector2(0f, .5f),
                new Vector2(1f, .5f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

               // right top face
                new Vector2(.5f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, .5f),
                new Vector2(.5f, .5f),

                // back face
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // left bottom
                new Vector2(0f, .5f),
                new Vector2(1f, .5f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // left top
                new Vector2(0f, 1f),
                new Vector2(.5f, 1f),
                new Vector2(.5f, .5f),
                new Vector2(0f, .5f),

                // top bottom
                new Vector2(0f, .5f),
                new Vector2(1f, .5f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),

                // top top
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, .5f),
                new Vector2(0f, .5f),

                // bottom
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),
            };

            List<uint> i = new List<uint>
            {
                // front bottom face
                0, 1, 2,
                2, 3, 0,

                // front top face
                4, 5, 6,
                6, 7, 4,

                // right bottom
                8, 9, 10,
                10, 11, 8,

                // right top
                12, 13, 14,
                14, 15, 12,

                // back 
                16, 17, 18,
                18, 19, 16,

                // left bottom
                20, 21, 22,
                22, 23, 20, 

                // left top
                24, 25, 26,
                26, 27, 24,
            
                // top bottom
                28, 29, 30,
                30, 31, 28,

                // top top
                32, 33, 34,
                34, 35, 32,

                36, 37, 38,
                38, 39, 36,
            };

            SetData(v, u, i);
        }
    }
}
