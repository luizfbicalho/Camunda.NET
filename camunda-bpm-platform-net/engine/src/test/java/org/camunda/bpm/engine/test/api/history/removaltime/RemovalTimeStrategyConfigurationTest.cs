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
namespace org.camunda.bpm.engine.test.api.history.removaltime
{
	using ProcessEngineConfigurationImpl = org.camunda.bpm.engine.impl.cfg.ProcessEngineConfigurationImpl;
	using HistoryRemovalTimeProvider = org.camunda.bpm.engine.impl.history.HistoryRemovalTimeProvider;
	using ProcessEngineTestRule = org.camunda.bpm.engine.test.util.ProcessEngineTestRule;
	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using AfterClass = org.junit.AfterClass;
	using Before = org.junit.Before;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;
	using ExpectedException = org.junit.rules.ExpectedException;
	using RuleChain = org.junit.rules.RuleChain;

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
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.core.Is.isA;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.mockito.Mockito.mock;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	public class RemovalTimeStrategyConfigurationTest
	{
		private bool InstanceFieldsInitialized = false;

		public RemovalTimeStrategyConfigurationTest()
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

	  protected internal static ProcessEngineConfigurationImpl processEngineConfiguration;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void init()
	  public virtual void init()
	  {
		processEngineConfiguration = engineRule.ProcessEngineConfiguration;

		processEngineConfiguration.setHistoryRemovalTimeStrategy(null).setHistoryRemovalTimeProvider(null).initHistoryRemovalTime();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void tearDown()
	  public static void tearDown()
	  {
		processEngineConfiguration.setHistoryRemovalTimeStrategy(null).setHistoryRemovalTimeProvider(null).initHistoryRemovalTime();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldAutomaticallyConfigure()
	  public virtual void shouldAutomaticallyConfigure()
	  {
		// given

		processEngineConfiguration.setHistoryRemovalTimeProvider(null).setHistoryRemovalTimeStrategy(null);

		// when
		processEngineConfiguration.initHistoryRemovalTime();

		// then
		assertThat(processEngineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_END));
		assertThat(processEngineConfiguration.HistoryRemovalTimeProvider, isA(typeof(HistoryRemovalTimeProvider)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToStart()
	  public virtual void shouldConfigureToStart()
	  {
		// given

		processEngineConfiguration.setHistoryRemovalTimeProvider(mock(typeof(HistoryRemovalTimeProvider))).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_START);

		// when
		processEngineConfiguration.initHistoryRemovalTime();

		// then
		assertThat(processEngineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_START));
		assertThat(processEngineConfiguration.HistoryRemovalTimeProvider, isA(typeof(HistoryRemovalTimeProvider)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToEnd()
	  public virtual void shouldConfigureToEnd()
	  {
		// given

		processEngineConfiguration.setHistoryRemovalTimeProvider(mock(typeof(HistoryRemovalTimeProvider))).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END);

		// when
		processEngineConfiguration.initHistoryRemovalTime();

		// then
		assertThat(processEngineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_END));
		assertThat(processEngineConfiguration.HistoryRemovalTimeProvider, isA(typeof(HistoryRemovalTimeProvider)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureToNone()
	  public virtual void shouldConfigureToNone()
	  {
		// given

		processEngineConfiguration.setHistoryRemovalTimeProvider(mock(typeof(HistoryRemovalTimeProvider))).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_NONE);

		// when
		processEngineConfiguration.initHistoryRemovalTime();

		// then
		assertThat(processEngineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_NONE));
		assertThat(processEngineConfiguration.HistoryRemovalTimeProvider, isA(typeof(HistoryRemovalTimeProvider)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureWithoutProvider()
	  public virtual void shouldConfigureWithoutProvider()
	  {
		// given

		processEngineConfiguration.setHistoryRemovalTimeProvider(null).setHistoryRemovalTimeStrategy(HISTORY_REMOVAL_TIME_STRATEGY_END);

		// when
		processEngineConfiguration.initHistoryRemovalTime();

		// then
		assertThat(processEngineConfiguration.HistoryRemovalTimeStrategy, @is(HISTORY_REMOVAL_TIME_STRATEGY_END));
		assertThat(processEngineConfiguration.HistoryRemovalTimeProvider, isA(typeof(HistoryRemovalTimeProvider)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldConfigureWithNotExistentStrategy()
	  public virtual void shouldConfigureWithNotExistentStrategy()
	  {
		// given

		processEngineConfiguration.HistoryRemovalTimeStrategy = "notExistentStrategy";

		// then
		thrown.expect(typeof(ProcessEngineException));
		thrown.expectMessage("history removal time strategy must be set to 'start', 'end' or 'none'");

		// when
		processEngineConfiguration.initHistoryRemovalTime();

		// assume
		assertThat(processEngineConfiguration.HistoryRemovalTimeProvider, isA(typeof(HistoryRemovalTimeProvider)));
	  }

	}

}