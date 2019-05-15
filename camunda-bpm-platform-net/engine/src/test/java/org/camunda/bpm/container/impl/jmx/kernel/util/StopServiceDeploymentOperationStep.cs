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
namespace org.camunda.bpm.container.impl.jmx.kernel.util
{
	using PlatformServiceContainer = org.camunda.bpm.container.impl.spi.PlatformServiceContainer;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;

	public class StopServiceDeploymentOperationStep : DeploymentOperationStep
	{

	  private string serviceName;

	  public StopServiceDeploymentOperationStep(string serviceName)
	  {
		this.serviceName = serviceName;
	  }

	  public override string Name
	  {
		  get
		  {
			return "stop " + serviceName;
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.container.impl.spi.PlatformServiceContainer serviceContainer = operationContext.getServiceContainer();
		PlatformServiceContainer serviceContainer = operationContext.ServiceContainer;

		serviceContainer.stopService(serviceName);
	  }

	}

}