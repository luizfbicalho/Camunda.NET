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
namespace org.camunda.bpm.engine.rest.helper
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.when;

	using HistoricVariableInstance = org.camunda.bpm.engine.history.HistoricVariableInstance;
	using ObjectValue = org.camunda.bpm.engine.variable.value.ObjectValue;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Creates variable mocks and simultaneously provides a ResponseSpecification
	/// to assert a result of this mock in json; expectations can be overriden using the
	/// methods <seealso cref="JsonSpec#setMatcher(String, org.hamcrest.Matcher)"/> and
	/// <seealso cref="JsonSpec#setEnclosedJsonSpec(String, JsonSpec)"/>.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class MockHistoricVariableInstanceBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
	  protected internal TypedValue value;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createTime_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime removalTime_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string rootProcessInstanceId_Renamed;

	  public virtual MockHistoricVariableInstanceBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder typedValue(TypedValue value)
	  {
		this.value = value;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Renamed = processDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Renamed = processDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Renamed = errorMessage;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseDefinitionKey(string caseDefinitionKey)
	  {
		this.caseDefinitionKey_Renamed = caseDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Renamed = caseDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder taskId(string taskId)
	  {
		this.taskId_Renamed = taskId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder createTime(DateTime createTime)
	  {
		this.createTime_Renamed = createTime;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder removalTime(DateTime removalTime)
	  {
		this.removalTime_Renamed = removalTime;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder rootProcessInstanceId(string rootProcessInstanceId)
	  {
		this.rootProcessInstanceId_Renamed = rootProcessInstanceId;
		return this;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id_Renamed;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name_Renamed;
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return value;
		  }
	  }

	  public virtual object Value
	  {
		  get
		  {
			return value.Value;
		  }
	  }

	  public virtual string ProcessDefinitionKey
	  {
		  get
		  {
			return processDefinitionKey_Renamed;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Renamed;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Renamed;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Renamed;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage_Renamed;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Renamed;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Renamed;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Renamed;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Renamed;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Renamed;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Renamed;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Renamed;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime_Renamed;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime_Renamed;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId_Renamed;
		  }
	  }

	  public virtual HistoricVariableInstance build()
	  {
		HistoricVariableInstance mockVariable = mock(typeof(HistoricVariableInstance));
		when(mockVariable.Id).thenReturn(id_Renamed);
		when(mockVariable.Name).thenReturn(name_Renamed);
		when(mockVariable.VariableName).thenReturn(name_Renamed);
		when(mockVariable.TypeName).thenReturn(value.Type.Name);
		when(mockVariable.VariableTypeName).thenReturn(value.Type.Name);

		if (value.GetType().IsAssignableFrom(typeof(ObjectValue)))
		{
		  ObjectValue objectValue = (ObjectValue) value;
		  if (objectValue.Deserialized)
		  {
			when(mockVariable.Value).thenReturn(value.Value);
		  }
		  else
		  {
			when(mockVariable.Value).thenReturn(null);
		  }
		}
		else
		{
		  when(mockVariable.Value).thenReturn(value.Value);
		}

		when(mockVariable.TypedValue).thenReturn(value);
		when(mockVariable.ProcessDefinitionKey).thenReturn(processDefinitionKey_Renamed);
		when(mockVariable.ProcessDefinitionId).thenReturn(processDefinitionId_Renamed);
		when(mockVariable.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
		when(mockVariable.ExecutionId).thenReturn(executionId_Renamed);
		when(mockVariable.ErrorMessage).thenReturn(errorMessage_Renamed);
		when(mockVariable.ActivtyInstanceId).thenReturn(activityInstanceId_Renamed);
		when(mockVariable.ActivityInstanceId).thenReturn(activityInstanceId_Renamed);
		when(mockVariable.CaseDefinitionKey).thenReturn(caseDefinitionKey_Renamed);
		when(mockVariable.CaseDefinitionId).thenReturn(caseDefinitionId_Renamed);
		when(mockVariable.CaseInstanceId).thenReturn(caseInstanceId_Renamed);
		when(mockVariable.CaseExecutionId).thenReturn(caseExecutionId_Renamed);
		when(mockVariable.TaskId).thenReturn(taskId_Renamed);
		when(mockVariable.TenantId).thenReturn(tenantId_Renamed);
		when(mockVariable.CreateTime).thenReturn(createTime_Renamed);
		when(mockVariable.RemovalTime).thenReturn(removalTime_Renamed);
		when(mockVariable.RootProcessInstanceId).thenReturn(rootProcessInstanceId_Renamed);

		return mockVariable;
	  }

	}

}