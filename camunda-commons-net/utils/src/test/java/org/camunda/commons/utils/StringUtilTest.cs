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
namespace org.camunda.commons.utils
{
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.assertThat;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.commons.utils.StringUtil.isExpression;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.commons.utils.StringUtil.join;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.commons.utils.StringUtil.split;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.commons.utils.StringUtil.defaultString;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.commons.utils.StringUtil.getStackTrace;

	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class StringUtilTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExpressionDetection()
	  public virtual void testExpressionDetection()
	  {
		assertThat(isExpression("${test}")).True;
		assertThat(isExpression("${a(b,c)}")).True;
		assertThat(isExpression("${ test }")).True;
		assertThat(isExpression(" ${test} ")).True;
		assertThat(isExpression(" \n${test} ")).True;

		assertThat(isExpression("#{test}")).True;
		assertThat(isExpression("#{a(b,c)}")).True;
		assertThat(isExpression("#{ test }")).True;
		assertThat(isExpression(" #{test} ")).True;
		assertThat(isExpression(" \n#{test} ")).True;

		assertThat(isExpression("test")).False;
		assertThat(isExpression("    test")).False;
		assertThat(isExpression("{test}")).False;
		assertThat(isExpression("(test)")).False;
		assertThat(isExpression("")).False;
		assertThat(isExpression(null)).False;
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStringSplit()
	  public virtual void testStringSplit()
	  {
		assertThat(split("a,b,c", ",")).hasSize(3).containsExactly("a", "b", "c");
		assertThat(split("aaaxbaaxc", "a{2}x")).hasSize(3).containsExactly("a", "b", "c");
		assertThat(split(null, ",")).Null;
		assertThat(split("abc", ",")).hasSize(1).containsExactly("abc");
		assertThat(split("a,b,c", null)).hasSize(1).containsExactly("a,b,c");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testStringJoin()
	  public virtual void testStringJoin()
	  {
		assertThat(join(",", "a", "b", "c")).isEqualTo("a,b,c");
		assertThat(join(", ", "a", "b", "c")).isEqualTo("a, b, c");
		assertThat(join(null, "a", "b", "c")).isEqualTo("abc");
		assertThat(join(",", "")).isEqualTo("");
		assertThat(join(null, (string[]) null)).Null;
		assertThat(join("aax", "a", "b", "c")).isEqualTo("aaaxbaaxc");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testDefaultString()
	  public virtual void testDefaultString()
	  {
		assertThat(defaultString(null)).isEqualTo("");
		assertThat(defaultString("")).isEqualTo("");
		assertThat(defaultString("bat")).isEqualTo("bat");
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetStacktrace()
	  public virtual void testGetStacktrace()
	  {
		Exception th = new System.ArgumentException("Wrong argument!", new System.NullReferenceException("This shouldn't have been empty"));
		assertThat(getStackTrace(th)).containsSequence("java.lang.IllegalArgumentException: Wrong argument!", "at org.camunda.commons.utils.StringUtilTest.testGetStacktrace(StringUtilTest.java:", "Caused by: java.lang.NullPointerException: This shouldn't have been empty");
	  }

	}

}