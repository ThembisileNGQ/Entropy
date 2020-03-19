namespace Transcoding.Transcoder.Model
{
    public class MediaFile
    {
        public string Filename { get; set; }
        public Metadata Metadata { get; }

        public MediaFile(
            string filename)
        {
            Filename = filename;
        }
    }
}