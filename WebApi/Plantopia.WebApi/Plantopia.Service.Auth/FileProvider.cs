﻿using System;
using System.Diagnostics;
using System.IO;

namespace Plantopia.Service.Auth
{
    internal class FileProvider
    {
        public string FilePath { get; set; }

        public FileProvider(string path)
        {
            FilePath = path;
        }

        public async void Save(string context)
        {
            using (var writer = new StreamWriter(FilePath))
            {
                await writer.WriteAsync(context);
            }

        }

        public string Open()
        {
            if (!new FileInfo(FilePath).Exists) return string.Empty;

            string result = string.Empty;

            try
            {
                using (var reader = new StreamReader(FilePath))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.Write(e);
            }

            return result;
        }

    }
}
