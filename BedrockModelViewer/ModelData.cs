
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Image = System.Drawing.Image;
using System.Net.NetworkInformation;
using OpenTK.Mathematics;
using System.Reflection.Metadata.Ecma335;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace BedrockModelViewer
{
    class ModelData
    {
        public class Cube
        {
            [JsonProperty(PropertyName = "origin")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 origin { get; set; } = new Vector3(0, 0, 0);
            [JsonProperty(PropertyName = "size")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 size { get; set; } = new Vector3(8, 8, 8);
            [JsonProperty(PropertyName = "uv")]

            [JsonConverter(typeof(Vector2Converter))]
            public Vector2 uv { get; set; }

        }

        public class Bone
        {
            [JsonProperty(PropertyName = "name")]
            public string name { get; set; }

            [JsonProperty(PropertyName = "cubes")]
            public List<Cube> cubes { get; set; }

            [JsonProperty(PropertyName = "inflate")]
            public double inflate { get; set; }

            [JsonProperty(PropertyName = "parent")]
            public string parent { get; set; }

            [JsonProperty(PropertyName = "pivot")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 pivot { get; set; }
        }

        public class Geometry
        {
            [JsonProperty(PropertyName = "bones")]
            public List<Bone> bones { get; set; }

            [JsonProperty(PropertyName = "textureheight")]
            public int textureHeight { get; set; }

            [JsonProperty(PropertyName = "texturewidth")]
            public int textureWidth { get; set; }
        }

        public class MinecraftModelConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                // Specify the type(s) that this converter can handle.
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                var jsonObject = JObject.Load(reader);

                List<JToken> matchingGeometryTokens = new List<JToken>();

                foreach (var property in jsonObject.Properties())
                {
                    if (property.Name.StartsWith("geometry.", StringComparison.OrdinalIgnoreCase))
                    {
                        matchingGeometryTokens.Add(property.Value);
                    }
                }

                JToken jToken = null;

                if (matchingGeometryTokens.Count > 0)
                {
                    jToken = matchingGeometryTokens[0];

                    return new MinecraftModel { minecraftgeometry = new Geometry[] { jToken.ToObject<Geometry>() } };
                }
                else
                {
                    Debug.WriteLine("NO GEOMETRY FOUND");
                }
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Implement if you need custom serialization logic (optional).
            }
        }

        public class Vector3Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                // Specify the type(s) that this converter can handle.
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken jsonObject = JToken.Load(reader);

                if (jsonObject.Type == JTokenType.Array)
                {
                    float[] array = jsonObject.ToObject<float[]>();
                    return new Vector3(array[0], array[1], array[2]);
                }
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Implement if you need custom serialization logic (optional).
            }
        }

        public class Vector2Converter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                // Specify the type(s) that this converter can handle.
                return true;
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JToken jsonObject = JToken.Load(reader);

                if (jsonObject.Type == JTokenType.Array)
                {
                    float[] array = jsonObject.ToObject<float[]>();
                    return new Vector2(array[0], array[1]);
                }
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Implement if you need custom serialization logic (optional).
            }
        }

        [JsonConverter(typeof(MinecraftModelConverter))]
        public class MinecraftModel
        {
            public Geometry[] minecraftgeometry { get; set; }
        }

        public List<Vector2> Normalize(List<Vector2> vectors, int textureWidth, int textureHeight)
        {
            float hTexel = 1f / textureHeight;
            float wTexel = 1f / textureWidth;
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 vector in vectors)
            {
                result.Add(new Vector2(vector.X * hTexel, 1 - (vector.Y * wTexel)));
            }
            return result;
        }

        public List<Vector2> GetUVCoordinates(Vector2 origin, Vector3 size, int textureWidth, int textureHeight)
        {


            List<Vector2> allSides = new List<Vector2> { };


            // front
            Vector2 topLeft = new Vector2(origin.X + size.Z, origin.Y + size.Z);
            Vector2 topRight = topLeft + new Vector2(size.X, 0);
            Vector2 bottomLeft = topLeft + new Vector2(0, size.Y);
            Vector2 bottomRight = bottomLeft + new Vector2(size.X, 0);

            allSides.AddRange(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft,   });
            //Debug.WriteLine($"FRONT SIDE ({topLeft.X},{topLeft.Y}) ({topRight.X},{topRight.Y}) ({bottomLeft.X},{bottomLeft.Y}) ({bottomRight.X},{bottomRight.Y}) ");
            
            // back
            topLeft = new Vector2(origin.X + (2 * size.Z) + size.X, origin.Y + size.Z);
            topRight = topLeft + new Vector2(size.X, 0);
            bottomLeft = topLeft + new Vector2(0, size.Y);
            bottomRight = bottomLeft + new Vector2(size.X, 0);

            allSides.AddRange(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft,   });
            //Debug.WriteLine($"BACK SIDE ({topLeft.X},{topLeft.Y}) ({topRight.X},{topRight.Y}) ({bottomLeft.X},{bottomLeft.Y}) ({bottomRight.X},{bottomRight.Y}) ");


            // left
            topLeft = new Vector2(origin.X, origin.Y + size.Z);
            topRight = topLeft + new Vector2(size.Z, 0);
            bottomLeft = topLeft + new Vector2(0, size.Y);
            bottomRight = bottomLeft + new Vector2(size.Z, 0);

            allSides.AddRange(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft,   });
            //Debug.WriteLine($"LEFT SIDE ({topLeft.X},{topLeft.Y}) ({topRight.X},{topRight.Y}) ({bottomLeft.X},{bottomLeft.Y}) ({bottomRight.X},{bottomRight.Y}) ");


            // right
            topLeft = new Vector2(origin.X + size.X + size.Z, origin.Y + size.Z);
            topRight = topLeft + new Vector2(size.Z, 0);
            bottomLeft = topLeft + new Vector2(0, size.Y);
            bottomRight = bottomLeft + new Vector2(size.Z, 0);

            allSides.AddRange(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft, });
            //Debug.WriteLine($"RIGHT SIDE ({topLeft.X},{topLeft.Y}) ({topRight.X},{topRight.Y}) ({bottomLeft.X},{bottomLeft.Y}) ({bottomRight.X},{bottomRight.Y}) ");

            // top 
            topLeft = new Vector2(origin.X + size.Z, origin.Y);
            topRight = topLeft + new Vector2(size.X, 0);
            bottomLeft = topLeft + new Vector2(0, size.Z);
            bottomRight = bottomLeft + new Vector2(size.X, 0);

            allSides.AddRange(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft, });
            //Debug.WriteLine($"TOP SIDE ({topLeft.X},{topLeft.Y}) ({topRight.X},{topRight.Y}) ({bottomLeft.X},{bottomLeft.Y}) ({bottomRight.X},{bottomRight.Y}) ");


            // bottom
            topLeft = new Vector2(origin.X + size.Z + size.X, origin.Y);
            topRight = topLeft + new Vector2(size.X, 0);
            bottomLeft = topLeft + new Vector2(0, size.Z);
            bottomRight = bottomLeft + new Vector2(size.X, 0);

            allSides.AddRange(new List<Vector2> { topLeft, topRight, bottomRight, bottomLeft,   });
            //Debug.WriteLine($"BOTTOM SIDE ({topLeft.X},{topLeft.Y}) ({topRight.X},{topRight.Y}) ({bottomLeft.X},{bottomLeft.Y}) ({bottomRight.X},{bottomRight.Y}) ");

            return Normalize(allSides, textureWidth, textureHeight);
        }

        private Vector3 RescaleVector(float scale, Vector3 vector)
        {
            return new Vector3(vector.X * scale, vector.Y * scale, vector.Z * scale);

        }

        
        public void GenerateGeometry(Vector3 origin, Vector2 uv, Vector3 size, string TexturePath, out List<Vector3> positions, out List<Vector2> uvs)
        {
            //float scalar = 1 / 8f;
            //origin = RescaleVector(scalar, origin);
            //size = RescaleVector(scalar, size);


            // The verticies (edges) of the cube
            positions = new List<Vector3>(){

                origin + new Vector3(0, size.Y, 0),             // front top left           
                origin + new Vector3(size.X, size.Y, 0),        // front top right          
                origin + new Vector3(size.X, 0, 0),             // front bottom right       
                origin,                                         // front bottom left        
                
                origin + new Vector3(size.X, size.Y, 0),        // right top left           
                origin + new Vector3(size.X, size.Y, -size.Z),  // right top right          
                origin + new Vector3(size.X, 0, -size.Z),       // right bottom right
                origin + new Vector3(size.X, 0, 0),             // right bottom left 

                origin + new Vector3(size.X, size.Y, -size.Z),  // back top left            
                origin + new Vector3(0, size.Y, -size.Z),       // back top right 
                origin + new Vector3(0, 0, -size.Z),            // back bottom right 
                origin + new Vector3(size.X, 0, -size.Z),       // back bottom left         
                       
                origin + new Vector3(0, size.Y, -size.Z),       // left top left            
                origin + new Vector3(0, size.Y, 0),             // left top right           
                origin,                                         // left bottom right
                origin + new Vector3(0, 0, -size.Z),            // left bottom left         
                        
                origin + new Vector3(0, size.Y, -size.Z),       // top top left             
                origin + new Vector3(size.X, size.Y, -size.Z),  // top top right          
                origin + new Vector3(size.X, size.Y, 0),        // top bottom right
                origin + new Vector3(0, size.Y, 0),             // top bottom left          
                         
                origin,                                         // bottom top left          
                origin + new Vector3(size.X, 0, 0),             // bottom top right             
                origin + new Vector3(size.X, 0, -size.Z),       // bottom bottom right 
                origin + new Vector3(0, 0, -size.Z),            // bottom bottom left    
            };

            //Debug.WriteLine($"Creating Box of size {size.X}x{size.Y}x{size.Z} at {origin.X},{origin.Y},{origin.Z}");

            // CCW
            List<uint> indices = new List<uint>
            {
                0,  1,  2,          // front bottom triangle
                1,  3,  2,          // front top triangle

                4,  5,  6,          // back bottom triangle
                5,  7,  6,          // back top triangle

                8,  9,  10,         // left bottom triangle
                9, 11,  10,         // left top triangle

                12, 13, 14,         // right bottom triangle
                13, 15, 14,         // right top triangle 

                16, 17, 18,         // top bottom triangle
                17, 19, 18,         // top top triangle

                20, 21, 22,         // bottom bottom triangle
                21, 23, 22,         // bottom top triangle
            };

            int width = 0;
            int height = 0;

            using (Image img = Image.FromFile(TexturePath))
            {
                width = img.Width;
                height = img.Height;
            }

            uvs = GetUVCoordinates(uv, RescaleVector(8f, size), width, height);
        }

        public ModelInfo model { get; set; } = new ModelInfo();

        public ModelData(string modelPath, string texturePath)
        {
            // Parse the JSON data
            var json = File.ReadAllText(modelPath); // Load your JSON file
            var modelData = JsonConvert.DeserializeObject<MinecraftModel>(json);

            // Create a list to store each cube mesh
            foreach (Geometry geometry in modelData.minecraftgeometry)
            {
                // Process the cubes and create vertices and indices
                foreach (Bone bone in geometry.bones)
                {
                    foreach (Cube cube in bone.cubes)
                    {
                        List<Vector3> cubePositions;
                        List<Vector2> cubeUVs;
                        List<int> cubeIndices;

                        GenerateGeometry(cube.origin, cube.uv, cube.size, texturePath, out cubePositions, out cubeUVs);

                        model.AddInfo(cubePositions, cubeUVs);
                    }
                }
            }
        }
    }

    public class ModelInfo
    {
        public List<Vector3> Vertices { get; set; } = new List<Vector3>();
        public List<Vector2> UVs { get; set; } = new List<Vector2>();
        public List<uint> Indices { get; set; } = new List<uint>();
        private uint indexCount = 0;

        public void AddInfo(List<Vector3> vertices, List<Vector2> uvs)
        {
            Vertices.AddRange(vertices);
            UVs.AddRange(uvs);

            AddIndices(vertices.Count / 4);
        }

        private void AddIndices(int numFaces)
        {
            for (int i = 0; i < numFaces; i++)
            {
                Indices.Add(0 + indexCount);
                Indices.Add(3 + indexCount);
                Indices.Add(2 + indexCount);
                Indices.Add(2 + indexCount);
                Indices.Add(1 + indexCount);
                Indices.Add(0 + indexCount);

                indexCount += 4;
            }
        }
    }
}
