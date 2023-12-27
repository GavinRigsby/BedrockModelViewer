# Bedrock Model Viewer

This model viewer was based off of the BlockBench Editor. 

I needed a way to given the model and texture files create a thumbnail image of the model.

This is not perfect by and means and does not allow editing of the models. 

Works with Minecraft Bedrock model .json files

Usage: BedRockModelViewer -t texture.png -m model.json -o thumb.png

-t or --texture defines the texture UV for the model
-m or --model defines the json model
-o or --output defines the output file 

Using Output will save the image to a file but not open live view. (Used for scripting mass model thumbnail generation)

Viewer Controls:

Use WASD for movement (forward, left, back, right)
Space to fly up
Shift to fly down
Arrow keys or mouse movement for rotating camera (Left Alt to activate mouse look).

Output Image:

Saves image to that specified in console arguments. 

Saved image has background removed.


Issues:

Note: These issues are occasional and not in all cases

Z index where overlapping layers will show as transparent or black.
Rotation rendering, on occasion heirarchial rotations will display incorrectly
