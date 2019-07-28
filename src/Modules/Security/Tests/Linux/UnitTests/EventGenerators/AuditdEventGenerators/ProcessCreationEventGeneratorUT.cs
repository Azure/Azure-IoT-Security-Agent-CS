// <copyright file="ProcessCreationEventGeneratorUT.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using Agent.Tests.Linux.UnitTests.EventGenerators.AuditdEventGenerators;
using Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Microsoft.Azure.Security.IoT.Contracts.Events.Events;
using Microsoft.Azure.IoT.Agent.Core.Utils;
using Security.Tests.Common.Helpers;

namespace Tests.Linux.UnitTests.EventGenerators.AuditdEventGenerators
{
    /// <summary>
    /// Unit tests for process event generator
    /// </summary>
    [TestClass]
    public class ProcessCreationEventGeneratorUT : AuditdUnitTestBase
    {
        private const string ProcessEvent = "----\r\ntime->Fri Dec 21 13:22:32 2018\r\ntype=PROCTITLE msg=audit(1545391352.695:3947714): proctitle=2F62696E2F62617368002D6300636174202F6574632F706173737764\r\ntype=PATH msg=audit(1545391352.695:3947714): item=1 name=\" / lib64 / ld) - linux - x86 - 64.so.2\" inode=922959 dev=08:01 mode=0100755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL cap_fp=0000000000000000 cap_fi=0000000000000000 cap_fe=0 cap_fver=0\r\ntype=PATH msg=audit(1545391352.695:3947714): item=0 name=\"/bin/cat\" inode=2883608 dev=08:01 mode=0100755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL cap_fp=0000000000000000 cap_fi=0000000000000000 cap_fe=0 cap_fver=0\r\ntype=CWD msg=audit(1545391352.695:3947714): cwd=\"/home/kfir/dev/Rome-IoT-DeviceAgent/src/Agent/Linux/bin/Debug/netcoreapp2.0/ubuntu.16.04-x64\"\r\ntype=EXECVE msg=audit(1545391352.695:3947714): argc=2 a0=\"cat\" a1=\"/etc/passwd\"\r\ntype=SYSCALL msg=audit(1545391352.695:3947714): arch=c000003e syscall=59 success=yes exit=0 a0=1b11268 a1=1b11388 a2=1b10c08 a3=598 items=2 ppid=10227 pid=19552 auid=4294967295 uid=1000 gid=1000 euid=1000 suid=1000 fsuid=1000 egid=1000 sgid=1000 fsgid=1000 tty=pts4 ses=4294967295 comm=\"cat\" exe=\"/bin/cat\" key=(null)\r\n----\r\ntime->Fri Dec 21 13:22:32 2018\r\ntype=PROCTITLE msg=audit(1545391352.719:3947715): proctitle=2F62696E2F62617368002D63007375646F206175736561726368202D6D20455845435645202D2D696E7075742D6C6F677320202D2D636865636B706F696E74202F7661722F746D702F50726F636573734372656174696F6E4576656E7447656E657261746F72436865636B706F696E74\r\ntype=PATH msg=audit(1545391352.719:3947715): item=1 name=\"/lib64/ld-linux-x86-64.so.2\" inode=922959 dev=08:01 mode=0100755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL cap_fp=0000000000000000 cap_fi=0000000000000000 cap_fe=0 cap_fver=0\r\ntype=PATH msg=audit(1545391352.719:3947715): item=0 name=\"/bin/bash\" inode=2883591 dev=08:01 mode=0100755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL cap_fp=0000000000000000 cap_fi=0000000000000000 cap_fe=0 cap_fver=0\r\ntype=CWD msg=audit(1545391352.719:3947715): cwd=\"/home/kfir/dev/Rome-IoT-DeviceAgent/src/Agent/Linux/bin/Debug/netcoreapp2.0/ubuntu.16.04-x64\"\r\ntype=EXECVE msg=audit(1545391352.719:3947715): argc=3 a0=\"/bin/bash\" a1=\"-c\" a2=7375646F206175736561726368202D6D20455845435645202D2D696E7075742D6C6F677320202D2D636865636B706F696E74202F7661722F746D702F50726F636573734372656174696F6E4576656E7447656E657261746F72436865636B706F696E74\r\ntype=SYSCALL msg=audit(1545391352.719:3947715): arch=c000003e syscall=59 success=yes exit=0 a0=7ffd28820990 a1=1b71b60 a2=1b73cb0 a3=598 items=2 ppid=10227 pid=19553 auid=4294967295 uid=1000 gid=1000 euid=1000 suid=1000 fsuid=1000 egid=1000 sgid=1000 fsgid=1000 tty=pts4 ses=4294967295 comm=\"bash\" exe=\"/bin/bash\" key=(null)\r\n";

