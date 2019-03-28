// <copyright file="AuditEventType.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

using System.ComponentModel.DataAnnotations;

namespace Microsoft.Azure.Security.IoT.Agent.EventGenerators.Linux.Audit
{
    /// <summary>
    /// The types of auditd events that we know how to parse
    /// </summary>
    public enum AuditEventType
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        [Display(Name = "USER_AUTH")]
        UserAuth,
        [Display(Name = "USER_LOGIN")]
        UserLogin,
        [Display(Name = "PROCTITLE")]
        ProcessTitle,
        [Display(Name = "SOCKADDR")]
        ConnectSockaddr,
        [Display(Name = "SYSCALL")]
        ConnectSyscall,
        [Display(Name = "EXECVE")]
        ProcessExecution
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}