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
namespace org.camunda.bpm.engine.test.api.mgmt
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using UserOperationLogEntry = org.camunda.bpm.engine.history.UserOperationLogEntry;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using After = org.junit.After;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using RuleChain = org.junit.rules.RuleChain;

	/// <summary>
	/// @author Tobias Metzke
	/// 
	/// </summary>
	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_FULL)]
	public class PropertyUserOperationLogTest
	{
		private bool InstanceFieldsInitialized = false;

		public PropertyUserOperationLogTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			ruleChain = RuleChain.outerRule(engineRule);
		}


	  private const string USER_ID = "testUserId";
	  private const string PROPERTY_NAME = "TEST_PROPERTY";

	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

	  protected internal HistoryService historyService;
	  protected internal IdentityService identityService;
	  protected internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		historyService = engineRule.HistoryService;
		identityService = engineRule.IdentityService;
		managementService = engineRule.ManagementService;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @After public void tearDown()
	  public virtual void tearDown()
	  {
		managementService.deleteProperty(PROPERTY_NAME);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCreateProperty()
	  public virtual void testCreateProperty()
	  {
		// given
		assertThat(historyService.createUserOperationLogQuery().count(), @is(0L));

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.setProperty(PROPERTY_NAME, "testValue");
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(1L));
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(entry.EntityType, @is(EntityTypes.PROPERTY));
		assertThat(entry.Category, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN));
		assertThat(entry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_CREATE));
		assertThat(entry.Property, @is("name"));
		assertThat(entry.OrgValue, nullValue());
		assertThat(entry.NewValue, @is(PROPERTY_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUpdateProperty()
	  public virtual void testUpdateProperty()
	  {
		// given
		managementService.setProperty(PROPERTY_NAME, "testValue");
		assertThat(historyService.createUserOperationLogQuery().count(), @is(0L));

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.setProperty(PROPERTY_NAME, "testValue2");
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(1L));
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(entry.EntityType, @is(EntityTypes.PROPERTY));
		assertThat(entry.Category, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN));
		assertThat(entry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_UPDATE));
		assertThat(entry.Property, @is("name"));
		assertThat(entry.OrgValue, nullValue());
		assertThat(entry.NewValue, @is(PROPERTY_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeleteProperty()
	  public virtual void testDeleteProperty()
	  {
		// given
		managementService.setProperty(PROPERTY_NAME, "testValue");
		assertThat(historyService.createUserOperationLogQuery().count(), @is(0L));

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.deleteProperty(PROPERTY_NAME);
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(1L));
		UserOperationLogEntry entry = historyService.createUserOperationLogQuery().singleResult();
		assertThat(entry.EntityType, @is(EntityTypes.PROPERTY));
		assertThat(entry.Category, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.CATEGORY_ADMIN));
		assertThat(entry.OperationType, @is(org.camunda.bpm.engine.history.UserOperationLogEntry_Fields.OPERATION_TYPE_DELETE));
		assertThat(entry.Property, @is("name"));
		assertThat(entry.OrgValue, nullValue());
		assertThat(entry.NewValue, @is(PROPERTY_NAME));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDeletePropertyNonExisting()
	  public virtual void testDeletePropertyNonExisting()
	  {
		// given
		assertThat(historyService.createUserOperationLogQuery().count(), @is(0L));

		// when
		identityService.AuthenticatedUserId = USER_ID;
		managementService.deleteProperty(PROPERTY_NAME);
		identityService.clearAuthentication();

		// then
		assertThat(historyService.createUserOperationLogQuery().count(), @is(0L));
	  }
	}

}