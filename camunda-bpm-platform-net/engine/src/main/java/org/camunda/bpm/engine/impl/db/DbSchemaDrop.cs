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
namespace org.camunda.bpm.engine.impl.db
{
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class DbSchemaDrop
	{

	  public static void Main(string[] args)
	  {
		ProcessEngineImpl processEngine = (ProcessEngineImpl) ProcessEngines.DefaultProcessEngine;
		CommandExecutor commandExecutor = processEngine.ProcessEngineConfiguration.CommandExecutorTxRequired;
		commandExecutor.execute(new CommandAnonymousInnerClass());
		processEngine.close();
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  public object execute(CommandContext commandContext)
		  {
			commandContext.getSession(typeof(PersistenceSession)).dbSchemaDrop();
			return null;
		  }
	  }
	}

}