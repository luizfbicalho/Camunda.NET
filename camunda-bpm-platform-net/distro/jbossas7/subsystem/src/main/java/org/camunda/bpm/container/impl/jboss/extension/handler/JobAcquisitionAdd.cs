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
//	import static org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.ACQUISITION_STRATEGY;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.NAME;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.container.impl.jboss.extension.ModelConstants_Fields.PROPERTIES;
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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.jboss.@as.controller.descriptions.ModelDescriptionConstants.VALUE_TYPE;


	using MscRuntimeContainerJobExecutor = org.camunda.bpm.container.impl.jboss.service.MscRuntimeContainerJobExecutor;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using RuntimeContainerJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.RuntimeContainerJobExecutor;
	using AbstractAddStepHandler = org.jboss.@as.controller.AbstractAddStepHandler;
	using AttributeDefinition = org.jboss.@as.controller.AttributeDefinition;
	using OperationContext = org.jboss.@as.controller.OperationContext;
	using OperationFailedException = org.jboss.@as.controller.OperationFailedException;
	using PathAddress = org.jboss.@as.controller.PathAddress;
	using ServiceVerificationHandler = org.jboss.@as.controller.ServiceVerificationHandler;
	using DescriptionProvider = org.jboss.@as.controller.descriptions.DescriptionProvider;
	using ModelDescriptionConstants = org.jboss.@as.controller.descriptions.ModelDescriptionConstants;
	using ModelNode = org.jboss.dmr.ModelNode;
	using ModelType = org.jboss.dmr.ModelType;
	using Property = org.jboss.dmr.Property;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;


	/// <summary>
	/// Provides the description and the implementation of the job-acquisition#add operation.
	/// 
	/// </summary>
	public class JobAcquisitionAdd : AbstractAddStepHandler, DescriptionProvider
	{

	  public static readonly JobAcquisitionAdd INSTANCE = new JobAcquisitionAdd();

	  public virtual ModelNode getModelDescription(Locale locale)
	  {
		ModelNode node = new ModelNode();
		node.get(DESCRIPTION).set("Adds a job acquisition");
		node.get(OPERATION_NAME).set(ADD);

		node.get(REQUEST_PROPERTIES, NAME, DESCRIPTION).set("Name of job acquisition thread");
		node.get(REQUEST_PROPERTIES, NAME, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, NAME, REQUIRED).set(true);

		node.get(REQUEST_PROPERTIES, ACQUISITION_STRATEGY, DESCRIPTION).set("Job acquisition strategy");
		node.get(REQUEST_PROPERTIES, ACQUISITION_STRATEGY, TYPE).set(ModelType.STRING);
		node.get(REQUEST_PROPERTIES, ACQUISITION_STRATEGY, REQUIRED).set(false);

		node.get(REQUEST_PROPERTIES, PROPERTIES, DESCRIPTION).set("Additional properties");
		node.get(REQUEST_PROPERTIES, PROPERTIES, TYPE).set(ModelType.OBJECT);
		node.get(REQUEST_PROPERTIES, PROPERTIES, VALUE_TYPE).set(ModelType.LIST);
		node.get(REQUEST_PROPERTIES, PROPERTIES, REQUIRED).set(false);

		return node;
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void populateModel(org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void populateModel(ModelNode operation, ModelNode model)
	  {
		foreach (AttributeDefinition attr in SubsystemAttributeDefinitons.JOB_ACQUISITION_ATTRIBUTES)
		{
		  attr.validateAndSet(operation, model);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performRuntime(org.jboss.as.controller.OperationContext context, org.jboss.dmr.ModelNode operation, org.jboss.dmr.ModelNode model, org.jboss.as.controller.ServiceVerificationHandler verificationHandler, java.util.List<org.jboss.msc.service.ServiceController<?>> newControllers) throws org.jboss.as.controller.OperationFailedException
	  protected internal override void performRuntime<T1>(OperationContext context, ModelNode operation, ModelNode model, ServiceVerificationHandler verificationHandler, IList<T1> newControllers)
	  {

		string acquisitionName = PathAddress.pathAddress(operation.get(ModelDescriptionConstants.ADDRESS)).LastElement.Value;

		MscRuntimeContainerJobExecutor mscRuntimeContainerJobExecutor = new MscRuntimeContainerJobExecutor();

		if (model.hasDefined(PROPERTIES))
		{

		  IList<Property> properties = SubsystemAttributeDefinitons.PROPERTIES.resolveModelAttribute(context, model).asPropertyList();

		  foreach (Property property in properties)
		  {
			string name = property.Name;
			string value = property.Value.asString();
			PropertyHelper.applyProperty(mscRuntimeContainerJobExecutor, name, value);
		  }

		}

		// start new service for job executor
		ServiceController<RuntimeContainerJobExecutor> serviceController = context.ServiceTarget.addService(ServiceNames.forMscRuntimeContainerJobExecutorService(acquisitionName), mscRuntimeContainerJobExecutor).addDependency(ServiceNames.forMscRuntimeContainerDelegate()).addDependency(ServiceNames.forMscExecutorService()).addListener(verificationHandler).setInitialMode(ServiceController.Mode.ACTIVE).install();

		newControllers.Add(serviceController);

	  }

	}

}