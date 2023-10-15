using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockModelViewer.Objects
{
    internal class Block : RectangularPrism
    {
        public Vector3 position;

        public List<Vector2> fullUV = new List<Vector2>
            {
                new Vector2(0f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, 0f),
                new Vector2(0f, 0f),
            };
        public Block(Vector3 position) : base(position, new Vector3(1f, 1f, 1f), new Vector2(), new Vector2())
        {

        }
        public List<Vector3> AddTransformedVertices(List<Vector3> vertices)
        {
            List<Vector3> transformedVertices = new List<Vector3>();
            foreach (var vert in vertices)
            {
                transformedVertices.Add(vert + position);
            }
            return transformedVertices;
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
