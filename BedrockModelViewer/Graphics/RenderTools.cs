using OpenTK.Mathematics;
using static BedrockModelViewer.Objects.ModelData;
using static BedrockModelViewer.Objects.ModelData.CustomUV;

namespace BedrockModelViewer.Graphics
{
    public class RenderTools
    {
        public static Vector3 GetMin(List<Vector3> vectors)
        {
            Vector3 min = new Vector3(float.MaxValue);

            foreach (var vect in vectors)
            {
                min = Vector3.ComponentMin(min, vect);
            }
            return min;
        }

        public static List<Vector3> CenterOnOrigin(List<Vector3> Vertices)
        {
            Vector3 offset = GetOffset(Vertices);
            List<Vector3> result = Vertices;
            if (offset != (0, 0, 0))
            {
                result.Clear();
                foreach (var vertex in Vertices)
                {
                    result.Add(vertex - offset);
                }
            }
            return result;
        }

        public static Vector3 GetCenter(List<Vector3> Vertices)
        {
            // Calculate the bounding box
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var vertex in Vertices)
            {
                min = Vector3.ComponentMin(min, vertex);
                max = Vector3.ComponentMax(max, vertex);
            }
            // Calculate the model's center
            Vector3 average = (min + max) / 2;

            Vector3 offset = (average.X, average.Y, average.Z);
            return offset;
        }

        public static Vector3 GetOffset(List<Vector3> Vertices)
        {
            // Calculate the bounding box
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);

            foreach (var vertex in Vertices)
            {
                min = Vector3.ComponentMin(min, vertex);
                max = Vector3.ComponentMax(max, vertex);
            }
            // Calculate the model's center
            Vector3 average = (min + max) / 2;

