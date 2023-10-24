using BedrockModelViewer.Graphics;
using BedrockModelViewer.Objects;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BedrockModelViewer
{
    internal class Camera
    {
        // CONSTANTS
        private float SPEED = 80f;
        private float SCREENWIDTH;
        private float SCREENHEIGHT;
        private float SENSITIVITY = 180f;

        // position vars
        public Vector3 position;
        private Vector3 lookAt;

        Matrix4 tranlationMatrix = Matrix4.Zero;

        Vector3 up = Vector3.UnitY;
        Vector3 front = -Vector3.UnitZ;
        Vector3 right = Vector3.UnitX;

        // --- view rotations ---
        private float pitch = 0f;
        private float yaw = 90f;

        private bool firstMove = true;
        private bool fixedPoint = false;

        public Vector2 lastPos;

        private float theta = 0.0f;
        private float phi = 0.0f;
        
        public void Resized(int width, int height)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
        }

        public void Rotate(Vector3 centerOfRotation, float rotationAmount, float distanceFromObject)
        {
            float radianRot = MathHelper.DegreesToRadians(rotationAmount);

            theta += radianRot;

            theta = (theta + MathHelper.TwoPi) % MathHelper.TwoPi;

            float x = (float)(distanceFromObject * Math.Sin(theta) * Math.Cos(phi));
            float y = (float)(distanceFromObject * Math.Sin(theta) * Math.Sin(phi));
            float z = (float)(distanceFromObject * Math.Cos(theta));

            position = new Vector3(x, y, z) + centerOfRotation;

            UpdateVectors();
        }

        public void LookAt(Vector3 point)
        {
            front = Vector3.Normalize(point - position);
            pitch = MathHelper.RadiansToDegrees(MathF.Asin(front.Y));
            yaw = MathHelper.RadiansToDegrees(MathF.Atan2(front.Z, front.X));
        }

        public void RotateAroundObject(RenderableObject obj, float rotation = 0)
        {
            rotation = (rotation % 360);

            // Gets the current position of the object (avgX, minY, avgZ)
            Vector3 center = RenderTools.GetCenter(obj.Vertices);

            List<Vector3> verts = obj.CenteredVertices;
            
            Vector3 min = RenderTools.GetMin(verts);
            Vector3 max = RenderTools.GetMax(verts);

            // Calculate the viewing distance (adjust as needed)
            float viewingDistance = (max - min).Length * 1f;

            Rotate(center, rotation, viewingDistance);
            LookAt(center);


            //position = new Vector3(0, (min-max).Y, viewingDistance);

            //float rot = MathHelper.DegreesToRadians(rotation);
            //tranlationMatrix = Matrix4.CreateRotationX(rot);

            //position = offset + position;

            UpdateVectors(); // Ensure the camera direction is updated based on the new yaw and pitch values

        }

        //public void RotateAroundObject(Vector3 target, float degrees)
        //{
        //    // Calculate the current direction from the camera position to the target
        //    Vector3 currentDirection = Vector3.Normalize(target - position);

        //    // Calculate the new yaw angle to rotate the camera
        //    float currentYaw = MathF.Atan2(-currentDirection.Z, currentDirection.X);
        //    float newYaw = currentYaw + MathHelper.DegreesToRadians(degrees);

        //    // Calculate the new camera position after the rotation
        //    float distanceToTarget = (position - target).Length;
        //    float newX = target.X + distanceToTarget * MathF.Cos(newYaw);
        //    float newZ = target.Z - distanceToTarget * MathF.Sin(newYaw);

        //    position.X = newX;
        //    position.Z = newZ;

        //    Vector3 viewDirection = Vector3.Normalize(target - position);

        //    yaw = MathHelper.RadiansToDegrees(MathF.Atan2(viewDirection.Z, viewDirection.X));

        //    if (yaw > 90)
        //    {
        //        yaw -= -10f;
        //    }
        //    else
        //    {
        //        yaw += 10f;
        //    }
        //    pitch = MathHelper.RadiansToDegrees(MathF.Asin(viewDirection.Y));

        //    // Update vectors and the look-at point
        //    UpdateVectors();
        //}

        public Camera(float width, float height, Vector3 position)
        {
            SCREENWIDTH = width;
            SCREENHEIGHT = height;
            this.position = position;
            this.lookAt = position + front;
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(position, lookAt, up);
        }
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60.0f), SCREENWIDTH / SCREENHEIGHT, 0.1f, 1000.0f);
        }

        private void UpdateVectors()
        {
            if (pitch > 89.0f)
            {
                pitch = 89.0f;
            }
            if (pitch < -89.0f)
            {
                pitch = -89.0f;
            }

            if (yaw > 360)
            {
                yaw -= 360;
            }

            if (yaw < 0)
            {
                yaw += 360;
            }

            //Debug.WriteLine($"Pos: {position} | Yaw: {yaw} | Pitch: {pitch}");

            front.X = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Cos(MathHelper.DegreesToRadians(yaw));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(pitch));
            front.Z = MathF.Cos(MathHelper.DegreesToRadians(pitch)) * MathF.Sin(MathHelper.DegreesToRadians(yaw));

            front = Vector3.Normalize(front);

            right = Vector3.Normalize(Vector3.Cross(front, Vector3.UnitY));
            up = Vector3.Normalize(Vector3.Cross(right, front));

            lookAt = position + front;
        }


        public void InputController(KeyboardState input, MouseState mouse, bool focused, FrameEventArgs e)
        {

            if (input.IsKeyDown(Keys.W))
            {
                position += front * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                position -= right * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                position -= front * SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                position += right * SPEED * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.Space))
            {
                position.Y += SPEED * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                position.Y -= SPEED * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.Left))
            {
                yaw += -.5f * SENSITIVITY * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Right))
            {
                yaw += .5f * SENSITIVITY * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Up))
            {
                pitch -= -.5f * SENSITIVITY * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.Down))
            {
                pitch -= .5f * SENSITIVITY * (float)e.Time;
            }

            if (focused)
            {
                if (firstMove)
                {
                    lastPos = new Vector2(mouse.X, mouse.Y);
                    firstMove = false;
                }
                else
                {
                    var deltaX = mouse.X - lastPos.X;
                    var deltaY = mouse.Y - lastPos.Y;
                    lastPos = new Vector2(mouse.X, mouse.Y);

                    yaw += deltaX * SENSITIVITY * (float)e.Time;
                    pitch -= deltaY * SENSITIVITY * (float)e.Time;
                }
            }
            UpdateVectors();
        }
        public void Update(KeyboardState input, MouseState mouse, bool focused, FrameEventArgs e)
        {
            InputController(input, mouse, focused, e);
        }

        public Matrix4 GetTranslationMatrix()
        {
            return tranlationMatrix;
        }
    }
}
