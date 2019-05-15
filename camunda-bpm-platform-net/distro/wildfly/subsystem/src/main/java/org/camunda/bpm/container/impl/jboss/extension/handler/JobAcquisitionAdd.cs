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
	using MscRuntimeContainerJobExecutor = org.camunda.bpm.container.impl.jboss.service.MscRuntimeContainerJobExecutor;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using PropertyHelper = org.camunda.bpm.container.impl.metadata.PropertyHelper;
	using RuntimeContainerJobExecutor = org.camunda.bpm.engine.impl.jobexecutor.RuntimeContainerJobExecutor;
	using org.jboss.@as.controller;
	using ModelDescriptionConstants = org.jboss.@as.controller.descriptions.ModelDescriptionConstants;
	using ModelNode = org.jboss.dmr.ModelNode;
	using Property = org.jboss.dmr.Property;
	using ServiceController = org.jboss.msc.service.ServiceController;
	using Mode = org.jboss.msc.service.ServiceController.Mode;


	/// <summary>
	/// Provides the description and the implementation of the job-acquisition#add operation.
	/// 
	/// @author Christian Lipphardt
	/// </summary>
	public class JobAcquisitionAdd : AbstractAddStepHandler
	{

	  public static readonly JobAcquisitionAdd INSTANCE = new JobAcquisitionAdd();

	  private JobAcquisitionAdd() : base(SubsystemAttributeDefinitons.JOB_ACQUISITION_ATTRIBUTES)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override protected void performRuntime(final OperationContext context, final org.jboss.dmr.ModelNode operation, final org.jboss.dmr.ModelNode model) throws OperationFailedException
//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
	  protected internal override void performRuntime(OperationContext context, ModelNode operation, ModelNode model)
	  {

		string acquisitionName = PathAddress.pathAddress(operation.get(ModelDescriptionConstants.ADDRESS)).LastElement.Value;

		MscRuntimeContainerJobExecutor mscRuntimeContainerJobExecutor = new MscRuntimeContainerJobExecutor();

		if (model.hasDefined(SubsystemAttributeDefinitons.PROPERTIES.Name))
		{
		  IList<Property> properties = SubsystemAttributeDefinitons.PROPERTIES.resolveModelAttribute(context, model).asPropertyList();
		  foreach (Property property in properties)
		  {
			PropertyHelper.applyProperty(mscRuntimeContainerJobExecutor, property.Name, property.Value.asString());
		  }
		}

		// start new service for job executor
		ServiceController<RuntimeContainerJobExecutor> serviceController = context.ServiceTarget.addService(ServiceNames.forMscRuntimeContainerJobExecutorService(acquisitionName), mscRuntimeContainerJobExecutor).addDependency(ServiceNames.forMscRuntimeContainerDelegate()).addDependency(ServiceNames.forMscExecutorService()).setInitialMode(ServiceController.Mode.ACTIVE).install();
	  }

	}

}