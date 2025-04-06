﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Ascon.Pilot.SDK;
using Microsoft.Win32;
using PilotObjectInfo.ViewModels.Commands;

namespace PilotObjectInfo.ViewModels
{
    class FilesViewModel : Base.ViewModel
    {
        private Guid _objectId;
        private ReadOnlyCollection<IFile> _files;
        private IFileProvider _fileProvider;
        private FileModifier _fileModifier;
        private RelayCommand _downloadCmd;
        private RelayCommand _downloadAllCmd;
        private RelayCommand _addFilesCmd;
        private RelayCommand _delFileCmd;

        public FilesViewModel(Guid objectId, ReadOnlyCollection<IFile> files, IFileProvider fileProvider, FileModifier fileModifier = null)
        {
            _objectId = objectId;
            _files = files;
            _fileProvider = fileProvider;
            _fileModifier = fileModifier;
            Files = new ObservableCollection<IFile>(_files);

        }

        public ObservableCollection<IFile> Files { get; set; }

        private IFile _selectedFile;

        [Obsolete]
        public IFile SelectedFile
        {
            get { return _selectedFile; }
            set
            {
                if (value != null)
                {
                    SignnaturesInfo = new SignnaturesInfoViewModel(value);
                    FileContent = GetFileContent(value);
                }
                else SignnaturesInfo = null;

            }
        }

        private string GetFileContent(IFile file)
        {
            using (var stream = _fileProvider.OpenRead(file))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                byte[] byteArray = memoryStream.ToArray();
                var str = Encoding.UTF8.GetString(byteArray);
                return str;
            }
        }

        private string _fileContent;

        public string FileContent
        {
            get {return _fileContent;}
            set => Set(ref _fileContent, value);
        }

        private SignnaturesInfoViewModel _signnaturesInfo;
        public SignnaturesInfoViewModel SignnaturesInfo
        {
            get { return _signnaturesInfo; }
            set => Set(ref _signnaturesInfo, value);

        }

        public RelayCommand DownloadCmd
        {
            get
            {
                if (_downloadCmd == null)
                {
                    _downloadCmd = new RelayCommand(DoDownLoad);
                }
                return _downloadCmd;

            }
        }


        public RelayCommand DownloadAllCmd
        {
            get
            {
                if (_downloadAllCmd == null)
                {
                    _downloadAllCmd = new RelayCommand(DoDownloadAllCmd, (o) => _files.Count() > 0);
                }
                return _downloadAllCmd;

            }
        }

        public RelayCommand AddFilesCmd
        {
            get
            {
                if (_addFilesCmd == null)
                {
                    _addFilesCmd = new RelayCommand(DoAddFiles, (o) => _fileModifier != null);
                }
                return _addFilesCmd;

            }
        }

        public RelayCommand DelFileCmd
        {
            get
            {
                if (_delFileCmd == null)
                {
                    _delFileCmd = new RelayCommand(DoDelFile, (o) => _fileModifier != null );
                }
                return _delFileCmd;

            }
        }

        private async void DoDelFile(object obj)
        {
            var file = obj as IFile;
            if (file == null) return;
            if (MessageBox.Show($"Do you really want to delete a file: [{file.Name}]?", "Delete file", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var files = await _fileModifier.RemoveFile(_objectId, file);
                Refresh(files);
            }
        }

        private async void DoAddFiles(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() != true) return;
            var files = await _fileModifier.AddFiles(_objectId, dialog.FileNames);
            Refresh(files);
        }

        private void Refresh(IEnumerable<IFile> files)
        {
            if (files == null) return;
            _files = new ReadOnlyCollection<IFile>(files.ToList());
            Files.Clear();
            _files.ToList().ForEach(x => Files.Add(x));
        }

        private void DoDownloadAllCmd(object obj)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                if (result != System.Windows.Forms.DialogResult.OK) return;

                foreach (var file in _files)
                {
                    using (var stream = _fileProvider.OpenRead(file))
                    {
                        try
                        {
                            using (FileStream output = new FileStream(Path.Combine(dialog.SelectedPath, file.Name), FileMode.Create))
                            {
                                stream.CopyTo(output);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                        }
                    }
                }
            }
        }

        private void DoDownLoad(object obj)
        {
            var file = obj as IFile;
            if (file == null) return;
            var dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = Path.GetExtension(file.Name);
            dlg.FileName = file.Name;
            if (dlg.ShowDialog() != true) return;
            using (var stream = _fileProvider.OpenRead(file))
            {
                try
                {
                    using (FileStream output = new FileStream(dlg.FileName, FileMode.Create))
                    {
                        stream.CopyTo(output);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            }
        }
    }
}