            Vector3 offset = (average.X, min.Y, average.X);
            return offset;
        }

        public static Vector3 GetMax(List<Vector3> vectors)
        {
            Vector3 max = new Vector3(float.MinValue);

            foreach (var vect in vectors)
            {
                max = Vector3.ComponentMax(max, vect);
            }
            return max;
        }

        public static List<Vector3> ScaleToBlock(List<Vector3> vectors)
        {
            List<Vector3> result = new List<Vector3>();
            Vector3 scaler = new Vector3(16, 16, 16);
            foreach (var vector in vectors)
            {
                result.Add(vector * scaler);
            }
            return result;
        }

        public static Vector3 RescaleVector(float scale, Vector3 vector)
        {
            return new Vector3(vector.X * scale, vector.Y * scale, vector.Z * scale);
        }

        private static Vector2 ConvertCoordinates(Vector2 coords, float textureHeight)
        {
            return new Vector2(coords.X, textureHeight - coords.Y);
        }

        public static List<Vector2> Normalize(List<Vector2> vectors, int maxWidth, int maxHeight)
        {
            float hTexel = 1f / maxHeight;
            float wTexel = 1f / maxWidth;
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 vector in vectors)
            {
                result.Add(new Vector2(vector.X * hTexel, vector.Y * wTexel));
            }
            return result;
        }


        public static void Swap(ref Vector2 a, ref Vector2 b)
        {
            Vector2 temp = a;
            a = b;
            b = temp;
        }

        public static List<Vector2> MirrorUV(List<Vector2> UVs)
        {

            List<List<Vector2>> result = new List<List<Vector2>>();

            List<Vector2> face = new List<Vector2>(); ;

            for (int i = 0; i < UVs.Count; i++)
            {
                face.Add(UVs[i]);
                if ((i + 1) % 4 == 0)
                {
                    result.Add(face);
                    face = new List<Vector2>();
                }
            }


            List<Vector2> tmp = new();
            // plane
            if (result.Count == 2)
            {
                tmp = result[0];
                result[0] = result[1];
                result[1] = tmp;
            }
            else
            {
                tmp = result[1];
                result[1] = result[3];
                result[3] = tmp;
            }
            
            List<Vector2> Mirrored = new List<Vector2>();
            foreach (List<Vector2> vec in result)
            {
                Vector2 temp = vec[0];
                vec[0] = vec[1];
                vec[1] = temp;

                temp = vec[2];
                vec[2] = vec[3];
                vec[3] = temp;

                foreach (Vector2 vec2 in vec)
                {
                    Mirrored.Add(vec2);
                }
            }

            return Mirrored;
        }

        public static List<Vector3> PostitionModel(Vector3 position, List<Vector3> vertices) {
            // Find the minimum and maximum values in X and Z coordinates.
            float minX = float.MaxValue;
            float minZ = float.MaxValue;
            float maxX = float.MinValue;
            float maxZ = float.MinValue;

            foreach (var vector in vertices)
            {
                minX = Math.Min(minX, vector.X);
                minZ = Math.Min(minZ, vector.Z);
                maxX = Math.Max(maxX, vector.X);
                maxZ = Math.Max(maxZ, vector.Z);
            }

            // Calculate the center of the model.
            float centerX = (minX + maxX) / 2.0f;
            float centerZ = (minZ + maxZ) / 2.0f;

            // Calculate the offset to move the model to the target position.
            float offsetX = position.X - centerX;
            float offsetZ = position.Z - centerZ;

            // Apply the offset to all Vector3 positions in the model list.
            List<Vector3> centeredModel = new List<Vector3>();

            foreach (var vector in vertices)
            {
                float newX = vector.X + offsetX;
                float newZ = vector.Z + offsetZ;
                float newY = vector.Y + position.Y;
                Vector3 centeredVector = new Vector3(newX, newY, newZ);
                centeredModel.Add(centeredVector);
            }

            return centeredModel;
        }
    

        private static bool rightDefined(UVDATA side)
        {
            return side.uvSize.X < 0;
        }

        private static bool bottomDefined(UVDATA side)
        {
            return side.uvSize.Y < 0;
        }

        public static List<Vector2> CustomPlaneUV(CustomUV custom, int textureWidth, int textureHeight)
        {
            List<Vector2> uvs = new List<Vector2>();

            Console.WriteLine($"CUSTOM UV (NESWUD): {custom?.north},{custom?.east},{custom?.south},{custom?.west},{custom?.up},{custom?.down}");

            List<UVDATA> sides = new List<UVDATA>();

            if (custom.east != null)
            {
                sides.Add(custom.east);
            }
            if (custom.west != null)
            {
                sides.Add(custom.west);
            }
            if (sides.Count < 2)
            {
                sides.Add(sides[0]);
            }

            foreach (UVDATA side in sides)
            {
                bool right = rightDefined(side);
                bool bottom = bottomDefined(side);

                Vector2 TopLeft;
                Vector2 TopRight;
                Vector2 BottomRight;
                Vector2 BottomLeft;

                if (right && bottom)
                {
                    BottomRight = side.uv;
                    BottomLeft = side.uv + (side.uvSize.X, 0);
                    TopRight = side.uv + (0, side.uvSize.Y);
                    TopLeft = side.uv + (side.uvSize.X, side.uvSize.Y);
                }
                else if (right)
                {
                    TopRight = side.uv;
                    TopLeft = side.uv + (side.uvSize.X, 0);
                    BottomRight = side.uv + (0, side.uvSize.Y);
                    BottomLeft = side.uv + (side.uvSize.X, side.uvSize.Y);
                }
                else if (bottom)
                {
                    BottomLeft = side.uv;
                    TopLeft = side.uv + (0, side.uvSize.Y);
                    BottomRight = side.uv + (side.uvSize.X, 0);
                    TopRight = side.uv + (side.uvSize.X, side.uvSize.Y);
                }
                else
                {
                    TopLeft = side.uv;
                    TopRight = side.uv + (side.uvSize.X, 0);
                    BottomRight = side.uv + (side.uvSize.X, side.uvSize.Y);
                    BottomLeft = side.uv + (0, side.uvSize.Y);
                }

                TopLeft = ConvertCoordinates(TopLeft, textureHeight);
                TopRight = ConvertCoordinates(TopRight, textureHeight);
                BottomRight = ConvertCoordinates(BottomRight, textureHeight);
                BottomLeft = ConvertCoordinates(BottomLeft, textureHeight);

                List<Vector2> coords = new List<Vector2>() { TopLeft, TopRight, BottomRight, BottomLeft };
                uvs.AddRange(Normalize(coords, textureWidth, textureHeight));
            }
            return uvs;
        }

        public static List<Vector2> EvaluateCustomUV(CustomUV custom, int textureWidth, int textureHeight)
        {
            List<Vector2> uvs = new List<Vector2>();

            foreach (UVDATA side in new List<UVDATA> { custom.north, custom.west, custom.south, custom.east, custom.up, custom.down})
            {
                bool right = rightDefined(side);
                bool bottom = bottomDefined(side);

                Vector2 TopLeft;
                Vector2 TopRight;
                Vector2 BottomRight;
                Vector2 BottomLeft;

                if (right && bottom)
                {
                    BottomRight = side.uv;
                    BottomLeft = side.uv + (side.uvSize.X,0);
                    TopRight = side.uv + (0, side.uvSize.Y);
                    TopLeft = side.uv + (side.uvSize.X, side.uvSize.Y);
                }
                else if (right)
                {
                    TopRight = side.uv;
                    TopLeft = side.uv + (side.uvSize.X, 0);
                    BottomRight = side.uv + (0, side.uvSize.Y);
                    BottomLeft = side.uv + (side.uvSize.X, side.uvSize.Y);
                }
                else if (bottom)
                {
                    BottomLeft = side.uv;
                    TopLeft = side.uv + (0, side.uvSize.Y);
                    BottomRight = side.uv + (side.uvSize.X, 0);
                    TopRight = side.uv + (side.uvSize.X, side.uvSize.Y);
                }
                else
                {
                    TopLeft = side.uv;
                    TopRight = side.uv + (side.uvSize.X, 0);
                    BottomRight = side.uv + (side.uvSize.X, side.uvSize.Y);
                    BottomLeft = side.uv + (0, side.uvSize.Y);
                }

                TopLeft = ConvertCoordinates(TopLeft, textureHeight);
                TopRight = ConvertCoordinates(TopRight, textureHeight);
                BottomRight = ConvertCoordinates(BottomRight, textureHeight);
                BottomLeft = ConvertCoordinates(BottomLeft, textureHeight);

                List<Vector2> coords = new List<Vector2>() { TopLeft, TopRight, BottomRight, BottomLeft };
                uvs.AddRange(Normalize(coords, textureWidth, textureHeight));
            }
            return uvs;
        }
    
    }
}
