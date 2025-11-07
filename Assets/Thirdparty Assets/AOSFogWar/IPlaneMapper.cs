using UnityEngine;

namespace FischlWorks_FogWar {
    public interface IPlaneMapper {
        // Map a Unity world-space point (game) -> module-space point (module expects X,Z ground, Y vertical).
        Vector3 MapWorldToModule(Vector3 world);
        // Map module-space point -> Unity world-space point
        Vector3 MapModuleToWorld(Vector3 module);
        // Convert module-space extents (half extents) into world-space extents
        Vector3 MapModuleExtentsToWorld(Vector3 moduleExtents);
        // Direction in world-space that corresponds to module negative Y (the "down" used by module)
        Vector3 ModuleDownDirectionInWorld();
        // Rotation to apply to the fog plane primitive so it lies on the correct ground plane
        Quaternion PlaneRotation();
    }
}