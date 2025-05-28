using HereticalSolutions.FSM.NonAlloc;

namespace HereticalSolutions.Networking
{
	public interface IHandshakeStep
		: INonAllocState
	{
		bool WillProcessPacket();

		void ProcessPacket();
	}
}