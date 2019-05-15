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
namespace org.camunda.bpm.engine.rest.standalone
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;


	using EqualsList = org.camunda.bpm.engine.rest.helper.EqualsList;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EqualsListTest
	{

	  protected internal IList<string> list1;
	  protected internal IList<string> list2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		list1 = new List<string>();
		list2 = new List<string>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListsSame()
	  public virtual void testListsSame()
	  {
		assertTrue((new EqualsList(list1)).matches(list1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListsEqual()
	  public virtual void testListsEqual()
	  {
		list1.Add("aString");
		list2.Add("aString");

		assertTrue((new EqualsList(list1)).matches(list2));
		assertTrue((new EqualsList(list2)).matches(list1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListsNotEqual()
	  public virtual void testListsNotEqual()
	  {
		list1.Add("aString");

		assertFalse((new EqualsList(list1)).matches(list2));
		assertFalse((new EqualsList(list2)).matches(list1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testListsNull()
	  public virtual void testListsNull()
	  {
		assertFalse((new EqualsList(null)).matches(list1));
		assertFalse((new EqualsList(list1)).matches(null));
		assertTrue((new EqualsList(null)).matches(null));
	  }

	}

}