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
namespace org.camunda.bpm.engine.impl.db
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.impl.util.EnsureUtil.ensureNotNull;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DbSchemaExecuteFile
	{

	  protected internal static readonly EnginePersistenceLogger LOG = ProcessEngineLogger.PERSISTENCE_LOGGER;

	  /// <param name="args"> </param>
	  public static void Main(string[] args)
	  {

		if (args.Length != 2)
		{
		  throw LOG.invokeSchemaResourceToolException(args.Length);
		}

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String configurationFileResourceName = args[0];
		string configurationFileResourceName = args[0];
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final String schemaFileResourceName = args[1];
		string schemaFileResourceName = args[1];

		ensureNotNull("Process engine configuration file name cannot be null", "configurationFileResourceName", configurationFileResourceName);
		ensureNotNull("Schema resource file name cannot be null", "schemaFileResourceName", schemaFileResourceName);

		ProcessEngineConfigurationImpl configuration = (ProcessEngineConfigurationImpl) ProcessEngineConfiguration.createProcessEngineConfigurationFromResource(configurationFileResourceName);
		ProcessEngine processEngine = configuration.buildProcessEngine();

		configuration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(schemaFileResourceName));

		processEngine.close();

	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private string schemaFileResourceName;

		  public CommandAnonymousInnerClass(string schemaFileResourceName)
		  {
			  this.schemaFileResourceName = schemaFileResourceName;
		  }


		  public Void execute(CommandContext commandContext)
		  {

			commandContext.DbSqlSession.executeSchemaResource(schemaFileResourceName);

			return null;
		  }

	  }

	}

}