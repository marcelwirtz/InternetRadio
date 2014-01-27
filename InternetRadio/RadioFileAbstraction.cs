using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace InternetRadio
{
    public class RadioFile
    {
        public RadioFile(string Name, Stream Stream)
        {
            this.Name = Name;
            this.Stream = Stream;
        }
        public string Name { get; set; }
        public Stream Stream { get; set; }
    }

    class RadioFileAbstraction : TagLib.File.IFileAbstraction
    {
        private RadioFile file;
  
        public RadioFileAbstraction(RadioFile file)
        {
            this.file = file;
        }
  
        public string Name
        {
            get { return file.Name; }
        }
  
        public System.IO.Stream ReadStream
        {
            get { return file.Stream; }
        }
  
        public System.IO.Stream WriteStream
        {
            get {  return file.Stream; }
        }
  
        public void CloseStream(System.IO.Stream stream) 
        {
            stream.Position = 0;
        }
    }
}
