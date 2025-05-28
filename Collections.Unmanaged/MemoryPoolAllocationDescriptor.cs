using HereticalSolutions.Allocations;

namespace HereticalSolutions.Collections.Unmanaged
{
    public unsafe struct MemoryPoolAllocationDescriptor
    {
        /// <summary>
        /// Pointer to the unmanaged heap memory the array is stored in.
        /// </summary>
        public byte* MemoryPointer;
        
        /// <summary>
        /// Allocation status.
        /// </summary>
        public EAllocationStatus Status;
        
        /// <summary>
        /// Next allocation descriptor in linked list.
        /// </summary>
        public MemoryPoolAllocationDescriptor* Next;
        
        /// <summary>
        /// Create the allocation. Its contents are initially undefined.
        /// </summary>
        /// <param name="memoryPointer">Pointer to the unmanaged heap memory the array is stored in.</param>
        public MemoryPoolAllocationDescriptor(
            byte* memoryPointer)
        {
            MemoryPointer = memoryPointer;
            
            Status = EAllocationStatus.FREE;
            
            Next = null;
        }
    }
}