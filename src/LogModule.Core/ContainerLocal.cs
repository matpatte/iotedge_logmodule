﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LogModule
{
    public class ContainerLocal : ContainerRemote, IContainerLocal
    {

        public new static ContainerLocal Create(string accountName, string accountKey)
        {
            if(instance == null)
            {
                instance = new ContainerLocal(accountName, accountKey);
            }

            return instance;
        }
        protected ContainerLocal(string accountName, string accountKey)
            : base(accountName, accountKey)
        {
        }

        private static ContainerLocal instance;

        public virtual async Task<byte[]> GetFile(string path, string filename)
        {
            if(string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if(string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            try
            {
                string srcFilename = operations.FixPath(path) + filename;
                return await operations.ReadFileAsync(srcFilename, CancellationToken.None);
            }
            catch (Exception ex)
            {
                operations.LogToDocker("GetFile", ex);
                throw ex;
            }

        }

        public virtual async Task<string[]> ListFiles(string path)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    return await Task.FromResult<string[]>(Directory.GetFiles(path));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                operations.LogToDocker("ListFiles", ex);
                throw ex;
            }
        }

        public virtual async Task WriteFile(string path, string filename, byte[] body, bool append = false)
        {
            if(string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            if(string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            if(body == null)
            {
                throw new ArgumentNullException("body");
            }


            try
            {
                string srcFilename = operations.FixPath(path) + filename;
                if (append)
                {
                    await operations.WriteAppendFileAsync(srcFilename, body);
                }
                else
                {
                    await operations.WriteFileAsync(srcFilename, body);
                }
            }
            catch (Exception ex)
            {
                operations.LogToDocker("WriteFile", ex);
                throw ex;
            }
        }
    }
}
