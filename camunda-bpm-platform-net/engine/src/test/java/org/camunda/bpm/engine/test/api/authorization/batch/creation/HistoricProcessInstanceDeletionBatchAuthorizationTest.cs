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
namespace org.camunda.bpm.engine.test.api.authorization.batch.creation
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario.scenario;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.authorization.util.AuthorizationSpec.grant;


	using BatchPermissions = org.camunda.bpm.engine.authorization.BatchPermissions;
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using Resources = org.camunda.bpm.engine.authorization.Resources;
	using AuthorizationScenario = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario;
	using AuthorizationTestRule = org.camunda.bpm.engine.test.api.authorization.util.AuthorizationTestRule;
	using Test = org.junit.Test;
	using Parameterized = org.junit.runners.Parameterized;

	public class HistoricProcessInstanceDeletionBatchAuthorizationTest : BatchCreationAuthorizationTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Parameterized.Parameters(name = "Scenario {index}") public static java.util.Collection<org.camunda.bpm.engine.test.api.authorization.util.AuthorizationScenario[]> scenarios()
	  public static ICollection<AuthorizationScenario[]> scenarios()
	  {
		return AuthorizationTestRule.asParameters(scenario().withoutAuthorizations().failsDueToRequired(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE), grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES)), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", Permissions.CREATE)), scenario().withAuthorizations(grant(Resources.BATCH, "batchId", "userId", BatchPermissions.CREATE_BATCH_DELETE_FINISHED_PROCESS_INSTANCES)).succeeds());
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT) public void testBatchHistoricProcessInstanceDeletion()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_AUDIT)]
	  public virtual void testBatchHistoricProcessInstanceDeletion()
	  {
		IList<string> historicProcessInstances = setupHistory();

		//given
		authRule.init(scenario).withUser("userId").bindResource("batchId", "*").start();

		// when
		historyService.deleteHistoricProcessInstancesAsync(historicProcessInstances, TEST_REASON);

		// then
		authRule.assertScenario(scenario);
	  }
	}

}