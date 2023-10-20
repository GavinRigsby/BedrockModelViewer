
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Image = System.Drawing.Image;
using System.Net.NetworkInformation;
using OpenTK.Mathematics;
using System.Reflection.Metadata.Ecma335;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using BedrockModelViewer.Graphics;
using OpenTK.Graphics.GL;
using static BedrockModelViewer.Objects.ModelData;
using System.Data;

namespace BedrockModelViewer.Objects
{
    public class ModelData
    {
        public class CustomUV
        {
            public class UVDATA
            {
                [JsonProperty(PropertyName = "uv")]
                [JsonConverter(typeof(Vector2Converter))]
                public Vector2 uv { get; set; }

                [JsonProperty(PropertyName = "uv_size")]
                [JsonConverter(typeof(Vector2Converter))]
                public Vector2 uvSize { get; set; }
            }

            [JsonProperty(PropertyName = "north")]
            public UVDATA north { get; set; }

            [JsonProperty(PropertyName = "east")]
            public UVDATA east { get; set; }

            [JsonProperty(PropertyName = "south")]
            public UVDATA south { get; set; }

            [JsonProperty(PropertyName = "west")]
            public UVDATA west { get; set; }

            [JsonProperty(PropertyName = "up")]
            public UVDATA up { get; set; }

            [JsonProperty(PropertyName = "down")]
            public UVDATA down { get; set; }
        }

