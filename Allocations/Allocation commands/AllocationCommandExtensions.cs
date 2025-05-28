namespace HereticalSolutions.Allocations
{
	public static class AllocationCommandExtensions
	{
		public static int CountInitialAllocationAmount(
			this AllocationCommandDescriptor descriptor)
		{
			int initialAmount = -1;

			switch (descriptor.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = descriptor.Amount;
					break;
			}

			return initialAmount;
		}

		public static int CountAdditionalAllocationAmount(
			this AllocationCommandDescriptor descriptor,
			int currentCapacity)
		{
			int addedCapacity = -1;

			switch (descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					addedCapacity = 1;
					break;

				case EAllocationAmountRule.DOUBLE_AMOUNT:
					addedCapacity = currentCapacity * 2;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					addedCapacity = descriptor.Amount;
					break;
			}

			return addedCapacity;
		}
	}
}