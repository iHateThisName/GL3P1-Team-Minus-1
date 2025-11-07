using UnityEngine;

namespace FischlWorks_FogWar {
    // Maps a game using XY ground to the module's expected XZ ground.
    // world (x, y, z) -> module (x, z, y)
    // module -> world is the inverse.
    public class PlaneMapper_XY : MonoBehaviour, IPlaneMapper {
        public Vector3 MapWorldToModule(Vector3 world) {
            // module.x = world.x
            // module.y (vertical) = world.z
            // module.z = world.y (second ground axis)
            return new Vector3(world.x, world.z, world.y);
        }
        public Vector3 MapModuleToWorld(Vector3 module) {
            // inverse mapping
            return new Vector3(module.x, module.z, module.y);
        }
        public Vector3 MapModuleExtentsToWorld(Vector3 moduleExtents) {
            // Swap extents Y <-> Z
            return new Vector3(moduleExtents.x, moduleExtents.z, moduleExtents.y);
        }
        public Vector3 ModuleDownDirectionInWorld() {
            // Module negative Y corresponds to decreasing world.z
            return Vector3.back;
        }
        public Quaternion PlaneRotation() {
            // The default Unity Plane primitive lies in XZ; rotate so it faces XY
            return Quaternion.Euler(-90f, 0f, 0f);
        }
    }
}