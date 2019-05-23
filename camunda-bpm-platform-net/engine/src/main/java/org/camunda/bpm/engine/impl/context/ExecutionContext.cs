using System;

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
namespace org.camunda.bpm.engine.impl.context
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;


	/// <summary>
	/// An <seealso cref="ExecutionEntity"/> execution context. Provides access to the process instance and the deployment.
	/// </summary>
	/// @deprecated since 7.2: use <seealso cref="BpmnExecutionContext"/>
	/// 
	/// @author Tom Baeyens
	/// @author Roman Smirnov
	/// @author Daniel Meyer 
	[Obsolete("since 7.2: use <seealso cref=\"BpmnExecutionContext\"/>")]
	public class ExecutionContext : CoreExecutionContext<ExecutionEntity>
	{

	  public ExecutionContext(ExecutionEntity execution) : base(execution)
	  {
	  }

	  public virtual ExecutionEntity ProcessInstance
	  {
		  get
		  {
			return execution.ProcessInstance;
		  }
	  }

	  public virtual ProcessDefinitionEntity ProcessDefinition
	  {
		  get
		  {
			return (ProcessDefinitionEntity) execution.ProcessDefinition;
		  }
	  }

	  protected internal override string DeploymentId
	  {
		  get
		  {
			return ProcessDefinition.DeploymentId;
		  }
	  }
	}

}