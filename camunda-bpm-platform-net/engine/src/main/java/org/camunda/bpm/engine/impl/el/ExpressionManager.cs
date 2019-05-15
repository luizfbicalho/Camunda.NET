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
namespace org.camunda.bpm.engine.impl.el
{

	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;
	using AbstractVariableScope = org.camunda.bpm.engine.impl.core.variable.scope.AbstractVariableScope;
	using ArrayELResolver = org.camunda.bpm.engine.impl.javax.el.ArrayELResolver;
	using CompositeELResolver = org.camunda.bpm.engine.impl.javax.el.CompositeELResolver;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ExpressionFactory = org.camunda.bpm.engine.impl.javax.el.ExpressionFactory;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ListELResolver = org.camunda.bpm.engine.impl.javax.el.ListELResolver;
	using MapELResolver = org.camunda.bpm.engine.impl.javax.el.MapELResolver;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;
	using ExpressionFactoryImpl = org.camunda.bpm.engine.impl.juel.ExpressionFactoryImpl;
	using MockElResolver = org.camunda.bpm.engine.test.mock.MockElResolver;
	using VariableContext = org.camunda.bpm.engine.variable.context.VariableContext;


	/// <summary>
	/// <para>
	/// Central manager for all expressions.
	/// </para>
	/// <para>
	/// Process parsers will use this to build expression objects that are stored in
	/// the process definitions.
	/// </para>
	/// <para>
	/// Then also this class is used as an entry point for runtime evaluation of the
	/// expressions.
	/// </para>
	/// 
	/// @author Tom Baeyens
	/// @author Dave Syer
	/// @author Frederik Heremans
	/// </summary>
	public class ExpressionManager
	{
		private bool InstanceFieldsInitialized = false;

		private void InitializeInstanceFields()
		{
			parsingElContext = new ProcessEngineElContext(functionMappers);
		}



	  protected internal IList<FunctionMapper> functionMappers = new List<FunctionMapper>();
	  protected internal ExpressionFactory expressionFactory;
	  // Default implementation (does nothing)
	  protected internal ELContext parsingElContext;
	  protected internal IDictionary<object, object> beans;
	  protected internal ELResolver elResolver;

	  public ExpressionManager() : this(null)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
	  }

	  public ExpressionManager(IDictionary<object, object> beans)
	  {
		  if (!InstanceFieldsInitialized)
		  {
			  InitializeInstanceFields();
			  InstanceFieldsInitialized = true;
		  }
		// Use the ExpressionFactoryImpl built-in version of juel, with parametrised method expressions enabled
		expressionFactory = new ExpressionFactoryImpl();
		this.beans = beans;
	  }

	  public virtual Expression createExpression(string expression)
	  {
		ValueExpression valueExpression = createValueExpression(expression);
		return new JuelExpression(valueExpression, this, expression);
	  }

	  public virtual ValueExpression createValueExpression(string expression)
	  {
		return expressionFactory.createValueExpression(parsingElContext, expression, typeof(object));
	  }

	  public virtual ExpressionFactory ExpressionFactory
	  {
		  set
		  {
			this.expressionFactory = value;
		  }
	  }

	  public virtual ELContext getElContext(VariableScope variableScope)
	  {
		ELContext elContext = null;
		if (variableScope is AbstractVariableScope)
		{
		  AbstractVariableScope variableScopeImpl = (AbstractVariableScope) variableScope;
		  elContext = variableScopeImpl.CachedElContext;
		}

		if (elContext == null)
		{
		  elContext = createElContext(variableScope);
		  if (variableScope is AbstractVariableScope)
		  {
			((AbstractVariableScope)variableScope).CachedElContext = elContext;
		  }
		}

		return elContext;
	  }

	  public virtual ELContext createElContext(VariableContext variableContext)
	  {
		ELResolver elResolver = CachedElResolver;
		ProcessEngineElContext elContext = new ProcessEngineElContext(functionMappers, elResolver);
		elContext.putContext(typeof(ExpressionFactory), expressionFactory);
		elContext.putContext(typeof(VariableContext), variableContext);
		return elContext;
	  }

	  protected internal virtual ProcessEngineElContext createElContext(VariableScope variableScope)
	  {
		ELResolver elResolver = CachedElResolver;
		ProcessEngineElContext elContext = new ProcessEngineElContext(functionMappers, elResolver);
		elContext.putContext(typeof(ExpressionFactory), expressionFactory);
		elContext.putContext(typeof(VariableScope), variableScope);
		return elContext;
	  }

	  protected internal virtual ELResolver CachedElResolver
	  {
		  get
		  {
			if (elResolver == null)
			{
			  lock (this)
			  {
				if (elResolver == null)
				{
				  elResolver = createElResolver();
				}
			  }
			}
    
			return elResolver;
		  }
	  }

	  protected internal virtual ELResolver createElResolver()
	  {
		CompositeELResolver elResolver = new CompositeELResolver();
		elResolver.add(new VariableScopeElResolver());
		elResolver.add(new VariableContextElResolver());
		elResolver.add(new MockElResolver());

		if (beans != null)
		{
		  // ACT-1102: Also expose all beans in configuration when using standalone engine, not
		  // in spring-context
		  elResolver.add(new ReadOnlyMapELResolver(beans));
		}

		elResolver.add(new ProcessApplicationElResolverDelegate());

		elResolver.add(new ArrayELResolver());
		elResolver.add(new ListELResolver());
		elResolver.add(new MapELResolver());
		elResolver.add(new ProcessApplicationBeanElResolverDelegate());

		return elResolver;
	  }

	  /// <param name="elFunctionMapper"> </param>
	  public virtual void addFunctionMapper(FunctionMapper elFunctionMapper)
	  {
		this.functionMappers.Add(elFunctionMapper);
	  }
	}

}