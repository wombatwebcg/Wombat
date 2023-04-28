
using System.IO;

namespace Wombat.Core
{
    /// <summary>
    /// FileStorageStream。非线程安全。
    /// </summary>
    public partial class FileStorageStream : Stream
    {
        private readonly FileStorage _fileStorage;
        private long _position;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileStorage"></param>
        public FileStorageStream(FileStorage fileStorage)
        {
            _fileStorage = fileStorage ?? throw new System.ArgumentNullException(nameof(fileStorage));
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~FileStorageStream()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: false);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanRead => _fileStorage.FileStream.CanRead;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanSeek => _fileStorage.FileStream.CanSeek;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override bool CanWrite => _fileStorage.FileStream.CanWrite;

        /// <summary>
        /// 文件存储器
        /// </summary>
        public FileStorage FileStorage => _fileStorage;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override long Length => _fileStorage.FileStream.Length;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override long Position { get => _position; set => _position = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public override void Flush()
        {
            _fileStorage.Flush();
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int r = _fileStorage.Read(_position, buffer, offset, count);
            _position += r;
            return r;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    break;

                case SeekOrigin.Current:
                    _position += offset;
                    break;

                case SeekOrigin.End:
                    _position = Length + offset;
                    break;
            }
            return _position;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            _fileStorage.FileStream.SetLength(value);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _fileStorage.Write(_position, buffer, offset, count);
            _position += count;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            FilePool.TryReleaseFile(_fileStorage.Path);
            base.Dispose(disposing);
        }
    }
}