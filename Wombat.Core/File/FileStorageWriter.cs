

using System;

namespace Wombat.Core
{
    /// <summary>
    /// 文件写入器。
    /// </summary>
    public partial class FileStorageWriter :IDisposable
    {
        private readonly FileStorage _fileStorage;
        private long _position;
        private bool disposedValue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileStorage"></param>
        public FileStorageWriter(FileStorage fileStorage)
        {
            _fileStorage = fileStorage ?? throw new System.ArgumentNullException(nameof(fileStorage));
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        public void Write(byte[] buffer)
        {
            Write(buffer, 0, buffer.Length);
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
        /// 移动Pos到流末尾
        /// </summary>
        /// <returns></returns>
        public long SeekToEnd()
        {
            return Position = FileStorage.Length;
        }

        /// <summary>
        /// 读取数据到缓存区
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public void Write(byte[] buffer, int offset, int length)
        {
            _fileStorage.Write(_position, buffer, offset, length);
            _position += length;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    FilePool.TryReleaseFile(_fileStorage.Path);
                    // TODO: 释放托管状态(托管对象)
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        ~FileStorageWriter()
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