﻿/*
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
namespace org.camunda.commons.utils
{
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Fail.fail;

	/// <summary>
	/// @author Stefan Hentschel.
	/// </summary>
	public class EnsureUtilTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensureNotNull()
	  public virtual void ensureNotNull()
	  {
		string @string = "string";

		try
		{
		  EnsureUtil.ensureNotNull("string", @string);

		}
		catch (System.ArgumentException e)
		{
		  fail("Not expected the following exception: ", e);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailEnsureNotNull()
	  public virtual void shouldFailEnsureNotNull()
	  {
		string @string = null;

		try
		{
		  EnsureUtil.ensureNotNull("string", @string);
		  fail("Expected: IllegalArgumentException");

		}
		catch (System.ArgumentException)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void ensureParameterInstanceOfClass()
	  public virtual void ensureParameterInstanceOfClass()
	  {
		object @string = "string";

		try
		{
		  assertThat(EnsureUtil.ensureParamInstanceOf("string", @string, typeof(string))).isInstanceOf(typeof(string));

		}
		catch (System.ArgumentException e)
		{
		  fail("Not expected the following exception: ", e);
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFailEnsureParameterInstanceOfClass()
	  public virtual void shouldFailEnsureParameterInstanceOfClass()
	  {
		object @string = "string";

		try
		{
		  EnsureUtil.ensureParamInstanceOf("string", @string, typeof(Integer));
		  fail("Expected: IllegalArgumentException");
		}
		catch (System.ArgumentException)
		{
		  // happy path
		}
	  }
	}

}