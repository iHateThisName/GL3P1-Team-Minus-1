using UnityEngine;

namespace FischlWorks_FogWar {
    // Identity mapper: module expects XZ ground and Y vertical -> matches Unity default 3D
    public class PlaneMapper_XZ : MonoBehaviour, IPlaneMapper {
        public Vector3 MapWorldToModule(Vector3 world) => world;
        public Vector3 MapModuleToWorld(Vector3 module) => module;
        public Vector3 MapModuleExtentsToWorld(Vector3 moduleExtents) => moduleExtents;
        public Vector3 ModuleDownDirectionInWorld() => Vector3.down;
        public Quaternion PlaneRotation() => Quaternion.identity;
    }
}