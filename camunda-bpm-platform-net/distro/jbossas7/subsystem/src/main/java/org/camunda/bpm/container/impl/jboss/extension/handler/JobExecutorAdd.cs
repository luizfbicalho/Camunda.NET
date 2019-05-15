using System.Collections.Generic;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.THREAD_POOL_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.ADD;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.DESCRIPTION;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.OPERATION_NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.REQUEST_PROPERTIES;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.REQUIRED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.TYPE;


	using MscExecutorService = org.camunda.bpm.container.impl.jboss.service.MscExecutorService;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using AbstractAddStepHandler = org.jboss.@as.controller.AbstractAddStepHandler;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using ServiceVerificationHandler = org.jboss.@as.controller.ServiceVerificationHandler;
	using DescriptionProvider = org.jboss.@as.controller.descriptions.DescriptionProvider;
	using ManagedQueueExecutorService = org.jboss.@as.threads.ManagedQueueExecutorService;
	using ThreadsServices = org.jboss.@as.threads.ThreadsServices;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ModelType = org.jboss.dmr.ModelType;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;


	/// <summary>
	/// Installs the JobExecutor service into the container.
	/// 
	/// </summary>
	public class JobExecutorAdd : AbstractAddStepHandler, DescriptionProvider
	{

	  public static readonly JobExecutorAdd INSTANCE = new JobExecutorAdd();

	  public virtual ModelNode getModelDescription(Locale locale)
	  {
		ModelNode node = new ModelNode();
		node.get(DESCRIPTION).set("Adds a job executor");
		node.get(OPERATION_NAME).set(ADD);

		node.get(REQUEST_PROPERTIES, THREAD_POOL_NAME, DESCRIPTION).set("Thread pool name for global job executor");
		node.get(REQUEST_PROPERTIES, THREAD_POOL_NAME, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, THREAD_POOL_NAME, REQUIRED).set(true);

		return node;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void populateModel(org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void populateModel(ModelNode operation, ModelNode model)
	  {
		foreach (AttributeDefinition attr in SubsystemAttributeDefinitons.JOB_EXECUTOR_ATTRIBUTES)
		{
		  attr.validateAndSet(operation, model);
		}
	  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performRuntime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model, org.jboss.as.controller.ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController<?>> newControllers) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void performRuntime<T1>(OperationContext context, ModelNode operation, ModelNode model, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		if (!operation.hasDefined(THREAD_POOL_NAME))
		{
		  throw new ProcessEngineException("Unable to configure threadpool for ContainerJobExecutorService, missing element '" + THREAD_POOL_NAME + "' in JobExecutor configuration.");
		}

		string jobExecutorThreadPoolName = SubsystemAttributeDefinitons.THREAD_POOL_NAME.resolveModelAttribute(context, model).asString();

		MscExecutorService service = new MscExecutorService();
		ServiceController<MscExecutorService> serviceController = context.ServiceTarget.addService(ServiceNames.forMscExecutorService(), service).addDependency(ThreadsServices.EXECUTOR.append(jobExecutorThreadPoolName), typeof(ManagedQueueExecutorService), service.ManagedQueueInjector).addListener(verificationHandler).setInitialMode(ServiceController.Mode.ACTIVE).install();

		newControllers.Add(serviceController);

	  }

	}

}