﻿using System;
using System.Text;

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
namespace org.camunda.bpm.engine.impl.scripting.engine
{


	using ExpressionFactoryResolver = org.camunda.bpm.engine.impl.el.ExpressionFactoryResolver;
	using ArrayELResolver = org.camunda.bpm.engine.impl.javax.el.ArrayELResolver;
	using BeanELResolver = org.camunda.bpm.engine.impl.javax.el.BeanELResolver;
	using CompositeELResolver = org.camunda.bpm.engine.impl.javax.el.CompositeELResolver;
	using ELContext = org.camunda.bpm.engine.impl.javax.el.ELContext;
	using ELException = org.camunda.bpm.engine.impl.javax.el.ELException;
	using ELResolver = org.camunda.bpm.engine.impl.javax.el.ELResolver;
	using ExpressionFactory = org.camunda.bpm.engine.impl.javax.el.ExpressionFactory;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ListELResolver = org.camunda.bpm.engine.impl.javax.el.ListELResolver;
	using MapELResolver = org.camunda.bpm.engine.impl.javax.el.MapELResolver;
	using ResourceBundleELResolver = org.camunda.bpm.engine.impl.javax.el.ResourceBundleELResolver;
	using ValueExpression = org.camunda.bpm.engine.impl.javax.el.ValueExpression;
	using VariableMapper = org.camunda.bpm.engine.impl.javax.el.VariableMapper;
	using SimpleResolver = org.camunda.bpm.engine.impl.juel.SimpleResolver;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;


	/// <summary>
	/// ScriptEngine that used JUEL for script evaluation and compilation (JSR-223).
	/// 
	/// Uses EL 1.1 if available, to resolve expressions. Otherwise it reverts to EL
	/// 1.0, using <seealso cref="ExpressionFactoryResolver"/>.
	/// 
	/// @author Frederik Heremans
	/// </summary>
	public class JuelScriptEngine : AbstractScriptEngine
	{

	  private ScriptEngineFactory scriptEngineFactory;
	  private ExpressionFactory expressionFactory;

	  public JuelScriptEngine(ScriptEngineFactory scriptEngineFactory)
	  {
		this.scriptEngineFactory = scriptEngineFactory;
		// Resolve the ExpressionFactory
		expressionFactory = ExpressionFactoryResolver.resolveExpressionFactory();
	  }

