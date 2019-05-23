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
namespace org.camunda.bpm.engine.impl.core.model
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using CaseDefinitionEntity = org.camunda.bpm.engine.impl.cmmn.entity.repository.CaseDefinitionEntity;
	using CaseExecutionEntity = org.camunda.bpm.engine.impl.cmmn.entity.runtime.CaseExecutionEntity;
	using ParameterValueProvider = org.camunda.bpm.engine.impl.core.variable.mapping.value.ParameterValueProvider;
	using ExecutionEntity = org.camunda.bpm.engine.impl.persistence.entity.ExecutionEntity;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;

	/// <summary>
	/// Default implementation for <seealso cref="BaseCallableElement.getTenantIdProvider()"/>.
	/// Uses the tenant id of the calling definition.
	/// </summary>
	public class DefaultCallableElementTenantIdProvider : ParameterValueProvider
	{

	  public virtual object getValue(VariableScope execution)
	  {
		if (execution is ExecutionEntity)
		{
		  return getProcessDefinitionTenantId((ExecutionEntity) execution);

		}
		else if (execution is CaseExecutionEntity)
		{
		  return getCaseDefinitionTenantId((CaseExecutionEntity) execution);

		}
		else
		{
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
		  throw new ProcessEngineException("Unexpected execution of type " + execution.GetType().FullName);
		}
	  }

	  protected internal virtual string getProcessDefinitionTenantId(ExecutionEntity execution)
	  {
		ProcessDefinitionEntity processDefinition = (ProcessDefinitionEntity) execution.getProcessDefinition();
		return processDefinition.TenantId;
	  }

	  protected internal virtual string getCaseDefinitionTenantId(CaseExecutionEntity caseExecution)
	  {
		CaseDefinitionEntity caseDefinition = (CaseDefinitionEntity) caseExecution.CaseDefinition;
		return caseDefinition.TenantId;
	  }

	}

}