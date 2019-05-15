﻿/*
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
namespace org.camunda.bpm.container.impl.deployment.jobexecutor
{
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using ServiceTypes = org.camunda.bpm.container.impl.spi.ServiceTypes;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class StopManagedThreadPoolStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "Stop managed thread pool";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;
		serviceContainer.stopService(ServiceTypes.BPM_PLATFORM, RuntimeContainerDelegateImpl.SERVICE_NAME_EXECUTOR);
	  }

	}

}