﻿using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BedrockModelViewer.Graphics
{
    public class Texture
    {
        public int ID;

        public Texture(string filepath)
        {
            ID = GL.GenTexture();

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, ID);

            // texture parameters
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);


            StbImage.stbi_set_flip_vertically_on_load(1);
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string resourceDirectory = Path.Combine(appDirectory, "Resources");
            string textureFile = Path.Combine(resourceDirectory, filepath);

            ImageResult texture = ImageResult.FromStream(File.OpenRead(textureFile), ColorComponents.RedGreenBlueAlpha);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, texture.Width, texture.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, texture.Data);

            // unbind the texture
            Unbind();
        }
        public void Bind() { GL.BindTexture(TextureTarget.Texture2D, ID); }
        public void Unbind() { GL.BindTexture(TextureTarget.Texture2D, 0); }
        public void Delete() { GL.DeleteTexture(ID); }
    }
}
