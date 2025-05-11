using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Ascon.Pilot.SDK;
using PilotObjectInfo.Core;

namespace PilotObjectInfo.ViewModels
{
    class SnapshotsViewModel
    {
        public SnapshotsViewModel(Guid objectId,  IEnumerable<IFilesSnapshot> filesSnapshot)
        {
            Snapshots = new ObservableCollection<SnapshotViewModel>(filesSnapshot.Select(x => new SnapshotViewModel(objectId, x)));
        }

        public ObservableCollection<SnapshotViewModel> Snapshots { get; }
    }
}
