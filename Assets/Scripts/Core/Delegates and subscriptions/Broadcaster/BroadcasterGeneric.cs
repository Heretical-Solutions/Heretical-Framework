using System;

namespace HereticalSolutions.Delegates.Broadcasting
{
	public class BroadcasterGeneric<TValue>
		: IPublisherSingleArgGeneric<TValue>,
		  IPublisherSingleArg,
		  ISubscribableSingleArgGeneric<TValue>,
		  ISubscribableSingleArg
	{
		private Action<TValue> multicastDelegate;
		
		#region IPublisherSingleArgGeneric

		public void Publish(TValue value)
		{
			multicastDelegate?.Invoke(value);
		}
		
		#endregion

		#region IPublisherSingleArg

		public void Publish<TArgument>(TArgument value)
		{
			if (!(typeof(TArgument).Equals(typeof(TValue))))
				throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");
			
			//DIRTY HACKS DO NOT REPEAT
			object valueObject = (object)value;
			
			Publish((TValue)valueObject); //It doesn't want to convert TArgument into TValue. Bastard
		}

		public void Publish(Type valueType, object value)
		{
			if (!(valueType.Equals(typeof(TValue))))
				throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");
			
			Publish((TValue)value);
		}

		#endregion

		#region ISubscribableSingleArgGeneric
		
		public void Subscribe(Action<TValue> @delegate)
		{
			multicastDelegate += @delegate;
		}

		public void Subscribe(object @delegate)
		{
			multicastDelegate += (Action<TValue>)@delegate;
		}

		public void Unsubscribe(Action<TValue> @delegate)
		{
			multicastDelegate -= @delegate;
		}
		
		public void Unsubscribe(object @delegate)
		{
			multicastDelegate -= (Action<TValue>)@delegate;
		}
		
		#endregion

		#region ISubscribableSingleArg

		public void Subscribe<TArgument>(Action<TArgument> @delegate)
		{
			if (!(typeof(TArgument).Equals(typeof(TValue))))
				throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");

			//DIRTY HACKS DO NOT REPEAT
			object delegateObject = (object)@delegate;
			
			multicastDelegate += (Action<TValue>)delegateObject;
		}

		public void Subscribe(Type valueType, object @delegate)
		{
			if (!(valueType.Equals(typeof(TValue))))
				throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

			multicastDelegate += (Action<TValue>)@delegate;
		}

		public void Unsubscribe<TArgument>(Action<TArgument> @delegate)
		{
			if (!(typeof(TArgument).Equals(typeof(TValue))))
				throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{typeof(TArgument).ToString()}\"");

			//DIRTY HACKS DO NOT REPEAT
			object delegateObject = (object)@delegate;
			
			multicastDelegate -= (Action<TValue>)delegateObject;
		}

		public void Unsubscribe(Type valueType, object @delegate)
		{
			if (!(valueType.Equals(typeof(TValue))))
				throw new Exception($"[BroadcasterGeneric] INVALID ARGUMENT TYPE. EXPECTED: \"{typeof(TValue).ToString()}\" RECEIVED: \"{valueType.ToString()}\"");

			multicastDelegate -= (Action<TValue>)@delegate;
		}

		#endregion
	}
}