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
namespace org.camunda.bpm.qa.upgrade.scenarios7110.gson
{
	using FilterService = org.camunda.bpm.engine.FilterService;
	using Filter = org.camunda.bpm.engine.filter.Filter;
	using ProcessEngineRule = org.camunda.bpm.engine.test.ProcessEngineRule;
	using Rule = org.junit.Rule;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	/// <summary>
	/// @author Tassilo Weidner
	/// </summary>
	[ScenarioUnderTest("TaskFilterPropertiesScenario"), Origin("7.11.0")]
	public class TaskFilterPropertiesTest
	{
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Rule public org.camunda.bpm.engine.test.ProcessEngineRule engineRule = new org.camunda.bpm.engine.test.ProcessEngineRule("camunda.cfg.xml");
		public ProcessEngineRule engineRule = new ProcessEngineRule("camunda.cfg.xml");

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTaskFilterProperties.1") @Test public void testMapContainingListContainingMapProperty_DeserializePrimitives()
	  [ScenarioUnderTest("initTaskFilterProperties.1")]
	  public virtual void testMapContainingListContainingMapProperty_DeserializePrimitives()
	  {
		FilterService filterService = engineRule.FilterService;

		// when
		Filter filterOne = filterService.createFilterQuery().filterName("taskFilterOne").singleResult();

		System.Collections.IDictionary deserialisedProperties = filterOne.Properties;

		System.Collections.IList list = (System.Collections.IList) deserialisedProperties["foo"];
		System.Collections.IDictionary map = (System.Collections.IDictionary) list[0];

		// then
		assertThat(deserialisedProperties.Count, @is(1));
		assertThat((string) map["string"], @is("aStringValue"));
		assertThat((int) map["int"], @is(47));
		assertThat((long) map["intOutOfRange"], @is(int.MaxValue + 1L));
		assertThat((long) map["long"], @is(long.MaxValue));
		assertThat((double) map["double"], @is(3.14159265359D));
		assertThat((bool) map["boolean"], @is(true));
		assertThat(map["null"], nullValue());
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @ScenarioUnderTest("initTaskFilterProperties.1") @Test public void testMapContainingMapContainingListProperty_DeserializePrimitives()
	  [ScenarioUnderTest("initTaskFilterProperties.1")]
	  public virtual void testMapContainingMapContainingListProperty_DeserializePrimitives()
	  {
		FilterService filterService = engineRule.FilterService;

		// when
		Filter filterTwo = filterService.createFilterQuery().filterName("taskFilterTwo").singleResult();

		System.Collections.IDictionary deserialisedProperties = filterTwo.Properties;

		System.Collections.IList list = (System.Collections.IList)((System.Collections.IDictionary) deserialisedProperties["foo"])["bar"];

		// then
		assertThat(deserialisedProperties.Count, @is(1));

		assertThat((string) list[0], @is("aStringValue"));
		assertThat((int) list[1], @is(47));
		assertThat((long) list[2], @is(int.MaxValue + 1L));
		assertThat((long) list[3], @is(long.MaxValue));
		assertThat((double) list[4], @is(3.14159265359D));
		assertThat((bool) list[5], @is(true));
		assertThat(list[6], nullValue());
	  }

	}
}