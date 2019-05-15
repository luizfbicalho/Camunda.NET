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
namespace org.camunda.bpm.engine.test.api.authorization
{

	using Groups = org.camunda.bpm.engine.authorization.Groups;
	using ProcessDefinitionEntity = org.camunda.bpm.engine.impl.persistence.entity.ProcessDefinitionEntity;
	using TableMetaData = org.camunda.bpm.engine.management.TableMetaData;
	using TablePage = org.camunda.bpm.engine.management.TablePage;

	/// <summary>
	/// @author Roman Smirnov
	/// 
	/// </summary>
	public class ManagementAuthorizationTest : AuthorizationTest
	{

	  private const string REQUIRED_ADMIN_AUTH_EXCEPTION = "ENGINE-03029 Required admin authenticated group or user.";

	  // get table count //////////////////////////////////////////////

	  public virtual void testGetTableCountWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.TableCount;
		  fail("Exception expected: It should not be possible to get the table count");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  public virtual void testGetTableCountAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		// when
		IDictionary<string, long> tableCount = managementService.TableCount;

		// then
		assertFalse(tableCount.Count == 0);
	  }

	  // get table name //////////////////////////////////////////////

	  public virtual void testGetTableNameWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.getTableName(typeof(ProcessDefinitionEntity));
		  fail("Exception expected: It should not be possible to get the table name");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  public virtual void testGetTableNameAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		// when
		string tableName = managementService.getTableName(typeof(ProcessDefinitionEntity));

		// then
		assertEquals(tablePrefix + "ACT_RE_PROCDEF", tableName);
	  }

	  // get table meta data //////////////////////////////////////////////

	  public virtual void testGetTableMetaDataWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.getTableMetaData("ACT_RE_PROCDEF");
		  fail("Exception expected: It should not be possible to get the table meta data");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  public virtual void testGetTableMetaDataAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		// when
		TableMetaData tableMetaData = managementService.getTableMetaData("ACT_RE_PROCDEF");

		// then
		assertNotNull(tableMetaData);
	  }

	  // table page query //////////////////////////////////

	  public virtual void testTablePageQueryWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.createTablePageQuery().tableName("ACT_RE_PROCDEF").listPage(0, int.MaxValue);
		  fail("Exception expected: It should not be possible to get a table page");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}

	  }

	  public virtual void testTablePageQueryAsCamundaAdmin()
	  {
		// given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));
		string tablePrefix = processEngineConfiguration.DatabaseTablePrefix;

		// when
		TablePage page = managementService.createTablePageQuery().tableName(tablePrefix + "ACT_RE_PROCDEF").listPage(0, int.MaxValue);

		// then
		assertNotNull(page);
	  }

	  // get history level /////////////////////////////////

	  public virtual void testGetHistoryLevelWithoutAuthorization()
	  {
		//given

		try
		{
		  // when
		  managementService.HistoryLevel;
		  fail("Exception expected: It should not be possible to get the history level");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  public virtual void testGetHistoryLevelAsCamundaAdmin()
	  {
		//given
		identityService.setAuthentication(userId, Collections.singletonList(org.camunda.bpm.engine.authorization.Groups_Fields.CAMUNDA_ADMIN));

		// when
		int historyLevel = managementService.HistoryLevel;

		// then
		assertEquals(processEngineConfiguration.HistoryLevel.Id, historyLevel);
	  }

	  // database schema upgrade ///////////////////////////

	  public virtual void testDataSchemaUpgradeWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.databaseSchemaUpgrade(null, null, null);
		  fail("Exception expected: It should not be possible to upgrade the database schema");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  // get properties & set/delete property ///////////////////////////

	  public virtual void testGetPropertiesWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.Properties;
		  fail("Exception expected: It should not be possible to get properties");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  public virtual void testSetPropertyWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.setProperty("aPropertyKey", "aPropertyValue");
		  fail("Exception expected: It should not be possible to set a property");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	  public virtual void testDeletePropertyWithoutAuthorization()
	  {
		// given

		try
		{
		  // when
		  managementService.deleteProperty("aPropertyName");
		  fail("Exception expected: It should not be possible to delete a property");
		}
		catch (AuthorizationException e)
		{
		  // then
		  string message = e.Message;
		  assertTextPresent(REQUIRED_ADMIN_AUTH_EXCEPTION, message);
		}
	  }

	}

}