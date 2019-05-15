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
	using Permissions = org.camunda.bpm.engine.authorization.Permissions;
	using StandaloneInMemProcessEngineConfiguration = org.camunda.bpm.engine.impl.cfg.StandaloneInMemProcessEngineConfiguration;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.*;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.*;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class DefaultUserPermissionNameForTaskCfgTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void updateIsDefaultTaskPermission()
	  public virtual void updateIsDefaultTaskPermission()
	  {
		assertEquals("UPDATE", (new StandaloneInMemProcessEngineConfiguration()).DefaultUserPermissionNameForTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldInitUpdatePermission()
	  public virtual void shouldInitUpdatePermission()
	  {
		TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

		// given
		testProcessEngineCfg.DefaultUserPermissionNameForTask = "UPDATE";

		// if
		testProcessEngineCfg.initDefaultUserPermissionForTask();

		// then
		assertEquals(Permissions.UPDATE, testProcessEngineCfg.DefaultUserPermissionForTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldInitTaskWorkPermission()
	  public virtual void shouldInitTaskWorkPermission()
	  {
		TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

		// given
		testProcessEngineCfg.DefaultUserPermissionNameForTask = "TASK_WORK";

		// if
		testProcessEngineCfg.initDefaultUserPermissionForTask();

		// then
		assertEquals(Permissions.TASK_WORK, testProcessEngineCfg.DefaultUserPermissionForTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionOnUnsupportedPermission()
	  public virtual void shouldThrowExceptionOnUnsupportedPermission()
	  {
		TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

		// given
		testProcessEngineCfg.DefaultUserPermissionNameForTask = "UNSUPPORTED";

		// if
		try
		{
		  testProcessEngineCfg.initDefaultUserPermissionForTask();
		  fail("Exception expected");

		}
		catch (ProcessEngineException e)
		{
		  string expectedExceptionMessage = string.Format("Invalid value '{0}' for configuration property 'defaultUserPermissionNameForTask'.", "UNSUPPORTED");
		  assertThat(e.Message, containsString(expectedExceptionMessage));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldThrowExceptionOnNullPermissionName()
	  public virtual void shouldThrowExceptionOnNullPermissionName()
	  {
		TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

		// given
		testProcessEngineCfg.DefaultUserPermissionNameForTask = null;

		// if
		try
		{
		  testProcessEngineCfg.initDefaultUserPermissionForTask();
		  fail("Exception expected");

		}
		catch (ProcessEngineException e)
		{
		  string expectedExceptionMessage = "Invalid value 'null' for configuration property 'defaultUserPermissionNameForTask'.";
		  assertThat(e.Message, containsString(expectedExceptionMessage));
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldNotInitIfAlreadySet()
	  public virtual void shouldNotInitIfAlreadySet()
	  {
		TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

		// given
		testProcessEngineCfg.DefaultUserPermissionForTask = Permissions.ALL;

		// if
		testProcessEngineCfg.initDefaultUserPermissionForTask();

		// then
		assertEquals(Permissions.ALL, testProcessEngineCfg.DefaultUserPermissionForTask);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldInitTaskPermission()
	  public virtual void shouldInitTaskPermission()
	  {
		ProcessEngine engine = null;
		try
		{
		  // if
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();
		  TestProcessEngineCfg testProcessEngineCfg = new TestProcessEngineCfg();

		  engine = testProcessEngineCfg.setProcessEngineName("DefaultTaskPermissionsCfgTest-engine").setJdbcUrl(string.Format("jdbc:h2:mem:{0}", "DefaultTaskPermissionsCfgTest-engine-db")).setMetricsEnabled(false).setJobExecutorActivate(false).buildProcessEngine();

		  // then
		  assertTrue(testProcessEngineCfg.initMethodCalled);
		}
		finally
		{
		  if (engine != null)
		  {
			engine.close();
		  }
		}
	  }

	  internal class TestProcessEngineCfg : StandaloneInMemProcessEngineConfiguration
	  {

		internal bool initMethodCalled = false;

		public override void initDefaultUserPermissionForTask()
		{
		  base.initDefaultUserPermissionForTask();
		  initMethodCalled = true;
		}
	  }


	}

}