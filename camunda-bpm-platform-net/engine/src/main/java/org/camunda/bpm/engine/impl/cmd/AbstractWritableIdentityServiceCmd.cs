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
namespace org.camunda.bpm.engine.impl.cmd
{

	using WritableIdentityProvider = org.camunda.bpm.engine.impl.identity.WritableIdentityProvider;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	[Serializable]
	public abstract class AbstractWritableIdentityServiceCmd<T> : Command<T>
	{

	  private const long serialVersionUID = 1L;

	  public T execute(CommandContext commandContext)
	  {

		// check identity service implementation
		if (!commandContext.SessionFactories.ContainsKey(typeof(WritableIdentityProvider)))
		{
		  throw new System.NotSupportedException("This identity service implementation is read-only.");
		}

		T result = executeCmd(commandContext);
		return result;
	  }

	  protected internal abstract T executeCmd(CommandContext commandContext);

	}

}