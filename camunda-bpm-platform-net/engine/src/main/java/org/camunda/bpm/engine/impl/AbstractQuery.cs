using System;
using System.Collections.Generic;
using System.Reflection;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNull;


	using NotValidException = org.camunda.bpm.engine.exception.NotValidException;
	using AdhocQueryValidator = org.camunda.bpm.engine.impl.QueryValidators.AdhocQueryValidator;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using ListQueryParameterObject = org.camunda.bpm.engine.impl.db.ListQueryParameterObject;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using Query = org.camunda.bpm.engine.query.Query;
	using QueryProperty = org.camunda.bpm.engine.query.QueryProperty;
	using DateTime = org.joda.time.DateTime;


	/// <summary>
	/// Abstract superclass for all query types.
	/// 
	/// @author Joram Barrez
	/// </summary>
	[Serializable]
	public abstract class AbstractQuery<T, U> : ListQueryParameterObject, Command<object>, Query<T, U>
	{

	  private const long serialVersionUID = 1L;

	  public const string SORTORDER_ASC = "asc";
	  public const string SORTORDER_DESC = "desc";

	  protected internal enum ResultType
	  {
		LIST,
		LIST_PAGE,
		LIST_IDS,
		SINGLE_RESULT,
		COUNT
	  }
	  [NonSerialized]
	  protected internal CommandExecutor commandExecutor;

	  protected internal ResultType resultType;

	  protected internal IDictionary<string, string> expressions = new Dictionary<string, string>();

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Set<Validator<AbstractQuery<?, ?>>> validators = new java.util.HashSet<Validator<AbstractQuery<?, ?>>>();
	  protected internal ISet<Validator<AbstractQuery<object, ?>>> validators = new HashSet<Validator<AbstractQuery<object, ?>>>();

	  protected internal AbstractQuery()
	  {
	  }

	  protected internal AbstractQuery(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;

		// all queries that are created with a dedicated command executor
		// are treated as adhoc queries (i.e. queries not created in the context
		// of a command)
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: addValidator(org.camunda.bpm.engine.impl.QueryValidators.AdhocQueryValidator.get<AbstractQuery<?, ?>>());
		addValidator(AdhocQueryValidator.get<AbstractQuery<object, ?>>());
	  }

	  public virtual AbstractQuery<T, U> setCommandExecutor(CommandExecutor commandExecutor)
	  {
		this.commandExecutor = commandExecutor;
		return this;
	  }

	  public virtual T orderBy(QueryProperty property)
	  {
		return orderBy(new QueryOrderingProperty(null, property));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T orderBy(QueryOrderingProperty orderProperty)
	  public virtual T orderBy(QueryOrderingProperty orderProperty)
	  {
		this.orderingProperties.add(orderProperty);
		return (T) this;
	  }

	  public virtual T asc()
	  {
		return direction(Direction.ASCENDING);
	  }

	  public virtual T desc()
	  {
		return direction(Direction.DESCENDING);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public T direction(Direction direction)
	  public virtual T direction(Direction direction)
	  {
		QueryOrderingProperty currentOrderingProperty = null;

		if (!orderingProperties.Empty)
		{
		  currentOrderingProperty = orderingProperties.get(orderingProperties.size() - 1);
		}

		ensureNotNull(typeof(NotValidException), "You should call any of the orderBy methods first before specifying a direction", "currentOrderingProperty", currentOrderingProperty);

		if (currentOrderingProperty.Direction != null)
		{
		  ensureNull(typeof(NotValidException), "Invalid query: can specify only one direction desc() or asc() for an ordering constraint", "direction", direction);
		}

		currentOrderingProperty.Direction = direction;
		return (T) this;
	  }

	  protected internal virtual void checkQueryOk()
	  {

		foreach (QueryOrderingProperty orderingProperty in orderingProperties)
		{
		  ensureNotNull(typeof(NotValidException), "Invalid query: call asc() or desc() after using orderByXX()", "direction", orderingProperty.Direction);
		}

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public U singleResult()
	  public virtual U singleResult()
	  {
		this.resultType = ResultType.SINGLE_RESULT;
		if (commandExecutor != null)
		{
		  return (U) commandExecutor.execute(this);
		}
		return executeSingleResult(Context.CommandContext);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<U> list()
	  public virtual IList<U> list()
	  {
		this.resultType = ResultType.LIST;
		if (commandExecutor != null)
		{
		  return (IList<U>) commandExecutor.execute(this);
		}
		return evaluateExpressionsAndExecuteList(Context.CommandContext, null);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<U> listPage(int firstResult, int maxResults)
	  public virtual IList<U> listPage(int firstResult, int maxResults)
	  {
		this.firstResult = firstResult;
		this.maxResults = maxResults;
		this.resultType = ResultType.LIST_PAGE;
		if (commandExecutor != null)
		{
		  return (IList<U>) commandExecutor.execute(this);
		}
		return evaluateExpressionsAndExecuteList(Context.CommandContext, new Page(firstResult, maxResults));
	  }

	  public virtual long count()
	  {
		this.resultType = ResultType.COUNT;
		if (commandExecutor != null)
		{
		  return (long?) commandExecutor.execute(this).Value;
		}
		return evaluateExpressionsAndExecuteCount(Context.CommandContext);
	  }

	  public virtual object execute(CommandContext commandContext)
	  {
		if (resultType == ResultType.LIST)
		{
		  return evaluateExpressionsAndExecuteList(commandContext, null);
		}
		else if (resultType == ResultType.SINGLE_RESULT)
		{
		  return executeSingleResult(commandContext);
		}
		else if (resultType == ResultType.LIST_PAGE)
		{
		  return evaluateExpressionsAndExecuteList(commandContext, null);
		}
		else if (resultType == ResultType.LIST_IDS)
		{
		  return evaluateExpressionsAndExecuteIdsList(commandContext);
		}
		else
		{
		  return evaluateExpressionsAndExecuteCount(commandContext);
		}
	  }

	  public virtual long evaluateExpressionsAndExecuteCount(CommandContext commandContext)
	  {
		validate();
		evaluateExpressions();
		return !hasExcludingConditions() ? executeCount(commandContext) : 0l;
	  }

	  public abstract long executeCount(CommandContext commandContext);

	  public virtual IList<U> evaluateExpressionsAndExecuteList(CommandContext commandContext, Page page)
	  {
		validate();
		evaluateExpressions();
		return !hasExcludingConditions() ? executeList(commandContext, page) : new List<U>();
	  }

	  /// <summary>
	  /// Whether or not the query has excluding conditions. If the query has excluding conditions,
	  /// (e.g. task due date before and after are excluding), the SQL query is avoided and a default result is
	  /// returned. The returned result is the same as if the SQL was executed and there were no entries.
	  /// </summary>
	  /// <returns> {@code true} if the query does have excluding conditions, {@code false} otherwise </returns>
	  protected internal virtual bool hasExcludingConditions()
	  {
		return false;
	  }

	  /// <summary>
	  /// Executes the actual query to retrieve the list of results. </summary>
	  /// <param name="page"> used if the results must be paged. If null, no paging will be applied. </param>
	  public abstract IList<U> executeList(CommandContext commandContext, Page page);

	  public virtual U executeSingleResult(CommandContext commandContext)
	  {
		IList<U> results = evaluateExpressionsAndExecuteList(commandContext, null);
		if (results.Count == 1)
		{
		  return results[0];
		}
		else if (results.Count > 1)
		{
		 throw new ProcessEngineException("Query return " + results.Count + " results instead of max 1");
		}
		return default(U);
	  }

	  public virtual IDictionary<string, string> Expressions
	  {
		  get
		  {
			return expressions;
		  }
		  set
		  {
			this.expressions = value;
		  }
	  }


	  public virtual void addExpression(string key, string expression)
	  {
		this.expressions[key] = expression;
	  }

	  protected internal virtual void evaluateExpressions()
	  {
		// we cannot iterate directly on the entry set cause the expressions
		// are removed by the setter methods during the iteration
		List<KeyValuePair<string, string>> entries = new List<KeyValuePair<string, string>>(expressions.SetOfKeyValuePairs());

		foreach (KeyValuePair<string, string> entry in entries)
		{
		  string methodName = entry.Key;
		  string expression = entry.Value;

		  object value;

		  try
		  {
			value = Context.ProcessEngineConfiguration.ExpressionManager.createExpression(expression).getValue(null);
		  }
		  catch (ProcessEngineException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			throw new ProcessEngineException("Unable to resolve expression '" + expression + "' for method '" + methodName + "' on class '" + this.GetType().FullName + "'", e);
		  }

		  // automatically convert DateTime to date
		  if (value is DateTime)
		  {
			value = ((DateTime) value).toDate();
		  }

		  try
		  {
			System.Reflection.MethodInfo method = getMethod(methodName);
			method.invoke(this, value);
		  }
		  catch (InvocationTargetException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			throw new ProcessEngineException("Unable to invoke method '" + methodName + "' on class '" + this.GetType().FullName + "'", e);
		  }
		  catch (IllegalAccessException e)
		  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
			throw new ProcessEngineException("Unable to access method '" + methodName + "' on class '" + this.GetType().FullName + "'", e);
		  }
		}
	  }

	  protected internal virtual System.Reflection.MethodInfo getMethod(string methodName)
	  {
		foreach (System.Reflection.MethodInfo method in this.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
		{
		  if (method.Name.Equals(methodName))
		  {
			return method;
		  }
		}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		throw new ProcessEngineException("Unable to find method '" + methodName + "' on class '" + this.GetType().FullName + "'");
	  }

	  public virtual T extend(T extendingQuery)
	  {
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getCanonicalName method:
		throw new ProcessEngineException("Extending of query type '" + extendingQuery.GetType().FullName + "' currently not supported");
	  }

	  protected internal virtual void mergeOrdering<T1, T2>(AbstractQuery<T1> extendedQuery, AbstractQuery<T2> extendingQuery)
	  {
		extendedQuery.orderingProperties = this.orderingProperties;
		if (extendingQuery.orderingProperties != null)
		{
		   if (extendedQuery.orderingProperties == null)
		   {
			 extendedQuery.orderingProperties = extendingQuery.orderingProperties;
		   }
		   else
		   {
			 extendedQuery.orderingProperties.addAll(extendingQuery.orderingProperties);
		   }
		}
	  }

	  protected internal virtual void mergeExpressions<T1, T2>(AbstractQuery<T1> extendedQuery, AbstractQuery<T2> extendingQuery)
	  {
		IDictionary<string, string> mergedExpressions = new Dictionary<string, string>(extendingQuery.Expressions);
		foreach (KeyValuePair<string, string> entry in this.Expressions.SetOfKeyValuePairs())
		{
		  if (!mergedExpressions.ContainsKey(entry.Key))
		  {
			mergedExpressions[entry.Key] = entry.Value;
		  }
		}
		extendedQuery.Expressions = mergedExpressions;
	  }

	  public virtual void validate()
	  {
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: for (Validator<AbstractQuery<?, ?>> validator : validators)
		foreach (Validator<AbstractQuery<object, ?>> validator in validators)
		{
		  validate(validator);
		}
	  }

	  public virtual void validate<T1>(Validator<T1> validator)
	  {
		validator.validate(this);
	  }

	  public virtual void addValidator<T1>(Validator<T1> validator)
	  {
		validators.Add(validator);
	  }

	  public virtual void removeValidator<T1>(Validator<T1> validator)
	  {
		validators.remove(validator);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @SuppressWarnings("unchecked") public java.util.List<String> listIds()
	  public virtual IList<string> listIds()
	  {
		this.resultType = ResultType.LIST_IDS;
		if (commandExecutor != null)
		{
		  return (IList<string>) commandExecutor.execute(this);
		}
		return evaluateExpressionsAndExecuteIdsList(Context.CommandContext);
	  }

	  public virtual IList<string> evaluateExpressionsAndExecuteIdsList(CommandContext commandContext)
	  {
		validate();
		evaluateExpressions();
		return !hasExcludingConditions() ? executeIdsList(commandContext) : new List<string>();
	  }

	  public virtual IList<string> executeIdsList(CommandContext commandContext)
	  {
		throw new System.NotSupportedException();
	  }

	}

}