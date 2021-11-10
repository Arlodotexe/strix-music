namespace OwlCore.Tests.Remoting
{
    public enum MethodReturnType
    {
        None = 0,
        Void = 1,
        Enum = 2,
        Struct = 4,
        Object = 8,
        Primitive = 16,

        Task = 32,
        TaskEnum = 64,
        TaskStruct = 128,
        TaskObject = 256,
        TaskPrimitive = 512,
    }
}
