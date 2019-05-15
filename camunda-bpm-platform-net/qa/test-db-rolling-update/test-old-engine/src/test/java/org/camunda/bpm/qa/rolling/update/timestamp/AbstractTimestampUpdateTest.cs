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
namespace org.camunda.bpm.qa.rolling.update.timestamp
{
	using ExternalTaskService = org.camunda.bpm.engine.ExternalTaskService;
	using HistoryService = org.camunda.bpm.engine.HistoryService;
	using IdentityService = org.camunda.bpm.engine.IdentityService;
	using ManagementService = org.camunda.bpm.engine.ManagementService;
	using RepositoryService = org.camunda.bpm.engine.RepositoryService;
	using RuntimeService = org.camunda.bpm.engine.RuntimeService;
	using TaskService = org.camunda.bpm.engine.TaskService;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.notNullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Nikola Koevski
	/// </summary>
	public abstract class AbstractTimestampUpdateTest : AbstractRollingUpdateTestCase
	{

	  protected internal const long TIME = 1548082136000L;
	  protected internal static readonly DateTime TIMESTAMP = new DateTime(TIME);

	  protected internal RuntimeService runtimeService;
	  protected internal ManagementService managementService;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		runtimeService = rule.RuntimeService;
		managementService = rule.ManagementService;
	  }

	  protected internal virtual void assertNotNull(object @object)
	  {
		assertThat(@object, @is(notNullValue()));
	  }
	}

}