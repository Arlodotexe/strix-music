using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Extensions;
using OwlCore.Remoting;
using OwlCore.Remoting.Transfer;
using OwlCore.Remoting.Transfer.MessageConverters;
using OwlCore.Tests.Remoting.Mock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting
{
    public partial class NewtonsoftRemoteMessageConverterTests
    {
        [TestMethod, Timeout(2000)]
        public async Task MethodCall_NoParams()
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var methodInfo = ((Func<Task>)MethodCall_NoParams).Method;
            var originalMessage = new RemoteMethodCallMessage(MEMBER_REMOTE_ID, MemberRemote.CreateMemberSignature(methodInfo, MemberSignatureScope.AssemblyQualifiedName), Enumerable.Empty<ParameterData>());
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedMethodCallMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        public Task MethodCall_SingleParam_Null()
        {
            var parameterData = new ParameterData
            {
                Value = null,
                AssemblyQualifiedName = typeof(object).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<Task>)MethodCall_SingleParam_Null).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        public Task MethodCall_SingleParam_List_Null()
        {
            var parameterData = new ParameterData
            {
                Value = new List<object>() { null, null },
                AssemblyQualifiedName = typeof(List<object>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<Task>)MethodCall_SingleParam_List_Null).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        public Task MethodCall_SingleParam_Array_Null()
        {
            var parameterData = new ParameterData
            {
                Value = new object[] { null, null },
                AssemblyQualifiedName = typeof(object[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<Task>)MethodCall_SingleParam_Array_Null).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow("")]
        [DataRow("sampleValue")]
        [DataRow("123")]
        [DataRow("sampleValue123")]
        [DataRow("!@#$%^&*()-=[]\\;',./?><\":{}|_+~`")]
        public Task MethodCall_SingleParam_String(string param)
        {
            var parameterData = new ParameterData
            {
                Value = param,
                AssemblyQualifiedName = typeof(string).AssemblyQualifiedName
            };

            return ValidateMethodAsync(((Func<string, Task>)MethodCall_SingleParam_String).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow("", "sampleValue", "123", "sampleValue123", "!@#$%^&*()-=[]\\;',./?><\":{}|_+~`")]
        public Task MethodCall_SingleParam_List_String(params string[] parameters)
        {
            var parameterData = new ParameterData
            {
                Value = parameters.ToList(),
                AssemblyQualifiedName = typeof(List<string>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_SingleParam_List_String).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow("", "sampleValue", "123", "sampleValue123", "!@#$%^&*()-=[]\\;',./?><\":{}|_+~`")]
        public Task MethodCall_SingleParam_Array_String(params string[] parameters)
        {
            var parameterData = new ParameterData
            {
                Value = parameters,
                AssemblyQualifiedName = typeof(string[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_SingleParam_Array_String).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives")]
        [DataRow("", "sampleValue", "123", "sampleValue123", "!@#$%^&*()-=[]\\;',./?><\":{}|_+~`")]
        public Task MethodCall_MultiParam_String(params string[] parameters)
        {
            var parameterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(string).AssemblyQualifiedName
            });

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_MultiParam_String).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow(byte.MaxValue)]
        [DataRow(byte.MinValue)]
        [DataRow((byte)1)]
        public Task MethodCall_SingleParam_Byte(byte param)
        {
            var parameterData = new ParameterData
            {
                Value = param,
                AssemblyQualifiedName = typeof(byte).AssemblyQualifiedName
            };

            return ValidateMethodAsync(((Func<byte, Task>)MethodCall_SingleParam_Byte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(byte.MaxValue, byte.MinValue, (byte)1)]
        public Task MethodCall_SingleParam_List_Byte(params byte[] bytes)
        {
            var parameterData = new ParameterData
            {
                Value = bytes.ToList(),
                AssemblyQualifiedName = typeof(List<byte>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<byte[], Task>)MethodCall_SingleParam_List_Byte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(byte.MaxValue, byte.MinValue, (byte)1)]
        public Task MethodCall_SingleParam_Array_Byte(params byte[] bytes)
        {
            var parameterData = new ParameterData
            {
                Value = bytes,
                AssemblyQualifiedName = typeof(byte[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<byte[], Task>)MethodCall_SingleParam_Array_Byte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow(byte.MaxValue, byte.MinValue, (byte)1)]
        public Task MethodCall_MultiParam_Byte(params byte[] parameters)
        {
            var parameterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(byte).AssemblyQualifiedName
            });

            return ValidateMethodAsync(((Func<byte[], Task>)MethodCall_MultiParam_Byte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow(sbyte.MaxValue)]
        [DataRow(sbyte.MinValue)]
        [DataRow((sbyte)1)]
        public Task MethodCall_SingleParam_SByte(sbyte param)
        {
            var parameterData = new ParameterData
            {
                Value = param,
                AssemblyQualifiedName = typeof(sbyte).AssemblyQualifiedName
            };

            return ValidateMethodAsync(((Func<sbyte, Task>)MethodCall_SingleParam_SByte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(sbyte.MaxValue, sbyte.MinValue, (sbyte)1)]
        public Task MethodCall_SingleParam_List_SByte(params sbyte[] sbytes)
        {
            var parameterData = new ParameterData
            {
                Value = sbytes.ToList(),
                AssemblyQualifiedName = typeof(List<sbyte>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<sbyte[], Task>)MethodCall_SingleParam_List_SByte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(sbyte.MaxValue, sbyte.MinValue, (sbyte)1)]
        public Task MethodCall_SingleParam_Array_SByte(params sbyte[] sbytes)
        {
            var parameterData = new ParameterData
            {
                Value = sbytes,
                AssemblyQualifiedName = typeof(sbyte[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<sbyte[], Task>)MethodCall_SingleParam_Array_SByte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow(sbyte.MaxValue, sbyte.MinValue, (sbyte)1)]
        public Task MethodCall_MultiParam_SByte(params sbyte[] parameters)
        {
            var parameterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(sbyte).AssemblyQualifiedName,
            });

            return ValidateMethodAsync(((Func<sbyte[], Task>)MethodCall_MultiParam_SByte).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue)]
        [DataRow(int.MinValue)]
        [DataRow(1)]
        public Task MethodCall_SingleParam_Int(int param)
        {
            var parameterData = new ParameterData
            {
                Value = param,
                AssemblyQualifiedName = typeof(int).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<int, Task>)MethodCall_SingleParam_Int).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue, int.MinValue, 1)]
        public Task MethodCall_SingleParam_List_Int(params int[] ints)
        {
            var parameterData = new ParameterData
            {
                Value = ints.ToList(),
                AssemblyQualifiedName = typeof(List<int>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<int[], Task>)MethodCall_SingleParam_List_Int).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue, int.MinValue, 1)]
        public Task MethodCall_SingleParam_Array_Int(params int[] ints)
        {
            var parameterData = new ParameterData
            {
                Value = ints,
                AssemblyQualifiedName = typeof(int[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<int[], Task>)MethodCall_SingleParam_Array_Int).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue, int.MinValue, 1)]
        public Task MethodCall_MultiParam_Int(params int[] parameters)
        {
            var parmeterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(int).AssemblyQualifiedName,
            });

            return ValidateMethodAsync(((Func<int[], Task>)MethodCall_MultiParam_Int).Method, parmeterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow((uint)0)]
        [DataRow((uint)1)]
        [DataRow(uint.MaxValue)]
        [DataRow(uint.MinValue)]
        public Task MethodCall_SingleParam_UInt(uint param)
        {
            var parameterData = new ParameterData
            {
                Value = param,
                AssemblyQualifiedName = typeof(uint).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<uint, Task>)MethodCall_SingleParam_UInt).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((uint)0, (uint)1, uint.MaxValue, uint.MinValue)]
        public Task MethodCall_SingleParam_List_UInt(params uint[] uints)
        {
            var parameterData = new ParameterData
            {
                Value = uints.ToList(),
                AssemblyQualifiedName = typeof(List<uint>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<uint[], Task>)MethodCall_SingleParam_List_UInt).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((uint)0, (uint)1, uint.MaxValue, uint.MinValue)]
        public Task MethodCall_SingleParam_Array_UInt(params uint[] uints)
        {
            var parameterData = new ParameterData
            {
                Value = uints,
                AssemblyQualifiedName = typeof(uint[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<uint[], Task>)MethodCall_SingleParam_Array_UInt).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow((uint)0, (uint)1, uint.MaxValue, uint.MinValue)]
        public Task MethodCall_MultiParam_UInt(params uint[] parameters)
        {
            var parameterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(uint).AssemblyQualifiedName,
            });

            return ValidateMethodAsync(((Func<uint[], Task>)MethodCall_MultiParam_UInt).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow((double)0)]
        [DataRow((double)1)]
        [DataRow((double)-1)]
        [DataRow(double.MaxValue)]
        [DataRow(double.MinValue)]
        [DataRow(double.NaN)]
        [DataRow(double.NegativeInfinity)]
        [DataRow(double.PositiveInfinity)]
        [DataRow(double.Epsilon)]
        public Task MethodCall_SingleParam_Double(double param)
        {
            var parameterData = new ParameterData()
            {
                Value = param,
                AssemblyQualifiedName = typeof(double).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<double, Task>)MethodCall_SingleParam_Double).Method, parameterData);
        }

        [TestMethod, Timeout(200000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((double)0, (double)1, (double)-1, double.MaxValue, double.MinValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity, double.Epsilon)]
        public Task MethodCall_SingleParam_List_Double(params double[] doubles)
        {
            var parameterData = new ParameterData
            {
                Value = doubles.ToList(),
                AssemblyQualifiedName = typeof(List<double>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<double[], Task>)MethodCall_SingleParam_List_Double).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((double)0, (double)1, (double)-1, double.MaxValue, double.MinValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity, double.Epsilon)]
        public Task MethodCall_SingleParam_Array_Double(params double[] doubles)
        {
            var parameterData = new ParameterData
            {
                Value = doubles,
                AssemblyQualifiedName = typeof(double[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<double[], Task>)MethodCall_SingleParam_Array_Double).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((double)0, (double)1, (double)-1, double.MaxValue, double.MinValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity, double.Epsilon)]
        public Task MethodCall_SingleParam_Dictionary_StringKey_DoubleVal(params double[] parameters)
        {
            var dict = new Dictionary<string, double>();

            foreach (var item in parameters)
                dict.Add(item.ToString(), item);

            var parameterData = new ParameterData
            {
                Value = dict,
                AssemblyQualifiedName = typeof(Dictionary<string, double>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<double[], Task>)MethodCall_SingleParam_Dictionary_StringKey_DoubleVal).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow((double)0, (double)1, (double)-1, double.MaxValue, double.MinValue, double.NaN, double.NegativeInfinity, double.PositiveInfinity, double.Epsilon)]
        public Task MethodCall_MultiParam_Double(params double[] parameters)
        {
            var parameterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(double).AssemblyQualifiedName
            });

            return ValidateMethodAsync(((Func<double[], Task>)MethodCall_MultiParam_Double).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow((float)0)]
        [DataRow((float)1)]
        [DataRow((float)-1)]
        [DataRow(float.MaxValue)]
        [DataRow(float.MinValue)]
        [DataRow(float.NaN)]
        [DataRow(float.NegativeInfinity)]
        [DataRow(float.PositiveInfinity)]
        [DataRow(float.Epsilon)]
        public Task MethodCall_SingleParam_Float(float param)
        {
            var parameterData = new ParameterData
            {
                Value = param,
                AssemblyQualifiedName = typeof(float).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<float, Task>)MethodCall_SingleParam_Float).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((float)0, (float)1, (float)-1, float.MaxValue, float.MinValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity, float.Epsilon)]
        public Task MethodCall_SingleParam_List_Float(params float[] floats)
        {
            var parameterData = new ParameterData
            {
                Value = floats.ToList(),
                AssemblyQualifiedName = typeof(List<float>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<float[], Task>)MethodCall_SingleParam_List_Float).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow((float)0, (float)1, (float)-1, float.MaxValue, float.MinValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity, float.Epsilon)]
        public Task MethodCall_SingleParam_Array_Float(params float[] floats)
        {
            var parameterData = new ParameterData
            {
                Value = floats,
                AssemblyQualifiedName = typeof(float[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<float[], Task>)MethodCall_SingleParam_Array_Float).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Primitives"), TestCategory("RemoteMethodCall")]
        [DataRow((float)0, (float)1, (float)-1, float.MaxValue, float.MinValue, float.NaN, float.NegativeInfinity, float.PositiveInfinity, float.Epsilon)]
        public Task MethodCall_MultiParam_Float(params float[] parameters)
        {
            var parameterData = parameters.Select(x => new ParameterData
            {
                Value = x,
                AssemblyQualifiedName = typeof(float).AssemblyQualifiedName
            });

            return ValidateMethodAsync(((Func<float[], Task>)MethodCall_MultiParam_Float).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Structs"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue)]
        [DataRow(int.MinValue)]
        [DataRow(0)]
        [DataRow(5000)]
        [DataRow(999999999)]
        public Task MethodCall_SingleParam_TimeSpan(int timeSpanMs)
        {
            var parameterData = new ParameterData
            {
                Value = TimeSpan.FromMilliseconds(timeSpanMs),
                AssemblyQualifiedName = typeof(TimeSpan).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<int, Task>)MethodCall_SingleParam_TimeSpan).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue, int.MinValue, 0, 5000, 999999999)]
        public Task MethodCall_SingleParam_List_TimeSpan(params int[] timeSpansMs)
        {
            var parameterData = new ParameterData
            {
                Value = timeSpansMs.Select(x => TimeSpan.FromMilliseconds(x)).ToList(),
                AssemblyQualifiedName = typeof(List<TimeSpan>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<int[], Task>)MethodCall_SingleParam_List_TimeSpan).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue, int.MinValue, 0, 5000, 999999999)]
        public Task MethodCall_SingleParam_Array_TimeSpan(params int[] timeSpansMs)
        {
            var parameterData = new ParameterData
            {
                Value = timeSpansMs.Select(x => TimeSpan.FromMilliseconds(x)).ToArray(),
                AssemblyQualifiedName = typeof(TimeSpan[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<int[], Task>)MethodCall_SingleParam_Array_TimeSpan).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Structs"), TestCategory("RemoteMethodCall")]
        [DataRow(int.MaxValue, int.MinValue, 0, 5000, 999999999)]
        public Task MethodCall_MultiParam_TimeSpan(params int[] timeSpansMs)
        {
            var parameterData = timeSpansMs.Select(x => new ParameterData
            {
                Value = TimeSpan.FromMilliseconds(x),
                AssemblyQualifiedName = typeof(TimeSpan).AssemblyQualifiedName,
            });

            return ValidateMethodAsync(((Func<int[], Task>)MethodCall_MultiParam_TimeSpan).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Structs"), TestCategory("RemoteMethodCall")]
        [DataRow("00000000-0000-0000-0000-000000000000")]
        [DataRow("DF2EE175-6303-4726-8FEB-53FB2FF9A870")]
        [DataRow("BF90552A-ACC8-46FE-9152-96E3B71B2D56")]
        [DataRow("21FB90A7-154A-4961-AE3E-2E4099E6FC83")]
        [DataRow("9A735EBC-6DCB-4D7D-87C4-4A5F588C4D89")]
        [DataRow("D32D71A3-989C-4D79-ABB9-255BF611801F")]
        [DataRow("A3EBBCF7-F06D-465C-86E3-A0B234075B77")]
        [DataRow("072F2045-F627-4C2B-816E-C1BA3EEFBEA2")]
        [DataRow("FAADFFE4-6CE3-4765-9D8C-78EC417BDB2D")]
        [DataRow("8D4D56F2-EBE7-4F9C-9D4E-5509D6525C2A")]
        [DataRow("FC295560-88EE-4BCA-8F78-F12A81CFBB5C")]
        [DataRow("C09BBBB0-9CE7-44A0-99D4-ED834B208607")]
        public Task MethodCall_SingleParam_Guid(string guidStr)
        {
            var parameterData = new ParameterData
            {
                Value = Guid.Parse(guidStr),
                AssemblyQualifiedName = typeof(Guid).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string, Task>)MethodCall_SingleParam_Guid).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(
            "00000000-0000-0000-0000-000000000000",
            "DF2EE175-6303-4726-8FEB-53FB2FF9A870",
            "BF90552A-ACC8-46FE-9152-96E3B71B2D56",
            "21FB90A7-154A-4961-AE3E-2E4099E6FC83",
            "9A735EBC-6DCB-4D7D-87C4-4A5F588C4D89",
            "D32D71A3-989C-4D79-ABB9-255BF611801F",
            "A3EBBCF7-F06D-465C-86E3-A0B234075B77",
            "072F2045-F627-4C2B-816E-C1BA3EEFBEA2",
            "FAADFFE4-6CE3-4765-9D8C-78EC417BDB2D",
            "8D4D56F2-EBE7-4F9C-9D4E-5509D6525C2A",
            "FC295560-88EE-4BCA-8F78-F12A81CFBB5C",
            "C09BBBB0-9CE7-44A0-99D4-ED834B208607"
        )]
        public Task MethodCall_SingleParam_List_Guid(params string[] guidStrs)
        {
            var parameterData = new ParameterData
            {
                Value = guidStrs.Select(x => Guid.Parse(x)).ToList(),
                AssemblyQualifiedName = typeof(List<Guid>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_SingleParam_List_Guid).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(
            "00000000-0000-0000-0000-000000000000",
            "DF2EE175-6303-4726-8FEB-53FB2FF9A870",
            "BF90552A-ACC8-46FE-9152-96E3B71B2D56",
            "21FB90A7-154A-4961-AE3E-2E4099E6FC83",
            "9A735EBC-6DCB-4D7D-87C4-4A5F588C4D89",
            "D32D71A3-989C-4D79-ABB9-255BF611801F",
            "A3EBBCF7-F06D-465C-86E3-A0B234075B77",
            "072F2045-F627-4C2B-816E-C1BA3EEFBEA2",
            "FAADFFE4-6CE3-4765-9D8C-78EC417BDB2D",
            "8D4D56F2-EBE7-4F9C-9D4E-5509D6525C2A",
            "FC295560-88EE-4BCA-8F78-F12A81CFBB5C",
            "C09BBBB0-9CE7-44A0-99D4-ED834B208607"
        )]
        public Task MethodCall_SingleParam_Array_Guid(params string[] guidStrs)
        {
            var parameterData = new ParameterData
            {
                Value = guidStrs.Select(x => Guid.Parse(x)).ToArray(),
                AssemblyQualifiedName = typeof(Guid[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_SingleParam_Array_Guid).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Structs"), TestCategory("RemoteMethodCall")]
        [DataRow(
            "00000000-0000-0000-0000-000000000000",
            "DF2EE175-6303-4726-8FEB-53FB2FF9A870",
            "BF90552A-ACC8-46FE-9152-96E3B71B2D56",
            "21FB90A7-154A-4961-AE3E-2E4099E6FC83",
            "9A735EBC-6DCB-4D7D-87C4-4A5F588C4D89",
            "D32D71A3-989C-4D79-ABB9-255BF611801F",
            "A3EBBCF7-F06D-465C-86E3-A0B234075B77",
            "072F2045-F627-4C2B-816E-C1BA3EEFBEA2",
            "FAADFFE4-6CE3-4765-9D8C-78EC417BDB2D",
            "8D4D56F2-EBE7-4F9C-9D4E-5509D6525C2A",
            "FC295560-88EE-4BCA-8F78-F12A81CFBB5C",
            "C09BBBB0-9CE7-44A0-99D4-ED834B208607"
        )]
        public Task MethodCall_MultiParam_Guid(params string[] guidStrs)
        {
            var parameterData = guidStrs.Select(x => new ParameterData
            {
                Value = Guid.Parse(x),
                AssemblyQualifiedName = typeof(Guid).AssemblyQualifiedName,
            });

            return ValidateMethodAsync(((Func<string, Task>)MethodCall_SingleParam_Guid).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Structs"), TestCategory("RemoteMethodCall")]
        [DataRow("12/31/9999 11:59:59 PM")]
        [DataRow("1/1/0001 12:00:00 AM")]
        [DataRow("10/7/2021 1:45:34 PM")]
        [DataRow("10/7/2021 12:00:00 AM")]
        public Task MethodCall_SingleParam_DateTime(string dateTimeStr)
        {
            var parameterData = new ParameterData
            {
                Value = DateTime.Parse(dateTimeStr),
                AssemblyQualifiedName = typeof(DateTime).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string, Task>)MethodCall_SingleParam_DateTime).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(
            "12/31/9999 11:59:59 PM",
            "1/1/0001 12:00:00 AM",
            "10/7/2021 1:45:34 PM",
            "10/7/2021 12:00:00 AM"
        )]
        public Task MethodCall_SingleParam_List_DateTime(params string[] dateTimeStrs)
        {
            var parameterData = new ParameterData
            {
                Value = dateTimeStrs.Select(x => DateTime.Parse(x)).ToList(),
                AssemblyQualifiedName = typeof(List<DateTime>).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_SingleParam_List_DateTime).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("Enumerables"), TestCategory("RemoteMethodCall")]
        [DataRow(
            "12/31/9999 11:59:59 PM",
            "1/1/0001 12:00:00 AM",
            "10/7/2021 1:45:34 PM",
            "10/7/2021 12:00:00 AM"
        )]
        public Task MethodCall_SingleParam_Array_DateTime(params string[] dateTimeStrs)
        {
            var parameterData = new ParameterData
            {
                Value = dateTimeStrs.Select(x => DateTime.Parse(x)).ToArray(),
                AssemblyQualifiedName = typeof(DateTime[]).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_SingleParam_Array_DateTime).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Structs"), TestCategory("RemoteMethodCall")]
        [DataRow(
            "12/31/9999 11:59:59 PM",
            "1/1/0001 12:00:00 AM",
            "10/7/2021 1:45:34 PM",
            "10/7/2021 12:00:00 AM"
        )]
        public Task MethodCall_MultiParam_DateTime(params string[] dateTimeStrs)
        {
            var parameterData = dateTimeStrs.Select(x => new ParameterData
            {
                Value = DateTime.Parse(x),
                AssemblyQualifiedName = typeof(DateTime).AssemblyQualifiedName
            });

            return ValidateMethodAsync(((Func<string[], Task>)MethodCall_MultiParam_DateTime).Method, parameterData);
        }

        [TestMethod, Timeout(2000)]
        [TestCategory("Objects"), TestCategory("RemoteMethodCall")]
        public Task MethodCall_SingleParam_ObjectInstance()
        {
            var parameterData = new ParameterData
            {
                Value = new AbstractUI.Models.AbstractButton("someId", "My button")
                {
                    Title = "Test title",
                },
                AssemblyQualifiedName = typeof(AbstractUI.Models.AbstractButton).AssemblyQualifiedName,
            };

            return ValidateMethodAsync(((Func<Task>)MethodCall_SingleParam_ObjectInstance).Method, parameterData);
        }

        private Task ValidateMethodAsync(MethodInfo methodInfo, ParameterData parameter)
        {
            return ValidateMethodAsync(methodInfo, parameter.IntoList());
        }

        private async Task ValidateMethodAsync(MethodInfo methodInfo, IEnumerable<ParameterData> parameters)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemoteMethodCallMessage(MEMBER_REMOTE_ID, MemberRemote.CreateMemberSignature(methodInfo, MemberSignatureScope.AssemblyQualifiedName), parameters.ToArray());
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedMethodCallMessage(deserializedMessage, originalMessage);
        }

        private void ValidateDeserializedMethodCallMessage(IRemoteMessage deserializedMessage, RemoteMethodCallMessage originalMessage)
        {
            Assert.IsNotNull(deserializedMessage);
            Assert.IsInstanceOfType(deserializedMessage, typeof(RemoteMethodCallMessage));

            var deserializedMethodCallMsg = (RemoteMethodCallMessage)deserializedMessage;

            var originalMessageParams = originalMessage.Parameters.ToList();
            var deserializedMessageParams = deserializedMethodCallMsg.Parameters.ToList();

            // Compare original msg with deserialized msg.
            Assert.AreEqual(originalMessage.MemberRemoteId, deserializedMethodCallMsg.MemberRemoteId);
            Assert.AreEqual(originalMessage.TargetMemberSignature, deserializedMethodCallMsg.TargetMemberSignature);
            Assert.AreEqual(originalMessage.Action, deserializedMethodCallMsg.Action);
            Assert.AreEqual(originalMessage.CustomActionName, deserializedMethodCallMsg.CustomActionName);
            Assert.AreEqual(originalMessageParams.Count, deserializedMessageParams.Count);

            // Compare params
            Assert.AreEqual(originalMessageParams.Count, deserializedMessageParams.Count);

            for (var i = 0; i < originalMessageParams.Count; i++)
            {
                var originalMsgParam = originalMessageParams[i];
                Assert.IsNotNull(originalMsgParam?.AssemblyQualifiedName);

                var deserMsgParam = deserializedMessageParams[i];
                Assert.IsNotNull(deserMsgParam?.AssemblyQualifiedName);

                var originalMsgParamType = Type.GetType(originalMsgParam.AssemblyQualifiedName);
                var deserMsgParamType = Type.GetType(deserMsgParam.AssemblyQualifiedName);

                Assert.IsNotNull(originalMsgParamType);
                Assert.IsNotNull(deserMsgParamType);
                Assert.AreEqual(originalMsgParam.AssemblyQualifiedName, deserMsgParam.AssemblyQualifiedName);

                Helpers.SmartAssertEqual(originalMsgParam.Value, originalMsgParamType, deserMsgParam.Value, deserMsgParamType);
            }
        }
    }
}
