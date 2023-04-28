
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Wombat.Core
{
    /// <summary>
    /// 文件池。
    /// </summary>
    public static partial class FilePool
    {

        private static readonly ConcurrentDictionary<string, FileStorage> _pathStorage = new ConcurrentDictionary<string, FileStorage>();

        private static readonly Timer _timer;

       static AsyncLock @lock;
        static FilePool()
        {
            _timer = new Timer(OnTimer, null, 60000, 60000);
            @lock = new AsyncLock();
        }

        /// <summary>
        /// 获取所有的路径。
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllPaths()
        {
            return _pathStorage.Keys.ToArray();
        }

        /// <summary>
        /// 加载文件为读取流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorage GetFileStorageForRead(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));
            }
            return GetFileStorageForRead(new FileInfo(path));
        }

        /// <summary>
        /// 加载文件为读取流
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorage GetFileStorageForRead(FileInfo fileInfo)
        {
            if (_pathStorage.TryGetValue(fileInfo.FullName, out FileStorage storage))
            {
                if (storage.FileAccess != FileAccess.Read)
                {
                    throw new Exception("该路径的文件已经被加载为仅写入模式。");
                }
                Interlocked.Increment(ref storage._reference);
                return storage;
            }
            using (@lock.Lock())
            {
                if (_pathStorage.TryGetValue(fileInfo.FullName, out storage))
                {
                    return storage;
                }
                FileStorage fileStorage = new FileStorage(fileInfo, FileAccess.Read);
                _pathStorage.TryAdd(fileInfo.FullName, fileStorage);
                return fileStorage;
            }
        }

        /// <summary>
        /// 加载文件为写流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorage GetFileStorageForWrite(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));
            }
            return GetFileStorageForWrite(new FileInfo(path));
        }

        /// <summary>
        /// 加载文件为写流
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorage GetFileStorageForWrite(FileInfo fileInfo)
        {
            if (_pathStorage.TryGetValue(fileInfo.FullName, out FileStorage storage))
            {
                if (storage.FileAccess != FileAccess.Write)
                {
                    throw new Exception("该路径的文件已经被加载为仅读取模式。");
                }
                Interlocked.Increment(ref storage._reference);
                return storage;
            }
            using (@lock.Lock())
            {
                if (_pathStorage.TryGetValue(fileInfo.FullName, out storage))
                {
                    return storage;
                }
                FileStorage fileStorage = new FileStorage(fileInfo, FileAccess.Write);
                _pathStorage.TryAdd(fileInfo.FullName, fileStorage);
                return fileStorage;
            }
        }

        /// <summary>
        /// 获取一个可读可写的Stream对象。
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorageStream GetFileStorageStream(string path)
        {
            return new FileStorageStream(GetFileStorageForWrite(path));
        }

        /// <summary>
        /// 获取一个可读可写的Stream对象。
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorageStream GetFileStorageStream(FileInfo fileInfo)
        {
            return new FileStorageStream(GetFileStorageForWrite(fileInfo));
        }

        /// <summary>
        /// 获取一个文件读取访问器
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorageReader GetReader(string path)
        {
            return new FileStorageReader(GetFileStorageForRead(path));
        }

        /// <summary>
        /// 获取一个文件读取访问器
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorageReader GetReader(FileInfo fileInfo)
        {
            return new FileStorageReader(GetFileStorageForRead(fileInfo));
        }

        /// <summary>
        /// 获取引用次数。
        /// </summary>
        /// <param name="path">必须是全路径。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static int GetReferenceCount(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return 0;
            }
            if (_pathStorage.TryGetValue(path, out FileStorage fileStorage))
            {
                return fileStorage._reference;
            }
            return 0;
        }

        /// <summary>
        /// 获取一个文件写入访问器
        /// </summary>
        /// <param name="path">路径</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorageWriter GetWriter(string path)
        {
            return new FileStorageWriter(GetFileStorageForWrite(path));
        }

        /// <summary>
        /// 获取一个文件写入访问器
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static FileStorageWriter GetWriter(FileInfo fileInfo)
        {
            return new FileStorageWriter(GetFileStorageForWrite(fileInfo));
        }

        /// <summary>
        /// 加载文件为缓存读取流
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static void LoadFileForCacheRead(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new System.ArgumentException($"“{nameof(path)}”不能为 null 或空。", nameof(path));
            }

            path = Path.GetFullPath(path);
            if (_pathStorage.TryGetValue(path, out FileStorage storage))
            {
                if (storage.FileAccess != FileAccess.Read || !storage.Cache)
                {
                    throw new Exception("该路径的文件已经被加载为其他模式。");
                }
                return;
            }
            if (FileStorage.TryCreateCacheFileStorage(path, out FileStorage fileStorage, out string msg))
            {
                _pathStorage.TryAdd(path, fileStorage);
            }
            else
            {
                throw new Exception(msg);
            }
        }


        /// <summary>
        /// 减少引用次数，并尝试释放流。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="delayTime">延迟释放时间。当设置为0时，立即释放,单位毫秒。</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        public static void TryReleaseFile(string path, int delayTime = 0)
        {
            if (string.IsNullOrEmpty(path))
            {
                return ;
            }
            path = Path.GetFullPath(path);
            if (_pathStorage.TryGetValue(path, out FileStorage fileStorage))
            {
                Interlocked.Decrement(ref fileStorage._reference);
                if (fileStorage._reference <= 0)
                {
                    if (delayTime > 0)
                    {
                        new Timer((o) =>
                        {
                            if (GetReferenceCount(path) == 0)
                            {
                                if (_pathStorage.TryRemove((string)path, out fileStorage))
                                {
                                    fileStorage.Dispose();
                                }
                            }
                        }, null, delayTime, Timeout.Infinite);
                    }
                    else
                    {
                        if (_pathStorage.TryRemove(path, out fileStorage))
                        {
                            fileStorage.Dispose();
                        }
                    }
                }
                else
                {
                }
            }
        }

        private static void OnTimer(object state)
        {
            List<string> keys = new List<string>();
            foreach (var item in _pathStorage)
            {
                if (DateTime.Now - item.Value.AccessTime > item.Value.AccessTimeout)
                {
                    keys.Add(item.Key);
                }
            }
            foreach (var item in keys)
            {
                TryReleaseFile(item);
            }
        }


    }
}