using System;
using System.IO;

using UnityEngine;

namespace HereticalSolutions.Persistence.Unity
{
	[Serializable]
	public class FileAtApplicationDataPathSettings
		: IPathSettings
	{
		public string RelativePath;

		public string ApplicationDataFolder
		{
			get
			{
				return Application.dataPath;
			}
		}

		public string FullPath
		{
			get
			{
				return Path
					.Combine(
						ApplicationDataFolder,
						RelativePath)
					.SanitizePath();
			}
		}
	}
}