using BedrockModelViewer.Objects;
using OpenTK.Mathematics;

namespace ModelViewer.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Vector2 textureOrigin = (0,0);
            Vector3 origin = (0,0,0);
            Vector3 size = (8,8,8);
            Vector2 textureData = (64,64);

            RectangularPrism.UVData data = new RectangularPrism.UVData(textureOrigin, size, textureData);

            Assert.AreEqual(new Vector2((1 / 8f), (7 / 8f)), data.UVOffsets[RectangularPrism.Faces.FRONT]);
        }
    }
}