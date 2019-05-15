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
namespace org.camunda.bpm.engine.test.history.useroperationlog
{

	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using CommandExecutor = org.camunda.bpm.engine.impl.interceptor.CommandExecutor;
	using UserOperationLogContext = org.camunda.bpm.engine.impl.oplog.UserOperationLogContext;
	using UserOperationLogContextEntry = org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntry;
	using PropertyChange = org.camunda.bpm.engine.impl.persistence.entity.PropertyChange;
	using ProcessEngineBootstrapRule = org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using Before = org.junit.Before;
	using ClassRule = org.junit.ClassRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	public class CustomUserOperationLogTest
	{

		public const string USER_ID = "demo";

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ClassRule public static org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule bootstrapRule = new org.camunda.bpm.engine.test.util.ProcessEngineBootstrapRule("org/camunda/bpm/engine/test/history/useroperationlog/enable.legacy.user.operation.log.camunda.cfg.xml");
		public static ProcessEngineBootstrapRule bootstrapRule = new ProcessEngineBootstrapRule("org/camunda/bpm/engine/test/history/useroperationlog/enable.legacy.user.operation.log.camunda.cfg.xml");


		private static readonly string TASK_ID = System.Guid.randomUUID().ToString();

		private CommandExecutor commandExecutor;
		private HistoryService historyService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void setUp()
		{
			commandExecutor = ((ProcessEngineConfigurationImpl)bootstrapRule.ProcessEngine.ProcessEngineConfiguration).CommandExecutorTxRequired;
			historyService = bootstrapRule.ProcessEngine.HistoryService;
		}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDoNotOverwriteUserId() throws Exception
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
		public virtual void testDoNotOverwriteUserId()
		{
			commandExecutor.execute(new CommandAnonymousInnerClass(this));

			// and check its there
			assertThat(historyService.createUserOperationLogQuery().taskId(TASK_ID).singleResult().UserId, @is("kermit"));
		}

		private class CommandAnonymousInnerClass : Command<Void>
		{
			private readonly CustomUserOperationLogTest outerInstance;

			public CommandAnonymousInnerClass(CustomUserOperationLogTest outerInstance)
			{
				this.outerInstance = outerInstance;
			}

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public Void execute(final org.camunda.bpm.engine.impl.interceptor.CommandContext commandContext)
			public Void execute(CommandContext commandContext)
			{
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.oplog.UserOperationLogContext userOperationLogContext = new org.camunda.bpm.engine.impl.oplog.UserOperationLogContext();
				UserOperationLogContext userOperationLogContext = new UserOperationLogContext();
				userOperationLogContext.UserId = "kermit";

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntry entry = new org.camunda.bpm.engine.impl.oplog.UserOperationLogContextEntry("foo", "bar");
				UserOperationLogContextEntry entry = new UserOperationLogContextEntry("foo", "bar");
				entry.PropertyChanges = Arrays.asList(new PropertyChange(null, null, null));
				entry.TaskId = TASK_ID;
				userOperationLogContext.addEntry(entry);

				commandContext.OperationLogManager.logUserOperations(userOperationLogContext);
				return null;
			}
		}
	}

}