using System;

public class PartitionMappingFailedException :
    Exception
{
    public PartitionMappingFailedException(string message) : base(message)
    {
    }
}