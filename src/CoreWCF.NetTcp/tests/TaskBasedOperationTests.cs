﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using CoreWCF.Configuration;
using Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AsyncServices
{
    public class TaskBasedOperationTests
    {
        private readonly ITestOutputHelper _output;

        public TaskBasedOperationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task BufferedSynchronouslyCompletingTask()
        {
            string testString = new string('a', 3000);
            IWebHost host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                System.ServiceModel.ChannelFactory<Contract.ITaskService> factory = null;
                Contract.ITaskService channel = null;
                host.Start();
                try
                {
                    System.ServiceModel.NetTcpBinding binding = ClientHelper.GetBufferedModeBinding();
                    factory = new System.ServiceModel.ChannelFactory<Contract.ITaskService>(binding,
                        new System.ServiceModel.EndpointAddress(host.GetNetTcpAddressInUse() + Startup.BufferedRelatveAddress));
                    channel = factory.CreateChannel();
                    ((IChannel)channel).Open();
                    await channel.SynchronousCompletion();
                    ((IChannel)channel).Close();
                    factory.Close();
                }
                finally
                {
                    ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
                }
            }
        }

        [Fact]
        public async Task BufferedAsynchronouslyCompletingTask()
        {
            string testString = new string('a', 3000);
            IWebHost host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                System.ServiceModel.ChannelFactory<Contract.ITaskService> factory = null;
                Contract.ITaskService channel = null;
                host.Start();
                try
                {
                    System.ServiceModel.NetTcpBinding binding = ClientHelper.GetBufferedModeBinding();
                    factory = new System.ServiceModel.ChannelFactory<Contract.ITaskService>(binding,
                        new System.ServiceModel.EndpointAddress(host.GetNetTcpAddressInUse() + Startup.BufferedRelatveAddress));
                    channel = factory.CreateChannel();
                    ((IChannel)channel).Open();
                    await channel.AsynchronousCompletion();
                    ((IChannel)channel).Close();
                    factory.Close();
                }
                finally
                {
                    ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
                }
            }
        }

        [Fact]
        public async Task StreamedSynchronouslyCompletingTask()
        {
            string testString = new string('a', 3000);
            IWebHost host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                System.ServiceModel.ChannelFactory<Contract.ITaskService> factory = null;
                Contract.ITaskService channel = null;
                host.Start();
                try
                {
                    System.ServiceModel.NetTcpBinding binding = ClientHelper.GetStreamedModeBinding();
                    factory = new System.ServiceModel.ChannelFactory<Contract.ITaskService>(binding,
                        new System.ServiceModel.EndpointAddress(host.GetNetTcpAddressInUse() + Startup.StreamedRelatveAddress));
                    channel = factory.CreateChannel();
                    ((IChannel)channel).Open();
                    await channel.SynchronousCompletion();
                    ((IChannel)channel).Close();
                    factory.Close();
                }
                finally
                {
                    ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
                }
            }
        }

        [Fact]
        public async Task StreamedAsynchronouslyCompletingTask()
        {
            string testString = new string('a', 3000);
            IWebHost host = ServiceHelper.CreateWebHostBuilder<Startup>(_output).Build();
            using (host)
            {
                System.ServiceModel.ChannelFactory<Contract.ITaskService> factory = null;
                Contract.ITaskService channel = null;
                host.Start();
                try
                {
                    System.ServiceModel.NetTcpBinding binding = ClientHelper.GetStreamedModeBinding();
                    factory = new System.ServiceModel.ChannelFactory<Contract.ITaskService>(binding,
                        new System.ServiceModel.EndpointAddress(host.GetNetTcpAddressInUse() + Startup.StreamedRelatveAddress));
                    channel = factory.CreateChannel();
                    ((IChannel)channel).Open();
                    await channel.AsynchronousCompletion();
                    ((IChannel)channel).Close();
                    factory.Close();
                }
                finally
                {
                    ServiceHelper.CloseServiceModelObjects((IChannel)channel, factory);
                }
            }
        }
    }

    public class Startup
    {
        public const string BufferedRelatveAddress = "/nettcp.svc/Buffered";
        public const string StreamedRelatveAddress = "/nettcp.svc/Streamed";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddServiceModelServices();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseServiceModel(builder =>
            {
                builder.AddService<Services.TaskService>();
                builder.AddServiceEndpoint<Services.TaskService, Contract.ITaskService>(new CoreWCF.NetTcpBinding(CoreWCF.SecurityMode.None), BufferedRelatveAddress);
                builder.AddServiceEndpoint<Services.TaskService, Contract.ITaskService>(
                    new CoreWCF.NetTcpBinding(CoreWCF.SecurityMode.None)
                    {
                        TransferMode = CoreWCF.TransferMode.Streamed
                    }, StreamedRelatveAddress);
            });
        }
    }
}
