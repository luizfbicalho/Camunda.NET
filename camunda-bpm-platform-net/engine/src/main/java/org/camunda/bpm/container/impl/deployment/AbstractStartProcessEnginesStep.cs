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
namespace org.camunda.bpm.container.impl.deployment
{

	using ProcessEngineXml = org.camunda.bpm.container.impl.metadata.spi.ProcessEngineXml;
	using DeploymentOperation = org.camunda.bpm.container.impl.spi.DeploymentOperation;
	using DeploymentOperationStep = org.camunda.bpm.container.impl.spi.DeploymentOperationStep;

	/// <summary>
	/// <para>Deployment operation step that is responsible for starting all process
	/// engines declared in a <seealso cref="System.Collections.IList"/> of <seealso cref="ProcessEngineXml"/> files.</para>
	/// 
	/// <para>This step does not start the process engines directly but rather creates
	/// individual <seealso cref="StartProcessEngineStep"/> instances that each start a process
	/// engine.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public abstract class AbstractStartProcessEnginesStep : DeploymentOperationStep
	{

	  public override string Name
	  {
		  get
		  {
			return "Start process engines";
		  }
	  }

	  public override void performOperationStep(DeploymentOperation operationContext)
	  {

		IList<ProcessEngineXml> processEngines = getProcessEnginesXmls(operationContext);

		foreach (ProcessEngineXml parsedProcessEngine in processEngines)
		{
		  // for each process engine add a new deployment step
		  operationContext.addStep(createStartProcessEngineStep(parsedProcessEngine));
		}

	  }

	  protected internal virtual StartProcessEngineStep createStartProcessEngineStep(ProcessEngineXml parsedProcessEngine)
	  {
		return new StartProcessEngineStep(parsedProcessEngine);
	  }

	  protected internal abstract IList<ProcessEngineXml> getProcessEnginesXmls(DeploymentOperation operationContext);

	}

}