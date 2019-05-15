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
namespace org.camunda.bpm.engine.impl.externaltask
{
	using ExternalTaskActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.ExternalTaskActivityBehavior;
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;

	/// <summary>
	/// Represents the default priority provider for external tasks.
	/// 
	/// @author Christopher Zell <christopher.zell@camunda.com>
	/// </summary>
	public class DefaultExternalTaskPriorityProvider : DefaultPriorityProvider<ExternalTaskActivityBehavior>
	{

	  public static readonly ExternalTaskLogger LOG = ProcessEngineLogger.EXTERNAL_TASK_LOGGER;

	  protected internal override void logNotDeterminingPriority(ExecutionEntity execution, object value, ProcessEngineException e)
	  {
		LOG.couldNotDeterminePriority(execution, value, e);
	  }

	  public override long? getSpecificPriority(ExecutionEntity execution, ExternalTaskActivityBehavior param, string jobDefinitionId)
	  {
		ParameterValueProvider priorityProvider = param.PriorityValueProvider;
		if (priorityProvider != null)
		{
		  return evaluateValueProvider(priorityProvider, execution, "");
		}
		return null;
	  }

	  protected internal override long? getProcessDefinitionPriority(ExecutionEntity execution, ExternalTaskActivityBehavior param)
	  {
		return getProcessDefinedPriority(execution.getProcessDefinition(), BpmnParse.PROPERTYNAME_TASK_PRIORITY, execution, "");
	  }
	}

}