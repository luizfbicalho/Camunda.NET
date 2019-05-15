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

	using ModuleDependencyProcessor = org.camunda.bpm.container.impl.jboss.deployment.processor.ModuleDependencyProcessor;
	using ProcessApplicationDeploymentProcessor = org.camunda.bpm.container.impl.jboss.deployment.processor.ProcessApplicationDeploymentProcessor;
	using ProcessApplicationProcessor = org.camunda.bpm.container.impl.jboss.deployment.processor.ProcessApplicationProcessor;
	using ProcessEngineStartProcessor = org.camunda.bpm.container.impl.jboss.deployment.processor.ProcessEngineStartProcessor;
	using ProcessesXmlProcessor = org.camunda.bpm.container.impl.jboss.deployment.processor.ProcessesXmlProcessor;
	using MscBpmPlatformPlugins = org.camunda.bpm.container.impl.jboss.service.MscBpmPlatformPlugins;
	using MscRuntimeContainerDelegate = org.camunda.bpm.container.impl.jboss.service.MscRuntimeContainerDelegate;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using BpmPlatformPlugins = org.camunda.bpm.container.impl.plugin.BpmPlatformPlugins;
	using AbstractBoottimeAddStepHandler = org.jboss.@as.controller.AbstractBoottimeAddStepHandler;
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using ServiceVerificationHandler = org.jboss.@as.controller.ServiceVerificationHandler;
	using AbstractDeploymentChainStep = org.jboss.@as.server.AbstractDeploymentChainStep;
	using DeploymentProcessorTarget = org.jboss.@as.server.DeploymentProcessorTarget;
	using Phase = org.jboss.@as.server.deployment.Phase;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;


	/// <summary>
	/// Provides the description and the implementation of the subsystem#add operation.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class BpmPlatformSubsystemAdd : AbstractBoottimeAddStepHandler
	{

	  public static readonly BpmPlatformSubsystemAdd INSTANCE = new BpmPlatformSubsystemAdd();

	  private BpmPlatformSubsystemAdd()
	  {
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void populateModel(org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void populateModel(ModelNode operation, ModelNode model)
	  {
		model.get(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.PROCESS_ENGINES);
	  }

	  /// <summary>
	  /// {@inheritDoc} </summary>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performBoottime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model, org.jboss.as.controller.ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController< ? >> newControllers) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void performBoottime<T1>(OperationContext context, ModelNode operation, ModelNode model, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		// add deployment processors
		context.addStep(new AbstractDeploymentChainStepAnonymousInnerClass(this)
	   , OperationContext.Stage.RUNTIME);

		// create and register the MSC container delegate.
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.jboss.service.MscRuntimeContainerDelegate processEngineService = new org.camunda.bpm.container.impl.jboss.service.MscRuntimeContainerDelegate();
		MscRuntimeContainerDelegate processEngineService = new MscRuntimeContainerDelegate();

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.msc.service.ServiceController<org.camunda.bpm.container.impl.jboss.service.MscRuntimeContainerDelegate> controller = context.getServiceTarget().addService(org.camunda.bpm.container.impl.jboss.service.ServiceNames.forMscRuntimeContainerDelegate(), processEngineService).addListener(verificationHandler).setInitialMode(org.jboss.msc.service.ServiceController.Mode.ACTIVE).install();
		ServiceController<MscRuntimeContainerDelegate> controller = context.ServiceTarget.addService(ServiceNames.forMscRuntimeContainerDelegate(), processEngineService).addListener(verificationHandler).setInitialMode(ServiceController.Mode.ACTIVE).install();

		newControllers.Add(controller);

		// discover and register bpm platform plugins
		BpmPlatformPlugins plugins = BpmPlatformPlugins.load(this.GetType().ClassLoader);
		MscBpmPlatformPlugins managedPlugins = new MscBpmPlatformPlugins(plugins);

		ServiceController<BpmPlatformPlugins> serviceController = context.ServiceTarget.addService(ServiceNames.forBpmPlatformPlugins(), managedPlugins).addListener(verificationHandler).setInitialMode(ServiceController.Mode.ACTIVE).install();

		newControllers.Add(serviceController);
	  }

	  private class AbstractDeploymentChainStepAnonymousInnerClass : AbstractDeploymentChainStep
	  {
		  private readonly BpmPlatformSubsystemAdd outerInstance;

		  public AbstractDeploymentChainStepAnonymousInnerClass(BpmPlatformSubsystemAdd outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public void execute(DeploymentProcessorTarget processorTarget)
		  {
			processorTarget.addDeploymentProcessor(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, Phase.PARSE, ProcessApplicationProcessor.PRIORITY, new ProcessApplicationProcessor());
			processorTarget.addDeploymentProcessor(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, Phase.DEPENDENCIES, ModuleDependencyProcessor.PRIORITY, new ModuleDependencyProcessor());
			processorTarget.addDeploymentProcessor(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, Phase.POST_MODULE, ProcessesXmlProcessor.PRIORITY, new ProcessesXmlProcessor());
			processorTarget.addDeploymentProcessor(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, Phase.INSTALL, ProcessEngineStartProcessor.PRIORITY, new ProcessEngineStartProcessor());
			processorTarget.addDeploymentProcessor(org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.SUBSYSTEM_NAME, Phase.INSTALL, ProcessApplicationDeploymentProcessor.PRIORITY, new ProcessApplicationDeploymentProcessor());
		  }
	  }

	}

}