	  public JuelScriptEngine() : this(null)
	  {
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object eval(String script, javax.script.ScriptContext scriptContext) throws javax.script.ScriptException
	  public virtual object eval(string script, ScriptContext scriptContext)
	  {
		ValueExpression expr = parse(script, scriptContext);
		return evaluateExpression(expr, scriptContext);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object eval(java.io.Reader reader, javax.script.ScriptContext scriptContext) throws javax.script.ScriptException
	  public virtual object eval(Reader reader, ScriptContext scriptContext)
	  {
		return eval(readFully(reader), scriptContext);
	  }

	  public virtual ScriptEngineFactory Factory
	  {
		  get
		  {
			lock (this)
			{
			  if (scriptEngineFactory == null)
			  {
				scriptEngineFactory = new JuelScriptEngineFactory();
			  }
			}
			return scriptEngineFactory;
		  }
	  }

	  public virtual Bindings createBindings()
	  {
		return new SimpleBindings();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private Object evaluateExpression(org.camunda.bpm.engine.impl.javax.el.ValueExpression expr, javax.script.ScriptContext ctx) throws javax.script.ScriptException
	  private object evaluateExpression(ValueExpression expr, ScriptContext ctx)
	  {
		try
		{
		  return expr.getValue(createElContext(ctx));
		}
		catch (ELException elexp)
		{
		  throw new ScriptException(elexp);
		}
	  }

	  private ELResolver createElResolver()
	  {
		CompositeELResolver compositeResolver = new CompositeELResolver();
		compositeResolver.add(new ArrayELResolver());
		compositeResolver.add(new ListELResolver());
		compositeResolver.add(new MapELResolver());
		compositeResolver.add(new ResourceBundleELResolver());
		compositeResolver.add(new BeanELResolver());
		return new SimpleResolver(compositeResolver);
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private String readFully(java.io.Reader reader) throws javax.script.ScriptException
	  private string readFully(Reader reader)
	  {
		char[] array = new char[8192];
		StringBuilder strBuffer = new StringBuilder();
		int count;
		try
		{
		  while ((count = reader.read(array, 0, array.Length)) > 0)
		  {
			strBuffer.Append(array, 0, count);
		  }
		}
		catch (IOException exp)
		{
		  throw new ScriptException(exp);
		}
		return strBuffer.ToString();
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private org.camunda.bpm.engine.impl.javax.el.ValueExpression parse(String script, javax.script.ScriptContext scriptContext) throws javax.script.ScriptException
	  private ValueExpression parse(string script, ScriptContext scriptContext)
	  {
		try
		{
		  return expressionFactory.createValueExpression(createElContext(scriptContext), script, typeof(object));
		}
		catch (ELException ele)
		{
		  throw new ScriptException(ele);
		}
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private org.camunda.bpm.engine.impl.javax.el.ELContext createElContext(final javax.script.ScriptContext scriptCtx)
	  private ELContext createElContext(ScriptContext scriptCtx)
	  {
		// Check if the ELContext is already stored on the ScriptContext
		object existingELCtx = scriptCtx.getAttribute("elcontext");
		if (existingELCtx is ELContext)
		{
		  return (ELContext) existingELCtx;
		}

		scriptCtx.setAttribute("context", scriptCtx, ScriptContext.ENGINE_SCOPE);

		// Built-in function are added to ScriptCtx
		scriptCtx.setAttribute("out:print", PrintMethod, ScriptContext.ENGINE_SCOPE);

		SecurityManager securityManager = System.SecurityManager;
		if (securityManager == null)
		{
		  scriptCtx.setAttribute("lang:import", ImportMethod, ScriptContext.ENGINE_SCOPE);
		}

		ELContext elContext = new ELContextAnonymousInnerClass(this, scriptCtx);
		// Store the elcontext in the scriptContext to be able to reuse
		scriptCtx.setAttribute("elcontext", elContext, ScriptContext.ENGINE_SCOPE);
		return elContext;
	  }

	  private class ELContextAnonymousInnerClass : ELContext
	  {
		  private readonly JuelScriptEngine outerInstance;

		  private ScriptContext scriptCtx;

		  public ELContextAnonymousInnerClass(JuelScriptEngine outerInstance, ScriptContext scriptCtx)
		  {
			  this.outerInstance = outerInstance;
			  this.scriptCtx = scriptCtx;
			  resolver = outerInstance.createElResolver();
			  varMapper = new ScriptContextVariableMapper(outerInstance, scriptCtx);
			  funcMapper = new ScriptContextFunctionMapper(outerInstance, scriptCtx);
		  }


		  internal ELResolver resolver;
		  internal VariableMapper varMapper;
		  internal FunctionMapper funcMapper;

		  public override ELResolver ELResolver
		  {
			  get
			  {
				return resolver;
			  }
		  }

		  public override VariableMapper VariableMapper
		  {
			  get
			  {
				return varMapper;
			  }
		  }

		  public override FunctionMapper FunctionMapper
		  {
			  get
			  {
				return funcMapper;
			  }
		  }
	  }

	  private static System.Reflection.MethodInfo PrintMethod
	  {
		  get
		  {
			try
			{
			  return typeof(JuelScriptEngine).GetMethod("print", new Type[] {typeof(object)});
			}
			catch (Exception)
			{
			  // Will never occur
			  return null;
			}
		  }
	  }

	  private static System.Reflection.MethodInfo ImportMethod
	  {
		  get
		  {
			try
			{
			  return typeof(JuelScriptEngine).GetMethod("importFunctions", new Type[] {typeof(ScriptContext), typeof(string), typeof(object)});
			}
			catch (Exception)
			{
			  // Will never occur
			  return null;
			}
		  }
	  }

	  public static void importFunctions(ScriptContext ctx, string @namespace, object obj)
	  {
		Type clazz = null;
		if (obj is Type)
		{
		  clazz = (Type) obj;
		}
		else if (obj is string)
		{
		  try
		  {
			clazz = ReflectUtil.loadClass((string) obj);
		  }
		  catch (ProcessEngineException ae)
		  {
			throw new ELException(ae);
		  }
		}
		else
		{
		  throw new ELException("Class or class name is missing");
		}
		System.Reflection.MethodInfo[] methods = clazz.GetMethods();
		foreach (System.Reflection.MethodInfo m in methods)
		{
		  int mod = m.Modifiers;
		  if (Modifier.isStatic(mod) && Modifier.isPublic(mod))
		  {
			string name = @namespace + ":" + m.Name;
			ctx.setAttribute(name, m, ScriptContext.ENGINE_SCOPE);
		  }
		}
	  }

	  /// <summary>
	  /// Class representing a compiled script using JUEL.
	  /// 
	  /// @author Frederik Heremans
	  /// </summary>
	  private class JuelCompiledScript : CompiledScript
	  {
		  private readonly JuelScriptEngine outerInstance;


		internal ValueExpression valueExpression;

		internal JuelCompiledScript(JuelScriptEngine outerInstance, ValueExpression valueExpression)
		{
			this.outerInstance = outerInstance;
		  this.valueExpression = valueExpression;
		}

		public virtual ScriptEngine Engine
		{
			get
			{
			  // Return outer class instance
			  return outerInstance;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public Object eval(javax.script.ScriptContext ctx) throws javax.script.ScriptException
		public virtual object eval(ScriptContext ctx)
		{
		  return outerInstance.evaluateExpression(valueExpression, ctx);
		}
	  }

	  /// <summary>
	  /// ValueMapper that uses the ScriptContext to get variable values or value
	  /// expressions.
	  /// 
	  /// @author Frederik Heremans
	  /// </summary>
	  private class ScriptContextVariableMapper : VariableMapper
	  {
		  private readonly JuelScriptEngine outerInstance;


		internal ScriptContext scriptContext;

		internal ScriptContextVariableMapper(JuelScriptEngine outerInstance, ScriptContext scriptCtx)
		{
			this.outerInstance = outerInstance;
		  this.scriptContext = scriptCtx;
		}

		public override ValueExpression resolveVariable(string variableName)
		{
		  int scope = scriptContext.getAttributesScope(variableName);
		  if (scope != -1)
		  {
			object value = scriptContext.getAttribute(variableName, scope);
			if (value is ValueExpression)
			{
			  // Just return the existing ValueExpression
			  return (ValueExpression) value;
			}
			else
			{
			  // Create a new ValueExpression based on the variable value
			  return outerInstance.expressionFactory.createValueExpression(value, typeof(object));
			}
		  }
		  return null;
		}

		public override ValueExpression setVariable(string name, ValueExpression value)
		{
		  ValueExpression previousValue = resolveVariable(name);
		  scriptContext.setAttribute(name, value, ScriptContext.ENGINE_SCOPE);
		  return previousValue;
		}
	  }

	  /// <summary>
	  /// FunctionMapper that uses the ScriptContext to resolve functions in EL.
	  /// 
	  /// @author Frederik Heremans
	  /// </summary>
	  private class ScriptContextFunctionMapper : FunctionMapper
	  {
		  private readonly JuelScriptEngine outerInstance;


		internal ScriptContext scriptContext;

		internal ScriptContextFunctionMapper(JuelScriptEngine outerInstance, ScriptContext ctx)
		{
			this.outerInstance = outerInstance;
		  this.scriptContext = ctx;
		}

		internal virtual string getFullFunctionName(string prefix, string localName)
		{
		  return prefix + ":" + localName;
		}

		public override System.Reflection.MethodInfo resolveFunction(string prefix, string localName)
		{
		  string functionName = getFullFunctionName(prefix, localName);
		  int scope = scriptContext.getAttributesScope(functionName);
		  if (scope != -1)
		  {
			// Methods are added as variables in the ScriptScope
			object attributeValue = scriptContext.getAttribute(functionName);
			return (attributeValue is System.Reflection.MethodInfo) ? (System.Reflection.MethodInfo) attributeValue : null;
		  }
		  else
		  {
			return null;
		  }
		}
	  }

	}

}