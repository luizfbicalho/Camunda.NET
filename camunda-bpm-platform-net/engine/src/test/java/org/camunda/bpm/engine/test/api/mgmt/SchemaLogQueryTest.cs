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
namespace org.camunda.bpm.engine.test.api.mgmt
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.inverted;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.schemaLogEntryByTimestamp;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.test.api.runtime.TestOrderingUtil.verifySorting;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.not;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.greaterThan;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;


	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using Context = org.camunda.bpm.engine.impl.context.Context;
	using DbEntityManager = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManager;
	using DbEntityManagerFactory = org.camunda.bpm.engine.impl.db.entitymanager.DbEntityManagerFactory;
	using Command = org.camunda.bpm.engine.impl.interceptor.Command;
	using CommandContext = org.camunda.bpm.engine.impl.interceptor.CommandContext;
	using SchemaLogEntryEntity = org.camunda.bpm.engine.impl.persistence.entity.SchemaLogEntryEntity;
	using SchemaLogEntry = org.camunda.bpm.engine.management.SchemaLogEntry;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Miklas Boskamp
	/// 
	/// </summary>
	public class SchemaLogQueryTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule();
	  public ProcessEngineRule engineRule = new ProcessEngineRule();
	  internal ManagementService managementService;
	  private ProcessEngineConfigurationImpl processEngineConfiguration;

	  internal SchemaLogEntryEntity dummySchemaLogEntry;
	  internal long initialEntryCount;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		managementService = engineRule.ManagementService;
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

		initialEntryCount = managementService.createSchemaLogQuery().count();
		dummySchemaLogEntry = createDummySchemaLogEntry();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testQuerySchemaLogEntryList()
	  public virtual void testQuerySchemaLogEntryList()
	  {
		// given

		// when
		IList<SchemaLogEntry> schemaLogEntries = managementService.createSchemaLogQuery().list();

		// then expect one entry
		assertThat(managementService.createSchemaLogQuery().count(), @is(greaterThan(0L)));
		SchemaLogEntry schemaLogEntry = schemaLogEntries[0];
		assertThat(schemaLogEntry.Id, @is("0"));
		assertThat(schemaLogEntry.Timestamp, notNullValue());
		assertThat(schemaLogEntry.Version, notNullValue());
		managementService.Properties;
		cleanupTable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOrderByTimestamp()
	  public virtual void testOrderByTimestamp()
	  {
		// given (at least) two schema log entries
		populateTable();

		// then sorting works
		verifySorting(managementService.createSchemaLogQuery().orderByTimestamp().asc().list(), schemaLogEntryByTimestamp());
		verifySorting(managementService.createSchemaLogQuery().orderByTimestamp().desc().list(), inverted(schemaLogEntryByTimestamp()));

		cleanupTable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFilterByVersion()
	  public virtual void testFilterByVersion()
	  {
		// given (at least) two schema log entries
		populateTable();

		// when
		SchemaLogEntry schemaLogEntry = managementService.createSchemaLogQuery().version("dummyVersion").singleResult();

		// then
		assertThat(schemaLogEntry.Id, @is(dummySchemaLogEntry.Id));
		assertThat(schemaLogEntry.Timestamp, notNullValue());
		assertThat(schemaLogEntry.Version, @is(dummySchemaLogEntry.Version));

		cleanupTable();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSortedPagedQuery()
	  public virtual void testSortedPagedQuery()
	  {
		// given (at least) two schema log entries
		populateTable();

		// then paging works
		// ascending order
		IList<SchemaLogEntry> schemaLogEntry = managementService.createSchemaLogQuery().orderByTimestamp().asc().listPage(0, 1);
		assertThat(schemaLogEntry.Count, @is(1));
		assertThat(schemaLogEntry[0].Id, @is("0"));

		schemaLogEntry = managementService.createSchemaLogQuery().orderByTimestamp().asc().listPage(1, 1);
		assertThat(schemaLogEntry.Count, @is(1));
		assertThat(schemaLogEntry[0].Id, @is(not("0")));

		// descending order
		schemaLogEntry = managementService.createSchemaLogQuery().orderByTimestamp().desc().listPage(0, 1);
		assertThat(schemaLogEntry.Count, @is(1));
		assertThat(schemaLogEntry[0].Id, @is(not("0")));

		schemaLogEntry = managementService.createSchemaLogQuery().orderByTimestamp().desc().listPage(1, 1);
		assertThat(schemaLogEntry.Count, @is(1));
		assertThat(schemaLogEntry[0].Id, @is("0"));

		cleanupTable();
	  }

	  private void populateTable()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass(this));
		assertThat(managementService.createSchemaLogQuery().count(), @is(initialEntryCount + 1));
	  }

	  private class CommandAnonymousInnerClass : Command<Void>
	  {
		  private readonly SchemaLogQueryTest outerInstance;

		  public CommandAnonymousInnerClass(SchemaLogQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {

			DbEntityManagerFactory dbEntityManagerFactory = new DbEntityManagerFactory(Context.ProcessEngineConfiguration.IdGenerator);
			DbEntityManager newEntityManager = dbEntityManagerFactory.openSession();
			newEntityManager.insert(outerInstance.dummySchemaLogEntry);
			newEntityManager.flush();
			return null;
		  }
	  }

	  private void cleanupTable()
	  {
		processEngineConfiguration.CommandExecutorTxRequired.execute(new CommandAnonymousInnerClass2(this));
		assertThat(managementService.createSchemaLogQuery().count(), @is(initialEntryCount));
	  }

	  private class CommandAnonymousInnerClass2 : Command<Void>
	  {
		  private readonly SchemaLogQueryTest outerInstance;

		  public CommandAnonymousInnerClass2(SchemaLogQueryTest outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }

		  public Void execute(CommandContext commandContext)
		  {
			DbEntityManager dbEntityManager = commandContext.DbEntityManager;
			dbEntityManager.delete(outerInstance.dummySchemaLogEntry);
			dbEntityManager.flush();
			return null;
		  }
	  }

	  private SchemaLogEntryEntity createDummySchemaLogEntry()
	  {
		SchemaLogEntryEntity dummy = new SchemaLogEntryEntity();
		dummy.Id = "uniqueId";
		dummy.Timestamp = DateTime.Now;
		dummy.Version = "dummyVersion";
		return dummy;
	  }
	}
}