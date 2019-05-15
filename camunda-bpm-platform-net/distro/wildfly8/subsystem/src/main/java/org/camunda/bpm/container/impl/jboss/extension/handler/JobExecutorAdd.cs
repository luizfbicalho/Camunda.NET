﻿using System.Collections.Generic;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.container.impl.jboss.extension.handler
{
	using MscExecutorService = org.camunda.bpm.container.impl.jboss.service.MscExecutorService;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using AbstractAddStepHandler = org.jboss.@as.controller.AbstractAddStepHandler;
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using ServiceVerificationHandler = org.jboss.@as.controller.ServiceVerificationHandler;
	using BoundedQueueThreadPoolService = org.jboss.@as.threads.BoundedQueueThreadPoolService;
	using ManagedQueueExecutorService = org.jboss.@as.threads.ManagedQueueExecutorService;
	using ThreadFactoryService = org.jboss.@as.threads.ThreadFactoryService;
	using TimeSpec = org.jboss.@as.threads.TimeSpec;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ServiceBuilder = org.jboss.msc.service.ServiceBuilder;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;
	using ServiceTarget = org.jboss.msc.service.ServiceTarget;



	/// <summary>
	/// Installs the JobExecutor service into the container.
	/// 
	/// @author Christian Lipphardt
	/// </summary>
	public class JobExecutorAdd : AbstractAddStepHandler
	{

	  public const string THREAD_POOL_GRP_NAME = "Camunda BPM ";

	  public static readonly JobExecutorAdd INSTANCE = new JobExecutorAdd();

	  private JobExecutorAdd() : base(SubsystemAttributeDefinitons.JOB_EXECUTOR_ATTRIBUTES)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performRuntime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model, org.jboss.as.controller.ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController<?>> newControllers) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void performRuntime<T1>(OperationContext context, ModelNode operation, ModelNode model, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		string jobExecutorThreadPoolName = SubsystemAttributeDefinitons.THREAD_POOL_NAME.resolveModelAttribute(context, model).asString();
		ServiceName jobExecutorThreadPoolServiceName = ServiceNames.forManagedThreadPool(jobExecutorThreadPoolName);

		performRuntimeThreadPool(context, model, jobExecutorThreadPoolName, jobExecutorThreadPoolServiceName, verificationHandler, newControllers);

		MscExecutorService service = new MscExecutorService();
		ServiceController<MscExecutorService> serviceController = context.ServiceTarget.addService(ServiceNames.forMscExecutorService(), service).addDependency(jobExecutorThreadPoolServiceName, typeof(ManagedQueueExecutorService), service.ManagedQueueInjector).addListener(verificationHandler).setInitialMode(ServiceController.Mode.ACTIVE).install();

		newControllers.Add(serviceController);

	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: protected void performRuntimeThreadPool(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode model, String name, org.jboss.msc.service.ServiceName jobExecutorThreadPoolServiceName, org.jboss.as.controller.ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController<?>> newControllers) throws org.jboss.as.controller.OperationFailedException
	  protected internal virtual void performRuntimeThreadPool<T1>(OperationContext context, ModelNode model, string name, ServiceName jobExecutorThreadPoolServiceName, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		ServiceTarget serviceTarget = context.ServiceTarget;

		ThreadFactoryService threadFactory = new ThreadFactoryService();
		threadFactory.ThreadGroupName = THREAD_POOL_GRP_NAME + name;

		ServiceName threadFactoryServiceName = ServiceNames.forThreadFactoryService(name);

		ServiceBuilder<ThreadFactory> factoryBuilder = serviceTarget.addService(threadFactoryServiceName, threadFactory);
		if (verificationHandler != null)
		{
		  factoryBuilder.addListener(verificationHandler);
		}
		if (newControllers != null)
		{
		  newControllers.Add(factoryBuilder.install());
		}
		else
		{
		  factoryBuilder.install();
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.threads.BoundedQueueThreadPoolService threadPoolService = new org.jboss.as.threads.BoundedQueueThreadPoolService(org.camunda.bpm.container.impl.jboss.extension.SubsystemAttributeDefinitons.CORE_THREADS.resolveModelAttribute(context, model).asInt(), org.camunda.bpm.container.impl.jboss.extension.SubsystemAttributeDefinitons.MAX_THREADS.resolveModelAttribute(context, model).asInt(), org.camunda.bpm.container.impl.jboss.extension.SubsystemAttributeDefinitons.QUEUE_LENGTH.resolveModelAttribute(context, model).asInt(), false, new org.jboss.as.threads.TimeSpec(java.util.concurrent.TimeUnit.SECONDS, org.camunda.bpm.container.impl.jboss.extension.SubsystemAttributeDefinitons.KEEPALIVE_TIME.resolveModelAttribute(context,model).asInt()), org.camunda.bpm.container.impl.jboss.extension.SubsystemAttributeDefinitons.ALLOW_CORE_TIMEOUT.resolveModelAttribute(context, model).asBoolean());
		BoundedQueueThreadPoolService threadPoolService = new BoundedQueueThreadPoolService(SubsystemAttributeDefinitons.CORE_THREADS.resolveModelAttribute(context, model).asInt(), SubsystemAttributeDefinitons.MAX_THREADS.resolveModelAttribute(context, model).asInt(), SubsystemAttributeDefinitons.QUEUE_LENGTH.resolveModelAttribute(context, model).asInt(), false, new TimeSpec(TimeUnit.SECONDS, SubsystemAttributeDefinitons.KEEPALIVE_TIME.resolveModelAttribute(context,model).asInt()), SubsystemAttributeDefinitons.ALLOW_CORE_TIMEOUT.resolveModelAttribute(context, model).asBoolean());

		ServiceBuilder<ManagedQueueExecutorService> builder = serviceTarget.addService(jobExecutorThreadPoolServiceName, threadPoolService).addDependency(threadFactoryServiceName, typeof(ThreadFactory), threadPoolService.ThreadFactoryInjector).setInitialMode(ServiceController.Mode.ACTIVE);
		if (verificationHandler != null)
		{
		  builder.addListener(verificationHandler);
		}
		if (newControllers != null)
		{
		  newControllers.Add(builder.install());
		}
		else
		{
		  builder.install();
		}
	  }

	}

}