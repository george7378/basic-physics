# This script outputs the vertices of a Blender mesh object as a list of Vector3's for use in MonoGame.
# It's useful for exporting the bounding vertices of a mesh to add to its rigid body in the physics simulation.

# To use in Blender, open the script in the text editor and make sure you've selected a single mesh object.
# Then run the script and look for the result in the console output window.

# It's recommended to run this on a much lower-poly version of your mesh, since you only need an approximation of the
# shape when doing collision checks. You can simplify an object like this using the decimate modifier.

import bpy

selectedMeshObjects = []
for object in bpy.context.selected_objects:
    if object.type == 'MESH':
        selectedMeshObjects.append(object)
        
if len(selectedMeshObjects) != 1:
    print('Please select a single mesh object.')
else:
    print()
    for vertex in selectedMeshObjects[0].data.vertices:
        print('new Vector3(' + '{:.6f}f'.format(vertex.co.x) + ', ' + '{:.6f}f'.format(vertex.co.y) + ', ' + '{:.6f}f'.format(vertex.co.z) + '),')