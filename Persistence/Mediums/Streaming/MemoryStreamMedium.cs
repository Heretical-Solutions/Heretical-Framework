using System.IO;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Persistence
{
	[SerializationMedium]
	public class MemoryStreamMedium
		: AStreamingMedium<MemoryStream>,
		  IMediumWithTypeFilter
	{
		private byte[] buffer;

		public byte[] Buffer
		{
			get
			{
				if (buffer != null)
					return buffer;

				if (stream != null)
					return stream.GetBuffer();

				return null;
			}
		}

		public MemoryStream MemoryStream => stream;


		private int index = -1;

		public int Index => index;


		private int count = -1;

		public int Count => count;


		public MemoryStreamMedium(
			ILogger logger,
			
			byte[] buffer = null,
			int index = -1,
			int count = -1)
			: base(
				logger)
		{
			this.buffer = buffer;

			this.index = index;

			this.count = count;
		}

		protected override bool OpenReadStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				false,
				out dataStream);
		}

		protected override bool OpenWriteStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				true,
				out dataStream);
		}

		protected override bool OpenAppendStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				true,
				out dataStream);
		}

		protected override bool OpenReadWriteStream(
			out MemoryStream dataStream)
		{
			return OpenStream(
				true,
				out dataStream);
		}

		private bool OpenStream(
			bool write,
			out MemoryStream dataStream)
		{
			if (write)
			{
				dataStream = new MemoryStream();
			}
			else if (buffer == null)
			{
				dataStream = new MemoryStream();
			}
			else if (index < 0 && count < 0)
			{
				dataStream = new MemoryStream(
					buffer);
			}
			else
			{
				dataStream = new MemoryStream(
					buffer,
					index,
					count);
			}

			return true;
		}

		protected override void CloseStream(
			MemoryStream stream)
		{
			if (CurrentMode != EStreamMode.READ)
			{
				buffer = stream.GetBuffer();
			}

			base.CloseStream(stream);
		}
	}
}