        private const string ManyArgumentsEvent = "----\r\ntime->Thu Dec 27 11:51:04 2018\r\ntype=PROCTITLE msg=audit(1545904264.116:136899772): proctitle=7375646F006C73002D61002D2D617574686F72002D62002D42002D63002D6C74002D43002D64002D44002D46002D66002D67002D68\r\ntype=PATH msg=audit(1545904264.116:136899772): item=1 name=\"/lib64/ld-linux-x86-64.so.2\" inode=9572672 dev=08:01 mode=0100755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL\r\ntype=PATH msg=audit(1545904264.116:136899772): item=0 name=\"/usr/bin/sudo\" inode=50464143 dev=08:01 mode=0104755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL\r\ntype=CWD msg=audit(1545904264.116:136899772):  cwd=\"/home/agent\"\r\ntype=EXECVE msg=audit(1545904264.116:136899772): argc=15 a0=\"sudo\" a1=\"ls\" a2=\"-a\" a3=\"--author\" a4=\"-b\" a5=\"-B\" a6=\"-c\" a7=\"-lt\" a8=\"-C\" a9=\"-d\" a10=\"-D\" a11=\"-F\" a12=\"-f\" a13=\"-g\" a14=\"-h\"\r\ntype=BPRM_FCAPS msg=audit(1545904264.116:136899772): fver=0 fp=0000000000000000 fi=0000000000000000 fe=0 old_pp=0000000000000000 old_pi=0000000000000000 old_pe=0000000000000000 new_pp=0000003fffffffff new_pi=0000000000000000 new_pe=0000003fffffffff\r\ntype=SYSCALL msg=audit(1545904264.116:136899772): arch=c000003e syscall=59 success=yes exit=0 a0=1a67d88 a1=1ba8908 a2=1b2e008 a3=598 items=2 ppid=27103 pid=29229 auid=1001 uid=1001 gid=1001 euid=0 suid=0 fsuid=0 egid=1001 sgid=1001 fsgid=1001 tty=pts10 ses=83115 comm=\"sudo\" exe=\"/usr/bin/sudo\" key=(null)";

        private const string EncodedProcName = "time->Tue Jan 29 15:40:19 2019 type=PROCTITLE msg=audit(1548776419.703:2143054): proctitle=2F686F6D652F6B6669722F6465762F73706163652020202020 type=PATH msg=audit(1548776419.703:2143054): item=1 name=\"/lib64/ld-linux-x86-64.so.2\" inode=786449 dev=08:01 mode=0100755 ouid=0 ogid=0 rdev=00:00 nametype=NORMAL type=PATH msg=audit(1548776419.703:2143054): item=0 name=2F686F6D652F6B6669722F6465762F73706163652020202020 inode=1180871 dev=08:01 mode=0100755 ouid=1000 ogid=1000 rdev=00:00 nametype=NORMAL type=CWD msg=audit(1548776419.703:2143054): cwd=\"/home/kfir/bb2/Install\" type=EXECVE msg=audit(1548776419.703:2143054): argc=1 a0=2F686F6D652F6B6669722F6465762F73706163652020202020 type=SYSCALL msg=audit(1548776419.703:2143054): arch=c000003e syscall=59 success=yes exit=0 a0=aa3ec8 a1=aa3f88 a2=a53008 a3=59a items=2 ppid=51661 pid=52362 auid=1000 uid=1000 gid=1000 euid=1000 suid=1000 fsuid=1000 egid=1000 sgid=1000 fsgid=1000 tty=pts0 ses=156 comm=73706163652020202020 exe=2F686F6D652F6B6669722F6465762F73706163652020202020 key=(null)";

        /// <inheritdoc />
        public override AuditEventGeneratorBase CreateInstance(IProcessUtil processUtil)
        {
            return new ProcessCreationEventGenerator(processUtil);
        }

