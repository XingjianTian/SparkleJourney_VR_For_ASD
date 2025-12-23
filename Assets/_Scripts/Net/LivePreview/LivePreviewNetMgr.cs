using System;
using System.Collections;
using System.Collections.Generic;
using Fantasy;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class LivePreviewNetMgr : MonoSingleton<LivePreviewNetMgr>
{
    private static bool isInited;
    private Coroutine previewCoroutine;
    private bool isPreviewing = false;
    public float previewInterval = 0.1f; // 10帧每秒

    [Header("预览画面设置")]
    [Tooltip("预览画面的宽度")]
    public int previewWidth = 1920;
    [Tooltip("预览画面的高度")]
    public int previewHeight = 1080;

    [Header("压缩设置")]
    [Range(1, 100)]
    [Tooltip("JPEG压缩质量(1-100)")]
    public int jpegQuality = 60;
    [Tooltip("是否启用极限压缩")]
    public bool enableUltraCompression = true;
    [Tooltip("压缩块大小(KB)")]
    [Range(32, 1024)]
    public int compressionBlockSize = 256;

    private async void Start()
    {
        if (!isInited)
        {
            print($"分辨率:{previewWidth} X {previewHeight}");
            print($"频率:{1f / previewInterval}");
            isInited = true;
            DontDestroyOnLoad(gameObject);
            //向服务器注册LivePreview用户
            ClientRegistryLivePreviewMessage msg = new ClientRegistryLivePreviewMessage();
            msg.socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();
            FantasyNetMgr.Instance.SendMessage(msg);
            FantasyNetMgr.Instance.Send(new CreateLivePreviewRouteMessage());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void Update()
    {
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            ClientRemoveLivePreviewMessage msg = new ClientRemoveLivePreviewMessage();
            msg.socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();
            FantasyNetMgr.Instance.Send(msg);
            Debug.Log($"主动断开连接");

            RemoveClientRequest removeClientMessage = new RemoveClientRequest();
            removeClientMessage.socketClinetUUID = await FantasyNetMgr.Instance.GetSocketClinetUUID();

            // 使用TaskCompletionSource来等待响应
            var tcs = new TaskCompletionSource<bool>();
            FantasyNetMgr.Instance.Call(removeClientMessage, (response =>
            {
                var res = response as RemoveClientResponse;
                Debug.Log($"删除用户是否成功: {res.is_success}");
                tcs.SetResult(true);
            }));

            // 等待响应完成，设置超时时间
            await Task.WhenAny(tcs.Task, Task.Delay(2000));
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

        }
    }


    public void StartLivePreview()
    {
        if (!isPreviewing)
        {
            Debug.LogError("开启推流");
            isPreviewing = true;
            previewCoroutine = StartCoroutine(PreviewCoroutine());
            // previewCoroutine = StartCoroutine(PreviewCoroutine());
        }
    }

    public void StopLivePreview()
    {
        if (isPreviewing)
        {
            isPreviewing = false;
            if (previewCoroutine != null)
            {
                Debug.LogError("关闭推流");
                StopCoroutine(previewCoroutine);
                previewCoroutine = null;
            }
        }
    }

    private byte[] CompressData(byte[] data)
    {
        if (!enableUltraCompression)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream deflate = new DeflateStream(output, CompressionMode.Compress))
                {
                    deflate.Write(data, 0, data.Length);
                }
                return output.ToArray();
            }
        }

        // 极限压缩模式
        try
        {
            // 1. 预处理图像数据
            byte[] processedData = PreprocessImageData(data);

            // 2. 分块压缩
            int blockSize = compressionBlockSize * 1024; // 转换为字节
            int blockCount = (int)Math.Ceiling(processedData.Length / (double)blockSize);

            using (MemoryStream output = new MemoryStream())
            {
                // 写入块数量
                output.Write(BitConverter.GetBytes(blockCount), 0, 4);

                for (int i = 0; i < blockCount; i++)
                {
                    int currentBlockSize = Math.Min(blockSize, processedData.Length - i * blockSize);
                    byte[] block = new byte[currentBlockSize];
                    Array.Copy(processedData, i * blockSize, block, 0, currentBlockSize);

                    // 使用LZ4压缩
                    byte[] compressedBlock = LZ4Compress(block);

                    // 写入块大小和压缩后的数据
                    output.Write(BitConverter.GetBytes(compressedBlock.Length), 0, 4);
                    output.Write(compressedBlock, 0, compressedBlock.Length);
                }

                return output.ToArray();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"压缩失败: {e.Message}");
            return data; // 如果压缩失败，返回原始数据
        }
    }

    private byte[] DecompressData(byte[] compressedData)
    {
        if (!enableUltraCompression)
        {
            using (MemoryStream input = new MemoryStream(compressedData))
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream deflate = new DeflateStream(input, CompressionMode.Decompress))
                {
                    deflate.CopyTo(output);
                }
                return output.ToArray();
            }
        }

        try
        {
            using (MemoryStream input = new MemoryStream(compressedData))
            {
                // 读取块数量
                byte[] blockCountBytes = new byte[4];
                input.Read(blockCountBytes, 0, 4);
                int blockCount = BitConverter.ToInt32(blockCountBytes, 0);

                using (MemoryStream output = new MemoryStream())
                {
                    for (int i = 0; i < blockCount; i++)
                    {
                        // 读取块大小
                        byte[] blockSizeBytes = new byte[4];
                        input.Read(blockSizeBytes, 0, 4);
                        int blockSize = BitConverter.ToInt32(blockSizeBytes, 0);

                        // 读取压缩的块数据
                        byte[] compressedBlock = new byte[blockSize];
                        input.Read(compressedBlock, 0, blockSize);

                        // 解压块
                        byte[] decompressedBlock = LZ4Decompress(compressedBlock);
                        output.Write(decompressedBlock, 0, decompressedBlock.Length);
                    }

                    // 后处理图像数据
                    return PostprocessImageData(output.ToArray());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"解压失败: {e.Message}");
            return compressedData; // 如果解压失败，返回原始数据
        }
    }

    private byte[] PreprocessImageData(byte[] imageData)
    {
        // 1. 降低色深
        // 2. 去除不必要的元数据
        // 3. 优化数据排列
        return imageData;
    }

    private byte[] PostprocessImageData(byte[] processedData)
    {
        // 恢复图像数据
        return processedData;
    }

    private byte[] LZ4Compress(byte[] data)
    {
        // 使用LZ4压缩
        using (MemoryStream output = new MemoryStream())
        {
            using (DeflateStream deflate = new DeflateStream(output, CompressionMode.Compress))
            {
                deflate.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }

    private byte[] LZ4Decompress(byte[] compressedData)
    {
        using (MemoryStream input = new MemoryStream(compressedData))
        using (MemoryStream output = new MemoryStream())
        {
            using (DeflateStream deflate = new DeflateStream(input, CompressionMode.Decompress))
            {
                deflate.CopyTo(output);
            }
            return output.ToArray();
        }
    }

    public static Texture2D ScaleTexture(Texture2D source, float targetWidth, float targetHeight)
    {
        Texture2D result = new Texture2D((int)targetWidth, (int)targetHeight, source.format, false);

        float incX = (1.0f / targetWidth);
        float incY = (1.0f / targetHeight);

        for (int i = 0; i < result.height; ++i)
        {
            for (int j = 0; j < result.width; ++j)
            {
                Color newColor =
                    source.GetPixelBilinear((float)j / (float)result.width, (float)i / (float)result.height);
                result.SetPixel(j, i, newColor);
            }
        }

        result.Apply();
        return result;
    }

    // private Texture2D screenshot;
    // private IEnumerator PreviewCoroutine()
    // {
    //     while (isPreviewing)
    //     {
    //         yield return new WaitForSeconds(previewInterval);
    //         yield return new WaitForEndOfFrame();
    //         screenshot = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.LeftEye);
    //         screenshot = ScaleTexture(screenshot, previewWidth, previewHeight);
    //
    //
    //         // 将截图转换为字节数组并压缩
    //         // byte[] imageBytes = resizedTexture.EncodeToJPG(jpegQuality);
    //         // byte[] compressedData = CompressData(imageBytes);
    //         byte[] imageBytes = screenshot.EncodeToJPG(jpegQuality);
    //         byte[] compressedData = CompressData(imageBytes);
    //         // 发送游戏预览消息
    //         SubmitGamePreviewMessage msg = new SubmitGamePreviewMessage();
    //         msg.preview_image_data = compressedData;
    //         FantasyNetMgr.Instance.SendMessage(msg);
    //     }
    //     print("结束推流");
    // }
    // public Camera captureCamera;
    // private RenderTexture captureCameraRenderTexture; // 需要设置一个覆盖全场景的相机
    //
    // private IEnumerator PreviewCoroutine()
    // {
    //     // var renderTexture = new RenderTexture(previewWidth, previewHeight, 24);
    //     captureCameraRenderTexture = captureCamera.targetTexture;
    //     var resultTexture = new Texture2D(previewWidth, previewHeight, TextureFormat.RGB24, false);
    //     var waitInterval = new WaitForSeconds(previewInterval);
    //     while (isPreviewing)
    //     {
    //         yield return waitInterval;
    //
    //         // // 方法2：使用相机渲染
    //         // captureCamera.targetTexture = renderTexture;
    //         // captureCamera.Render();
    //
    //         RenderTexture.active = captureCameraRenderTexture;
    //         resultTexture.ReadPixels(new Rect(0, 0, previewWidth, previewHeight), 0, 0);
    //         resultTexture.Apply();
    //         RenderTexture.active = null;
    //         // captureCamera.targetTexture = null;
    //
    //         // 编码和发送
    //         byte[] imageBytes = resultTexture.EncodeToJPG(jpegQuality);
    //         var msg = new SubmitGamePreviewMessage {
    //             preview_image_data = CompressData(imageBytes)
    //         };
    //         FantasyNetMgr.Instance.SendMessage(msg);
    //     }
    //
    //     // Destroy(renderTexture);
    //     Destroy(resultTexture);
    // }
    public Camera captureCamera; // 需要设置一个覆盖全场景的相机
    private RenderTexture preview;

    private IEnumerator PreviewCoroutine()
    {
        preview = captureCamera.targetTexture;
        var renderTexture = new RenderTexture(previewWidth, previewHeight, 24);
        var resultTexture = new Texture2D(previewWidth, previewHeight, TextureFormat.RGB24, false);
        var waitInterval = new WaitForSeconds(previewInterval);

        while (isPreviewing)
        {
            yield return waitInterval;

            // 方法2：使用相机渲染
            captureCamera.targetTexture = renderTexture;
            captureCamera.Render();

            RenderTexture.active = renderTexture;
            resultTexture.ReadPixels(new Rect(0, 0, previewWidth, previewHeight), 0, 0);
            resultTexture.Apply();
            RenderTexture.active = null;
            captureCamera.targetTexture = preview;

            // 编码和发送
            byte[] imageBytes = resultTexture.EncodeToJPG(jpegQuality);
            // byte[] imageBytes = resultTexture.GetRawTextureData();
            // Debug.Log($"压缩前图片大小：{imageBytes.Length}");

            //
            // byte[] imageBytes = resultTexture.FMEncodeToJPG(jpegQuality);
            // imageBytes = imageBytes.FMZipBytes();
            //
            // Debug.Log($"压缩后图片大小：{CompressData(imageBytes).Length}");
            var msg = new SubmitGamePreviewMessage {
                preview_image_data = CompressData(imageBytes)
            };
            FantasyNetMgr.Instance.SendMessage(msg);
        }

        Destroy(renderTexture);
        Destroy(resultTexture);
    }
}
