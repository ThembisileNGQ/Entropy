namespace Transcoding.Transcoder.Model
{
    public class MediaFile
    {
        public string Filename { get; set; }
        public Metadata Metadata { get; internal set; }

        public MediaFile(string filename)
        {
            Filename = filename;
        }
    }
}