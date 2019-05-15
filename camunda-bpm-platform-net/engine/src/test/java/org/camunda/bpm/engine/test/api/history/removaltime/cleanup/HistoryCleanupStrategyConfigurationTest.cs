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
namespace org.camunda.bpm.engine.test.api.history.removaltime.cleanup
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using AfterClass = org.junit.AfterClass;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_END;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_NONE;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_REMOVAL_TIME_STRATEGY_START;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.@is;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class HistoryCleanupStrategyConfigurationTest
	{
		private bool InstanceFieldsInitialized = false;

		public HistoryCleanupStrategyConfigurationTest()
		{
			if (!InstanceFieldsInitialized)
			{
				InitializeInstanceFields();
				InstanceFieldsInitialized = true;
			}
		}

		private void InitializeInstanceFields()
		{
			testRule = new ProcessEngineTestRule(engineRule);
			ruleChain = RuleChain.outerRule(engineRule).around(testRule);
		}


	  protected internal ProcessEngineRule engineRule = new ProvidedProcessEngineRule();
	  protected internal ProcessEngineTestRule testRule;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.RuleChain ruleChain = org.junit.rules.RuleChain.outerRule(engineRule).around(testRule);
	  public RuleChain ruleChain;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.junit.rules.ExpectedException thrown = org.junit.rules.ExpectedException.none();
	  public ExpectedException thrown = ExpectedException.none();

	  protected internal static ProcessEngineConfigurationImpl engineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		engineConfiguration = engineRule.ProcessEngineConfiguration;

		engineConfiguration.setHistoryCleanupStrategy(null).setHistoryRemovalTimeStrategy(null).initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDown()
	  public static void tearDown()
	  {
		engineConfiguration.setHistoryCleanupStrategy(null).setHistoryRemovalTimeStrategy(null).initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAutomaticallyConfigure()
	  public virtual void shouldAutomaticallyConfigure()
	  {
		// given

		engineConfiguration.HistoryCleanupStrategy = null;

		// when
		engineConfiguration.initHistoryCleanup();

		// then
		assertThat(engineConfiguration.HistoryCleanupStrategy, @is(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToRemovalTimeBased()
	  public virtual void shouldConfigureToRemovalTimeBased()
	  {
		// given

		engineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED;

		// when
		engineConfiguration.initHistoryCleanup();

		// then
		assertThat(engineConfiguration.HistoryCleanupStrategy, @is(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToRemovalTimeBasedWithRemovalTimeStrategyToEnd()
	  public virtual void shouldConfigureToRemovalTimeBasedWithRemovalTimeStrategyToEnd()
	  {
		// given

		engineConfiguration.setHistoryCleanupStrategy(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END);

		// when
		engineConfiguration.initHistoryCleanup();

		// then
		assertThat(engineConfiguration.HistoryCleanupStrategy, @is(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED));
		assertThat(engineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_END));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToRemovalTimeBasedWithRemovalTimeStrategyToStart()
	  public virtual void shouldConfigureToRemovalTimeBasedWithRemovalTimeStrategyToStart()
	  {
		// given

		engineConfiguration.setHistoryCleanupStrategy(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START);

		// when
		engineConfiguration.initHistoryCleanup();

		// then
		assertThat(engineConfiguration.HistoryCleanupStrategy, @is(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED));
		assertThat(engineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_START));
	  }


//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToEndTimeBased()
	  public virtual void shouldConfigureToEndTimeBased()
	  {
		// given

		engineConfiguration.HistoryCleanupStrategy = HISTORY_CLEANUP_STRATEGY_END_TIME_BASED;

		// when
		engineConfiguration.initHistoryCleanup();

		// then
		assertThat(engineConfiguration.HistoryCleanupStrategy, @is(HISTORY_CLEANUP_STRATEGY_END_TIME_BASED));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureWithNotExistentStrategy()
	  public virtual void shouldConfigureWithNotExistentStrategy()
	  {
		// given

		engineConfiguration.HistoryCleanupStrategy = "nonExistentStrategy";

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("history cleanup strategy must be either set to 'removalTimeBased' or 'endTimeBased'.");

		// when
		engineConfiguration.initHistoryCleanup();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToRemovalTimeBasedWithRemovalTimeStrategyToNone()
	  public virtual void shouldConfigureToRemovalTimeBasedWithRemovalTimeStrategyToNone()
	  {
		// given

		engineConfiguration.setHistoryCleanupStrategy(HISTORY_CLEANUP_STRATEGY_REMOVAL_TIME_BASED).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE);

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("history removal time strategy cannot be set to 'none' in conjunction with 'removalTimeBased' history cleanup strategy.");

		// when
		engineConfiguration.initHistoryCleanup();
	  }

	}

}