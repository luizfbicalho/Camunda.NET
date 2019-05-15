using System;
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
namespace org.camunda.bpm.engine.rest.hal
{
	using org.camunda.bpm.engine.rest;
	using VariableValueDto = org.camunda.bpm.engine.rest.dto.VariableValueDto;
	using RestException = org.camunda.bpm.engine.rest.exception.RestException;
	using VariableResource = org.camunda.bpm.engine.rest.sub.VariableResource;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class HalVariableValue : HalResource<HalVariableValue>
	{

	  // add leading / by hand because otherwise it will be encoded as %2F (see CAM-3091)
	  public static HalRelation REL_SELF = HalRelation.build("self", typeof(VariableResource), UriBuilder.fromPath("/{scopeResourcePath}").path("{scopeId}").path("{variablesName}").path("{variableName}"));

	  protected internal string name;
	  protected internal object value;
	  protected internal string type;
	  protected internal IDictionary<string, object> valueInfo;

	  public static HalVariableValue generateVariableValue(VariableInstance variableInstance, string variableScopeId)
	  {
		if (variableScopeId.Equals(variableInstance.TaskId))
		{
		  return generateTaskVariableValue(variableInstance, variableScopeId);
		}
		else if (variableScopeId.Equals(variableInstance.ProcessInstanceId))
		{
		  return generateProcessInstanceVariableValue(variableInstance, variableScopeId);
		}
		else if (variableScopeId.Equals(variableInstance.ExecutionId))
		{
		  return generateExecutionVariableValue(variableInstance, variableScopeId);
		}
		else if (variableScopeId.Equals(variableInstance.CaseInstanceId))
		{
		  return generateCaseInstanceVariableValue(variableInstance, variableScopeId);
		}
		else if (variableScopeId.Equals(variableInstance.CaseExecutionId))
		{
		  return generateCaseExecutionVariableValue(variableInstance, variableScopeId);
		}
		else
		{
		  throw new RestException("Variable scope id '" + variableScopeId + "' does not match with variable instance '" + variableInstance + "'");
		}
	  }

	  public static HalVariableValue generateTaskVariableValue(VariableInstance variableInstance, string taskId)
	  {
		return fromVariableInstance(variableInstance).link(REL_SELF, TaskRestService_Fields.PATH, taskId, "localVariables");
	  }

	  public static HalVariableValue generateExecutionVariableValue(VariableInstance variableInstance, string executionId)
	  {
		return fromVariableInstance(variableInstance).link(REL_SELF, ExecutionRestService_Fields.PATH, executionId, "localVariables");
	  }

	  public static HalVariableValue generateProcessInstanceVariableValue(VariableInstance variableInstance, string processInstanceId)
	  {
		return fromVariableInstance(variableInstance).link(REL_SELF, ProcessInstanceRestService_Fields.PATH, processInstanceId, "variables");
	  }

	  public static HalVariableValue generateCaseExecutionVariableValue(VariableInstance variableInstance, string caseExecutionId)
	  {
		return fromVariableInstance(variableInstance).link(REL_SELF, CaseExecutionRestService_Fields.PATH, caseExecutionId, "localVariables");
	  }

	  public static HalVariableValue generateCaseInstanceVariableValue(VariableInstance variableInstance, string caseInstanceId)
	  {
		return fromVariableInstance(variableInstance).link(REL_SELF, CaseInstanceRestService_Fields.PATH, caseInstanceId, "variables");
	  }

	  private HalVariableValue link(HalRelation relation, string resourcePath, string resourceId, string variablesPath)
	  {
		if (resourcePath.StartsWith("/", StringComparison.Ordinal))
		{
		  // trim leading / because otherwise it will be encode as %2F (see CAM-3091)
		  resourcePath = resourcePath.Substring(1);
		}
		this.linker.createLink(relation, resourcePath, resourceId, variablesPath, this.name);
		return this;
	  }

	  public static HalVariableValue fromVariableInstance(VariableInstance variableInstance)
	  {
		HalVariableValue dto = new HalVariableValue();

		VariableValueDto variableValueDto = VariableValueDto.fromTypedValue(variableInstance.TypedValue);

		dto.name = variableInstance.Name;
		dto.value = variableValueDto.Value;
		dto.type = variableValueDto.Type;
		dto.valueInfo = variableValueDto.ValueInfo;

		return dto;
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual string Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual IDictionary<string, object> ValueInfo
	  {
		  get
		  {
			return valueInfo;
		  }
	  }

	}

}