        public class UVConverter : JsonConverter
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
                else if (jsonObject.Type == JTokenType.Object)
                {
                    CustomUV custom = jsonObject.ToObject<CustomUV>();
                    return custom;
                }
                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                // Implement if you need custom serialization logic (optional).
            }
        }

        public class Cube
        {
            [JsonProperty(PropertyName = "origin")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 origin { get; set; } = new Vector3(0, 0, 0);
            [JsonProperty(PropertyName = "size")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 size { get; set; } = new Vector3(8, 8, 8);
            [JsonProperty(PropertyName = "uv")]

            [JsonConverter(typeof(UVConverter))]
            public object uv { get; set; }

            [JsonProperty(PropertyName = "inflate")]
            public float inflate { get; set; }

            [JsonProperty(PropertyName = "pivot")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 pivot { get; set; }

            [JsonProperty(PropertyName = "rotation")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 rotation { get; set; }


            [JsonProperty(PropertyName = "mirror")]
            public bool mirror { get; set; }
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

            [JsonProperty(PropertyName = "mirror")]
            public bool mirror { get; set; }

            [JsonProperty(PropertyName = "pivot")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 pivot { get; set; }

            [JsonProperty(PropertyName = "rotation")]
            [JsonConverter(typeof(Vector3Converter))]
            public Vector3 rotation { get; set; }

            public List<Bone> children = new();

            public List<RectangularPrism> rectangles = new();

            public void Rotate()
            {
                Rotate(rotation, pivot);
            }

            public void Rotate(Vector3 rotation, Vector3 pivot)
            {
                if (rotation != (0, 0, 0))
                {
                    // Rotate all boxes
                    foreach (RectangularPrism child in rectangles)
                    {
                        child.Rotate(rotation, pivot);
                    }
                }
            }

            public void RotateChildren(Vector3 rotation, Vector3 pivot)
            {
                if (rotation != (0, 0, 0))
                {
                    // First, apply the parent's rotation to the current bone
                    Rotate(rotation, pivot);

                    // Then, recursively apply the rotation to all child bones
                    foreach (Bone child in children)
                    {
                        // Recursively rotate the child bone and its hierarchy
                        child.RotateChildren(rotation, pivot);
                    }
                }
            }

            public void TranslateVerts(Vector3 offset)
            {
                foreach (RectangularPrism prism in rectangles)
                {
                    prism.OffsetModel(offset);
                }
            }


            private Vector3 TransformRelativePivot(Vector3 parentPivot, Vector3 relativePivot, Vector3 rotation)
            {
                float xRotInRadians = MathHelper.DegreesToRadians(rotation.X);
                float yRotInRadians = -MathHelper.DegreesToRadians(rotation.Y);
                float zRotInRadians = -MathHelper.DegreesToRadians(rotation.Z);

                Quaternion xRotation = Quaternion.FromAxisAngle(Vector3.UnitX, xRotInRadians);
                Quaternion yRotation = Quaternion.FromAxisAngle(Vector3.UnitY, yRotInRadians);
                Quaternion zRotation = Quaternion.FromAxisAngle(Vector3.UnitZ, zRotInRadians);

                Vector3 translatedPivot = (relativePivot * (1,1,-1)) - (parentPivot * (1, 1, -1));

                // Apply the rotations in the order: x, y, z
                Vector3 rotatedPivot = Vector3.Transform(Vector3.Transform(Vector3.Transform(translatedPivot, xRotation), yRotation), zRotation);

                rotatedPivot += (parentPivot * (1, 1, -1));

                rotatedPivot = rotatedPivot * (1, 1, -1);

                return rotatedPivot;
            }
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
                    else if (property.Name.StartsWith("minecraft:geometry"))
                    {
                        if (property.Value.Type == JTokenType.Array)
                        {
                            matchingGeometryTokens.Add(property.Value[0]);
                        }
                        else
                        {
                            matchingGeometryTokens.Add(property.Value);
                        }


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

        public ModelInfo model { get; set; } = new ModelInfo();

        public ModelData(string modelPath, string texturePath)
        {
            // Parse the JSON data
            var json = File.ReadAllText(modelPath); // Load your JSON file
            var modelData = JsonConvert.DeserializeObject<MinecraftModel>(json);

            // Create a list to store each cube mesh
            foreach (Geometry geometry in modelData.minecraftgeometry)
            {

                Dictionary<string, RectangularPrism> rects = new();

                // Process the cubes and create vertices and indices
                foreach (Bone bone in geometry.bones)
                {
                    if (bone != null && bone.cubes != null)
                    {
                        bool mirrorUV = false;

                        Vector3 pivot = bone.pivot;
                        Vector3 rotation = bone.rotation;
                        string parent = bone.parent;
                        string name = bone.name;

                        foreach (Cube cube in bone.cubes)
                        {
                            List<Vector3> cubePositions;
                            List<Vector2> cubeUVs;
                            List<int> cubeIndices;

                            Vector3 origin = cube.origin;
                            Vector3 size = cube.size;

                            Vector3 cubePivot = cube.pivot;
                            Vector3 cubeRot = cube.rotation;

                            if (bone.mirror || cube.mirror)
                            {
                                mirrorUV = true;
                            }

                            // Check for Vector2 or Custom UV
                            Vector2 uv = new();
                            CustomUV custom = null;
                            if (cube.uv is Vector2)
                            {
                                uv = (Vector2)cube.uv;
                            }
                            else if (cube.uv is CustomUV)
                            {
                                custom = (CustomUV)cube.uv;
                            }
                            
                            float inflate = cube.inflate;

                            int width = 0;
                            int height = 0;

                            using (Image img = Image.FromFile(texturePath))
                            {
                                width = img.Width;
                                height = img.Height;
                            }

                            if (uv == (0, 2))
                            {
                                Debug.WriteLine("TEST");
                            }

                            if (size.X < 0.5f)
                            {
                                Debug.WriteLine("WATCH");
                            }

                            RectangularPrism prism = new RectangularPrism(origin, size, (width, height), uv);

                            if (custom != null)
                            {
                                if (size.X < .05f)
                                {
                                    prism.UVs = RenderTools.CustomPlaneUV(custom, width, height);
                                }
                                else
                                {
                                    prism.UVs = RenderTools.EvaluateCustomUV(custom, width, height);
                                }
                            }

                            if (inflate != 0)
                            {
                                prism.Inflate(inflate);
                            }

                            if (mirrorUV)
                            {
                                prism.UVs = RenderTools.MirrorUV(prism.UVs);
                            }

                            prism.rotation = cubeRot;
                            prism.pivot = cubePivot;

                            bone.rectangles.Add(prism); 

                            if (prism.UVs.Count != prism.Vertices.Count)
                            {
                                Debug.WriteLine("Nope");
                            }
                        }

                        if (parent != null)
                        {
                            Bone Parent = geometry.bones.FirstOrDefault(x => x.name == parent);

                            if (Parent != null)
                            {
                                Parent.children.Add(bone);
                            }
                            else
                            {
                                Debug.WriteLine($"Parent bone not found for {name}");
                            }
                        }
                    }
                }

                RotateModel(geometry);

                //// Position each box first
                //foreach (Bone b in geometry.bones)
                //{
                //    if (b != null && b.rectangles != null)
                //    {
                //        // Rotate the boxes to their own positions
                //        foreach (RectangularPrism prism in b.rectangles)
                //        {
                //            prism.Rotate();
                //        }
                //    }
                //}

                //// position all boxes according to bone rotation 
                //foreach (Bone b in geometry.bones)
                //{
                //    if (b.rotation != (0, 0, 0))
                //    {
                //        Debug.WriteLine($"ROTATE {b.name}");
                //        b.Rotate();
                //    }
                //}

                //// Rotate children & boxes if set (BONE HIERARCHY)
                //foreach (Bone b in geometry.bones)
                //{
                //    foreach (Bone child in b.children)
                //    {
                //        child.Rotate(b.rotation, b.pivot);
                //    }
                //}

                ////// once all rotations are completed
                //foreach (Bone b in geometry.bones)
                //{
                //    foreach (RectangularPrism rect in b.rectangles)
                //    {
                //        model.AddInfo(rect.Vertices, rect.UVs, rect.Indices);
                //    }
                //}


            }
        }


        public void RotateModel(Geometry geometry)
        {

            List<Bone> topmostParent = new List<Bone>();

            // Stage 1: Rotate the boxes to their own positions
            // Also find topmost parents
            foreach (Bone b in geometry.bones)
            {
                if (b != null && b.rectangles != null)
                {
                    // Rotate the boxes to their own positions
                    foreach (RectangularPrism prism in b.rectangles)
                    {
                        prism.Rotate();
                    }
                }
                if (b.parent == null)
                {
                    topmostParent.Add(b);
                }
            }

            // Stage 2: Position bones and child bones
            foreach (Bone b in geometry.bones)
            {
                if (b.rotation != Vector3.Zero)
                {
                    Debug.WriteLine($"ROTATE {b.name}");
                    b.RotateChildren(b.rotation, b.pivot);
                }
            }



            // Stage 3: Rotate children & boxes if set (BONE HIERARCHY)
            //foreach (Bone b in topmostParent)
            // {
            //    foreach (Bone child in b.children)
            //    {
            //        child.RotateChildren(b.rotation, b.pivot);
            //    }
            //}

            // Stage 4: Collect the final positions of all vertices
            foreach (Bone b in geometry.bones)
            {
                foreach (RectangularPrism rect in b.rectangles)
                {
                    // Add vertex information to your model
                    model.AddInfo(rect.Vertices, rect.UVs, rect.Indices);
                }
            }
        }

    }

    

    public class ModelInfo
    {
        public List<Vector3> Vertices { get; set; } = new List<Vector3>();
        public List<Vector2> UVs { get; set; } = new List<Vector2>();
        public List<uint> Indices { get; set; } = new List<uint>();

        public void AddInfo(List<Vector3> vertices, List<Vector2> uvs, List<uint> indicies)
        {
            if (uvs.Count != vertices.Count)
            {
                Debug.WriteLine("NOPE");
            }

            Vertices.AddRange(vertices);
            UVs.AddRange(uvs);
            AddIndices(indicies);
        }

        private void AddIndices(List<uint> indicies)
        {
            uint max = 0;
            if (Indices.Count > 0)
            {
                max = Indices.Max() + 1;
            }

            foreach (uint i in indicies)
            {
                Indices.Add(i + max);
            }
        }
    }
}
