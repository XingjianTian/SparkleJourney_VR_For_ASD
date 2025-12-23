using System;
using UnityEngine;
using System.IO;

public class WavUtility
{
    public static AudioClip ToAudioClip(byte[] wavData)
    {
        // WAV文件格式的头部信息
        int channels = wavData[22];     // 声道数
        int sampleRate = wavData[24] | (wavData[25] << 8); // 采样率
        int bitsPerSample = wavData[34]; // 每个样本的位数

        // 找到数据块的起始位置
        int dataStartIndex = 44; // 通常WAV文件的数据块从第44字节开始
        int dataSize = wavData.Length - dataStartIndex;

        // 将字节数据转换为浮点数数组
        float[] audioData = new float[dataSize / 2]; // 16位样本，每个样本2字节
        for (int i = 0; i < audioData.Length; i++)
        {
            short sample = (short)(wavData[dataStartIndex + i * 2] | (wavData[dataStartIndex + i * 2 + 1] << 8));
            audioData[i] = sample / 32768.0f; // 将16位样本转换为-1到1之间的浮点数
        }

        // 创建AudioClip
        AudioClip audioClip = AudioClip.Create("LoadedAudioClip", audioData.Length, channels, sampleRate, false);
        audioClip.SetData(audioData, 0);

        return audioClip;
    }

    public static AudioClip LoadWavFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Debug.LogError("File not found: " + filePath);
            return null;
        }

        byte[] wavData = File.ReadAllBytes(filePath);
        return ToAudioClip(wavData);
    }
    public static byte[] ConvertAudioClipToWav(AudioClip clip)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                // 写入WAV文件头
                WriteWavHeader(writer, clip);

                // 获取音频数据并写入
                float[] samples = new float[clip.samples * clip.channels];
                clip.GetData(samples, 0);

                // 将float转换为16位PCM数据
                Int16[] intData = new Int16[samples.Length];
                for (int i = 0; i < samples.Length; i++)
                {
                    intData[i] = (short)(samples[i] * 32767);
                }

                byte[] bytesData = new byte[intData.Length * 2];
                Buffer.BlockCopy(intData, 0, bytesData, 0, bytesData.Length);

                writer.Write(bytesData);

                // 更新文件头中的文件大小信息
                writer.Seek(4, SeekOrigin.Begin);
                writer.Write((int)(memoryStream.Length - 8));

                return memoryStream.ToArray();
            }
        }
    }

    // 写入WAV文件头
    private static void WriteWavHeader(BinaryWriter writer, AudioClip clip)
    {
        int frequency = clip.frequency;
        int channels = clip.channels;
        int samples = clip.samples;

        writer.Write(new char[4] { 'R', 'I', 'F', 'F' });
        writer.Write(0); // 文件长度占位，后面会更新
        writer.Write(new char[4] { 'W', 'A', 'V', 'E' });

        writer.Write(new char[4] { 'f', 'm', 't', ' ' });
        writer.Write(16); // PCM格式块大小
        writer.Write((short)1); // PCM格式标志
        writer.Write((short)channels);
        writer.Write(frequency);
        writer.Write(frequency * channels * 2); // 字节率
        writer.Write((short)(channels * 2)); // 块对齐
        writer.Write((short)16); // 位深度

        writer.Write(new char[4] { 'd', 'a', 't', 'a' });
        writer.Write(samples * channels * 2); // 数据大小
    }

}