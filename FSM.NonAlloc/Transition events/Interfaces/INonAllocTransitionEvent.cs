using System;

namespace HereticalSolutions.FSM.NonAlloc
{
	public interface INonAllocTransitionEvent<TBaseState>
		where TBaseState : INonAllocState
	{
		Type From { get; }

		Type To { get; }
	}
}