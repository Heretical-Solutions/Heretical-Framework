using System;

using UnityEngine;

namespace HereticalSolutions.Logging
{
	public class UnityDebugLogSink
		: ILoggerSink
	{
		#region ILogger

		#region Log

		public void Log(
			string value)
		{
			Debug.Log(
				value);
		}

		public void Log<TSource>(
			string value)
		{
			if (typeof(TSource) == typeof(Application))
				return;
            
			Log(value);
		}

		public void Log(
			Type logSource,
			string value)
		{
			if (logSource == typeof(Application))
				return;
			
			Log(value);
		}

		public void Log(
			string value,
			object[] arguments)
		{
			if (arguments != null
			    && arguments.Length > 0
			    && arguments[0] is UnityEngine.Object)
			{
				Debug.Log(
					value,
					(UnityEngine.Object)arguments[0]);
			}
			else
				Log(value);
		}

		public void Log<TSource>(
			string value,
			object[] arguments)
		{
			if (typeof(TSource) == typeof(Application))
				return;
			
			Log(
				value,
				arguments);
		}

		public void Log(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (logSource == typeof(Application))
				return;
			
			Log(
				value,
				arguments);
		}

		#endregion

		#region Warning

		public void LogWarning(
			string value)
		{
			Debug.LogWarning(
				value);
		}

		public void LogWarning<TSource>(
			string value)
		{
			if (typeof(TSource) == typeof(Application))
				return;
			
			LogWarning(value);
		}

		public void LogWarning(
			Type logSource,
			string value)
		{
			if (logSource == typeof(Application))
				return;
			
			LogWarning(value);
		}

		public void LogWarning(
			string value,
			object[] arguments)
		{
			if (arguments != null
			    && arguments.Length > 0
			    && arguments[0] is UnityEngine.Object)
			{
				Debug.LogWarning(
					value,
					(UnityEngine.Object)arguments[0]);
			}
			else
				LogWarning(value);
		}

		public void LogWarning<TSource>(
			string value,
			object[] arguments)
		{
			if (typeof(TSource) == typeof(Application))
				return;
			
			LogWarning(
				value,
				arguments);
		}

		public void LogWarning(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (logSource == typeof(Application))
				return;
			
			LogWarning(
				value,
				arguments);
		}

		#endregion

		#region Error

		public void LogError(
			string value)
		{
			Debug.LogError(
				value);
		}

		public void LogError<TSource>(
			string value)
		{
			if (typeof(TSource) == typeof(Application))
				return;
			
			LogError(value);
		}

		public void LogError(
			Type logSource,
			string value)
		{
			if (logSource == typeof(Application))
				return;
			
			LogError(value);
		}

		public void LogError(
			string value,
			object[] arguments)
		{
			if (arguments != null
			    && arguments.Length > 0
			    && arguments[0] is UnityEngine.Object)
			{
				Debug.LogError(
					value,
					(UnityEngine.Object)arguments[0]);
			}
			else
				LogError(value);
		}

		public void LogError<TSource>(
			string value,
			object[] arguments)
		{
			if (typeof(TSource) == typeof(Application))
				return;
			
			LogError(
				value,
				arguments);
		}

		public void LogError(
			Type logSource,
			string value,
			object[] arguments)
		{
			if (logSource == typeof(Application))
				return;
			
			LogError(
				value,
				arguments);
		}

		#endregion

		#region Exception

		public string FormatException(
			string value)
		{
			return value;
		}

		public string FormatException<TSource>(
			string value)
		{
			return value;
		}

		public string FormatException(
			Type logSource,
			string value)
		{
			return value;
		}

		#endregion

		#endregion
	}
}