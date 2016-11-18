// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;

namespace Microsoft.EntityFrameworkCore.Tools.DotNet.TestUtilities
{
    public class FileUtility
    {
        public static void DirectoryCopy(string source, string dest, bool recursive)
        {
            var dir = new DirectoryInfo(source);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + source);
            }

            var dirs = dir.GetDirectories();
            if (!Directory.Exists(dest))
            {
                Directory.CreateDirectory(dest);
            }

            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = Path.Combine(dest, file.Name);
                file.CopyTo(temppath, false);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    var temppath = Path.Combine(dest, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, recursive);
                }
            }
        }
    }
}
