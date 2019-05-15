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
namespace org.camunda.bpm.engine.cdi.impl.annotation
{


	using ProcessVariable = org.camunda.bpm.engine.cdi.annotation.ProcessVariable;
	using ProcessVariableTyped = org.camunda.bpm.engine.cdi.annotation.ProcessVariableTyped;
	using StartProcess = org.camunda.bpm.engine.cdi.annotation.StartProcess;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using VariableMapImpl = org.camunda.bpm.engine.variable.impl.VariableMapImpl;

	/// <summary>
	/// implementation of the <seealso cref="StartProcess"/> annotation
	/// 
	/// @author Daniel Meyer
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Interceptor @StartProcess("") public class StartProcessInterceptor implements java.io.Serializable
	[StartProcess(""), Serializable]
	public class StartProcessInterceptor
	{

	  private const long serialVersionUID = 1L;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject BusinessProcess businessProcess;
	  internal BusinessProcess businessProcess;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AroundInvoke public Object invoke(javax.interceptor.InvocationContext ctx) throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
	  public virtual object invoke(InvocationContext ctx)
	  {
		try
		{
		  object result = ctx.proceed();

		  StartProcess startProcessAnnotation = ctx.Method.getAnnotation(typeof(StartProcess));

		  string key = startProcessAnnotation.value();

		  IDictionary<string, object> variables = extractVariables(startProcessAnnotation, ctx);

		  businessProcess.startProcessByKey(key, variables);

		  return result;
		}
		catch (InvocationTargetException e)
		{
		  Exception cause = e.InnerException;
		  if (cause != null && cause is Exception)
		  {
			throw (Exception) cause;
		  }
		  else
		  {
			throw e;
		  }
		}
		catch (Exception e)
		{
		  throw new ProcessEngineException("Error while starting process using @StartProcess on method  '" + ctx.Method + "': " + e.Message, e);
		}
	  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: private java.util.Map<String, Object> extractVariables(org.camunda.bpm.engine.cdi.annotation.StartProcess startProcessAnnotation, javax.interceptor.InvocationContext ctx) throws Exception
	  private IDictionary<string, object> extractVariables(StartProcess startProcessAnnotation, InvocationContext ctx)
	  {
		VariableMap variables = new VariableMapImpl();
		foreach (System.Reflection.FieldInfo field in ctx.Method.DeclaringClass.DeclaredFields)
		{
		  if (!field.isAnnotationPresent(typeof(ProcessVariable)) && !field.isAnnotationPresent(typeof(ProcessVariableTyped)))
		  {
			continue;
		  }
		  field.Accessible = true;

		  string fieldName = null;

		  ProcessVariable processStartVariable = field.getAnnotation(typeof(ProcessVariable));
		  if (processStartVariable != null)
		  {
			fieldName = processStartVariable.value();

		  }
		  else
		  {
			ProcessVariableTyped processStartVariableTyped = field.getAnnotation(typeof(ProcessVariableTyped));
			fieldName = processStartVariableTyped.value();
		  }

		  if (string.ReferenceEquals(fieldName, null) || fieldName.Length == 0)
		  {
			fieldName = field.Name;
		  }
		  object value = field.get(ctx.Target);
		  variables.put(fieldName, value);
		}

		return variables;
	  }

	}

}