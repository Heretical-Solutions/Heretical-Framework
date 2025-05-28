using System;
using HereticalSolutions.FSM.NonAlloc;

namespace HereticalSolutions.Networking
{
	public interface IHandshakeProtocol
		: INonAllocStateMachine<IHandshakeStep>
	{
		IHandshakeStep CurrentStep { get; }

		IHandshakeStep GetStep<TStep>()
			where TStep : IHandshakeStep;

		IHandshakeStep GetStep(
			Type stepType);
	}
}