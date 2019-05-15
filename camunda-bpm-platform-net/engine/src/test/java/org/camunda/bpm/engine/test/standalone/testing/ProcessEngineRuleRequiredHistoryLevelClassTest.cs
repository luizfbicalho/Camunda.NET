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
namespace org.camunda.bpm.engine.test.standalone.testing
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	using ProvidedProcessEngineRule = org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule;
	using CoreMatchers = org.hamcrest.CoreMatchers;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;

	[RequiredHistoryLevel(ProcessEngineConfiguration.HISTORY_AUDIT)]
	public class ProcessEngineRuleRequiredHistoryLevelClassTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public final org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.util.ProvidedProcessEngineRule();
		public readonly ProcessEngineRule engineRule = new ProvidedProcessEngineRule();

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void requiredHistoryLevelOnClass()
	  public virtual void requiredHistoryLevelOnClass()
	  {

		assertThat(currentHistoryLevel(), CoreMatchers.either<string>(@is(ProcessEngineConfiguration.HISTORY_AUDIT)).or(@is(ProcessEngineConfiguration.HISTORY_FULL)));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test @RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY) public void overrideRequiredHistoryLevelOnClass()
	  [RequiredHistoryLevel(org.camunda.bpm.engine.ProcessEngineConfiguration.HISTORY_ACTIVITY)]
	  public virtual void overrideRequiredHistoryLevelOnClass()
	  {

		assertThat(currentHistoryLevel(), CoreMatchers.either<string>(@is(ProcessEngineConfiguration.HISTORY_ACTIVITY)).or(@is(ProcessEngineConfiguration.HISTORY_AUDIT)).or(@is(ProcessEngineConfiguration.HISTORY_FULL)));
	  }

	  protected internal virtual string currentHistoryLevel()
	  {
		return engineRule.ProcessEngine.ProcessEngineConfiguration.History;
	  }

	}

}