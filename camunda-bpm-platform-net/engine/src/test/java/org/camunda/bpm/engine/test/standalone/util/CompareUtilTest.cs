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
namespace org.camunda.bpm.engine.test.standalone.util
{
	using CompareUtil = org.camunda.bpm.engine.impl.util.CompareUtil;
	using Test = org.junit.Test;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.Matchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertThat;

	/// <summary>
	/// @author Filip Hrisafov
	/// </summary>
	public class CompareUtilTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDateNotInAnAscendingOrder()
	  public virtual void testDateNotInAnAscendingOrder()
	  {
		DateTime calendar = new DateTime();
		calendar = new DateTime(2015, 3, 15);
		DateTime first = calendar;
		calendar = new DateTime(2015, 8, 15);
		DateTime second = calendar;
		DateTime nullDate = null;
		assertThat(CompareUtil.areNotInAscendingOrder(null, first, null, second), @is(false));
		assertThat(CompareUtil.areNotInAscendingOrder(null, first, null, first), @is(false));
		assertThat(CompareUtil.areNotInAscendingOrder(null, second, null, first), @is(true));
		assertThat(CompareUtil.areNotInAscendingOrder(nullDate, nullDate, nullDate), @is(false));

		assertThat(CompareUtil.areNotInAscendingOrder(Arrays.asList(first, second)), @is(false));
		assertThat(CompareUtil.areNotInAscendingOrder(Arrays.asList(first, first)), @is(false));
		assertThat(CompareUtil.areNotInAscendingOrder(Arrays.asList(second, first)), @is(true));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsNotContainedIn()
	  public virtual void testIsNotContainedIn()
	  {
		string element = "test";
		string[] values = new string[] {"test", "test1", "test2"};
		string[] values2 = new string[] {"test1", "test2"};
		string[] nullValues = null;
		IList<string> nullList = null;

		assertThat(CompareUtil.elementIsNotContainedInArray(element, values), @is(false));
		assertThat(CompareUtil.elementIsNotContainedInArray(element, values2), @is(true));
		assertThat(CompareUtil.elementIsNotContainedInArray(null, values), @is(false));
		assertThat(CompareUtil.elementIsNotContainedInArray(null, nullValues), @is(false));
		assertThat(CompareUtil.elementIsNotContainedInArray(element, nullValues), @is(false));

		assertThat(CompareUtil.elementIsNotContainedInList(element, Arrays.asList(values)), @is(false));
		assertThat(CompareUtil.elementIsNotContainedInList(element, Arrays.asList(values2)), @is(true));
		assertThat(CompareUtil.elementIsNotContainedInList(null, Arrays.asList(values)), @is(false));
		assertThat(CompareUtil.elementIsNotContainedInList(null, nullList), @is(false));
		assertThat(CompareUtil.elementIsNotContainedInList(element, nullList), @is(false));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testIsContainedIn()
	  public virtual void testIsContainedIn()
	  {
		string element = "test";
		string[] values = new string[] {"test", "test1", "test2"};
		string[] values2 = new string[] {"test1", "test2"};
		string[] nullValues = null;
		IList<string> nullList = null;

		assertThat(CompareUtil.elementIsContainedInArray(element, values), @is(true));
		assertThat(CompareUtil.elementIsContainedInArray(element, values2), @is(false));
		assertThat(CompareUtil.elementIsContainedInArray(null, values), @is(false));
		assertThat(CompareUtil.elementIsContainedInArray(null, nullValues), @is(false));
		assertThat(CompareUtil.elementIsContainedInArray(element, nullValues), @is(false));

		assertThat(CompareUtil.elementIsContainedInList(element, Arrays.asList(values)), @is(true));
		assertThat(CompareUtil.elementIsContainedInList(element, Arrays.asList(values2)), @is(false));
		assertThat(CompareUtil.elementIsContainedInList(null, Arrays.asList(values)), @is(false));
		assertThat(CompareUtil.elementIsContainedInList(null, nullList), @is(false));
		assertThat(CompareUtil.elementIsContainedInList(element, nullList), @is(false));
	  }
	}

}