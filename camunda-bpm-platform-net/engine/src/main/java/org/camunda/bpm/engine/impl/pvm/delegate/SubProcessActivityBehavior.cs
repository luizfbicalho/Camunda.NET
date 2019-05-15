﻿/*
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
namespace org.camunda.bpm.engine.impl.pvm.@delegate
{
	using VariableScope = org.camunda.bpm.engine.@delegate.VariableScope;



	/// <summary>
	/// behavior for activities that delegate to a complete separate execution of
	/// a process definition.  In BPMN terminology this can be used to implement a reusable subprocess.
	/// 
	/// @author Tom Baeyens
	/// </summary>
	public interface SubProcessActivityBehavior : ActivityBehavior
	{

	  /// <summary>
	  /// Pass the output variables from the process instance of the subprocess to the given execution.
	  /// This should be called before the process instance is destroyed.
	  /// </summary>
	  /// <param name="targetExecution"> execution of the calling process instance to pass the variables to </param>
	  /// <param name="calledElementInstance"> instance of the called element that serves as the variable source </param>
	  void passOutputVariables(ActivityExecution targetExecution, VariableScope calledElementInstance);

	  /// <summary>
	  /// Called after the process instance is destroyed for
	  /// this activity to perform its outgoing control flow logic.
	  /// </summary>
	  /// <param name="execution"> </param>
	  /// <exception cref="java.lang.Exception"> </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void completed(ActivityExecution execution) throws Exception;
	  void completed(ActivityExecution execution);
	}

}