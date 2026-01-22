using UnityEngine;
#if UNITY_RENDER_PIPELINE_UNIVERSAL
using UnityEngine.Rendering.Universal;
#endif

/// <summary>
/// Simple mobile optimization hook: lower quality and adjust frame rate on mobile platforms.
/// Attach to a bootstrap object in your first scene.
/// </summary>
public class MobileOptimizer : MonoBehaviour
{
    [Header("Quality")]
    [Tooltip("Quality level index to use on mobile (0 = lowest).")]
    public int mobileQualityLevel = 0;

    [Header("Frame Rate")]
    public int mobileTargetFrameRate = 60;
    public int desktopTargetFrameRate = -1; // -1 = default

    [Header("Shadows")]
    public float mobileShadowDistance = 25f;
    public float desktopShadowDistance = 150f;

#if UNITY_RENDER_PIPELINE_UNIVERSAL
    [Header("URP")]
    public UniversalRenderPipelineAsset mobileURPAsset;
    public UniversalRenderPipelineAsset desktopURPAsset;
#endif

    void Awake()
    {
        bool isMobile = Application.isMobilePlatform;

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = isMobile ? mobileTargetFrameRate : desktopTargetFrameRate;

        if (isMobile && mobileQualityLevel >= 0 && mobileQualityLevel < QualitySettings.names.Length)
        {
            QualitySettings.SetQualityLevel(mobileQualityLevel, applyExpensiveChanges: true);
        }

        QualitySettings.shadowDistance = isMobile ? mobileShadowDistance : desktopShadowDistance;

#if UNITY_RENDER_PIPELINE_UNIVERSAL
        if (isMobile && mobileURPAsset != null)
        {
            GraphicsSettings.renderPipelineAsset = mobileURPAsset;
            QualitySettings.renderPipeline = mobileURPAsset;
        }
        else if (!isMobile && desktopURPAsset != null)
        {
            GraphicsSettings.renderPipelineAsset = desktopURPAsset;
            QualitySettings.renderPipeline = desktopURPAsset;
        }
#endif
    }
}
