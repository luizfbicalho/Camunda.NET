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
namespace org.camunda.bpm.engine.rest.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.camunda.bpm.engine.rest.helper.NoIntermediaryInvocation.immediatelyAfter;

	using Assert = org.junit.Assert;
	using Before = org.junit.Before;
	using Test = org.junit.Test;
	using InOrder = org.mockito.InOrder;
	using Mockito = org.mockito.Mockito;
	using MockitoAssertionError = org.mockito.exceptions.@base.MockitoAssertionError;

	/// <summary>
	/// @author Thorben Lindhauer
	/// </summary>
	public class NoIntermediaryInvocationTest
	{

	  protected internal Foo foo;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void setUp()
	  public virtual void setUp()
	  {
		foo = Mockito.mock(typeof(Foo));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testSucceess()
	  public virtual void testSucceess()
	  {

		// given
		foo.getFoo();
		foo.Bar;

		// when
		InOrder inOrder = Mockito.inOrder(foo);
		inOrder.verify(foo).Foo;

		// then
		inOrder.verify(foo, immediatelyAfter()).Bar;

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailureWhenInvocationNotPresent()
	  public virtual void testFailureWhenInvocationNotPresent()
	  {
		// given
		foo.getFoo();
		foo.Baz;

		// when
		InOrder inOrder = Mockito.inOrder(foo);
		inOrder.verify(foo).Foo;

		// then
		try
		{
		  inOrder.verify(foo, immediatelyAfter()).Bar;
		  Assert.fail("should not verify");
		}
		catch (MockitoAssertionError)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailureWhenInvocationNotPresentCase2()
	  public virtual void testFailureWhenInvocationNotPresentCase2()
	  {
		// given
		foo.getFoo();

		// when
		InOrder inOrder = Mockito.inOrder(foo);
		inOrder.verify(foo).Foo;

		// then
		try
		{
		  inOrder.verify(foo, immediatelyAfter()).Bar;
		  Assert.fail("should not verify");
		}
		catch (MockitoAssertionError)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailureOnWrongInvocationOrder()
	  public virtual void testFailureOnWrongInvocationOrder()
	  {
		// given
		foo.Bar;
		foo.getFoo();

		// when
		InOrder inOrder = Mockito.inOrder(foo);
		inOrder.verify(foo).Foo;

		// then
		try
		{
		  inOrder.verify(foo, immediatelyAfter()).Bar;
		  Assert.fail("should not verify");
		}
		catch (MockitoAssertionError)
		{
		  // happy path
		}
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testFailureWithIntermittentInvocations()
	  public virtual void testFailureWithIntermittentInvocations()
	  {
		// given
		foo.getFoo();
		foo.Baz;
		foo.Bar;

		// when
		InOrder inOrder = Mockito.inOrder(foo);
		inOrder.verify(foo).Foo;

		// then
		try
		{
		  inOrder.verify(foo, immediatelyAfter()).Bar;
		  Assert.fail("should not verify");
		}
		catch (MockitoAssertionError)
		{
		  // happy path
		}
	  }

	  public interface Foo
	  {

		string getFoo();

		string Bar {get;}

		string Baz {get;}
	  }
	}

}