using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;
using Transcoding.Transcoder.Util;

namespace Transcoding.Transcoder
{
    public class EngineBase : IDisposable
    {
        private bool isDisposed;

        /// <summary>   Used for locking the FFmpeg process to one thread. </summary>
        private const string LockName = "MediaToolkit.Engine.LockName";

        private const string DefaultFFmpegFilePath = @"/MediaToolkit/ffmpeg.exe";

        /// <summary>   Full pathname of the FFmpeg file. </summary>
        protected readonly string FFmpegFilePath;

        /// <summary>   The Mutex. </summary>
        /// <remarks>Null if concurrently running FFmpeg instances are allowed.</remarks>
        protected readonly Mutex Mutex;

        /// <summary>   The ffmpeg process. </summary>
        protected Process FFmpegProcess;


        protected EngineBase()
           //: this(ConfigurationManager.AppSettings["mediaToolkit.ffmpeg.path"])
        {
        }

        protected EngineBase(bool enableMultipleRunningProcesses)
            //: this(ConfigurationManager.AppSettings["mediaToolkit.ffmpeg.path"], enableMultipleRunningProcesses)
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <para> Initializes FFmpeg.exe; Ensuring that there is a copy</para>
        ///     <para> in the clients temp folder &amp; isn't in use by another process.</para>
        /// </summary>
        protected EngineBase(string ffMpegPath) 
            : this(ffMpegPath, false)
        {
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///     <para> Initializes FFmpeg.exe; Ensuring that there is a copy</para>
        ///     <para> in the clients temp folder &amp; isn't in use by another process.</para>
        /// </summary>
        /// <param name="enableMultipleRunningProcesses">Whether or not to allow multiple instances of FFmpeg to run concurrently.</param>
        protected EngineBase(string ffMpegPath, bool enableMultipleRunningProcesses)
        {
            if (!enableMultipleRunningProcesses)
            {
                this.Mutex = new Mutex(false, LockName);
            }

            this.isDisposed = false;

            if (string.IsNullOrWhiteSpace(ffMpegPath))
            {
                ffMpegPath = DefaultFFmpegFilePath;
            }

            this.FFmpegFilePath = ffMpegPath;

            this.EnsureDirectoryExists();
            this.EnsureFFmpegFileExists();

            if (!enableMultipleRunningProcesses)
            {
                this.EnsureFFmpegIsNotUsed();
            }
        }

        private void EnsureFFmpegIsNotUsed()
        {
            try
            {
                this.Mutex.WaitOne();
                //Process.GetProcessesByName(Resources.FFmpegProcessName)
                Process.GetProcessesByName("ffmpeg")
                       .ForEach(process =>
                       {
                           process.Kill();
                           process.WaitForExit();
                       });
            }
            finally
            {
                this.Mutex.ReleaseMutex();
            }
        }

        private void EnsureDirectoryExists()
        {
            string directory = Path.GetDirectoryName(this.FFmpegFilePath) ?? Directory.GetCurrentDirectory(); ;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void EnsureFFmpegFileExists()
        {
            if (!File.Exists(this.FFmpegFilePath))
            {
                throw new FileNotFoundException($"{FFmpegFilePath}");
            }
        }




        ///-------------------------------------------------------------------------------------------------
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        /// <remarks>   Aydin Aydin, 30/03/2015. </remarks>
        public virtual void Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing || this.isDisposed)
            {
                return;
            }

            if (FFmpegProcess != null)
            {
                this.FFmpegProcess.Dispose();
            }
            this.FFmpegProcess = null;
            this.isDisposed = true;
        }
    }
}