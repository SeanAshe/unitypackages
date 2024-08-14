using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cosmos
{
    public class ScreenShotManager : CMonoConcurrentSingleton<ScreenShotManager>
    {
        private Texture2D texture;
        private Camera used_camera;
        public IEnumerator TakeCameraScreenShotTask(RectTransform frame, Camera camera, Action callback)
        {
            used_camera = camera;
            var shotcontent = GetScreenRect(frame);
            texture = new Texture2D((int)shotcontent.width, (int)shotcontent.height);
            yield return DoShot(texture, shotcontent, callback);
        }
        private Rect GetScreenRect(RectTransform frame)
        {
            var arr = new Vector3[4];
            frame.GetWorldCorners(arr);
            var screenSize = RectTransformUtility.WorldToScreenPoint(used_camera, arr[2]) - RectTransformUtility.WorldToScreenPoint(used_camera, arr[0]);
            var cameraPostion = RectTransformUtility.WorldToScreenPoint(used_camera, arr[0]);
            return new Rect(cameraPostion.x, cameraPostion.y, screenSize.x, screenSize.y);
        }
        private IEnumerator DoShot(Texture2D texture, Rect shotSize, Action callback)
        {
            var done = false;
            TakeCameraScreenShot(texture, shotSize, used_camera, () =>
            {
                callback?.Invoke();
                texture.Apply();
                done = true;
            });
            yield return new WaitUntil(() => done);
        }
        /// <summary>
        /// 异步的截图
        /// </summary>
        /// <remarks>
        /// 协程某些时候不能直接使用<see cref="TakeCameraScreenShot(Camera, Camera[])"/>,需要异步等待渲染
        /// </remarks>
        /// <param name="output"></param>
        /// <param name="area"></param>
        /// <param name="camera"></param>
        /// <param name="callback"></param>
        public static void TakeCameraScreenShot(Texture2D output, Rect area, Camera camera, Action callback)
        {
            RenderPipelineManager.endCameraRendering += endCameraRendering;
            void endCameraRendering(ScriptableRenderContext _, Camera reneringCamera)
            {
                if (camera == reneringCamera)
                {
                    RenderPipelineManager.endCameraRendering -= endCameraRendering;
                    try
                    {
                        output.ReadPixels(area, 0, 0);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                    callback?.Invoke();
                }
            }
        }
        /// <summary>
        /// 截图
        /// </summary>
        /// <remarks>
        /// 完整参数配置.注意截图后需要手动<paramref name="output"/>.Apply();
        /// </remarks>
        /// <param name="output"></param>
        /// <param name="renderTexture"></param>
        /// <param name="area"></param>
        /// <param name="targetPosition"></param>
        /// <param name="cameras"></param>
        /// <returns></returns>
        public static bool TakeCameraScreenShot(in Texture2D output, in RenderTexture renderTexture,
            in Rect area, in Vector2Int targetPosition, in IEnumerable<Camera> cameras)
        {
            try
            {
                if (cameras != null)
                {
                    foreach (var camera in cameras)
                    {
                        if (camera == null || !camera.enabled) continue;
                        camera.targetTexture = renderTexture;
                        camera.Render();
                        camera.targetTexture = null;
                    }
                }
                RenderTexture currentRT = RenderTexture.active;
                RenderTexture.active = renderTexture;
                output.ReadPixels(area, targetPosition.x, targetPosition.y);
                RenderTexture.active = currentRT;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// 截图
        /// </summary>
        /// <remarks>
        /// 当前屏幕
        /// </remarks>
        /// <param name="camera"></param>
        /// <param name="otherCameras"></param>
        /// <returns></returns>
        public static Texture2D TakeCameraScreenShot(in Camera camera, in Camera[] otherCameras = null)
        {
            RenderTexture rt;
            rt = new RenderTexture(Screen.width, Screen.height, 16);

            RenderTexture currentRT = RenderTexture.active;
            if (otherCameras != null)
            {
                foreach (var otherCam in otherCameras)
                {
                    if (otherCam == null || !otherCam.enabled) continue;
#if UNITY_ANDROID
                RootCameraManager.InsertCameraToStack(otherCam, 1, true);
#endif
                    otherCam.targetTexture = rt;
                    otherCam.Render();
                    Debug.Log("TakeCameraScreenShot::" + otherCam.name);
                    otherCam.targetTexture = null;
                }
            }
            camera.targetTexture = rt;
            camera.Render();
            RenderTexture.active = rt;
            Texture2D imageOverview = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            imageOverview.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            imageOverview.Apply();

            RenderTexture.active = currentRT;
            camera.targetTexture = null;
            if (otherCameras != null)
            {
                foreach (var otherCam in otherCameras)
                {
                    if (otherCam == null || !otherCam.enabled) continue;
#if UNITY_ANDROID
                RootCameraManager.RemoveCameraFromStack(1);
#endif
                }
            }
            return imageOverview;
        }
    }

}
