﻿using Ascon.Pilot.SDK;
using System.Collections.ObjectModel;

namespace PilotObjectInfo.ViewModels
{
    class SignnaturesInfoViewModel : Base.ViewModel
    {
        [System.Obsolete]
        public SignnaturesInfoViewModel(IFile file)
        {
            File = file;
            Signatures = new ObservableCollection<ISignature>(file.Signatures);
        }

        public IFile File { get; set; }
        public ObservableCollection<ISignature> Signatures { get; set; }
    }
}
