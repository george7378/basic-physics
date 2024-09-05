# This script outputs the body inertia tensor of a Blender mesh object by modelling it as a field of regularly spaced point masses.

# To use in Blender, open the script in the text editor and make sure you've selected a single mesh object.
# Then run the script and look for the result in the console output window.

# The inertia tensor is calculated about the selected object's local origin, so you should ensure this
# is in a sensible place within the geometry. Usually it should be at the object's centre of mass.

# As an intermediate step, the script adds a new test object at the scene's global origin containing the sample points
# used in the calculation. This is useful for doing a visual sanity check to confirm the points line up with the geometry.
# Once the script has run and you've inspected the sample points, this test object should be deleted from the scene.

import bpy
import mathutils

SamplePointSeparation = 0.05 # A smaller value gives a more accurate result, but takes longer to calculate
BoundingBoxScaleFactor = 0.999 # This squeezes the bounding box in a bit when generating sample points to avoid end points always being clipped
ClosestPointAngleTolerance = 0.4 # Set to the lowest value where you don't see false positive sample points outside the mesh geometry
ObjectMass = 1

def AddSamplePointsObjectToScene(samplePoints):
    samplePointsMesh = bpy.data.meshes.new('SamplePointsMesh')
    samplePointsMesh.from_pydata(samplePoints, [], [])
    samplePointsMesh.update()
    
    samplePointsObject = bpy.data.objects.new('SamplePointsObject', samplePointsMesh)
    
    bpy.context.scene.objects.link(samplePointsObject)
    bpy.context.scene.update()

def GetSingleSelectedMeshObject():
    selectedMeshObjects = []
    for o in bpy.context.selected_objects:
        if o.type == 'MESH':
            selectedMeshObjects.append(o)
            
    result = selectedMeshObjects[0] if len(selectedMeshObjects) == 1 else None

    return result
    
def IsPointInsideObject(point, object):
    (success, closestPoint, closestFaceNormal, _) = object.closest_point_on_mesh(point)

    result = True if success and (closestPoint - point).normalized().dot(closestFaceNormal) > ClosestPointAngleTolerance else False

    return result

def CalculateObjectSamplePoints(object):
    xBoundingBoxLimits = [BoundingBoxScaleFactor*min([c[0] for c in object.bound_box]), BoundingBoxScaleFactor*max([c[0] for c in object.bound_box])]
    yBoundingBoxLimits = [BoundingBoxScaleFactor*min([c[1] for c in object.bound_box]), BoundingBoxScaleFactor*max([c[1] for c in object.bound_box])]
    zBoundingBoxLimits = [BoundingBoxScaleFactor*min([c[2] for c in object.bound_box]), BoundingBoxScaleFactor*max([c[2] for c in object.bound_box])]
    
    xSampleLength = xBoundingBoxLimits[1] - xBoundingBoxLimits[0]
    ySampleLength = yBoundingBoxLimits[1] - yBoundingBoxLimits[0]
    zSampleLength = zBoundingBoxLimits[1] - zBoundingBoxLimits[0]
    
    xSampleStartValue = xBoundingBoxLimits[0] + (xSampleLength % SamplePointSeparation)/2
    ySampleStartValue = yBoundingBoxLimits[0] + (ySampleLength % SamplePointSeparation)/2
    zSampleStartValue = zBoundingBoxLimits[0] + (zSampleLength % SamplePointSeparation)/2
    
    result = []
    for x in range(int(xSampleLength/SamplePointSeparation) + 1):
        for y in range(int(ySampleLength/SamplePointSeparation) + 1):
            for z in range(int(zSampleLength/SamplePointSeparation) + 1):
                boundingBoxSamplePoint = mathutils.Vector((xSampleStartValue + x*SamplePointSeparation, ySampleStartValue + y*SamplePointSeparation, zSampleStartValue + z*SamplePointSeparation))
                if IsPointInsideObject(boundingBoxSamplePoint, object):
                    result.append(boundingBoxSamplePoint)
    
    return result

def CalculateInertiaTensor(object):
    objectSamplePoints = CalculateObjectSamplePoints(object)
    
    AddSamplePointsObjectToScene(objectSamplePoints) # For debug purposes, to check for coverage and false positives

    massPerSamplePoint = ObjectMass/len(objectSamplePoints)
 
    i_xx = massPerSamplePoint*sum([p[1]**2 + p[2]**2 for p in objectSamplePoints])
    i_yy = massPerSamplePoint*sum([p[0]**2 + p[2]**2 for p in objectSamplePoints])
    i_zz = massPerSamplePoint*sum([p[0]**2 + p[1]**2 for p in objectSamplePoints])
    i_xy_yx = -massPerSamplePoint*sum([p[0]*p[1] for p in objectSamplePoints])
    i_yz_zy = -massPerSamplePoint*sum([p[1]*p[2] for p in objectSamplePoints])
    i_xz_zx = -massPerSamplePoint*sum([p[0]*p[2] for p in objectSamplePoints])
    
    return [[i_xx, i_xy_yx, i_xz_zx], [i_xy_yx, i_yy, i_yz_zy], [i_xz_zx, i_yz_zy, i_zz]]

singleSelectedMeshObject = GetSingleSelectedMeshObject()
if singleSelectedMeshObject == None:
    print('Please select a single mesh object.')
else:
    inertiaTensor = CalculateInertiaTensor(singleSelectedMeshObject)
    
    # Note that the inertia tensor is symmetric, so rows and columns at the same index are interchangeable
    print()
    for row in inertiaTensor:
        print('[ ' + '{:.6f}'.format(row[0]) + ', ' + '{:.6f}'.format(row[1]) + ', ' + '{:.6f}'.format(row[2]) + ' ]')