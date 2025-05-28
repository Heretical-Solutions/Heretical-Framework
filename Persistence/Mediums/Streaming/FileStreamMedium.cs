using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
    [SerializationMedium]
    public class FileStreamMedium
        : AStreamingMedium<FileStream>,
          IHasIODestination
    {
        private readonly bool flushAutomatically;

        public string FullPath { get; private set; }

        public FileStream FileStream => stream;


        public FileStreamMedium(
            string fullPath,

            ILogger logger,
            
            bool flushAutomatically = true)
            : base(
                logger)
        {
            FullPath = fullPath;

            this.flushAutomatically = flushAutomatically;
        }

        #region IHasIODestination

        public void EnsureIODestinationExists()
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);

            if (!IOHelpers.FileExists(
                FullPath,
                logger))
            {
                //Courtesy of https://stackoverflow.com/questions/44656364/when-writing-a-txt-file-in-unity-it-says-sharing-violation-on-path
                File
                    .Create(
                        FullPath)
                    .Dispose();
            }
        }

        public bool IODestinationExists()
        {
            return IOHelpers.FileExists(
                FullPath,
                logger);
        }

        public void CreateIODestination()
        {
            EraseIODestination();

            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);

            //Courtesy of https://stackoverflow.com/questions/44656364/when-writing-a-txt-file-in-unity-it-says-sharing-violation-on-path
            File
                .Create(
                    FullPath)
                .Dispose();
        }

        public void EraseIODestination()
        {
            if (IOHelpers.FileExists(
                FullPath,
                logger))
            {
                File.Delete(FullPath);
            }
        }

        #endregion

        public override bool FlushAutomatically => flushAutomatically;

        protected override bool OpenReadStream(
            out FileStream dataStream)
        {
            dataStream = default(FileStream);

            if (!IOHelpers.FileExists(
                FullPath,
                logger))
            {
                return false;
            }

            dataStream = new FileStream(
                FullPath,
                FileMode.Open,
                FileAccess.Read);

            return true;
        }

        protected override bool OpenWriteStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);

            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                FullPath,
                FileMode.Create,
                FileAccess.Write);

            return true;
        }

        protected override bool OpenAppendStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);

            //Courtesy of https://stackoverflow.com/questions/7306214/append-lines-to-a-file-using-a-fileStream
            //Courtesy of https://learn.microsoft.com/en-us/dotnet/api/system.io.filemode?view=net-8.0
            dataStream = new FileStream(
                FullPath,
                FileMode.Append,
                FileAccess.Write);

            return true;
        }

        protected override bool OpenReadWriteStream(
            out FileStream dataStream)
        {
            IOHelpers.EnsureDirectoryExists(
                FullPath,
                logger);

            dataStream = new FileStream(
                FullPath,
                FileMode.OpenOrCreate,
                FileAccess.ReadWrite);

            return true;
        }
    }
}