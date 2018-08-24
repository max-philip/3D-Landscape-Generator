The diamond-square algorithm was used to generate a heightmap for our terrain. A 1-dimensional array was used to store the vertices, and a mesh was created using these vertices. A mesh collider was created as well, preventing rigidbodies from passing through the generated terrain.

The water effect was achieved by (1) creating a "waterplane" prefab which was then instantiated at a specified height relative to the average height of vertices (slightly below) in the terrain. (2) The wave effect was done by applying a sin wave effect to the "waterplane" in its shader. The transparency of the water was also done in its shader, by making changes to the selected water texture's alpha value.

A simple script was used to enable the flying camera, which was applied to a sphere GameObject that is the parent object of the Main Camera.

The boundaries were achieved using a discrete approach to collision detection, just by checking the flying camera's x and z axis positions in Update() and resetting their values if the camera moves past the boundaries.





MENTION WHERE TEXTURE WAS FROM???


PUT LINKS TO STUFF LOOKED AT, AS WELL AS WORKSHOPS WHERE I PULLED CODE (wAVE SHADER)

transparent shader: https://unity3d.com/learn/tutorials/topics/graphics/making-transparent-shader


fly cam (kinda? didn't really use it in the end): https://forum.unity.com/threads/fly-cam-simple-cam-script.67042/
