using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.PostProcessing.Utilities;
using DG.Tweening;

[RequireComponent(typeof(PostProcessingController))]
public class CameraController : Singleton<CameraController> {

    public Camera cam;
    public PostProcessingProfile bulletTimeEffectProfile;
    public PostProcessingProfile standardCameraEffectsProfile;
    [Range(0, 60)]
    public float zoomFOV = 35f;
    public Vector3 movePointOffset = Vector3.zero;
    public Vector3 lookPointOffset = Vector3.zero;
    public Ease easing = Ease.OutExpo;
    [Range(0f, 1f)]
    public float defaultTimescale = 0.1f;
    public float defaultDuration = 0.8f;

    private float StandardFOV;
    private Vector3 StandardCamPos;
    private Vector3 standardCamRot;
    public bool isZoomed { get; private set; }
    private Transform _lookTarget;
    private PostProcessingController _postProcessingController;
    private Sequence zoomInSequence;
    private Sequence camEffectsSequence;
    private Sequence zoomOutSequence;

    void Start() {
        
        DOTween.Init(false, false, LogBehaviour.ErrorsOnly);
        DOTween.defaultTimeScaleIndependent = true;
        DOTween.useSmoothDeltaTime = true;

        if (cam == null)
            cam = Camera.main;

        StandardFOV = cam.fieldOfView;
        StandardCamPos = cam.transform.position;
        standardCamRot = cam.transform.eulerAngles;

        _postProcessingController = GetComponent<PostProcessingController>();
    }

    void Update() {
        if (isZoomed) {
            transform.LookAt(_lookTarget.position + lookPointOffset);
        }
    }

    public void ZoomInWithSlowMotion(Transform moveTarget, Transform lookTarget, float duration, float timeScale) {
        
        zoomOutSequence.Kill();
        zoomInSequence = DOTween.Sequence();

        isZoomed = true;
        _lookTarget = lookTarget;
        zoomInSequence.Join(DOTween.To(()=> Time.timeScale, x => Time.timeScale = x, timeScale, duration).SetEase(easing));
        zoomInSequence.Join(cam.transform.DOMove(new Vector3(moveTarget.position.x, moveTarget.position.y, moveTarget.position.z) + movePointOffset, duration * 5f).SetEase(Ease.OutQuart));
        zoomInSequence.Join(cam.DOFieldOfView(zoomFOV, duration).SetEase(easing));

        //PostProcessingProfileTransition(bulletTimeEffectProfile, _postProcessingController, defaultDuration);

        zoomInSequence.PlayForward();
    }

    //private void PostProcessingProfileTransition(PostProcessingProfile to, PostProcessingController controller, float duration) {

    //    camEffectsSequence = DOTween.Sequence();

    //    if (controller.controlDepthOfField) {
    //        controller.enableDepthOfField = to.depthOfField.enabled ? true : false;
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.depthOfField.focusDistance, x => controller.depthOfField.focusDistance = x, to.depthOfField.settings.focusDistance, duration).SetEase(easing));
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.depthOfField.aperture, x => controller.depthOfField.aperture = x, to.depthOfField.settings.aperture, duration).SetEase(easing));
    //        controller.depthOfField.useCameraFov = to.depthOfField.settings.useCameraFov ? true : false;
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.depthOfField.focalLength, x => controller.depthOfField.focalLength = x, to.depthOfField.settings.aperture, duration).SetEase(easing));
    //    }

    //    if (controller.controlBloom) {
    //        controller.enableBloom = to.bloom.enabled ? true : false;
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.bloom.bloom.intensity, x => controller.bloom.bloom.intensity = x, to.bloom.settings.bloom.intensity, duration).SetEase(easing));
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.bloom.bloom.threshold, x => controller.bloom.bloom.threshold = x, to.bloom.settings.bloom.threshold, duration).SetEase(easing));
    //    }

    //    if (controller.controlColorGrading) {
    //        controller.enableColorGrading = to.colorGrading.enabled ? true : false;
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.colorGrading.basic.postExposure, x => controller.colorGrading.basic.postExposure = x, to.colorGrading.settings.basic.postExposure, duration).SetEase(easing));
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.colorGrading.basic.saturation, x => controller.colorGrading.basic.saturation = x, to.colorGrading.settings.basic.saturation, duration).SetEase(easing));
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.colorGrading.basic.contrast, x => controller.colorGrading.basic.contrast = x, to.colorGrading.settings.basic.contrast, duration).SetEase(easing));
    //        camEffectsSequence.Insert(0, DOTween.To(() => controller.colorGrading.basic.temperature, x => controller.colorGrading.basic.temperature = x, to.colorGrading.settings.basic.temperature, duration).SetEase(easing));
    //    }

    //    camEffectsSequence.PlayForward();

    //}

    public void ZoomInWithSlowMotion(Transform moveTarget, Transform lookTarget, float duration) {
        ZoomInWithSlowMotion(moveTarget, lookTarget, duration, defaultTimescale);
    }

    public void ZoomInWithSlowMotion(Transform moveTarget, Transform lookTarget) {
        ZoomInWithSlowMotion(moveTarget, lookTarget, defaultDuration);
    }

    public void ZoomIn(Transform moveTarget, Transform lookTarget, float duration) {
        ZoomInWithSlowMotion(moveTarget, lookTarget);
    }

    public void ZoomIn(Transform moveTarget, Transform lookTarget) {
        ZoomIn(moveTarget, lookTarget, defaultDuration);
    }

    public void ZoomOut(float duration) {
        
        zoomInSequence.Kill();
        zoomOutSequence = DOTween.Sequence();

        //PostProcessingProfileTransition(standardCameraEffectsProfile, _postProcessingController, defaultDuration);

        if (!isZoomed)
            return;

        isZoomed = false;
        zoomOutSequence.Insert(0, DOTween.To(() => Time.timeScale, x => Time.timeScale = x, 1f, duration).SetEase(easing));
        zoomOutSequence.Insert(0, cam.transform.DOMove(StandardCamPos, duration).SetEase(easing));
        zoomOutSequence.Insert(0, cam.transform.DORotate(standardCamRot, duration).SetEase(easing));
        zoomOutSequence.Insert(0, cam.DOFieldOfView(StandardFOV, duration).SetEase(easing));

        zoomOutSequence.PlayForward();

    }

    public void ZoomOut() {
        ZoomOut(defaultDuration);
    }

    public void CancelBulletTime() {
        ZoomOut(0f);
    }

}
