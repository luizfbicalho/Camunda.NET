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
namespace org.camunda.bpm.engine.impl.el
{

	using Context = org.camunda.bpm.engine.impl.context.Context;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using FunctionMapper = org.camunda.bpm.engine.impl.javax.el.FunctionMapper;
	using ReflectUtil = org.camunda.bpm.engine.impl.util.ReflectUtil;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class CommandContextFunctionMapper : FunctionMapper
	{

	  public static IDictionary<string, System.Reflection.MethodInfo> COMMAND_CONTEXT_FUNCTION_MAP = null;

	  public override System.Reflection.MethodInfo resolveFunction(string prefix, string localName)
	  {
		// Context functions are used un-prefixed
		ensureContextFunctionMapInitialized();
		return COMMAND_CONTEXT_FUNCTION_MAP[localName];
	  }

	  protected internal virtual void ensureContextFunctionMapInitialized()
	  {
		if (COMMAND_CONTEXT_FUNCTION_MAP == null)
		{
		  lock (typeof(CommandContextFunctionMapper))
		  {
			if (COMMAND_CONTEXT_FUNCTION_MAP == null)
			{
			  COMMAND_CONTEXT_FUNCTION_MAP = new Dictionary<string, System.Reflection.MethodInfo>();
			  createMethodBindings();
			}
		  }
		}
	  }

	  protected internal virtual void createMethodBindings()
	  {
		Type mapperClass = this.GetType();
		COMMAND_CONTEXT_FUNCTION_MAP["currentUser"] = ReflectUtil.getMethod(mapperClass, "currentUser");
		COMMAND_CONTEXT_FUNCTION_MAP["currentUserGroups"] = ReflectUtil.getMethod(mapperClass, "currentUserGroups");
	  }

	  public static string currentUser()
	  {
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  return commandContext.AuthenticatedUserId;
		}
		else
		{
		  return null;
		}
	  }

	  public static IList<string> currentUserGroups()
	  {
		CommandContext commandContext = Context.CommandContext;
		if (commandContext != null)
		{
		  return commandContext.AuthenticatedGroupIds;
		}
		else
		{
		  return null;
		}
	  }

	}

}