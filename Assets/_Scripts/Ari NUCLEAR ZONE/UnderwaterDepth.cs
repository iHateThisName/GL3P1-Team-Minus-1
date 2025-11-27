using UnityEngine;
using UnityEngine.Rendering;
public class UnderwaterDepth : MonoBehaviour
{
    [Header("Depth Parameters")]
    [SerializeField] private Transform mainCamera;
    [SerializeField] private int UpperWater = 0;
    [SerializeField] private int MiddleWater = -100;
    [SerializeField] private int VoidWater = -200;

    [Header("Post Processing Volume")]
    [SerializeField] private Volume postProcessingVolume;

    [Header("Post Processing Profiles")]
    [SerializeField] private VolumeProfile surfacePostProcessing;
    [SerializeField] private VolumeProfile UpperWaterPostProcessing;
    [SerializeField] private VolumeProfile MiddleWaterPostProcessing;
    [SerializeField] private VolumeProfile VoidWaterPostProcessing;

    private EnumVolumProfile currentVolumeProfile;

    private void Start()
    {
        currentVolumeProfile = EnumVolumProfile.Surface;
        RenderSettings.fog = false;
        postProcessingVolume.profile = surfacePostProcessing;
    }

    private void LateUpdate()
    {
        UpdateWaterVolume();
    }

    private void UpdateWaterVolume()
    {
        if (mainCamera.position.y < UpperWater)
        {
            if (mainCamera.position.y < MiddleWater)
            {

                if (mainCamera.position.y < VoidWater)
                {
                    EnableEffects(EnumVolumProfile.VoidWater);
                }
                else
                {
                    EnableEffects(EnumVolumProfile.MiddleWater);
                }

            }
            else
            {
                EnableEffects(EnumVolumProfile.UpperWater);
            }

        }
        else
        {
            EnableEffects(EnumVolumProfile.Surface);
        }
    }

    private void EnableEffects(EnumVolumProfile newProfile)
    {

        if (newProfile == currentVolumeProfile) return;

        currentVolumeProfile = newProfile;

        if (newProfile == EnumVolumProfile.Surface)
        {
            RenderSettings.fog = false;
        }
        else
        {
            RenderSettings.fog = true;
        }

        switch (newProfile)
        {
            case EnumVolumProfile.Surface:
                postProcessingVolume.profile = surfacePostProcessing;
                return;

            case EnumVolumProfile.UpperWater:
                postProcessingVolume.profile = UpperWaterPostProcessing;
                return;

            case EnumVolumProfile.MiddleWater:
                postProcessingVolume.profile = MiddleWaterPostProcessing;
                return;

            case EnumVolumProfile.VoidWater:
                postProcessingVolume.profile = VoidWaterPostProcessing;
                return;
        }

    }

    private enum EnumVolumProfile
    {
        Surface, UpperWater, MiddleWater, VoidWater
    }
}






