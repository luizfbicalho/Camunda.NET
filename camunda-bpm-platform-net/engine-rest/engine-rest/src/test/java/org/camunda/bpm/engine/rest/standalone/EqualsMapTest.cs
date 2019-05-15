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

	using EqualsMap = org.camunda.bpm.engine.rest.helper.EqualsMap;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertFalse;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertTrue;

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class EqualsMapTest
	{

	  protected internal IDictionary<string, object> map1;
	  protected internal IDictionary<string, object> map2;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		map1 = new Dictionary<string, object>();
		map2 = new Dictionary<string, object>();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapsSame()
	  public virtual void testMapsSame()
	  {
		assertTrue((new EqualsMap(map1)).matches(map1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapsEqual()
	  public virtual void testMapsEqual()
	  {
		map1["aKey"] = "aValue";
		map2["aKey"] = "aValue";

		assertTrue((new EqualsMap(map1)).matches(map2));
		assertTrue((new EqualsMap(map2)).matches(map1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapsNotEqual()
	  public virtual void testMapsNotEqual()
	  {
		map1["aKey"] = "aValue";

		assertFalse((new EqualsMap(map1)).matches(map2));
		assertFalse((new EqualsMap(map2)).matches(map1));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testMapsNull()
	  public virtual void testMapsNull()
	  {
		assertFalse((new EqualsMap(null)).matches(map1));
		assertFalse((new EqualsMap(map1)).matches(null));
		assertTrue((new EqualsMap(null)).matches(null));
	  }

	}

}