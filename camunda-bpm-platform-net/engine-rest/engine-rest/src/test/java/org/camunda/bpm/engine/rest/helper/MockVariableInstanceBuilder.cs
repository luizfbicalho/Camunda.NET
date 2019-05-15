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

	using VariableInstanceEntity = org.camunda.bpm.engine.impl.persistence.entity.VariableInstanceEntity;
	using VariableInstance = org.camunda.bpm.engine.runtime.VariableInstance;
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
	public class MockVariableInstanceBuilder
	{

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string id_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string name_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal TypedValue typedValue_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string processInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string executionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string caseExecutionId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string taskId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string tenantId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string errorMessage_Renamed;

	  public virtual MockVariableInstanceBuilder id(string id)
	  {
		this.id_Renamed = id;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder name(string name)
	  {
		this.name_Renamed = name;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder typedValue(TypedValue value)
	  {
		this.typedValue_Renamed = value;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder processInstanceId(string processInstanceId)
	  {
		this.processInstanceId_Renamed = processInstanceId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder executionId(string executionId)
	  {
		this.executionId_Renamed = executionId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder caseInstanceId(string caseInstanceId)
	  {
		this.caseInstanceId_Renamed = caseInstanceId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder caseExecutionId(string caseExecutionId)
	  {
		this.caseExecutionId_Renamed = caseExecutionId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder taskId(string taskId)
	  {
		this.taskId_Renamed = taskId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder tenantId(string tenantId)
	  {
		this.tenantId_Renamed = tenantId;
		return this;
	  }

	  public virtual MockVariableInstanceBuilder errorMessage(string errorMessage)
	  {
		this.errorMessage_Renamed = errorMessage;
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

	  public virtual object Value
	  {
		  get
		  {
			return typedValue_Renamed.Value;
		  }
	  }

	  public virtual TypedValue TypedValue
	  {
		  get
		  {
			return typedValue_Renamed;
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

	  public virtual string ActivityInstanceId
	  {
		  get
		  {
			return activityInstanceId_Renamed;
		  }
	  }

	  public virtual string TenantId
	  {
		  get
		  {
			return tenantId_Renamed;
		  }
	  }

	  public virtual string ErrorMessage
	  {
		  get
		  {
			return errorMessage_Renamed;
		  }
	  }

	  public virtual string TypeName
	  {
		  get
		  {
			return typedValue_Renamed.Type.Name;
		  }
	  }

	  public virtual VariableInstance build()
	  {
		VariableInstance mockVariable = mock(typeof(VariableInstance));
		return build(mockVariable);
	  }

	  public virtual VariableInstanceEntity buildEntity()
	  {
		VariableInstanceEntity mockVariable = mock(typeof(VariableInstanceEntity));
		if (!string.ReferenceEquals(taskId_Renamed, null))
		{
		  when(mockVariable.VariableScopeId).thenReturn(taskId_Renamed);
		}
		else if (!string.ReferenceEquals(executionId_Renamed, null))
		{
		  when(mockVariable.VariableScopeId).thenReturn(executionId_Renamed);
		}
		else
		{
		  when(mockVariable.VariableScopeId).thenReturn(caseExecutionId_Renamed);
		}
		return build(mockVariable);
	  }

	  protected internal virtual T build<T>(T mockVariable) where T : org.camunda.bpm.engine.runtime.VariableInstance
	  {
		when(mockVariable.Id).thenReturn(id_Renamed);
		when(mockVariable.Name).thenReturn(name_Renamed);
		when(mockVariable.TypeName).thenReturn(typedValue_Renamed.Type.Name);

		if (typedValue_Renamed.GetType().IsAssignableFrom(typeof(ObjectValue)))
		{
		  ObjectValue objectValue = (ObjectValue) typedValue_Renamed;
		  if (objectValue.Deserialized)
		  {
			when(mockVariable.Value).thenReturn(typedValue_Renamed.Value);
		  }
		  else
		  {
			when(mockVariable.Value).thenReturn(null);
		  }
		}
		else
		{
		  when(mockVariable.Value).thenReturn(typedValue_Renamed.Value);
		}

		when(mockVariable.TypedValue).thenReturn(typedValue_Renamed);
		when(mockVariable.ProcessInstanceId).thenReturn(processInstanceId_Renamed);
		when(mockVariable.ExecutionId).thenReturn(executionId_Renamed);
		when(mockVariable.CaseInstanceId).thenReturn(caseInstanceId_Renamed);
		when(mockVariable.CaseExecutionId).thenReturn(caseExecutionId_Renamed);
		when(mockVariable.TaskId).thenReturn(taskId_Renamed);
		when(mockVariable.ActivityInstanceId).thenReturn(activityInstanceId_Renamed);
		when(mockVariable.TenantId).thenReturn(tenantId_Renamed);
		when(mockVariable.ErrorMessage).thenReturn(errorMessage_Renamed);

		return mockVariable;
	  }

	}

}