using System;

namespace HereticalSolutions.FSM.NonAlloc.Async
{
	public interface IAsyncNonAllocTransitionEvent<TBaseState>
		where TBaseState : IAsyncNonAllocState
	{
		Type From { get; }

		Type To { get; }

		EAsyncTransitionRules Rules { get; }
	}
}