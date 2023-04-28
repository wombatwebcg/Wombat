
using System;

namespace Wombat.Core
{
    /// <summary>
    /// 文件读取器
    /// </summary>
    public  class FileStorageReader : IDisposable
    {
        private FileStorage _fileStorage;

        private long _position;
        private bool disposedValue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileStorage"></param>
        public FileStorageReader(FileStorage fileStorage)
        {
            _fileStorage = fileStorage ?? throw new System.ArgumentNullException(nameof(fileStorage));
        }


        /// <summary>
        /// 文件存储器
        /// </summary>
        public FileStorage FileStorage => _fileStorage;

        /// <summary>
        /// 游标位置
        /// </summary>
        public int Pos
        {
            get => (int)_position;
            set => _position = value;
        }

        /// <summary>
        /// 游标位置
        /// </summary>
        public long Position
        {
            get => _position;
            set => _position = value;
        }

        /// <summary>
        /// 读取数据到缓存区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public int Read(byte[] buffer, int offset, int length)
        {
            int r = _fileStorage.Read(_position, buffer, offset, length);
            _position += r;
            return r;
        }



        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FilePool.TryReleaseFile(_fileStorage.Path);
                    _fileStorage = null;
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~FileStorageReader()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}