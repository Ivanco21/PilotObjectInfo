using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Core;
using PilotObjectInfo.Extensions;

namespace PilotObjectInfo
{
    class FileModifier
    {
        public FileModifier()
        {
            
        }

        public async Task<IEnumerable<IFile>> AddFiles(Guid id, IEnumerable<string> filePaths)
        {
            if (filePaths == null) return null;
            if (filePaths.Count() == 0) return null;
            var builder = GI.Modifier.EditById(id);
            foreach (string filePath in filePaths)
            {
                builder.AddFile(filePath);
            }
            GI.Modifier.Apply();
            var obj = (await GI.Repository.GetObjectsAsync(new Guid[] { id }, o => o, System.Threading.CancellationToken.None)).FirstOrDefault();
            return obj.ActualFileSnapshot.Files;
        }

        public async Task<IEnumerable<IFile>> RemoveFile(Guid id, IFile file)
        {
            var builder = GI.Modifier.EditById(id);
            builder.RemoveFile(file.Id);
            GI.Modifier.Apply();
            var obj = (await GI.Repository.GetObjectsAsync(new Guid[] { id }, o => o, System.Threading.CancellationToken.None)).FirstOrDefault();
            return obj.ActualFileSnapshot.Files;
        }
    }
}
