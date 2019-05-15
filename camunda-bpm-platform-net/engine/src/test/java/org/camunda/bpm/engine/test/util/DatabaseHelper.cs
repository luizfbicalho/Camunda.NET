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
namespace org.camunda.bpm.engine.test.util
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

	public class DatabaseHelper
	{

	  public static int? getTransactionIsolationLevel(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final System.Nullable<int>[] transactionIsolation = new System.Nullable<int>[1];
		int?[] transactionIsolation = new int?[1];
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(transactionIsolation));
		return transactionIsolation[0];
	  }

	  private class CommandAnonymousInnerClass : Command<object>
	  {
		  private int?[] transactionIsolation;

		  public CommandAnonymousInnerClass(int?[] transactionIsolation)
		  {
			  this.transactionIsolation = transactionIsolation;
		  }

		  public object execute(CommandContext commandContext)
		  {
			try
			{
			  transactionIsolation[0] = commandContext.DbSqlSession.SqlSession.Connection.TransactionIsolation;
			}
			catch (SQLException)
			{

			}
			return null;
		  }
	  }

	  public static string getDatabaseType(ProcessEngineConfigurationImpl processEngineConfiguration)
	  {
		return processEngineConfiguration.DbSqlSessionFactory.DatabaseType;
	  }

	}

}