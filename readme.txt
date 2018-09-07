Implementation Description:

The diamond-square algorithm was used to generate a heightmap for our terrain. A 1-dimensional array was used to store the vertices, and a mesh was created using these vertices. A mesh collider was created as well, preventing rigidbodies from passing through the generated terrain.

The water effect was achieved by (1) creating a "waterplane" prefab which was then instantiated at a specified height relative to the average height of vertices (slightly below) in the terrain. (2) The wave effect was done by applying a sin wave effect to the "waterplane" in its shader. The transparency of the water was also done in its shader, by making changes to the selected water texture's alpha value.

A simple script was used to enable the flying camera, which was applied to a sphere GameObject that is the parent object of the Main Camera. The boundaries were achieved using a discrete approach to collision detection, just by checking the flying camera's x and z axis positions in Update() and resetting their values if the camera moves past the boundaries.

A Phong Shading model was used for the shader applied to the generated terrain. The parameters were set to values that produced a aesthetically pleasing landscape (predominantly a low specularity). Vertex colours were assigned by vertex height value, in order to give the appearance of different surface types (sand, grass and snow). This was done in the TerrainGenerator script.

A sun model was made for added realism, as per the specification. It is set to orbit at the exact same rate as the directional light used for the scence, keeping a consistent lighting.

Additionally, some UI elements were added just for a better user experience without negatively impacting the performance. The user is able to adjust the sun orbit speed and the player FOV/sensitivity with sliders. The stars were just added as an afterthought to improve the overall look of the project. The asset was downloaded free from the Asset Store.



Resources:
COMP30019 Lab 4: Used as the basis for the wave shader.
COMP30019 Lab 5: Used as the basis for the Phong shader.

Transparent shader: https://unity3d.com/learn/tutorials/topics/graphics/making-transparent-shader
Official Unity tutorial used as a reference

Daimond Square Video: https://www.youtube.com/watch?v=1HV8GbFnCik&pbjreload=10
Used as reference to understand how the algorithm works
