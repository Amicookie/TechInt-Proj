using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NativeApp.Models;

namespace NativeApp.ViewModels
{
    public class MainViewModel
    {
        private DocumentModel _document;
        public FileViewModel File { get; set; }

        public MainViewModel()
        {
            _document = new DocumentModel();

            File = new FileViewModel(_document);
        }
    }
}
