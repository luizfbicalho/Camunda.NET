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
	/// methods <seealso cref="JsonSpec.setMatcher(string, org.hamcrest.Matcher)"/> and
	/// <seealso cref="JsonSpec.setEnclosedJsonSpec(string, JsonSpec)"/>.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class MockHistoricVariableInstanceBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Conflict;
	  protected internal TypedValue value;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionKey_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseDefinitionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime createTime_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal DateTime removalTime_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string rootProcessInstanceId_Conflict;

	  public virtual MockHistoricVariableInstanceBuilder id(string id)
	  {
		this.id_Conflict = id;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder name(string name)
	  {
		this.name_Conflict = name;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder typedValue(TypedValue value)
	  {
		this.value = value;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder processDefinitionKey(string processDefinitionKey)
	  {
		this.processDefinitionKey_Conflict = processDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder processDefinitionId(string processDefinitionId)
	  {
		this.processDefinitionId_Conflict = processDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Conflict = processInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder executionId(string executionId)
	  {
		this.executionId_Conflict = executionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Conflict = errorMessage;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Conflict = activityInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseDefinitionKey(string caseDefinitionKey)
	  {
		this.caseDefinitionKey_Conflict = caseDefinitionKey;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseDefinitionId(string caseDefinitionId)
	  {
		this.caseDefinitionId_Conflict = caseDefinitionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Conflict = caseInstanceId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Conflict = caseExecutionId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder taskId(string taskId)
	  {
		this.taskId_Conflict = taskId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder tenantId(string tenantId)
	  {
		this.tenantId_Conflict = tenantId;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder createTime(DateTime createTime)
	  {
		this.createTime_Conflict = createTime;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder removalTime(DateTime removalTime)
	  {
		this.removalTime_Conflict = removalTime;
		return this;
	  }

	  public virtual MockHistoricVariableInstanceBuilder rootProcessInstanceId(string rootProcessInstanceId)
	  {
		this.rootProcessInstanceId_Conflict = rootProcessInstanceId;
		return this;
	  }

	  public virtual string Id
	  {
		  get
		  {
			return id_Conflict;
		  }
	  }

	  public virtual string Name
	  {
		  get
		  {
			return name_Conflict;
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
			return processDefinitionKey_Conflict;
		  }
	  }

	  public virtual string ProcessDefinitionId
	  {
		  get
		  {
			return processDefinitionId_Conflict;
		  }
	  }

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId_Conflict;
		  }
	  }

	  public virtual string ExecutionId
	  {
		  get
		  {
			return executionId_Conflict;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage_Conflict;
		  }
	  }

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionKey
	  {
		  get
		  {
			return caseDefinitionKey_Conflict;
		  }
	  }

	  public virtual string CaseDefinitionId
	  {
		  get
		  {
			return caseDefinitionId_Conflict;
		  }
	  }

	  public virtual string CaseInstanceId
	  {
		  get
		  {
			return caseInstanceId_Conflict;
		  }
	  }

	  public virtual string CaseExecutionId
	  {
		  get
		  {
			return caseExecutionId_Conflict;
		  }
	  }

	  public virtual string TaskId
	  {
		  get
		  {
			return taskId_Conflict;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Conflict;
		  }
	  }

	  public virtual DateTime CreateTime
	  {
		  get
		  {
			return createTime_Conflict;
		  }
	  }

	  public virtual DateTime RemovalTime
	  {
		  get
		  {
			return removalTime_Conflict;
		  }
	  }

	  public virtual string RootProcessInstanceId
	  {
		  get
		  {
			return rootProcessInstanceId_Conflict;
		  }
	  }

	  public virtual HistoricVariableInstance build()
	  {
		HistoricVariableInstance mockVariable = mock(typeof(HistoricVariableInstance));
		when(mockVariable.Id).thenReturn(id_Conflict);
		when(mockVariable.Name).thenReturn(name_Conflict);
		when(mockVariable.VariableName).thenReturn(name_Conflict);
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
		when(mockVariable.ProcessDefinitionKey).thenReturn(processDefinitionKey_Conflict);
		when(mockVariable.ProcessDefinitionId).thenReturn(processDefinitionId_Conflict);
		when(mockVariable.ProcessInstanceId).thenReturn(processInstanceId_Conflict);
		when(mockVariable.ExecutionId).thenReturn(executionId_Conflict);
		when(mockVariable.ErrorMessage).thenReturn(errorMessage_Conflict);
		when(mockVariable.ActivtyInstanceId).thenReturn(activityInstanceId_Conflict);
		when(mockVariable.ActivityInstanceId).thenReturn(activityInstanceId_Conflict);
		when(mockVariable.CaseDefinitionKey).thenReturn(caseDefinitionKey_Conflict);
		when(mockVariable.CaseDefinitionId).thenReturn(caseDefinitionId_Conflict);
		when(mockVariable.CaseInstanceId).thenReturn(caseInstanceId_Conflict);
		when(mockVariable.CaseExecutionId).thenReturn(caseExecutionId_Conflict);
		when(mockVariable.TaskId).thenReturn(taskId_Conflict);
		when(mockVariable.TenantId).thenReturn(tenantId_Conflict);
		when(mockVariable.CreateTime).thenReturn(createTime_Conflict);
		when(mockVariable.RemovalTime).thenReturn(removalTime_Conflict);
		when(mockVariable.RootProcessInstanceId).thenReturn(rootProcessInstanceId_Conflict);

		return mockVariable;
	  }

	}

}