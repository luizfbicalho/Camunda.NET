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
namespace org.camunda.bpm.engine.@delegate
{
	/// <summary>
	/// <para>A variable listener can be defined on a scope in a case model.
	/// Depending on its configuration, it is invoked when a variable is create/updated/deleted
	/// on a case execution that corresponds to that scope or to any of its descendant scopes.</para>
	/// 
	/// <para>
	/// <strong>Beware:</strong> If you set a variable inside a <seealso cref="VariableListener"/> implementation,
	/// this will result in new variable listener invocations. Make sure that your implementation
	/// allows to exit such a cascade as otherwise there will be an <strong>infinite loop</strong>.
	/// </para>
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public interface CaseVariableListener : VariableListener<DelegateCaseVariableInstance>
	{

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: void notify(DelegateCaseVariableInstance variableInstance) throws Exception;
	  void notify(DelegateCaseVariableInstance variableInstance);
	}

	public static class CaseVariableListener_Fields
	{
	  public const string CREATE = VariableListener_Fields.CREATE;
	  public const string UPDATE = VariableListener_Fields.UPDATE;
	  public const string DELETE = VariableListener_Fields.DELETE;
	}

}