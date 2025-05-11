using System;
using Ascon.Pilot.SDK;

namespace PilotObjectInfo.ViewModels
{
    class SnapshotViewModel
    {
        private IFilesSnapshot _filesSnapshot;
        private FilesViewModel _files;

        public SnapshotViewModel(Guid objectId,  IFilesSnapshot filesSnapshot)
        {
            _filesSnapshot = filesSnapshot;
            _files = new FilesViewModel(objectId, _filesSnapshot.Files);

        }

        public long Version => _filesSnapshot.Created.Ticks;
        public DateTime Created => _filesSnapshot.Created;

        public int CreatorId => _filesSnapshot.CreatorId;

        public string Reason => _filesSnapshot.Reason;

        public FilesViewModel Files => _files;
    }
}
