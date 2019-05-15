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
namespace org.camunda.bpm.engine.impl.jobexecutor
{
	using BpmnParse = org.camunda.bpm.engine.impl.bpmn.parser.BpmnParse;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using JobDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.JobDefinitionEntity;
	using ProcessDefinitionImpl = org.camunda.bpm.engine.impl.pvm.process.ProcessDefinitionImpl;

	/// <summary>
	/// @author Thorben Lindhauer
	/// @author Christopher Zell
	/// </summary>
	public class DefaultJobPriorityProvider : DefaultPriorityProvider<JobDeclaration<JavaToDotNetGenericWildcard, JavaToDotNetGenericWildcard>>
	{

	  private static readonly JobExecutorLogger LOG = ProcessEngineLogger.JOB_EXECUTOR_LOGGER;

	  protected internal override long? getSpecificPriority<T1>(ExecutionEntity execution, JobDeclaration<T1> param, string jobDefinitionId)
	  {
		long? specificPriority = null;
		JobDefinitionEntity jobDefinition = getJobDefinitionFor(jobDefinitionId);
		if (jobDefinition != null)
		{
		  specificPriority = jobDefinition.OverridingJobPriority;
		}

		if (specificPriority == null)
		{
		  ParameterValueProvider priorityProvider = param.JobPriorityProvider;
		  if (priorityProvider != null)
		  {
			specificPriority = evaluateValueProvider(priorityProvider, execution, describeContext(param, execution));
		  }
		}
		return specificPriority;
	  }

	  protected internal override long? getProcessDefinitionPriority<T1>(ExecutionEntity execution, JobDeclaration<T1> jobDeclaration)
	  {
		ProcessDefinitionImpl processDefinition = jobDeclaration.ProcessDefinition;
		return getProcessDefinedPriority(processDefinition, BpmnParse.PROPERTYNAME_JOB_PRIORITY, execution, describeContext(jobDeclaration, execution));
	  }

	  protected internal virtual JobDefinitionEntity getJobDefinitionFor(string jobDefinitionId)
	  {
		if (!string.ReferenceEquals(jobDefinitionId, null))
		{
		  return Context.CommandContext.JobDefinitionManager.findById(jobDefinitionId);
		}
		else
		{
		  return null;
		}
	  }

	  protected internal virtual long? getActivityPriority<T1>(ExecutionEntity execution, JobDeclaration<T1> jobDeclaration)
	  {
		if (jobDeclaration != null)
		{
		  ParameterValueProvider priorityProvider = jobDeclaration.JobPriorityProvider;
		  if (priorityProvider != null)
		  {
			return evaluateValueProvider(priorityProvider, execution, describeContext(jobDeclaration, execution));
		  }
		}
		return null;
	  }

	  protected internal override void logNotDeterminingPriority(ExecutionEntity execution, object value, ProcessEngineException e)
	  {
		LOG.couldNotDeterminePriority(execution, value, e);
	  }

	  protected internal virtual string describeContext<T1>(JobDeclaration<T1> jobDeclaration, ExecutionEntity executionEntity)
	  {
		return "Job " + jobDeclaration.ActivityId + "/" + jobDeclaration.JobHandlerType + " instantiated "
		  + "in context of " + executionEntity;
	  }
	}

}