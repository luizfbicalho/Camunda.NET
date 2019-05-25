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
namespace org.camunda.bpm.engine.cdi
{


	using ProcessVariable = org.camunda.bpm.engine.cdi.annotation.ProcessVariable;
	using ProcessVariableLocal = org.camunda.bpm.engine.cdi.annotation.ProcessVariableLocal;
	using ProcessVariableLocalTyped = org.camunda.bpm.engine.cdi.annotation.ProcessVariableLocalTyped;
	using ProcessVariableTyped = org.camunda.bpm.engine.cdi.annotation.ProcessVariableTyped;
	using ProcessVariableLocalMap = org.camunda.bpm.engine.cdi.impl.ProcessVariableLocalMap;
	using ProcessVariableMap = org.camunda.bpm.engine.cdi.impl.ProcessVariableMap;
	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using TypedValue = org.camunda.bpm.engine.variable.value.TypedValue;

	/// <summary>
	/// Allows to access the process variables of a managed process instance.
	/// A process instance can be managed, using the <seealso cref="BusinessProcess"/>-bean.
	/// 
	/// @author Daniel Meyer
	/// </summary>
	public class ProcessVariables
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private Logger logger = Logger.getLogger(typeof(ProcessVariables).FullName);

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private BusinessProcess businessProcess;
	  private BusinessProcess businessProcess;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.cdi.impl.ProcessVariableMap processVariableMap;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  private ProcessVariableMap processVariableMap_Conflict;
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Inject private org.camunda.bpm.engine.cdi.impl.ProcessVariableLocalMap processVariableLocalMap;
	  private ProcessVariableLocalMap processVariableLocalMap;

	  protected internal virtual string getVariableName(InjectionPoint ip)
	  {
		string variableName = ip.Annotated.getAnnotation(typeof(ProcessVariable)).value();
		if (variableName.Length == 0)
		{
		  variableName = ip.Member.Name;
		}
		return variableName;
	  }

	  protected internal virtual string getVariableTypedName(InjectionPoint ip)
	  {
		string variableName = ip.Annotated.getAnnotation(typeof(ProcessVariableTyped)).value();
		if (variableName.Length == 0)
		{
		  variableName = ip.Member.Name;
		}
		return variableName;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessVariable protected Object getProcessVariable(javax.enterprise.inject.spi.InjectionPoint ip)
	  protected internal virtual object getProcessVariable(InjectionPoint ip)
	  {
		string processVariableName = getVariableName(ip);

		if (logger.isLoggable(Level.FINE))
		{
		  logger.fine("Getting process variable '" + processVariableName + "' from ProcessInstance[" + businessProcess.ProcessInstanceId + "].");
		}

		return businessProcess.getVariable(processVariableName);
	  }

	  /// <summary>
	  /// @since 7.3
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessVariableTyped protected org.camunda.bpm.engine.variable.value.TypedValue getProcessVariableTyped(javax.enterprise.inject.spi.InjectionPoint ip)
	  protected internal virtual TypedValue getProcessVariableTyped(InjectionPoint ip)
	  {
		string processVariableName = getVariableTypedName(ip);

		if (logger.isLoggable(Level.FINE))
		{
		  logger.fine("Getting typed process variable '" + processVariableName + "' from ProcessInstance[" + businessProcess.ProcessInstanceId + "].");
		}

		return businessProcess.getVariableTyped(processVariableName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named protected java.util.Map<String, Object> processVariables()
	  protected internal virtual IDictionary<string, object> processVariables()
	  {
		return processVariableMap_Conflict;
	  }

	  /// <summary>
	  /// @since 7.3
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named protected org.camunda.bpm.engine.variable.VariableMap processVariableMap()
	  protected internal virtual VariableMap processVariableMap()
	  {
		return processVariableMap_Conflict;
	  }

	  protected internal virtual string getVariableLocalName(InjectionPoint ip)
	  {
		string variableName = ip.Annotated.getAnnotation(typeof(ProcessVariableLocal)).value();
		if (variableName.Length == 0)
		{
		  variableName = ip.Member.Name;
		}
		return variableName;
	  }

	  protected internal virtual string getVariableLocalTypedName(InjectionPoint ip)
	  {
		string variableName = ip.Annotated.getAnnotation(typeof(ProcessVariableLocalTyped)).value();
		if (variableName.Length == 0)
		{
		  variableName = ip.Member.Name;
		}
		return variableName;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessVariableLocal protected Object getProcessVariableLocal(javax.enterprise.inject.spi.InjectionPoint ip)
	  protected internal virtual object getProcessVariableLocal(InjectionPoint ip)
	  {
		string processVariableName = getVariableLocalName(ip);

		if (logger.isLoggable(Level.FINE))
		{
		  logger.fine("Getting local process variable '" + processVariableName + "' from ProcessInstance[" + businessProcess.ProcessInstanceId + "].");
		}

		return businessProcess.getVariableLocal(processVariableName);
	  }

	  /// <summary>
	  /// @since 7.3
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @ProcessVariableLocalTyped protected org.camunda.bpm.engine.variable.value.TypedValue getProcessVariableLocalTyped(javax.enterprise.inject.spi.InjectionPoint ip)
	  protected internal virtual TypedValue getProcessVariableLocalTyped(InjectionPoint ip)
	  {
		string processVariableName = getVariableLocalTypedName(ip);

		if (logger.isLoggable(Level.FINE))
		{
		  logger.fine("Getting local typed process variable '" + processVariableName + "' from ProcessInstance[" + businessProcess.ProcessInstanceId + "].");
		}

		return businessProcess.getVariableLocalTyped(processVariableName);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named protected java.util.Map<String, Object> processVariablesLocal()
	  protected internal virtual IDictionary<string, object> processVariablesLocal()
	  {
		return processVariableLocalMap;
	  }

	  /// <summary>
	  /// @since 7.3
	  /// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Produces @Named protected org.camunda.bpm.engine.variable.VariableMap processVariableMapLocal()
	  protected internal virtual VariableMap processVariableMapLocal()
	  {
		return processVariableLocalMap;
	  }


	}

}