        /// <summary>
        /// Verifies process events are generated correctly
        /// </summary>
        [TestMethod]
        public void TestProcessEventParsing()
        {
            SetupAusearchReturnValue(ProcessEvent);

            var events = GeneratorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(2, events.Count());

            var processEvent = (ProcessCreate)events.ToList()[0];
            var processCreationPayload = processEvent.Payload.First();

            Assert.AreEqual(false, processEvent.IsEmpty);
            Assert.AreEqual("cat /etc/passwd", processCreationPayload.CommandLine);
            Assert.AreEqual("/bin/cat", processCreationPayload.Executable);
            Assert.AreEqual((uint)19552, processCreationPayload.ProcessId);
            Assert.AreEqual((uint)10227, processCreationPayload.ParentProcessId);
            Assert.AreEqual("1000", processCreationPayload.UserId);
            Assert.AreEqual(2018, processCreationPayload.Time.Year);
            Assert.AreEqual(12, processCreationPayload.Time.Month);
            Assert.AreEqual(21, processCreationPayload.Time.Day);
            Assert.AreEqual(11, processCreationPayload.Time.Hour);
            Assert.AreEqual(22, processCreationPayload.Time.Minute);
            Assert.AreEqual(32, processCreationPayload.Time.Second);

            processEvent = (ProcessCreate)events.ToList()[1];
            processCreationPayload = processEvent.Payload.First();

            Assert.AreEqual(false, processEvent.IsEmpty);
            Assert.AreEqual("/bin/bash -c sudo ausearch -m EXECVE --input-logs  --checkpoint /var/tmp/ProcessCreationEventGeneratorCheckpoint", processCreationPayload.CommandLine);
            Assert.AreEqual("/bin/bash", processCreationPayload.Executable);
            Assert.AreEqual((uint)19553, processCreationPayload.ProcessId);
            Assert.AreEqual((uint)10227, processCreationPayload.ParentProcessId);
            Assert.AreEqual("1000", processCreationPayload.UserId);
            Assert.AreEqual(2018, processCreationPayload.Time.Year);
            Assert.AreEqual(12, processCreationPayload.Time.Month);
            Assert.AreEqual(21, processCreationPayload.Time.Day);
            Assert.AreEqual(11, processCreationPayload.Time.Hour);
            Assert.AreEqual(22, processCreationPayload.Time.Minute);
            Assert.AreEqual(32, processCreationPayload.Time.Second);

            MockedShell.VerifyAll();
        }

        /// <summary>
        /// Verifies process eventwith many arguments is processes correctly
        /// </summary>
        [TestMethod]
        public void TestManyArgumentProcessEventParsing()
        {
            SetupAusearchReturnValue(ManyArgumentsEvent);

            var events = GeneratorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(1, events.Count());

            var processEvent = (ProcessCreate)events.ToList()[0];
            var processCreationPayload = processEvent.Payload.First();

            Assert.AreEqual(false, processEvent.IsEmpty);
            Assert.AreEqual("sudo ls -a --author -b -B -c -lt -C -d -D -F -f -g -h", processCreationPayload.CommandLine);
            Assert.AreEqual("/usr/bin/sudo", processCreationPayload.Executable);
            Assert.AreEqual((uint)29229, processCreationPayload.ProcessId);
            Assert.AreEqual((uint)27103, processCreationPayload.ParentProcessId);
            Assert.AreEqual("1001", processCreationPayload.UserId);
            Assert.AreEqual(2018, processCreationPayload.Time.Year);
            Assert.AreEqual(12, processCreationPayload.Time.Month);
            Assert.AreEqual(27, processCreationPayload.Time.Day);
            Assert.AreEqual(9, processCreationPayload.Time.Hour);
            Assert.AreEqual(51, processCreationPayload.Time.Minute);
            Assert.AreEqual(4, processCreationPayload.Time.Second);

            MockedShell.VerifyAll();
        }

        /// <summary>
        /// Verifies process eventwith many arguments is processes correctly
        /// </summary>
        [TestMethod]
        public void TestEncodedExecutableName()
        {
            SetupAusearchReturnValue(EncodedProcName);

            var events = GeneratorUnderTest.GetEvents();
            events.ValidateSchema();

            Assert.AreEqual(1, events.Count());

            var processEvent = (ProcessCreate)events.ToList()[0];
            var processCreationPayload = processEvent.Payload.First();

            Assert.AreEqual(false, processEvent.IsEmpty);
            Assert.AreEqual("/home/kfir/dev/space     ", processCreationPayload.CommandLine);
            Assert.AreEqual("/home/kfir/dev/space     ", processCreationPayload.Executable);
            Assert.AreEqual((uint)52362, processCreationPayload.ProcessId);
            Assert.AreEqual((uint)51661, processCreationPayload.ParentProcessId);
            Assert.AreEqual("1000", processCreationPayload.UserId);
            Assert.AreEqual(2019, processCreationPayload.Time.Year);
            Assert.AreEqual(1, processCreationPayload.Time.Month);
            Assert.AreEqual(29, processCreationPayload.Time.Day);
            Assert.AreEqual(15, processCreationPayload.Time.Hour);
            Assert.AreEqual(40, processCreationPayload.Time.Minute);
            Assert.AreEqual(19, processCreationPayload.Time.Second);

            MockedShell.VerifyAll();
        }
    }
}
