using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using System.Net;
using System.IO;

namespace WebSnapIn
{
    [Cmdlet(VerbsCommon.Get, "url")]
    public class GetUrl : Cmdlet
    {
        public GetUrl()
        {
            Progress = new ProgressRecord(1, "Downloading...", "Starting...");
        }

        [Parameter(Mandatory=true, Position=0)]
        public Uri Url { get; set; }

        private ProgressRecord Progress { get; set; }

        protected override void ProcessRecord()
        {
            UpdateProgress("Connecting...");

            var request = WebRequest.Create(Url);
            var response = request.GetResponse();

            UpdateProgress("Connected");

            using (var responseStream = response.GetResponseStream())
            using (var fileStream = new FileStream(GetFilePath(), FileMode.OpenOrCreate))
            {
                CopyStream(
                    responseStream,
                    fileStream,
                    1024,
                    response.ContentLength,
                    (current, total) => UpdateDownloadProgress(current, total));
            }
        }

        private string GetFilePath()
        {
            var fileName = Url.Segments.Last();
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

            return fullPath;
        }

        private void CopyStream(Stream source, Stream sink, int bufferSize, long sourceLength, Action<long, long> notification)
        {
            byte[] buffer = new byte[bufferSize];

            long numberOfBytesCopied = 0;

            while (true)
            {
                var actual = source.Read(buffer, 0, buffer.Length);
                if (actual == 0)
                {
                    break;
                }
                sink.Write(buffer, 0, actual);
                numberOfBytesCopied += actual;
                notification(numberOfBytesCopied, sourceLength);
            }
        }

        private void UpdateProgress(string description)
        {
            Progress.StatusDescription = description;

            WriteProgress(Progress);
        }

        private void UpdateDownloadProgress(long current, long total)
        {
            int percent = (int)((100 * current) / total);

            Progress.StatusDescription = string.Format("{0} bytes of {1} ({2}%)", current, total, percent);
            Progress.PercentComplete = percent;

            WriteProgress(Progress);
        }
    }
}
