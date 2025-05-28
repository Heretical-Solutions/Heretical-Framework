using System;

namespace HereticalSolutions.Blackboard.Unity
{
	public interface IBlackboardCarrierReference
		: IBlackboardCarrier
	{
		IBlackboardCarrier ReferencedCarrier { get; }

		Action<IBlackboardCarrierReference> OnReferenceModified { get; set; }
	}
}