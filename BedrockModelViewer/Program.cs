using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace BedrockModelViewer
{
    public static class Program
    {
        private static void HandleArgs(string[] args, out string texture, out string model, out string output)
        {
            texture = null;
            model = null;
            output = null;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-t" || args[i] == "--texture")
                {
                    if (i + 1 < args.Length)
                    {
                        texture = args[i + 1];
                        i++; // Skip the next argument, which is the value
                    }
                    else
                    {
                        Console.WriteLine("Error: Missing value for -t or --texture flag.");
                    }
                }
                else if (args[i] == "-m" || args[i] == "--model")
                {
                    if (i + 1 < args.Length)
                    {
                        model = args[i + 1];
                        i++; // Skip the next argument, which is the value
                    }
                    else
                    {
                        Console.WriteLine("Error: Missing value for -m or --model flag.");
                    }
                }
                else if (args[i] == "-o" || args[i] == "--output")
                {
                    if (i + 1 < args.Length)
                    {
                        output = args[i + 1];
                        i++; // Skip the next argument, which is the value
                    }
                    else
                    {
                        Console.WriteLine("Error: Missing value for -o or --output flag.");
                    }
                }
            }

            if (model == null)
            {
                Console.WriteLine("No Model File Specified");
                Environment.Exit(0);
            }
            else if (!model.EndsWith(".json"))
            {
                Console.WriteLine("Model File must be a .json");
                Environment.Exit(0);
            }

            if (texture == null)
            {
                Console.WriteLine("No Texture File Specified");
                Environment.Exit(0);
            }
            else if (!texture.EndsWith(".jpg") && !texture.EndsWith(".png"))
            {
                Console.WriteLine("Texture File must be a .jpg or .png");
                Environment.Exit(0);
            }
        }

        private static void Main(string[] args)
        {
            string? texture = null;
            string? model = null;
            string? output = null;
            HandleArgs(args, out texture, out model, out output);

            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(500, 500),
                Title = "Bedrock Model Viewer",
                // This is needed to run on macos
                Flags = ContextFlags.ForwardCompatible,
            };

            using (var window = new Window(GameWindowSettings.Default, nativeWindowSettings, texture, model, output))
            {
                window.Run();
            }
        }
    }
}
