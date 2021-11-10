using Microsoft.VisualStudio.TestTools.UnitTesting;
using OwlCore.Remoting.Transfer;
using OwlCore.Remoting.Transfer.MessageConverters;
using System;
using System.Threading.Tasks;

namespace OwlCore.Tests.Remoting
{
    public partial class NewtonsoftRemoteMessageConverterTests
    {
        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        public async Task PropertyChange_NullObj()
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(object).AssemblyQualifiedName,  typeof(object).AssemblyQualifiedName, null, null);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(-1)]
        [DataRow(int.MaxValue)]
        [DataRow(int.MinValue)]
        public async Task PropertyChange_Int(int value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(int).AssemblyQualifiedName,  typeof(int).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow((uint)0)]
        [DataRow((uint)1)]
        [DataRow(uint.MaxValue)]
        [DataRow(uint.MinValue)]
        public async Task PropertyChange_UInt(uint value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(uint).AssemblyQualifiedName,  typeof(uint).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow((float)0)]
        [DataRow((float)1)]
        [DataRow((float)-1)]
        [DataRow(float.MaxValue)]
        [DataRow(float.MinValue)]
        [DataRow(float.NaN)]
        [DataRow(float.NegativeInfinity)]
        [DataRow(float.PositiveInfinity)]
        [DataRow(float.Epsilon)]
        public async Task PropertyChange_Float(float value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(float).AssemblyQualifiedName,  typeof(float).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow((double)0)]
        [DataRow((double)1)]
        [DataRow((double)-1)]
        [DataRow(double.MaxValue)]
        [DataRow(double.MinValue)]
        [DataRow(double.NaN)]
        [DataRow(double.NegativeInfinity)]
        [DataRow(double.PositiveInfinity)]
        [DataRow(double.Epsilon)]
        public async Task PropertyChange_Double(double value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(double).AssemblyQualifiedName,  typeof(double).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow((short)0)]
        [DataRow((short)1)]
        [DataRow((short)-1)]
        [DataRow(short.MaxValue)]
        [DataRow(short.MinValue)]
        public async Task PropertyChange_Short(short value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(short).AssemblyQualifiedName,  typeof(short).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow((ushort)0)]
        [DataRow((ushort)1)]
        [DataRow(ushort.MaxValue)]
        [DataRow(ushort.MinValue)]
        public async Task PropertyChange_UShort(ushort value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(ushort).AssemblyQualifiedName,  typeof(ushort).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow("")]
        [DataRow("sampleValue")]
        [DataRow("123")]
        [DataRow("sampleValue123")]
        [DataRow("!@#$%^&*()-=[]\\;',./?><\":{}|_+~`")]
        public async Task PropertyChange_String(string value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(string).AssemblyQualifiedName,  typeof(string).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow(byte.MaxValue)]
        [DataRow(byte.MinValue)]
        [DataRow((byte)1)]
        public async Task PropertyChange_Byte(byte value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(byte).AssemblyQualifiedName,  typeof(byte).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Primitives")]
        [DataRow(sbyte.MaxValue)]
        [DataRow(sbyte.MinValue)]
        [DataRow((sbyte)1)]
        public async Task PropertyChange_SByte(sbyte value)
        {
            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(sbyte).AssemblyQualifiedName,  typeof(sbyte).AssemblyQualifiedName, value, value);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Structs")]
        [DataRow(int.MaxValue)]
        [DataRow(int.MinValue)]
        [DataRow(0)]
        [DataRow(5000)]
        [DataRow(999999999)]
        public async Task PropertyChange_TimeSpan(int timeSpanMs)
        {
            var timeSpan = TimeSpan.FromMilliseconds(timeSpanMs);

            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(TimeSpan).AssemblyQualifiedName,  typeof(TimeSpan).AssemblyQualifiedName, timeSpan, timeSpan);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Structs")]
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
        public async Task PropertyChange_Guid(string guidStr)
        {
            var guid = Guid.Parse(guidStr);

            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(Guid).AssemblyQualifiedName,  typeof(Guid).AssemblyQualifiedName, guid, guid);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        [TestMethod, Timeout(150)]
        [TestCategory("Structs")]
        [DataRow("12/31/9999 11:59:59 PM")]
        [DataRow("1/1/0001 12:00:00 AM")]
        [DataRow("10/7/2021 1:45:34 PM")]
        [DataRow("10/7/2021 12:00:00 AM")]
        public async Task PropertyChange_DateTime(string dateTimeStr)
        {
            var dateTime = DateTime.Parse(dateTimeStr);

            var msgConverter = new NewtonsoftRemoteMessageConverter();

            var originalMessage = new RemotePropertyChangeMessage(MEMBER_REMOTE_ID, typeof(DateTime).AssemblyQualifiedName,  typeof(DateTime).AssemblyQualifiedName, dateTime, dateTime);
            var bytes = await msgConverter.SerializeAsync(originalMessage);

            Assert.IsNotNull(bytes);
            Assert.IsTrue(bytes.Length != 0);

            var deserializedMessage = await msgConverter.DeserializeAsync(bytes);

            ValidateDeserializedPropertyChangeMessage(deserializedMessage, originalMessage);
        }

        private void ValidateDeserializedPropertyChangeMessage(IRemoteMessage deserializedMessage, RemotePropertyChangeMessage originalMessage)
        {
            Assert.IsInstanceOfType(deserializedMessage, typeof(RemotePropertyChangeMessage));
            var deserializedPropertyChangedMessage = deserializedMessage as RemotePropertyChangeMessage;

            Assert.IsNotNull(deserializedPropertyChangedMessage);
            Assert.AreEqual(originalMessage.MemberRemoteId, deserializedPropertyChangedMessage.MemberRemoteId);
            Assert.AreEqual(originalMessage.TargetMemberSignature, deserializedPropertyChangedMessage.TargetMemberSignature);
            Assert.AreEqual(originalMessage.Action, deserializedPropertyChangedMessage.Action);
            Assert.AreEqual(originalMessage.CustomActionName, deserializedPropertyChangedMessage.CustomActionName);
            Assert.AreEqual(originalMessage.NewValue, deserializedPropertyChangedMessage.NewValue);
            Assert.AreEqual(originalMessage.OldValue, deserializedPropertyChangedMessage.OldValue);

            var originalType = Type.GetType(originalMessage.TargetMemberSignature);
            var deserializedType = Type.GetType(deserializedPropertyChangedMessage.TargetMemberSignature);

            Assert.IsNotNull(originalType);
            Assert.IsNotNull(deserializedType);

            Helpers.SmartAssertEqual(originalMessage.NewValue, originalType, deserializedPropertyChangedMessage.NewValue, deserializedType);
            Helpers.SmartAssertEqual(originalMessage.OldValue, originalType, deserializedPropertyChangedMessage.OldValue, deserializedType);
        }
    }
}
