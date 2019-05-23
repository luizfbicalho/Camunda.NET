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
namespace org.camunda.bpm.engine.impl.runtime
{
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using ProcessDefinition = org.camunda.bpm.engine.repository.ProcessDefinition;
	using Execution = org.camunda.bpm.engine.runtime.Execution;
	using MessageCorrelationResultType = org.camunda.bpm.engine.runtime.MessageCorrelationResultType;

	/// <summary>
	/// <para>The result of a message correlation. A message may be correlated to either
	/// a waiting execution (BPMN receive message event) or a process definition
	/// (BPMN message start event). The type of the correlation (execution vs.
	/// processDefinition) can be obtained using <seealso cref="getResultType()"/></para>
	/// 
	/// <para>Correlation is performed by a <seealso cref="CorrelationHandler"/>.</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class CorrelationHandlerResult
	{

	  /// <seealso cref= MessageCorrelationResultType#Execution </seealso>
	  /// <seealso cref= MessageCorrelationResultType#ProcessDefinition </seealso>
	  protected internal MessageCorrelationResultType resultType;

	  protected internal ExecutionEntity executionEntity;
	  protected internal ProcessDefinitionEntity processDefinitionEntity;
	  protected internal string startEventActivityId;

	  public static CorrelationHandlerResult matchedExecution(ExecutionEntity executionEntity)
	  {
		CorrelationHandlerResult messageCorrelationResult = new CorrelationHandlerResult();
		messageCorrelationResult.resultType = MessageCorrelationResultType.Execution;
		messageCorrelationResult.executionEntity = executionEntity;
		return messageCorrelationResult;
	  }

	  public static CorrelationHandlerResult matchedProcessDefinition(ProcessDefinitionEntity processDefinitionEntity, string startEventActivityId)
	  {
		CorrelationHandlerResult messageCorrelationResult = new CorrelationHandlerResult();
		messageCorrelationResult.processDefinitionEntity = processDefinitionEntity;
		messageCorrelationResult.startEventActivityId = startEventActivityId;
		messageCorrelationResult.resultType = MessageCorrelationResultType.ProcessDefinition;
		return messageCorrelationResult;
	  }

	  // getters ////////////////////////////////////////////

	  public virtual ExecutionEntity ExecutionEntity
	  {
		  get
		  {
			return executionEntity;
		  }
	  }

	  public virtual ProcessDefinitionEntity ProcessDefinitionEntity
	  {
		  get
		  {
			return processDefinitionEntity;
		  }
	  }

	  public virtual string StartEventActivityId
	  {
		  get
		  {
			return startEventActivityId;
		  }
	  }

	  public virtual MessageCorrelationResultType ResultType
	  {
		  get
		  {
			return resultType;
		  }
	  }

	  public virtual Execution Execution
	  {
		  get
		  {
			return executionEntity;
		  }
	  }

	  public virtual ProcessDefinition ProcessDefinition
	  {
		  get
		  {
			return processDefinitionEntity;
		  }
	  }
	}

}