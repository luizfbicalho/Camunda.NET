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
namespace org.camunda.bpm.engine.impl
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using VariableSerializers = org.camunda.bpm.engine.impl.variable.serializer.VariableSerializers;
	using Query = org.camunda.bpm.engine.query.Query;


	/// <summary>
	/// Abstract query class that adds methods to query for variable values.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public abstract class AbstractVariableQueryImpl<T, U> : AbstractQuery<T, U>
	{

	  private const long serialVersionUID = 1L;

	  protected internal IList<QueryVariableValue> queryVariableValues = new List<QueryVariableValue>();

	  public AbstractVariableQueryImpl()
	  {
	  }

	  public AbstractVariableQueryImpl(CommandExecutor commandExecutor) : base(commandExecutor)
	  {
	  }

	  public override abstract long executeCount(CommandContext commandContext);

	  public override abstract IList<U> executeList(CommandContext commandContext, Page page);


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueEquals(String name, Object value)
	  public virtual T variableValueEquals(string name, object value)
	  {
		addVariable(name, value, QueryOperator.EQUALS, true);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueNotEquals(String name, Object value)
	  public virtual T variableValueNotEquals(string name, object value)
	  {
		addVariable(name, value, QueryOperator.NOT_EQUALS, true);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueGreaterThan(String name, Object value)
	  public virtual T variableValueGreaterThan(string name, object value)
	  {
		addVariable(name, value, QueryOperator.GREATER_THAN, true);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueGreaterThanOrEqual(String name, Object value)
	  public virtual T variableValueGreaterThanOrEqual(string name, object value)
	  {
		addVariable(name, value, QueryOperator.GREATER_THAN_OR_EQUAL, true);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueLessThan(String name, Object value)
	  public virtual T variableValueLessThan(string name, object value)
	  {
		addVariable(name, value, QueryOperator.LESS_THAN, true);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueLessThanOrEqual(String name, Object value)
	  public virtual T variableValueLessThanOrEqual(string name, object value)
	  {
		addVariable(name, value, QueryOperator.LESS_THAN_OR_EQUAL, true);
		return (T) this;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T variableValueLike(String name, String value)
	  public virtual T variableValueLike(string name, string value)
	  {
		addVariable(name, value, QueryOperator.LIKE, true);
		return (T)this;
	  }

	  protected internal virtual void addVariable(string name, object value, QueryOperator @operator, bool processInstanceScope)
	  {
		ensureNotNull(typeof(NotValidException), "name", name);
		if (value == null || isBoolean(value))
		{
		  // Null-values and booleans can only be used in EQUALS and NOT_EQUALS
		  switch (@operator)
		  {
		  case org.camunda.bpm.engine.impl.QueryOperator.GREATER_THAN:
			throw new NotValidException("Booleans and null cannot be used in 'greater than' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.LESS_THAN:
			throw new NotValidException("Booleans and null cannot be used in 'less than' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.GREATER_THAN_OR_EQUAL:
			throw new NotValidException("Booleans and null cannot be used in 'greater than or equal' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.LESS_THAN_OR_EQUAL:
			throw new NotValidException("Booleans and null cannot be used in 'less than or equal' condition");
		  case org.camunda.bpm.engine.impl.QueryOperator.LIKE:
			throw new NotValidException("Booleans and null cannot be used in 'like' condition");
		  }
		}
		queryVariableValues.Add(new QueryVariableValue(name, value, @operator, processInstanceScope));
	  }

	  private bool isBoolean(object value)
	  {
		if (value == null)
		{
		  return false;
		}
		return value.GetType().IsAssignableFrom(typeof(Boolean)) || value.GetType().IsAssignableFrom(typeof(bool));
	  }

	  protected internal virtual void ensureVariablesInitialized()
	  {
		if (queryVariableValues.Count > 0)
		{
		  VariableSerializers variableSerializers = Context.ProcessEngineConfiguration.VariableSerializers;
		  foreach (QueryVariableValue queryVariableValue in queryVariableValues)
		  {
			queryVariableValue.initialize(variableSerializers);
		  }
		}
	  }

	  public virtual IList<QueryVariableValue> QueryVariableValues
	  {
		  get
		  {
			return queryVariableValues;
		  }
	  }


	}

}